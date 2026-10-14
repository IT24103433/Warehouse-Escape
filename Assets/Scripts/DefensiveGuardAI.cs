using System;
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
public class DefensiveGuardAI : MonoBehaviour
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
    private Transform playerTransform;
    private float repathTimer = 0f;

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
            health.SetMaxHealth(80);
        }

        var shooting = GetComponent<GuardShooting>();
        if (shooting == null)
        {
            shooting = gameObject.AddComponent<GuardShooting>();
            shooting.ConfigureRole(GuardShooting.WeaponType.Rifle, 16, 1.2f, 9f, 1, 2.5f);
        }

        if (GetComponent<EnemyHealthBar>() == null)
        {
            gameObject.AddComponent<EnemyHealthBar>();
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

    void Start()
    {
        FindPlayerReference();
        if (postPosition == Vector3.zero) postPosition = transform.position;
        TransitionToGuardingPost();
    }

    void Update()
    {
        if (playerTransform == null) FindPlayerReference();

        // 1. Evaluate perception and territory occupancy
        canSeePlayer = GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask
        );

        float distPlayerToPost = playerTransform != null 
            ? Vector3.Distance(playerTransform.position, postPosition) 
            : Mathf.Infinity;

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

    private static readonly Vector3[] DefensiveLocalInspections = new Vector3[]
    {
        new Vector3(16.0f, 1.08f,  8.0f), // Anchor post facing south entrance
        new Vector3(18.5f, 1.08f,  8.5f), // East perimeter check
        new Vector3(16.0f, 1.08f,  8.0f), // Return to anchor post
        new Vector3(14.5f, 1.08f,  9.5f), // Terminal desk inspection
        new Vector3(16.0f, 1.08f,  8.0f)  // Return to anchor post
    };

    private int localInspectionIndex = 0;
    private float inspectionWaitTimer = 2.5f;
    private bool isWaitingAtInspection = true;

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
            if (inspectionWaitTimer <= 0f)
            {
                isWaitingAtInspection = false;
                localInspectionIndex = (localInspectionIndex + 1) % DefensiveLocalInspections.Length;
                DispatchInspectionPoint();
            }
            return;
        }

        if (enemyMovement.HasReachedDestination(postArrivalThreshold))
        {
            isWaitingAtInspection = true;
            inspectionWaitTimer = 2.0f;
            enemyMovement.Stop();
            return;
        }

        if (!enemyMovement.PathPending && (!enemyMovement.HasPath || enemyMovement.PathStatus == NavMeshPathStatus.PathInvalid))
        {
            isWaitingAtInspection = true;
            inspectionWaitTimer = 1.0f;
        }
    }

    private void DispatchInspectionPoint()
    {
        Vector3 targetPos = DefensiveLocalInspections[localInspectionIndex];
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

    public void TransitionToPursueIntruder()
    {
        currentState = DefensiveGuardState.PursueIntruder;
        Debug.Log($"[{gameObject.name}] State -> PursueIntruder (Intruder entered defensive perimeter)");
        isWaitingAtInspection = false;
        repathTimer = 0f;
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
        if (playerTransform == null) return;
        enemyMovement.SetDestination(playerTransform.position);
        enemyMovement.Resume();
    }

    public void TransitionToReturnToPost()
    {
        currentState = DefensiveGuardState.ReturnToPost;
        Debug.Log($"[{gameObject.name}] State -> ReturnToPost (Returning to anchor post at {postPosition})");
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
        enemyMovement.Stop();
    }

    void OnDrawGizmosSelected()
    {
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
