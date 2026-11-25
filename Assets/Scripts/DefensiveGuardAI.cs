using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Operational states for Aqqil's Defensive Guard.
/// </summary>
public enum DefensiveGuardState
{
    GuardingPost,
    PursueIntruder,
    ReturnToPost
}

/// <summary>
/// Aqqil's Defensive Guard AI.
/// Assigned to protect the access-card terminal area.
/// Detects and pursues nearby intruders within a configurable defence radius,
/// then returns to its post when the player retreats beyond the defensive perimeter.
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
public class DefensiveGuardAI : MonoBehaviour, IGuardPerceptionTracker
{
    [Header("Current State")]
    [Tooltip("Current operational state.")]
    [SerializeField] private DefensiveGuardState currentState = DefensiveGuardState.GuardingPost;

    [Tooltip("Whether the intruder is currently within line of sight.")]
    [SerializeField] private bool canSeePlayer = false;

    [Tooltip("Whether the intruder is currently inside the guarded territory.")]
    [SerializeField] private bool isPlayerInsideTerritory = false;

    [Header("Defense Post Settings")]
    [Tooltip("Anchor post position guarded by this unit.")]
    [SerializeField] private Vector3 postPosition = new Vector3(-5.0f, 1.0f, 6.5f);

    [Tooltip("Desired facing direction when stationed at post.")]
    [SerializeField] private Vector3 postFacing = new Vector3(0f, 0f, -1f);

    [Tooltip("Radius around postPosition where intruders provoke defensive pursuit.")]
    [SerializeField] private float defenseRadius = 6.0f;

    [Tooltip("Maximum distance guard will travel from postPosition before breaking off pursuit.")]
    [SerializeField] private float leashDistance = 8.5f;

    [Tooltip("Distance threshold to consider post reached upon return.")]
    [SerializeField] private float postArrivalThreshold = 0.6f;

    [Header("Perception Settings")]
    [Tooltip("Maximum sight range in meters.")]
    [SerializeField] private float sightDistance = 10f;

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
    private GuardPerceptionMemory perceptionMemory;
    private Transform playerTransform;
    private float repathTimer = 0f;
    [SerializeField] private List<GuardDecisionCandidate> decisionCandidates = new List<GuardDecisionCandidate>();
    private readonly Dictionary<int, float> lastInspected = new Dictionary<int, float>();
    private Vector3 lastThreatPosition;
    private float threatTimestamp = -100;
    private float nextTacticalDecision;
    private int selectedTacticalIndex = -1;
    public IReadOnlyList<GuardDecisionCandidate> DecisionCandidates => decisionCandidates;
    public Vector3 SelectedDecisionTarget { get; private set; }
    public int DecisionCount { get; private set; }
    private bool HasThreat => Time.time - threatTimestamp < 7.5f;

    public DefensiveGuardState CurrentState => currentState;
    public bool CanSeePlayer => canSeePlayer;
    public bool IsPlayerInsideTerritory => isPlayerInsideTerritory;
    public Vector3 PostPosition { get => postPosition; set => postPosition = value; }
    public float DefenseRadius { get => defenseRadius; set => defenseRadius = value; }
    public float LeashDistance { get => leashDistance; set => leashDistance = value; }

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();

        var health = GetComponent<Health>();
        if (health == null)
        {
            health = gameObject.AddComponent<Health>();
            health.CharacterName = "Aqqil [Defensive]";
            health.SetMaxHealth(220);
        }

        var shooting = GetComponent<GuardShooting>();
        if (shooting == null)
        {
            shooting = gameObject.AddComponent<GuardShooting>();
            shooting.ConfigureWeapon(WeaponType.Combat_Shotgun, 12, 1.4f, 10f, 1, 3.8f, 6);
        }

        if (GetComponent<EnemyHealthBar>() == null)
        {
            gameObject.AddComponent<EnemyHealthBar>();
        }

        perceptionMemory = GetComponent<GuardPerceptionMemory>();
        if (perceptionMemory == null)
        {
            perceptionMemory = gameObject.AddComponent<GuardPerceptionMemory>();
        }

        CleanupRoom4PlaceholdersAndCapsule();
        EnsureHumanVisualAttached();
    }

    private void CleanupRoom4PlaceholdersAndCapsule()
    {
        // 1. Remove duplicate placeholder guard renderers in Room 4
        var room4 = GameObject.Find("Room_4_Defensive");
        if (room4 != null)
        {
            var placeholders = room4.transform.Find("[Aqqil_Placeholders]");
            if (placeholders != null)
            {
                var placeholderGuard = placeholders.Find("[Placeholder]_Guard_CharacterMesh");
                if (placeholderGuard != null)
                {
                    placeholderGuard.gameObject.SetActive(false);
                    if (Application.isPlaying) Destroy(placeholderGuard.gameObject);
                    else DestroyImmediate(placeholderGuard.gameObject);
                }
            }
        }

        var globalPlaceholder = GameObject.Find("[Placeholder]_Guard_CharacterMesh");
        if (globalPlaceholder != null)
        {
            globalPlaceholder.SetActive(false);
            if (Application.isPlaying) Destroy(globalPlaceholder);
            else DestroyImmediate(globalPlaceholder);
        }

        var mannequin = GameObject.Find("Mannequin_Body");
        if (mannequin != null)
        {
            mannequin.SetActive(false);
            if (Application.isPlaying) Destroy(mannequin);
            else DestroyImmediate(mannequin);
        }

        // 2. Hide / destroy original capsule MeshRenderer and MeshFilter on AI root while preserving
        // CapsuleCollider, NavMeshAgent, GuardCapture, EnemyMovement, and DefensiveGuardAI components
        var rootMr = GetComponent<MeshRenderer>();
        if (rootMr != null)
        {
            rootMr.enabled = false;
            if (Application.isPlaying) Destroy(rootMr);
            else DestroyImmediate(rootMr);
        }

        var rootMf = GetComponent<MeshFilter>();
        if (rootMf != null)
        {
            if (Application.isPlaying) Destroy(rootMf);
            else DestroyImmediate(rootMf);
        }
    }

    private void EnsureHumanVisualAttached()
    {
        HumanoidMeshBuilder.EnsureRuntimeVisuals(transform, CharacterRole.Defensive_Aqqil);
    }

    public bool HasConfirmedVisualContact() => canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position));
    public bool IsGuardActive() => isActiveAndEnabled && gameObject.activeInHierarchy;

    void OnEnable()
    {
        GuardPerception.RegisterTracker(this);
        if (perceptionMemory != null) perceptionMemory.OnDisturbanceDetected += HandleDisturbance;
    }

    void OnDisable()
    {
        GuardPerception.UnregisterTracker(this);
        if (perceptionMemory != null) perceptionMemory.OnDisturbanceDetected -= HandleDisturbance;
    }

    private Transform cachedEyeTransform;
    private Transform cachedPlayerCamera;
    private Collider cachedPlayerCollider;

    void Start()
    {
        FindPlayerReference();
        EnsureEyeTransform();
        if (postPosition == Vector3.zero) postPosition = transform.position;
        TransitionToGuardingPost();
    }

    void Update()
    {
        EnsureEyeTransform();
        if (playerTransform == null) FindPlayerReference();

        // 1. Evaluate perception and territory occupancy
        canSeePlayer = GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask,
            cachedEyeTransform, cachedPlayerCamera, cachedPlayerCollider
        );

        if (canSeePlayer && playerTransform != null && IsInsideRoom(playerTransform.position))
        {
            lastThreatPosition = playerTransform.position;
            threatTimestamp = Time.time;
        }
        float distPlayerToPost = HasThreat ? Vector3.Distance(lastThreatPosition, postPosition) : Mathf.Infinity;

        isPlayerInsideTerritory = distPlayerToPost <= defenseRadius;

        // 2. State Machine
        switch (currentState)
        {
            case DefensiveGuardState.GuardingPost:
                UpdateGuardingPostState(distPlayerToPost);
                break;

            case DefensiveGuardState.PursueIntruder:
                UpdatePursueIntruderState(distPlayerToPost);
                break;

            case DefensiveGuardState.ReturnToPost:
                UpdateReturnToPostState(distPlayerToPost);
                break;
        }
    }

    private void FindPlayerReference()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            var cam = playerObj.GetComponentInChildren<Camera>();
            if (cam != null) cachedPlayerCamera = cam.transform;
            cachedPlayerCollider = playerObj.GetComponent<CharacterController>();
            if (cachedPlayerCollider == null) cachedPlayerCollider = playerObj.GetComponent<Collider>();
        }
    }

    private void EnsureEyeTransform()
    {
        if (cachedEyeTransform != null) return;
        var anim = GetComponentInChildren<HumanoidAnimator>();
        if (anim != null && anim.head != null)
        {
            cachedEyeTransform = anim.head;
            return;
        }
        var headObj = transform.Find("Character_Visuals/Pelvis/Spine/Neck/Head");
        if (headObj != null) cachedEyeTransform = headObj;
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

    private float inspectionWaitTimer = 2.5f;
    private bool isWaitingAtInspection = true;
    private Vector3 baseInspectionForward = Vector3.back;

    private void UpdateGuardingPostState(float distPlayerToPost)
    {
        // If intruder enters territory, is seen, and is inside room, engage pursuit
        if (canSeePlayer && isPlayerInsideTerritory && (playerTransform == null || IsInsideRoom(playerTransform.position)))
        {
            TransitionToPursueIntruder();
            return;
        }

        if (isWaitingAtInspection)
        {
            inspectionWaitTimer -= Time.deltaTime;
            float sweepAngle = Mathf.Sin(Time.time * 1.8f) * 28f;
            Vector3 sweepDir = Quaternion.Euler(0f, sweepAngle, 0f) * baseInspectionForward;
            enemyMovement.SetAimTarget(transform.position + sweepDir * 4.0f);

            if (inspectionWaitTimer <= 0f)
            {
                isWaitingAtInspection = false;
                enemyMovement.ClearAimTarget();
                DispatchInspectionPoint();
            }
            return;
        }

        Vector3 remaining = SelectedDecisionTarget - transform.position;
        remaining.y = 0;
        if (DecisionCount > 0 && (enemyMovement.HasReachedDestination(postArrivalThreshold) || remaining.sqrMagnitude <= postArrivalThreshold * postArrivalThreshold))
        {
            if (selectedTacticalIndex >= 0) lastInspected[selectedTacticalIndex] = Time.time;
            isWaitingAtInspection = true;
            baseInspectionForward = HasThreat ? (lastThreatPosition - transform.position).normalized : postFacing;
            inspectionWaitTimer = 2.0f;
            enemyMovement.Stop();
            return;
        }

        if (!enemyMovement.PathPending && (!enemyMovement.HasPath || enemyMovement.PathStatus != NavMeshPathStatus.PathComplete))
        {
            isWaitingAtInspection = true;
            inspectionWaitTimer = 1.0f;
        }
    }

    private void HandleDisturbance(Vector3 position)
    {
        if (canSeePlayer || !IsInsideRoom(position) || Vector3.Distance(position, postPosition) > leashDistance) return;
        lastThreatPosition = position;
        threatTimestamp = Time.time;
        if (currentState == DefensiveGuardState.GuardingPost)
        {
            isWaitingAtInspection = false;
            DispatchInspectionPoint();
        }
    }

    private void DispatchInspectionPoint()
    {
        if (Time.time < nextTacticalDecision) return;
        nextTacticalDecision = Time.time + .75f;
        decisionCandidates.Clear();
        DecisionCount++;
        float best = float.NegativeInfinity;
        int selected = -1;
        Vector3 destination = postPosition;
        // Anchor plus two rings: changing threat and cover determine the chosen post.
        for (int i = 0; i < 17; i++)
        {
            Vector3 raw = postPosition;
            if (i > 0)
                raw += Quaternion.Euler(0, ((i - 1) % 8) * 45, 0) * Vector3.forward
                    * Mathf.Min(i <= 8 ? 2.5f : 5f, Mathf.Max(.5f, leashDistance - 1));
            var c = new GuardDecisionCandidate { position = raw, reason = "Outside room/leash or no complete path" };
            c.reachable = IsInsideRoom(raw)
                && GuardDecisionQueries.TryReachable(enemyMovement, raw, out c.position, out c.pathLength)
                && IsInsideRoom(c.position) && Vector3.Distance(c.position, postPosition) <= leashDistance;
            if (c.reachable)
            {
                c.objective = 1 - Mathf.Clamp01(Vector3.Distance(c.position, postPosition) / Mathf.Max(1, leashDistance));
                c.freshness = lastInspected.TryGetValue(i, out float time) ? Mathf.Clamp01((Time.time - time) / 20) : 1;
                c.visibility = GuardDecisionQueries.Visibility(c.position, obstacleMask, transform);
                if (HasThreat)
                {
                    float distance = Vector3.Distance(c.position, lastThreatPosition);
                    // Shotgun prefers ~5m; neither touching the target nor firing from across the room.
                    c.weaponRange = Mathf.Clamp01(1 - Mathf.Abs(distance - 5) / 6);
                    bool torsoClear = GuardDecisionQueries.Clear(c.position + Vector3.up * .8f,
                        lastThreatPosition + Vector3.up * .8f, obstacleMask, transform, playerTransform);
                    bool eyesClear = GuardDecisionQueries.Clear(c.position + Vector3.up * 1.5f,
                        lastThreatPosition + Vector3.up * 1.5f, obstacleMask, transform, playerTransform);
                    c.cover = !torsoClear ? (eyesClear ? 1f : .5f) : 0;
                    if (!eyesClear) c.weaponRange *= .25f;
                }
                c.score = DefenseDecisionPolicy.Score(c.objective, c.cover, c.weaponRange,
                    c.visibility, c.freshness, c.pathLength, HasThreat);
                // Small switching cost stabilises combat without a fixed waypoint circuit.
                if (HasThreat && i == selectedTacticalIndex) c.score += .04f;
                c.reason = HasThreat ? "Objective + cover + shotgun range - travel" : "Objective + visibility + neglect - travel";
                if (c.score > best) { best = c.score; selected = i; destination = c.position; }
            }
            decisionCandidates.Add(c);
        }
        if (selected < 0) { enemyMovement.Stop(); return; }
        selectedTacticalIndex = selected;
        SelectedDecisionTarget = destination;
        if (enemyMovement.SetDestination(destination)) enemyMovement.Resume();
    }

    public void TransitionToPursueIntruder()
    {
        currentState = DefensiveGuardState.PursueIntruder;
        Debug.Log($"[{gameObject.name}] State -> PursueIntruder (Intruder entered defensive perimeter)");
        isWaitingAtInspection = false;
        repathTimer = 0f;
        nextTacticalDecision = 0;
        DispatchPursuitDestination();
    }

    private void UpdatePursueIntruderState(float distPlayerToPost)
    {
        float distGuardToPost = Vector3.Distance(transform.position, postPosition);

        // Disengage condition:
        // 1. Intruder has retreated beyond leash distance
        // 2. Line of sight broken while intruder is outside core defence radius
        // 3. Either intruder or guard is outside assigned room boundary
        bool shouldDisengage = distPlayerToPost > leashDistance ||
                               distGuardToPost > leashDistance ||
                               (!canSeePlayer && !isPlayerInsideTerritory) ||
                               (playerTransform != null && !IsInsideRoom(playerTransform.position)) ||
                               !IsInsideRoom(transform.position);

        if (shouldDisengage)
        {
            TransitionToReturnToPost();
            return;
        }

        // Active pursuit repathing
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            DispatchPursuitDestination();
        }
    }

    private void DispatchPursuitDestination()
    {
        // Candidate generation uses only the last confirmed visual/sound observation.
        DispatchInspectionPoint();
    }

    public void TransitionToReturnToPost()
    {
        currentState = DefensiveGuardState.ReturnToPost;
        Debug.Log($"[{gameObject.name}] State -> ReturnToPost (Returning to anchor post at {postPosition})");
        if (perceptionMemory != null) perceptionMemory.SetReturningState();
        enemyMovement.SetDestination(postPosition);
        enemyMovement.Resume();
    }

    private void UpdateReturnToPostState(float distPlayerToPost)
    {
        // Re-engage if intruder steps back into defensive territory while visible
        if (canSeePlayer && isPlayerInsideTerritory)
        {
            TransitionToPursueIntruder();
            return;
        }

        // Arrival at defense post
        float distToPost = Vector3.Distance(transform.position, postPosition);
        if (distToPost <= postArrivalThreshold || enemyMovement.HasReachedDestination(postArrivalThreshold))
        {
            enemyMovement.Stop();
            if (postFacing.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(postFacing.normalized);
            }
            TransitionToGuardingPost();
        }
    }

    public void TransitionToGuardingPost()
    {
        currentState = DefensiveGuardState.GuardingPost;
        Debug.Log($"[{gameObject.name}] State -> GuardingPost (Stationed at defense post)");
        if (perceptionMemory != null) perceptionMemory.ResetPerception();
        enemyMovement.Stop();
    }

    void OnDrawGizmosSelected()
    {
        if (DecisionCount > 0)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(SelectedDecisionTarget, .35f);
            Gizmos.DrawLine(transform.position, SelectedDecisionTarget);
        }
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(eyePos, sightDistance);

        // Defense territorial boundary
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.8f);
        Gizmos.DrawWireSphere(postPosition, defenseRadius);

        // Leash boundary
        Gizmos.color = new Color(0f, 0.4f, 1f, 0.4f);
        Gizmos.DrawWireSphere(postPosition, leashDistance);

        // Post marker
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(postPosition, new Vector3(0.6f, 0.1f, 0.6f));
        Gizmos.DrawLine(transform.position, postPosition);
    }
}
