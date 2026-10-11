using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Operational states for the Flanking Guard AI.
/// Stage 1 implements perception and state transitions between Patrol, Pursue, and Search.
/// </summary>
public enum GuardState
{
    Patrol,
    Pursue,
    Search
}

/// <summary>
/// First stage of Jadavan's Flanking Guard AI.
/// Handles perception (distance, FOV, line-of-sight occlusion by walls/shelves),
/// explicit Patrol -> Pursue -> Search -> Patrol state transitions,
/// periodic pursuit re-pathing, investigation of last seen position,
/// search timeouts, and safe unreachable destination handling.
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(GuardPatrol))]
public class FlankingGuardAI : MonoBehaviour
{
    [Header("Current State")]
    [Tooltip("Current operational state of the guard AI.")]
    [SerializeField] private GuardState currentState = GuardState.Patrol;

    [Tooltip("Last known position where the player was visually confirmed.")]
    [SerializeField] private Vector3 lastSeenPosition;

    [Tooltip("Whether the player is currently within sight range, horizontal FOV, and unoccluded line of sight.")]
    [SerializeField] private bool canSeePlayer = false;

    [Header("Perception Settings")]
    [Tooltip("Maximum sight distance in meters.")]
    [SerializeField] private float sightDistance = 12f;

    [Tooltip("Total horizontal field of view angle in degrees.")]
    [SerializeField] private float fieldOfView = 90f;

    [Tooltip("Height offset above guard position for eye/raycast origin.")]
    [SerializeField] private float eyeHeight = 0.6f;

    [Tooltip("Height offset above player position for chest/raycast target.")]
    [SerializeField] private float playerTargetHeight = 0.0f;

    [Tooltip("Layer mask for line of sight occlusion (walls, shelves, crates, obstacles).")]
    [SerializeField] private LayerMask obstacleMask = ~0;

    [Header("Pursuit Settings")]
    [Tooltip("Interval in seconds between destination updates during active pursuit.")]
    [SerializeField] private float repathInterval = 0.25f;

    [Header("Search Settings")]
    [Tooltip("Time in seconds to investigate/search at the last seen position before giving up.")]
    [SerializeField] private float searchTimeout = 3.0f;

    [Tooltip("Distance threshold to consider last seen position reached.")]
    [SerializeField] private float searchArrivalThreshold = 1.0f;

    [Header("Navigation Safety")]
    [Tooltip("Maximum distance to sample destinations on the NavMesh.")]
    [SerializeField] private float maxSampleDistance = 2.0f;

    private EnemyMovement enemyMovement;
    private GuardPatrol guardPatrol;
    private Transform playerTransform;

    private float repathTimer = 0f;
    private float searchTimer = 0f;
    private bool isInvestigating = false;
    private bool hasLoggedUnreachableWarning = false;

    // Public state properties for inspection, verification, and telemetry
    public GuardState CurrentState => currentState;
    public bool CanSeePlayer => canSeePlayer;
    public Vector3 LastSeenPosition => lastSeenPosition;
    public float SightDistance { get => sightDistance; set => sightDistance = value; }
    public float FieldOfView { get => fieldOfView; set => fieldOfView = value; }
    public float SearchTimeout { get => searchTimeout; set => searchTimeout = value; }
    public void SetSearchTimeout(float timeout)
    {
        searchTimeout = timeout;
        if (currentState == GuardState.Search && searchTimer > timeout)
        {
            searchTimer = timeout;
        }
    }
    public float RepathInterval { get => repathInterval; set => repathInterval = value; }
    public void SetRepathInterval(float interval) => repathInterval = interval;
    public float SearchTimer => searchTimer;
    public bool IsInvestigating => isInvestigating;
    public Transform PlayerTransform => playerTransform;

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        guardPatrol = GetComponent<GuardPatrol>();
    }

    void Start()
    {
        FindPlayerReference();

        // Start in Patrol state
        TransitionToPatrol();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayerReference();
        }

        // 1. Evaluate perception every frame
        canSeePlayer = EvaluateLineOfSight();

        // 2. Execute active state logic
        switch (currentState)
        {
            case GuardState.Patrol:
                UpdatePatrolState();
                break;

            case GuardState.Pursue:
                UpdatePursueState();
                break;

            case GuardState.Search:
                UpdateSearchState();
                break;
        }
    }

    /// <summary>
    /// Locates the player Transform via tag 'Player'.
    /// </summary>
    public void FindPlayerReference()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    /// <summary>
    /// Sets the player transform manually (useful for testing and deterministic setups).
    /// </summary>
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    /// <summary>
    /// Evaluates if the player is within sight distance, inside horizontal FOV,
    /// and has an unobstructed line of sight (not occluded by walls, shelves, crates).
    /// </summary>
    public bool EvaluateLineOfSight()
    {
        return GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask
        );
    }

    /// <summary>
    /// Patrol state update: GuardPatrol controls movement; guard watches for the player.
    /// </summary>
    private void UpdatePatrolState()
    {
        if (canSeePlayer)
        {
            lastSeenPosition = playerTransform.position;
            TransitionToPursue();
        }
    }

    /// <summary>
    /// Transitions from another state into Pursue.
    /// Exclusively grants movement authority to pursuit behavior by disabling GuardPatrol.
    /// </summary>
    public void TransitionToPursue()
    {
        currentState = GuardState.Pursue;

        // Ensure single movement authority: disable GuardPatrol
        guardPatrol.enabled = false;

        repathTimer = 0f; // Force immediate path calculation
        hasLoggedUnreachableWarning = false;

        DispatchPursuitDestination();
    }

    /// <summary>
    /// Pursue state update: updates destination at configured repathInterval while player is visible.
    /// If line of sight is lost, transitions to Search.
    /// </summary>
    private void UpdatePursueState()
    {
        if (canSeePlayer)
        {
            lastSeenPosition = playerTransform.position;

            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                repathTimer = repathInterval;
                DispatchPursuitDestination();
            }
        }
        else
        {
            TransitionToSearch();
        }
    }

    /// <summary>
    /// Dispatches movement towards current player position.
    /// </summary>
    private void DispatchPursuitDestination()
    {
        if (playerTransform == null) return;
        DispatchDestinationSafely(playerTransform.position);
    }

    /// <summary>
    /// Safely samples and validates paths before commanding agent movement.
    /// Handles unreachable destinations without crashing or throwing errors.
    /// </summary>
    public bool DispatchDestinationSafely(Vector3 targetPos)
    {
        if (!enemyMovement.IsOnNavMesh) return false;

        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = enemyMovement.Agent.agentTypeID,
            areaMask = enemyMovement.Agent.areaMask
        };

        // Sample target on NavMesh
        if (!NavMesh.SamplePosition(targetPos, out NavMeshHit hit, maxSampleDistance, filter))
        {
            if (!hasLoggedUnreachableWarning)
            {
                Debug.LogWarning($"[FlankingGuardAI] Target position {targetPos} is not on NavMesh.", this);
                hasLoggedUnreachableWarning = true;
            }
            return false;
        }

        // Verify complete path
        NavMeshPath path = new NavMeshPath();
        if (!enemyMovement.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete)
        {
            if (!hasLoggedUnreachableWarning)
            {
                Debug.LogWarning($"[FlankingGuardAI] No complete path exists to target position {hit.position}.", this);
                hasLoggedUnreachableWarning = true;
            }
            return false;
        }

        hasLoggedUnreachableWarning = false;
        bool setOk = enemyMovement.SetDestination(hit.position);
        if (setOk)
        {
            enemyMovement.Resume();
        }
        return setOk;
    }

    /// <summary>
    /// Transitions from Pursue into Search when player is lost from sight.
    /// Dispatches agent to investigate the last confirmed position.
    /// </summary>
    public void TransitionToSearch()
    {
        currentState = GuardState.Search;

        // Ensure single movement authority: GuardPatrol remains disabled
        guardPatrol.enabled = false;

        searchTimer = searchTimeout;
        isInvestigating = true;
        hasLoggedUnreachableWarning = false;

        bool dispatched = DispatchDestinationSafely(lastSeenPosition);
        if (!dispatched)
        {
            // If last seen position cannot be reached completely, stop safely and start countdown
            enemyMovement.Stop();
            isInvestigating = false;
        }
    }

    /// <summary>
    /// Search state update: moves to last seen position, then counts down search timeout.
    /// If player is spotted again, immediately resumes pursuit.
    /// If timeout expires, transitions back to Patrol.
    /// </summary>
    private void UpdateSearchState()
    {
        // 1. If player spotted again during search, return immediately to Pursue
        if (canSeePlayer)
        {
            lastSeenPosition = playerTransform.position;
            TransitionToPursue();
            return;
        }

        // 2. While en route to last seen position, check arrival
        if (isInvestigating)
        {
            if (enemyMovement.HasReachedDestination(searchArrivalThreshold))
            {
                enemyMovement.Stop();
                isInvestigating = false;
            }
            else if (!enemyMovement.PathPending && (!enemyMovement.HasPath || enemyMovement.PathStatus == NavMeshPathStatus.PathInvalid))
            {
                enemyMovement.Stop();
                isInvestigating = false;
            }
        }

        // 3. Once at last seen position, countdown search timeout
        if (!isInvestigating)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f)
            {
                TransitionToPatrol();
            }
        }
    }

    /// <summary>
    /// Transitions from Search back to Patrol.
    /// Restores movement authority to GuardPatrol and resumes waypoint navigation.
    /// </summary>
    public void TransitionToPatrol()
    {
        currentState = GuardState.Patrol;
        isInvestigating = false;
        hasLoggedUnreachableWarning = false;

        // Re-enable GuardPatrol and resume waypoint navigation
        guardPatrol.enabled = true;
        guardPatrol.ResumePatrol();
    }

    void OnDrawGizmosSelected()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;

        // State color coding
        Color stateColor = Color.green;
        switch (currentState)
        {
            case GuardState.Patrol:
                stateColor = Color.green;
                break;
            case GuardState.Pursue:
                stateColor = Color.red;
                break;
            case GuardState.Search:
                stateColor = Color.yellow;
                break;
        }

        // Eye sphere
        Gizmos.color = stateColor;
        Gizmos.DrawWireSphere(eyePos, 0.2f);

        // Sight distance wire sphere
        Gizmos.DrawWireSphere(eyePos, sightDistance);

        // Horizontal FOV boundary rays and center line
        Vector3 forward = transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude > 0.001f)
        {
            forward.Normalize();
            Quaternion leftRot = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);
            Quaternion rightRot = Quaternion.AngleAxis(fieldOfView * 0.5f, Vector3.up);

            Vector3 leftRay = leftRot * forward * sightDistance;
            Vector3 rightRay = rightRot * forward * sightDistance;

            Gizmos.color = stateColor;
            Gizmos.DrawRay(eyePos, leftRay);
            Gizmos.DrawRay(eyePos, rightRay);
            Gizmos.DrawRay(eyePos, forward * sightDistance);

            // Connect arc segments for FOV visualization
            int arcSegments = 20;
            Vector3 prevArcPoint = eyePos + leftRay;
            float stepAngle = fieldOfView / arcSegments;
            for (int i = 1; i <= arcSegments; i++)
            {
                float currentAngle = -fieldOfView * 0.5f + stepAngle * i;
                Quaternion rot = Quaternion.AngleAxis(currentAngle, Vector3.up);
                Vector3 currentArcPoint = eyePos + rot * forward * sightDistance;
                Gizmos.DrawLine(prevArcPoint, currentArcPoint);
                prevArcPoint = currentArcPoint;
            }
        }

        // Line to player when visible
        if (canSeePlayer && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePos, playerTransform.position + Vector3.up * playerTargetHeight);
        }

        // Last seen position marker when in Pursue or Search
        if (currentState == GuardState.Search || currentState == GuardState.Pursue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastSeenPosition, searchArrivalThreshold);
            Gizmos.DrawLine(eyePos, lastSeenPosition);
        }
    }
}
