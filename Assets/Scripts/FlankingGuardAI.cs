using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Operational states for Jadavan's Flanking Guard AI.
/// Includes Patrol, Pursue (direct pursuit), Flank (tactical side/rear approach), and Search (investigate last seen).
/// </summary>
public enum GuardState
{
    Patrol,
    Pursue,
    Flank,
    Search
}

/// <summary>
/// Represents the chosen flanking side relative to the player's facing.
/// </summary>
public enum FlankSide
{
    None,
    Left,
    Right
}

/// <summary>
/// Evaluated candidate position for flanking maneuvers.
/// </summary>
[System.Serializable]
public class FlankCandidate
{
    public Vector3 rawPosition;
    public Vector3 navMeshPosition;
    public FlankSide side;
    public bool isSampled;
    public bool hasCompletePath;
    public float pathLength;
    public float approachAngle;
    public float divergenceAngle;
    public bool isEquivalentToDirect;
    public float score;
    [NonSerialized] public NavMeshPath path;
}

/// <summary>
/// Jadavan's Flanking Guard AI.
/// Senses the player via GuardPerception, evaluates alternative flanking approach vectors
/// through the warehouse's side aisles, avoids routes equivalent to direct pursuit,
/// falls back to direct pursuit when no viable flank exists, uses hysteresis to stabilize
/// side selection, and safely investigates last seen coordinates upon losing line of sight.
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(GuardPatrol))]
public class FlankingGuardAI : MonoBehaviour
{
    [Header("Current State")]
    [Tooltip("Current operational state of the guard AI.")]
    [SerializeField] private GuardState currentState = GuardState.Patrol;

    [Tooltip("Currently selected flank side relative to player facing.")]
    [SerializeField] private FlankSide currentFlankSide = FlankSide.None;

    [Tooltip("Current selected destination for flanking.")]
    [SerializeField] private Vector3 selectedFlankPosition;

    [Tooltip("Last known position where the player was visually confirmed.")]
    [SerializeField] private Vector3 lastSeenPosition;

    [Tooltip("Whether the player is currently within sight range, horizontal FOV, and unoccluded line of sight.")]
    [SerializeField] private bool canSeePlayer = false;

    [Header("Perception Settings")]
    [Tooltip("Maximum sight distance in meters.")]
    [SerializeField] private float sightDistance = 12f;

    [Tooltip("Total horizontal field of view angle in degrees.")]
    [SerializeField] private float fieldOfView = 90f;

    [Header("Speed Settings (Tuned for Boss Room)")]
    [Tooltip("Patrol movement speed.")]
    [SerializeField] private float patrolSpeed = 3.8f;

    [Tooltip("Tactical flanking / pursuit movement speed.")]
    [SerializeField] private float chaseSpeed = 5.2f;

    public float SightDistance { get => sightDistance; set => sightDistance = value; }
    public float FieldOfView { get => fieldOfView; set => fieldOfView = value; }
    public float PatrolSpeed { get => patrolSpeed; set => patrolSpeed = value; }
    public float ChaseSpeed { get => chaseSpeed; set => chaseSpeed = value; }

    [Tooltip("Height offset above guard position for eye/raycast origin.")]
    [SerializeField] private float eyeHeight = 0.6f;

    [Tooltip("Height offset above player position for chest/raycast target.")]
    [SerializeField] private float playerTargetHeight = 0.0f;

    [Tooltip("Layer mask for line of sight occlusion (walls, shelves, crates, obstacles).")]
    [SerializeField] private LayerMask obstacleMask = ~0;

    [Header("Flanking Geometry & Strategy")]
    [Tooltip("Lateral distance offsets (in meters) to player's left and right to test for flank positions.")]
    [SerializeField] private float[] lateralOffsets = new float[] { 3.5f, 5.5f, 7.5f };

    [Tooltip("Forward/backward offsets (in meters) along player's facing to test for flank positions.")]
    [SerializeField] private float[] forwardOffsets = new float[] { -1.5f, 0.0f, 1.5f };

    [Tooltip("Minimum distance to player below which flanking is abandoned in favor of direct pursuit.")]
    [SerializeField] private float minFlankPlayerDistance = 3.0f;

    [Tooltip("Minimum angular divergence in degrees between initial path direction and direct line to player to consider the route a non-direct flank.")]
    [SerializeField] private float minDivergenceAngle = 18f;

    [Tooltip("Maximum acceptable ratio of flank path length to direct path length before candidate is penalized.")]
    [SerializeField] private float maxFlankPathRatio = 2.5f;

    [Tooltip("Arrival distance threshold for the flank destination.")]
    [SerializeField] private float flankArrivalThreshold = 1.2f;

    [Header("Recalculation & Hysteresis")]
    [Tooltip("Interval in seconds between flank candidate re-evaluations while player is visible.")]
    [SerializeField] private float flankRecalcInterval = 0.5f;

    [Tooltip("Distance the player must move since last flank calculation to trigger an immediate recalculation.")]
    [SerializeField] private float playerMoveThreshold = 1.2f;

    [Tooltip("Rotation angle the player must turn since last flank calculation to trigger an immediate recalculation.")]
    [SerializeField] private float playerRotationThreshold = 35f;

    [Tooltip("Hysteresis bonus (in meters of effective path cost) favoring the currently chosen flank side to prevent jitter.")]
    [SerializeField] private float hysteresisBonus = 2.0f;

    [Header("Direct Pursuit Settings")]
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
    private float flankRecalcTimer = 0f;
    private float searchTimer = 0f;
    private float pathGraceTimer = 0f;
    private bool isInvestigating = false;
    private bool hasLoggedUnreachableWarning = false;

    private Vector3 lastPlannedPlayerPosition;
    private Vector3 lastPlannedPlayerForward;

    // Evaluated candidates cache for inspector/gizmo telemetry
    private List<FlankCandidate> currentCandidates = new List<FlankCandidate>();
    private NavMeshPath activeFlankPath = null;

    // Public state properties for inspection, verification, and telemetry
    public GuardState CurrentState => currentState;
    public FlankSide CurrentFlankSide => currentFlankSide;
    public Vector3 SelectedFlankPosition => selectedFlankPosition;
    public bool CanSeePlayer => canSeePlayer;
    public Vector3 LastSeenPosition => lastSeenPosition;
    public float SearchTimeout { get => searchTimeout; set => searchTimeout = value; }
    public void SetSearchTimeout(float timeout)
    {
        searchTimeout = timeout;
        if (currentState == GuardState.Search && searchTimer > timeout)
        {
            searchTimer = timeout;
        }
    }
    public float MinFlankPlayerDistance { get => minFlankPlayerDistance; set => minFlankPlayerDistance = value; }
    public float FlankRecalcInterval { get => flankRecalcInterval; set => flankRecalcInterval = value; }
    public float RepathInterval { get => repathInterval; set => repathInterval = value; }
    public void SetRepathInterval(float interval) => repathInterval = interval;
    public float SearchTimer => searchTimer;
    public bool IsInvestigating => isInvestigating;
    public Transform PlayerTransform => playerTransform;
    public List<FlankCandidate> CurrentCandidates => currentCandidates;
    public NavMeshPath ActiveFlankPath => activeFlankPath;
    public Vector3 LastPlannedPlayerPosition => lastPlannedPlayerPosition;
    public float[] LateralOffsets { get => lateralOffsets; set => lateralOffsets = value; }
    public float[] ForwardOffsets { get => forwardOffsets; set => forwardOffsets = value; }
    public float MinDivergenceAngle { get => minDivergenceAngle; set => minDivergenceAngle = value; }
    public float SideHysteresisBonus { get => hysteresisBonus; set => hysteresisBonus = value; }

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        guardPatrol = GetComponent<GuardPatrol>();
    }

    void Start()
    {
        FindPlayerReference();
        TransitionToPatrol();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayerReference();
        }

        // 1. Evaluate perception every frame using shared GuardPerception utility
        canSeePlayer = EvaluateLineOfSight();

        if (enemyMovement != null && enemyMovement.Agent != null)
        {
            float targetSpeed = (currentState == GuardState.Patrol || currentState == GuardState.Search) ? patrolSpeed : chaseSpeed;
            enemyMovement.Agent.speed = targetSpeed;
        }

        // 2. Execute active state logic
        switch (currentState)
        {
            case GuardState.Patrol:
                UpdatePatrolState();
                break;

            case GuardState.Flank:
                UpdateFlankState();
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

    /// <summary>
    /// Patrol state update: GuardPatrol controls movement; guard watches for the player.
    /// When player is spotted inside room, evaluates flanking opportunity.
    /// </summary>
    private void UpdatePatrolState()
    {
        if (canSeePlayer && playerTransform != null && IsInsideRoom(playerTransform.position))
        {
            lastSeenPosition = playerTransform.position;

            // Decide whether to initiate Flank or direct Pursue
            if (TrySelectFlankTarget(out Vector3 flankTarget, out FlankSide side, out NavMeshPath path))
            {
                TransitionToFlank(flankTarget, side, path);
            }
            else
            {
                TransitionToPursue();
            }
        }
    }

    /// <summary>
    /// Transitions from another state into Flank.
    /// Exclusively grants movement authority to flanking behavior by disabling GuardPatrol.
    /// </summary>
    public void TransitionToFlank(Vector3 targetPos, FlankSide side, NavMeshPath path)
    {
        currentState = GuardState.Flank;
        currentFlankSide = side;
        selectedFlankPosition = targetPos;
        activeFlankPath = path;

        // Ensure single movement authority: disable GuardPatrol
        if (guardPatrol != null) guardPatrol.enabled = false;

        flankRecalcTimer = flankRecalcInterval;
        hasLoggedUnreachableWarning = false;

        if (playerTransform != null)
        {
            lastPlannedPlayerPosition = playerTransform.position;
            lastPlannedPlayerForward = playerTransform.forward;
        }

        pathGraceTimer = 0.25f;
        bool setOk = enemyMovement.SetDestination(selectedFlankPosition);
        if (setOk)
        {
            enemyMovement.Resume();
        }
        else
        {
            // Destination assignment failed: fall back safely to direct pursuit without stalling
            TransitionToPursue();
        }
    }

    /// <summary>
    /// Flank state update: routes toward flank position using side aisles.
    /// Recalculates when player moves significantly or recalculation timer expires.
    /// Falls back to direct pursuit if player closes in or flank becomes unavailable.
    /// Transitions to search if player breaks line of sight.
    /// </summary>
    private void UpdateFlankState()
    {
        if (!canSeePlayer || (playerTransform != null && !IsInsideRoom(playerTransform.position)) || !IsInsideRoom(transform.position))
        {
            // Player lost from sight or outside assigned room: do NOT cheat by reading live position; investigate last seen!
            TransitionToSearch();
            return;
        }

        lastSeenPosition = playerTransform.position;
        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // If player is too close for a flanking detour, engage direct pursuit immediately
        if (distToPlayer <= minFlankPlayerDistance)
        {
            TransitionToPursue();
            return;
        }

        // Handle arrival at current flank position: update to subsequent flank waypoint or close in
        if (enemyMovement.HasReachedDestination(flankArrivalThreshold))
        {
            if (distToPlayer > minFlankPlayerDistance && TrySelectFlankTarget(out Vector3 nextTarget, out FlankSide nextSide, out NavMeshPath nextPath))
            {
                selectedFlankPosition = nextTarget;
                currentFlankSide = nextSide;
                activeFlankPath = nextPath;
                pathGraceTimer = 0.25f;
                lastPlannedPlayerPosition = playerTransform.position;
                lastPlannedPlayerForward = playerTransform.forward;
                bool setOk = enemyMovement.SetDestination(selectedFlankPosition);
                if (setOk)
                {
                    enemyMovement.Resume();
                }
                else
                {
                    TransitionToPursue();
                    return;
                }
            }
            else
            {
                TransitionToPursue();
                return;
            }
        }

        // Handle failed path or lost destination during flank with grace timer
        // Treat partial paths as failed flank routes alongside invalid or missing paths
        if (pathGraceTimer > 0f)
        {
            pathGraceTimer -= Time.deltaTime;
        }
        else if (!enemyMovement.PathPending && 
                 (!enemyMovement.HasPath || 
                  enemyMovement.PathStatus == NavMeshPathStatus.PathInvalid || 
                  enemyMovement.PathStatus == NavMeshPathStatus.PathPartial))
        {
            // Fall back safely to direct pursuit without stalling
            TransitionToPursue();
            return;
        }

        // Check if recalculation is needed
        flankRecalcTimer -= Time.deltaTime;
        float playerMovedDist = Vector3.Distance(playerTransform.position, lastPlannedPlayerPosition);
        float playerTurnedAngle = Vector3.Angle(playerTransform.forward, lastPlannedPlayerForward);

        bool shouldRecalculate = flankRecalcTimer <= 0f ||
                                 playerMovedDist >= playerMoveThreshold ||
                                 playerTurnedAngle >= playerRotationThreshold;

        if (shouldRecalculate)
        {
            flankRecalcTimer = flankRecalcInterval;

            if (TrySelectFlankTarget(out Vector3 newTarget, out FlankSide newSide, out NavMeshPath newPath))
            {
                selectedFlankPosition = newTarget;
                currentFlankSide = newSide;
                activeFlankPath = newPath;
                pathGraceTimer = 0.25f;
                lastPlannedPlayerPosition = playerTransform.position;
                lastPlannedPlayerForward = playerTransform.forward;

                bool setOk = enemyMovement.SetDestination(selectedFlankPosition);
                if (setOk)
                {
                    enemyMovement.Resume();
                }
                else
                {
                    TransitionToPursue();
                }
            }
            else
            {
                // No viable flank candidate remains; fallback to direct pursuit
                TransitionToPursue();
            }
        }
    }

    /// <summary>
    /// Transitions from another state into Pursue (direct pursuit).
    /// </summary>
    public void TransitionToPursue()
    {
        currentState = GuardState.Pursue;
        currentFlankSide = FlankSide.None;
        activeFlankPath = null;

        if (guardPatrol != null) guardPatrol.enabled = false;

        repathTimer = 0f; // Force immediate path calculation
        hasLoggedUnreachableWarning = false;

        DispatchPursuitDestination();
    }

    /// <summary>
    /// Pursue state update: updates destination at configured repathInterval while player is visible.
    /// Checks periodically if an alternative flank route has become viable.
    /// If line of sight is lost, transitions to Search.
    /// </summary>
    private void UpdatePursueState()
    {
        if (canSeePlayer && playerTransform != null && IsInsideRoom(playerTransform.position) && IsInsideRoom(transform.position))
        {
            lastSeenPosition = playerTransform.position;

            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                repathTimer = repathInterval;

                // Check if an alternative flanking path has become viable (and distance permits)
                float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distToPlayer > minFlankPlayerDistance &&
                    TrySelectFlankTarget(out Vector3 flankTarget, out FlankSide side, out NavMeshPath path))
                {
                    TransitionToFlank(flankTarget, side, path);
                    return;
                }

                DispatchPursuitDestination();
            }
        }
        else
        {
            TransitionToSearch();
        }
    }

    /// <summary>
    /// Dispatches movement directly towards current player position.
    /// </summary>
    private void DispatchPursuitDestination()
    {
        if (playerTransform == null) return;
        Vector3 target = playerTransform.position;
        if (roomBounds.size != Vector3.zero)
        {
            target.x = Mathf.Clamp(target.x, roomBounds.min.x + 0.5f, roomBounds.max.x - 0.5f);
            target.z = Mathf.Clamp(target.z, roomBounds.min.z + 0.5f, roomBounds.max.z - 0.5f);
        }
        DispatchDestinationSafely(target);
    }

    /// <summary>
    /// Evaluates left and right candidate positions relative to the player's horizontal facing,
    /// samples on NavMesh using agent settings, verifies complete paths, rejects routes equivalent
    /// to direct pursuit, applies hysteresis, and selects the optimal flanking route.
    /// Returns true if a valid flank was selected; false if direct pursuit should be used.
    /// </summary>
    public bool TrySelectFlankTarget(out Vector3 bestTarget, out FlankSide bestSide, out NavMeshPath bestPath)
    {
        bestTarget = Vector3.zero;
        bestSide = FlankSide.None;
        bestPath = null;
        currentCandidates.Clear();

        if (playerTransform == null || !enemyMovement.IsOnNavMesh || !IsInsideRoom(playerTransform.position)) return false;

        Vector3 playerPos = playerTransform.position;
        Vector3 guardPos = transform.position;

        float distToPlayer = Vector3.Distance(guardPos, playerPos);
        if (distToPlayer <= minFlankPlayerDistance)
        {
            return false; // Player too close for detour
        }

        // Horizontal player forward and right vectors
        Vector3 playerForward = playerTransform.forward;
        playerForward.y = 0f;
        if (playerForward.sqrMagnitude < 0.001f) playerForward = Vector3.forward;
        playerForward.Normalize();

        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward).normalized;

        // Direct line from guard to player
        Vector3 directVector = playerPos - guardPos;
        directVector.y = 0f;
        Vector3 directDir = directVector.normalized;

        // Calculate direct path for distance and geometry baseline
        NavMeshPath directPath = new NavMeshPath();
        bool hasDirectPath = enemyMovement.CalculatePath(playerPos, directPath);
        float directPathLength = hasDirectPath && directPath.status == NavMeshPathStatus.PathComplete
            ? CalculatePathLength(directPath)
            : distToPlayer;

        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = enemyMovement.Agent.agentTypeID,
            areaMask = enemyMovement.Agent.areaMask
        };

        // 1. Generate Candidates to Left and Right
        foreach (float lateral in lateralOffsets)
        {
            foreach (float forward in forwardOffsets)
            {
                // Left Candidate
                Vector3 rawLeft = playerPos - playerRight * lateral + playerForward * forward;
                currentCandidates.Add(new FlankCandidate
                {
                    rawPosition = rawLeft,
                    side = FlankSide.Left
                });

                // Right Candidate
                Vector3 rawRight = playerPos + playerRight * lateral + playerForward * forward;
                currentCandidates.Add(new FlankCandidate
                {
                    rawPosition = rawRight,
                    side = FlankSide.Right
                });
            }
        }

        // 2. Sample, Path, Filter, and Score Candidates
        FlankCandidate selectedCandidate = null;
        float bestScore = Mathf.Infinity;

        foreach (var c in currentCandidates)
        {
            // Reject candidates outside assigned encounter room
            if (!IsInsideRoom(c.rawPosition))
            {
                c.isSampled = false;
                continue;
            }

            // Sample on NavMesh
            if (!NavMesh.SamplePosition(c.rawPosition, out NavMeshHit hit, maxSampleDistance, filter))
            {
                c.isSampled = false;
                continue;
            }

            // Also verify sampled NavMesh position is within room
            if (!IsInsideRoom(hit.position))
            {
                c.isSampled = false;
                continue;
            }

            c.isSampled = true;
            c.navMeshPosition = hit.position;

            // Calculate complete path using guard's agent parameters
            NavMeshPath path = new NavMeshPath();
            if (!enemyMovement.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete)
            {
                c.hasCompletePath = false;
                continue;
            }

            c.hasCompletePath = true;
            c.path = path;
            c.pathLength = CalculatePathLength(path);

            // Path ratio filter: reject absurdly long detours
            if (c.pathLength > directPathLength * maxFlankPathRatio)
            {
                continue;
            }

            // Calculate initial path divergence angle
            if (path.corners.Length >= 2)
            {
                Vector3 initialPathDir = path.corners[1] - guardPos;
                initialPathDir.y = 0f;
                c.divergenceAngle = Vector3.Angle(directDir, initialPathDir.normalized);
            }
            else
            {
                c.divergenceAngle = 0f;
            }

            // Calculate approach angle relative to player facing
            Vector3 approachVector = hit.position - playerPos;
            approachVector.y = 0f;
            c.approachAngle = Vector3.Angle(playerForward, approachVector.normalized);

            // Avoid routes equivalent to direct pursuit:
            // Route is considered equivalent if initial path diverges very little AND candidate lies along the direct line of sight
            Vector3 guardToCandidate = hit.position - guardPos;
            guardToCandidate.y = 0f;
            float lineAngle = Vector3.Angle(directDir, guardToCandidate.normalized);

            if (c.divergenceAngle < minDivergenceAngle && lineAngle < 15f)
            {
                c.isEquivalentToDirect = true;
                continue; // Reject route equivalent to direct pursuit
            }
            c.isEquivalentToDirect = false;

            // 3. Compute score: lower score is better
            // Primary factors: path length, approach angle penalty (prefer 70-110 deg lateral flank), divergence reward
            float angleDeviationFrom90 = Mathf.Abs(c.approachAngle - 90f);
            float score = c.pathLength + (angleDeviationFrom90 * 0.05f);

            // Hysteresis: apply bonus (reduced cost) to currently chosen flank side to prevent jitter
            if (currentFlankSide != FlankSide.None && c.side == currentFlankSide)
            {
                score -= hysteresisBonus;
            }

            c.score = score;

            if (score < bestScore)
            {
                bestScore = score;
                selectedCandidate = c;
            }
        }

        if (selectedCandidate != null)
        {
            bestTarget = selectedCandidate.navMeshPosition;
            bestSide = selectedCandidate.side;
            bestPath = selectedCandidate.path;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Computes the total Euclidean distance along a series of NavMeshPath corners.
    /// </summary>
    public static float CalculatePathLength(NavMeshPath path)
    {
        if (path == null || path.corners == null || path.corners.Length < 2) return 0f;
        float length = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
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

        if (!NavMesh.SamplePosition(targetPos, out NavMeshHit hit, maxSampleDistance, filter))
        {
            if (!hasLoggedUnreachableWarning)
            {
                Debug.LogWarning($"[FlankingGuardAI] Target position {targetPos} is not on NavMesh.", this);
                hasLoggedUnreachableWarning = true;
            }
            return false;
        }

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
    /// Transitions into Search when player is lost from sight.
    /// Dispatches agent to investigate the last confirmed position.
    /// Does NOT read the hidden player's live position.
    /// </summary>
    public void TransitionToSearch()
    {
        currentState = GuardState.Search;
        currentFlankSide = FlankSide.None;
        activeFlankPath = null;

        if (guardPatrol != null) guardPatrol.enabled = false;

        searchTimer = searchTimeout;
        isInvestigating = true;
        hasLoggedUnreachableWarning = false;

        Vector3 searchTarget = lastSeenPosition;
        if (roomBounds.size != Vector3.zero)
        {
            searchTarget.x = Mathf.Clamp(searchTarget.x, roomBounds.min.x + 0.5f, roomBounds.max.x - 0.5f);
            searchTarget.z = Mathf.Clamp(searchTarget.z, roomBounds.min.z + 0.5f, roomBounds.max.z - 0.5f);
        }

        bool dispatched = DispatchDestinationSafely(searchTarget);
        if (!dispatched)
        {
            enemyMovement.Stop();
            isInvestigating = false;
        }
    }

    /// <summary>
    /// Search state update: moves to last seen position, then counts down search timeout.
    /// If player is spotted again, evaluates flank or pursuit.
    /// If timeout expires, transitions back to Patrol.
    /// </summary>
    private void UpdateSearchState()
    {
        // 1. If player spotted again during search, transition to Flank or Pursue
        if (canSeePlayer && playerTransform != null)
        {
            lastSeenPosition = playerTransform.position;
            if (TrySelectFlankTarget(out Vector3 flankTarget, out FlankSide side, out NavMeshPath path))
            {
                TransitionToFlank(flankTarget, side, path);
            }
            else
            {
                TransitionToPursue();
            }
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
        currentFlankSide = FlankSide.None;
        activeFlankPath = null;
        isInvestigating = false;
        hasLoggedUnreachableWarning = false;

        if (guardPatrol != null)
        {
            guardPatrol.enabled = true;
            guardPatrol.ResumePatrol();
        }
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
            case GuardState.Flank:
                stateColor = Color.cyan;
                break;
            case GuardState.Pursue:
                stateColor = Color.red;
                break;
            case GuardState.Search:
                stateColor = Color.yellow;
                break;
        }

        // Eye sphere & Sight distance
        Gizmos.color = stateColor;
        Gizmos.DrawWireSphere(eyePos, 0.2f);
        Gizmos.DrawWireSphere(eyePos, sightDistance);

        // Horizontal FOV boundary rays and arc
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

        // Draw Candidate Positions
        if (currentCandidates != null && currentCandidates.Count > 0)
        {
            foreach (var c in currentCandidates)
            {
                if (!c.isSampled || !c.hasCompletePath)
                {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.4f); // Red for unreachable
                    Gizmos.DrawWireSphere(c.rawPosition, 0.2f);
                }
                else if (c.isEquivalentToDirect)
                {
                    Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange for direct duplicate
                    Gizmos.DrawWireCube(c.navMeshPosition, new Vector3(0.3f, 0.3f, 0.3f));
                }
                else if (c.side == FlankSide.Left)
                {
                    Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.8f); // Blue for left flank candidates
                    Gizmos.DrawWireSphere(c.navMeshPosition, 0.3f);
                }
                else
                {
                    Gizmos.color = new Color(0.9f, 0.3f, 0.9f, 0.8f); // Magenta for right flank candidates
                    Gizmos.DrawWireSphere(c.navMeshPosition, 0.3f);
                }
            }
        }

        // Draw Selected Flank Target and Route
        if (currentState == GuardState.Flank && selectedFlankPosition != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(selectedFlankPosition + Vector3.up * 0.2f, 0.45f);

            // Draw route corners
            if (activeFlankPath != null && activeFlankPath.corners != null && activeFlankPath.corners.Length > 1)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < activeFlankPath.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(activeFlankPath.corners[i], activeFlankPath.corners[i + 1]);
                    Gizmos.DrawWireCube(activeFlankPath.corners[i] + Vector3.up * 0.1f, new Vector3(0.2f, 0.2f, 0.2f));
                }
            }
        }

        // Last seen position marker when in Search or Pursue
        if (currentState == GuardState.Search || currentState == GuardState.Pursue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastSeenPosition, searchArrivalThreshold);
            Gizmos.DrawLine(eyePos, lastSeenPosition);
        }
    }
}
