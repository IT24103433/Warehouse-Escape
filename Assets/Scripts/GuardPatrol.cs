using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Implements waypoint-based looping patrol for an enemy guard with robust path loss recovery,
/// agent-specific query sampling, and safe periodic retries.
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

    [Tooltip("Maximum distance to sample a waypoint position on the NavMesh.")]
    [SerializeField] private float maxSampleDistance = 2.0f;

    [Tooltip("Time interval in seconds to wait before retrying when no waypoints are reachable.")]
    [SerializeField] private float retryInterval = 2.0f;

    private EnemyMovement enemyMovement;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isRetrying = false;
    private float retryTimer = 0f;
    private bool hasLoggedUnreachableWarning = false;
    private bool hasLoggedNavMeshWarning = false;

    // Public state properties for inspection, verification, and telemetry
    public int CurrentWaypointIndex => currentWaypointIndex;
    public bool IsWaiting => isWaiting;
    public bool IsRetrying => isRetrying;
    public Transform[] Waypoints => waypoints;
    public int CompletedLoops { get; private set; } = 0;

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
        // 1. Validate NavMesh presence
        if (!enemyMovement.IsOnNavMesh)
        {
            enemyMovement.Stop();
            if (!hasLoggedNavMeshWarning)
            {
                Debug.LogWarning($"[GuardPatrol] Navigation unavailable: Agent on '{gameObject.name}' is not placed on a valid NavMesh. Suspending patrol.", this);
                hasLoggedNavMeshWarning = true;
            }
            return;
        }

        hasLoggedNavMeshWarning = false;

        // 2. Validate waypoints list configuration
        if (waypoints == null || waypoints.Length == 0)
        {
            if (!hasLoggedUnreachableWarning)
            {
                Debug.LogWarning($"[GuardPatrol] No waypoints assigned to '{gameObject.name}'.", this);
                hasLoggedUnreachableWarning = true;
            }
            return;
        }

        // 3. Periodic retry handler when previously unable to reach any waypoint
        if (isRetrying)
        {
            retryTimer -= Time.deltaTime;
            if (retryTimer <= 0f)
            {
                retryTimer = retryInterval;
                AttemptStartOrResumePatrol();
            }
            return;
        }

        // 4. Waypoint pause handler
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

        // 5. Normal arrival check: only triggers for complete, valid paths
        if (enemyMovement.HasReachedDestination(arrivalThreshold))
        {
            isWaiting = true;
            waitTimer = waypointWaitTime;
            enemyMovement.Stop();

            // Track loop completion when reaching the final waypoint of the circuit
            if (currentWaypointIndex == waypoints.Length - 1)
            {
                CompletedLoops++;
            }
            return;
        }

        // 6. Path calculation in progress: DO NOT treat as failure while agent computes the path
        if (enemyMovement.PathPending)
        {
            return;
        }

        // 7. Active patrol path loss detection:
        // If an active patrol has lost its path entirely (cleared path, cut off by dynamic blocker, or invalid path),
        // recover immediately by advancing to find the next reachable waypoint.
        bool hasLostPath = !enemyMovement.HasPath ||
                           enemyMovement.PathStatus == NavMeshPathStatus.PathInvalid ||
                           enemyMovement.PathStatus == NavMeshPathStatus.PathPartial;

        if (hasLostPath)
        {
            AdvanceToNextWaypoint();
        }
    }

    /// <summary>
    /// Validates waypoint reachability and starts the patrol.
    /// </summary>
    public void ValidateAndStartPatrol()
    {
        if (!enemyMovement.IsOnNavMesh)
        {
            if (!hasLoggedNavMeshWarning)
            {
                Debug.LogWarning($"[GuardPatrol] Navigation unavailable: Agent on '{gameObject.name}' is not placed on a valid NavMesh. Ensure NavMeshSurface is baked.", this);
                hasLoggedNavMeshWarning = true;
            }
            isRetrying = true;
            retryTimer = retryInterval;
            return;
        }

        AttemptStartOrResumePatrol();
    }

    /// <summary>
    /// Samples a waypoint position on the NavMesh using the agent's specific navigation query filter.
    /// </summary>
    public bool TrySampleWaypointPosition(Transform waypoint, out Vector3 sampledPosition)
    {
        sampledPosition = Vector3.zero;
        if (waypoint == null) return false;

        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = enemyMovement.Agent.agentTypeID,
            areaMask = enemyMovement.Agent.areaMask
        };

        if (NavMesh.SamplePosition(waypoint.position, out NavMeshHit hit, maxSampleDistance, filter))
        {
            sampledPosition = hit.position;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Verifies that a waypoint is valid and that a complete, unbroken path exists to it.
    /// </summary>
    public bool IsWaypointReachable(Transform waypoint, out Vector3 validTargetPosition)
    {
        validTargetPosition = Vector3.zero;
        if (waypoint == null) return false;
        if (!enemyMovement.IsOnNavMesh) return false;

        if (!TrySampleWaypointPosition(waypoint, out validTargetPosition))
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        if (!enemyMovement.CalculatePath(validTargetPosition, path))
        {
            return false;
        }

        return path.status == NavMeshPathStatus.PathComplete;
    }

    /// <summary>
    /// Attempts to set destination to the specified waypoint.
    /// Handles SetDestination returning false cleanly.
    /// </summary>
    private bool MoveToWaypoint(int index, Vector3 position)
    {
        if (!enemyMovement.SetDestination(position))
        {
            // SetDestination returned false: reject candidate
            return false;
        }

        currentWaypointIndex = index;
        enemyMovement.Resume();
        return true;
    }

    /// <summary>
    /// Iterates through waypoints to find and set the next valid and reachable one.
    /// Skips null and unreachable entries, and recovers from SetDestination returning false.
    /// </summary>
    private bool TryFindAndSetNextReachableWaypoint(int startIndex)
    {
        if (waypoints == null || waypoints.Length == 0) return false;

        int total = waypoints.Length;
        for (int offset = 0; offset < total; offset++)
        {
            int candidateIndex = (startIndex + offset) % total;
            Transform wp = waypoints[candidateIndex];

            // Skip null waypoints
            if (wp == null) continue;

            // Skip unreachable waypoints
            if (IsWaypointReachable(wp, out Vector3 targetPos))
            {
                // Attempt to dispatch agent
                if (MoveToWaypoint(candidateIndex, targetPos))
                {
                    return true;
                }
                // If SetDestination returned false, continue loop to next candidate
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to find a reachable waypoint to begin or resume patrol.
    /// If none are reachable, stops safely and initiates periodic retries without logging every frame.
    /// </summary>
    public void AttemptStartOrResumePatrol()
    {
        int searchStartIndex = isRetrying ? currentWaypointIndex : 0;
        if (TryFindAndSetNextReachableWaypoint(searchStartIndex))
        {
            isRetrying = false;
            hasLoggedUnreachableWarning = false;
        }
        else
        {
            HandleNoReachableWaypoints();
        }
    }

    /// <summary>
    /// Advances along the circuit to the next reachable waypoint, skipping null and unreachable entries.
    /// </summary>
    public void AdvanceToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        int nextStartIndex = (currentWaypointIndex + 1) % waypoints.Length;
        if (TryFindAndSetNextReachableWaypoint(nextStartIndex))
        {
            isRetrying = false;
            hasLoggedUnreachableWarning = false;
        }
        else
        {
            HandleNoReachableWaypoints();
        }
    }

    /// <summary>
    /// Safely halts the agent and schedules periodic retries, logging a warning only once.
    /// </summary>
    private void HandleNoReachableWaypoints()
    {
        enemyMovement.Stop();
        isRetrying = true;
        retryTimer = retryInterval;

        if (!hasLoggedUnreachableWarning)
        {
            Debug.LogWarning($"[GuardPatrol] No reachable waypoints found for '{gameObject.name}'. Halting safely and retrying every {retryInterval}s.", this);
            hasLoggedUnreachableWarning = true;
        }
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
