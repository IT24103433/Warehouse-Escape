using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Implements waypoint-based looping patrol for an enemy guard.
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
public class GuardPatrol : MonoBehaviour
{
    [Header("Patrol Waypoints")]
    [Tooltip("List of waypoint transforms defining the patrol circuit around obstacles.")]
    [SerializeField] private Transform[] waypoints;

    [Header("Patrol Behavior")]
    [Tooltip("Time in seconds to wait at each waypoint before advancing.")]
    [SerializeField] private float waypointWaitTime = 1.0f;

    [Tooltip("Distance threshold to consider a waypoint reached.")]
    [SerializeField] private float arrivalThreshold = 0.5f;

    private EnemyMovement enemyMovement;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool hasNavMeshWarningLogged = false;

    public int CurrentWaypointIndex => currentWaypointIndex;
    public bool IsWaiting => isWaiting;
    public Transform[] Waypoints => waypoints;

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Start()
    {
        ValidateAndStartPatrol();
    }

    void Update()
    {
        if (!enemyMovement.IsOnNavMesh)
        {
            if (!hasNavMeshWarningLogged)
            {
                Debug.LogWarning($"[GuardPatrol] Navigation unavailable: Agent on '{gameObject.name}' is not placed on a valid NavMesh. Patrol suspended.", this);
                hasNavMeshWarningLogged = true;
            }
            return;
        }

        hasNavMeshWarningLogged = false;

        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                AdvanceToNextWaypoint();
            }
            return;
        }

        if (enemyMovement.HasReachedDestination(arrivalThreshold))
        {
            isWaiting = true;
            waitTimer = waypointWaitTime;
            enemyMovement.Stop();
        }
    }

    /// <summary>
    /// Validates waypoint reachability on the NavMesh and begins the patrol.
    /// </summary>
    public void ValidateAndStartPatrol()
    {
        if (!enemyMovement.IsOnNavMesh)
        {
            Debug.LogWarning($"[GuardPatrol] Navigation unavailable: Agent on '{gameObject.name}' is not placed on a valid NavMesh. Ensure NavMeshSurface is baked.", this);
            hasNavMeshWarningLogged = true;
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"[GuardPatrol] No waypoints assigned to '{gameObject.name}'.", this);
            return;
        }

        // Validate each waypoint on the NavMesh
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null)
            {
                Debug.LogWarning($"[GuardPatrol] Waypoint at index {i} on '{gameObject.name}' is unassigned.", this);
                continue;
            }

            if (!NavMesh.SamplePosition(waypoints[i].position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                Debug.LogWarning($"[GuardPatrol] Waypoint {i} ({waypoints[i].name} at {waypoints[i].position}) is unreachable on the NavMesh!", this);
            }
        }

        MoveToWaypoint(currentWaypointIndex);
    }

    private void MoveToWaypoint(int index)
    {
        if (waypoints == null || waypoints.Length == 0 || index >= waypoints.Length || waypoints[index] == null)
            return;

        enemyMovement.Resume();
        enemyMovement.SetDestination(waypoints[index].position);
    }

    private void AdvanceToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        MoveToWaypoint(currentWaypointIndex);
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Gizmos.DrawWireSphere(waypoints[i].position, arrivalThreshold);

            int nextIndex = (i + 1) % waypoints.Length;
            if (waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
            }
        }
    }
}
