using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Operational states for Varun's Patrol Guard.
/// </summary>
public enum PatrolGuardState
{
    Patrol,
    Pursue
}

/// <summary>
/// Varun's Patrol Guard AI.
/// Continuously walks assigned waypoint routes across the warehouse.
/// Engages in direct pursuit when the player is sighted, and immediately resumes
/// regular waypoint patrol as soon as the intruder breaks line of sight.
/// Intentionally does not perform extended search or last-seen position investigations.
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
public class PatrolGuardAI : MonoBehaviour
{
    [Header("Current State")]
    [Tooltip("Current operational state.")]
    [SerializeField] private PatrolGuardState currentState = PatrolGuardState.Patrol;

    [Tooltip("Whether the intruder is currently within clear visual confirmation.")]
    [SerializeField] private bool canSeePlayer = false;

    [Header("Patrol Waypoint Settings")]
    [Tooltip("Assigned waypoints defining this unit's patrol circuit.")]
    [SerializeField] private Transform[] waypoints;

    [Tooltip("Arrival distance threshold for waypoints.")]
    [SerializeField] private float arrivalThreshold = 0.6f;

    [Tooltip("Time in seconds to wait at each waypoint.")]
    [SerializeField] private float waypointWaitTime = 1.0f;

    [Header("Perception Settings")]
    [Tooltip("Maximum sight range in meters.")]
    [SerializeField] private float sightDistance = 11f;

    [Tooltip("Horizontal field of view angle in degrees.")]
    [SerializeField] private float fieldOfView = 90f;

    [Tooltip("Eye height offset above guard transform.")]
    [SerializeField] private float eyeHeight = 0.6f;

    [Tooltip("Target height offset above player transform.")]
    [SerializeField] private float playerTargetHeight = 0.0f;

    [Tooltip("Layer mask for line of sight occlusion (walls, shelves, crates).")]
    [SerializeField] private LayerMask obstacleMask = ~0;

    [Header("Pursuit Settings")]
    [Tooltip("Interval in seconds between destination repaths during pursuit.")]
    [SerializeField] private float repathInterval = 0.25f;

    private EnemyMovement enemyMovement;
    private Transform playerTransform;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private float repathTimer = 0f;

    public PatrolGuardState CurrentState => currentState;
    public bool CanSeePlayer => canSeePlayer;
    public Transform[] Waypoints { get => waypoints; set => waypoints = value; }
    public int CurrentWaypointIndex => currentWaypointIndex;

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        var rootMr = GetComponent<MeshRenderer>();
        if (rootMr != null) { rootMr.enabled = false; if (Application.isPlaying) Destroy(rootMr); else DestroyImmediate(rootMr); }
        var rootMf = GetComponent<MeshFilter>();
        if (rootMf != null) { if (Application.isPlaying) Destroy(rootMf); else DestroyImmediate(rootMf); }

        var health = GetComponent<Health>();
        if (health == null)
        {
            health = gameObject.AddComponent<Health>();
            health.CharacterName = "Varun [Patrol]";
            health.SetMaxHealth(60);
        }

        var shooting = GetComponent<GuardShooting>();
        if (shooting == null)
        {
            shooting = gameObject.AddComponent<GuardShooting>();
            shooting.ConfigureRole(GuardShooting.WeaponType.Pistol, 12, 1.1f, 12f, 1, 3.5f);
        }

        if (GetComponent<EnemyHealthBar>() == null)
        {
            gameObject.AddComponent<EnemyHealthBar>();
        }

        HumanoidMeshBuilder.EnsureRuntimeVisuals(transform, CharacterRole.Patrol_Varun);
    }

    private static readonly Vector3[] StorageInspectionRoute = new Vector3[]
    {
        new Vector3(-3.0f, 1.08f, -15.5f), // South crossing near Room 1 entrance
        new Vector3( 4.5f, 1.08f, -14.0f), // East storage aisle (inspection point)
        new Vector3( 4.5f, 1.08f,  -9.5f), // East central corridor
        new Vector3( 4.0f, 1.08f,  -5.5f), // Northeast inspection (checks Route B doorway)
        new Vector3( 0.0f, 1.08f,  -6.5f), // North central crossing
        new Vector3(-4.5f, 1.08f,  -5.5f), // Northwest inspection (checks Route A doorway)
        new Vector3(-5.0f, 1.08f, -10.5f), // West storage aisle
        new Vector3(-4.5f, 1.08f, -14.5f)  // Southwest corner (inspection point)
    };

    private void EnsureNaturalRoute()
    {
        if (waypoints == null || waypoints.Length <= 4)
        {
            var wpParent = GameObject.Find("Waypoints_Varun");
            if (wpParent == null) wpParent = new GameObject("Waypoints_Varun");
            var list = new System.Collections.Generic.List<Transform>();

            for (int i = 0; i < StorageInspectionRoute.Length; i++)
            {
                string wpName = $"WP_Varun_Natural_{i + 1}";
                Transform existing = wpParent.transform.Find(wpName);
                if (existing == null)
                {
                    GameObject go = new GameObject(wpName);
                    go.transform.SetParent(wpParent.transform);
                    go.transform.position = StorageInspectionRoute[i];
                    existing = go.transform;
                }
                else
                {
                    existing.position = StorageInspectionRoute[i];
                }
                list.Add(existing);
            }
            waypoints = list.ToArray();
        }
    }

    void Start()
    {
        FindPlayerReference();
        EnsureNaturalRoute();
        TransitionToPatrol();
    }

    void Update()
    {
        if (playerTransform == null) FindPlayerReference();

        // 1. Evaluate perception using shared helper
        canSeePlayer = GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask
        );

        // 2. State Machine
        switch (currentState)
        {
            case PatrolGuardState.Patrol:
                UpdatePatrolState();
                break;

            case PatrolGuardState.Pursue:
                UpdatePursueState();
                break;
        }
    }

    private void FindPlayerReference()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    public void SetPlayer(Transform player) => playerTransform = player;

    [Header("Room Boundaries")]
    [Tooltip("Encounter room boundaries. Guard stays within this area and disengages if player leaves the room.")]
    [SerializeField] private Bounds roomBounds;
    public Bounds RoomBounds { get => roomBounds; set => roomBounds = value; }

    public bool IsInsideRoom(Vector3 pos)
    {
        if (roomBounds.size == Vector3.zero) return true;
        Vector3 min = roomBounds.min;
        Vector3 max = roomBounds.max;
        return pos.x >= min.x && pos.x <= max.x && pos.z >= min.z && pos.z <= max.z;
    }

    private void UpdatePatrolState()
    {
        if (canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position)))
        {
            TransitionToPursue();
            return;
        }

        if (waypoints == null || waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                AdvanceWaypoint();
            }
            return;
        }

        if (enemyMovement.HasReachedDestination(arrivalThreshold))
        {
            isWaiting = true;
            // Inspection points (odd indices) pause longer (2.0s) for head/body scanning
            waitTimer = (currentWaypointIndex % 2 == 1) ? 2.0f : waypointWaitTime;
            enemyMovement.Stop();
            return;
        }

        if (!enemyMovement.PathPending && (!enemyMovement.HasPath || enemyMovement.PathStatus == NavMeshPathStatus.PathInvalid))
        {
            AdvanceWaypoint();
        }
    }

    private void AdvanceWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        DispatchCurrentWaypoint();
    }

    private void DispatchCurrentWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        Transform wp = waypoints[currentWaypointIndex];
        if (wp != null)
        {
            Vector3 targetPos = wp.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPos, out hit, 1.5f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    enemyMovement.SetDestination(hit.position);
                    enemyMovement.Resume();
                    return;
                }
            }

            enemyMovement.SetDestination(targetPos);
            enemyMovement.Resume();
        }
    }

    public void TransitionToPatrol()
    {
        currentState = PatrolGuardState.Patrol;
        Debug.Log($"[{gameObject.name}] State -> Patrol");
        isWaiting = false;
        DispatchCurrentWaypoint();
    }

    public void TransitionToPursue()
    {
        currentState = PatrolGuardState.Pursue;
        Debug.Log($"[{gameObject.name}] State -> Pursue (Intruder detected)");
        repathTimer = 0f;
        DispatchPursuitDestination();
    }

    private void UpdatePursueState()
    {
        // When sight is lost, or either intruder or guard is outside room boundaries, IMMEDIATELY resume patrol
        if (!canSeePlayer || (playerTransform != null && !IsInsideRoom(playerTransform.position)) || !IsInsideRoom(transform.position))
        {
            TransitionToPatrol();
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            DispatchPursuitDestination();
        }
    }

    private void DispatchPursuitDestination()
    {
        if (playerTransform == null) return;
        enemyMovement.SetDestination(playerTransform.position);
        enemyMovement.Resume();
    }

    void OnDrawGizmosSelected()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = new Color(0.7f, 0.2f, 0.9f); // Violet / Purple
        Gizmos.DrawWireSphere(eyePos, sightDistance);

        if (waypoints != null)
        {
            Gizmos.color = new Color(0.7f, 0.2f, 0.9f, 0.5f);
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                Gizmos.DrawWireSphere(waypoints[i].position, arrivalThreshold);
                int next = (i + 1) % waypoints.Length;
                if (waypoints[next] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
                }
            }
        }
    }
}
