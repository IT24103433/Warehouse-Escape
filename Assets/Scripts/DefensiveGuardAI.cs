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

    private void UpdateGuardingPostState(float distPlayerToPost)
    {
        // If intruder enters territory and is seen, engage pursuit
        if (canSeePlayer && isPlayerInsideTerritory)
        {
            TransitionToPursueIntruder();
            return;
        }

        // If displaced from post, return
        float distToPost = Vector3.Distance(transform.position, postPosition);
        if (distToPost > postArrivalThreshold && !enemyMovement.HasPath)
        {
            enemyMovement.SetDestination(postPosition);
            enemyMovement.Resume();
        }
    }

    public void TransitionToPursueIntruder()
    {
        currentState = DefensiveGuardState.PursueIntruder;
        repathTimer = 0f;
        DispatchPursuitDestination();
    }

    private void UpdatePursueIntruderState(float distPlayerToPost)
    {
        float distGuardToPost = Vector3.Distance(transform.position, postPosition);

        // Disengage condition:
        // 1. Intruder has retreated beyond leash distance
        // 2. Line of sight broken while intruder is outside core defence radius
        bool shouldDisengage = distPlayerToPost > leashDistance ||
                               distGuardToPost > leashDistance ||
                               (!canSeePlayer && !isPlayerInsideTerritory);

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
