using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Operational states for Varun's Patrol Guard.
/// </summary>
public enum PatrolGuardState
{
    Patrol,
    Pursue,
    Investigate
}

/// <summary>
/// Varun's Patrol Guard AI.
/// Continuously inspects varied reachable aisles and corners in Room 2 (Patrol Hall).
/// Pauses at inspection points to actively scan surroundings with smooth head/body sweeps.
/// Senses audio and visual disturbances, investigates suspicious locations within Room 2,
/// and resumes patrol circuit when clear. Stays strictly inside Room 2 (Safe Room protection).
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

    [Header("Pursuit & Investigation Settings")]
    [Tooltip("Interval in seconds between destination repaths during pursuit.")]
    [SerializeField] private float repathInterval = 0.25f;

    [Tooltip("Duration in seconds spent investigating a disturbance point.")]
    [SerializeField] private float investigateDuration = 2.5f;

    private EnemyMovement enemyMovement;
    private GuardPerceptionMemory perceptionMemory;
    private Transform playerTransform;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private float repathTimer = 0f;
    private Vector3 investigatePosition;
    private float investigateTimer = 0f;
    private bool isInvestigatingArrival = false;

    public PatrolGuardState CurrentState => currentState;
    public bool CanSeePlayer => canSeePlayer;
    public Vector3 InvestigatePosition => investigatePosition;
    public Vector3 LastSeenPosition => lastSeenPosition;
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
        }
        shooting.ConfigureWeapon(WeaponType.UMP_SMG, 14, 1.1f, 13f, 2, 2.5f);

        perceptionMemory = GetComponent<GuardPerceptionMemory>();
        if (perceptionMemory == null)
        {
            perceptionMemory = gameObject.AddComponent<GuardPerceptionMemory>();
        }
        perceptionMemory.VisualOnlyDetection = true;

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
            var list = new List<Transform>();

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

    void OnEnable()
    {
        if (perceptionMemory != null)
        {
            perceptionMemory.OnDisturbanceDetected += HandleDisturbance;
        }
    }

    void OnDisable()
    {
        if (perceptionMemory != null)
        {
            perceptionMemory.OnDisturbanceDetected -= HandleDisturbance;
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
        if (playerTransform == null)
        {
            FindPlayerReference();
        }

        // Evaluate visual line of sight
        canSeePlayer = GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask
        );

        switch (currentState)
        {
            case PatrolGuardState.Patrol:
                UpdatePatrolState();
                break;

            case PatrolGuardState.Pursue:
                UpdatePursueState();
                break;

            case PatrolGuardState.Investigate:
                UpdateInvestigateState();
                break;
        }
    }

    private void FindPlayerReference()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    public void SetPlayer(Transform player) => playerTransform = player;

    [Header("Room Boundaries")]
    [Tooltip("Encounter room boundaries. Guard stays within this area and disengages if player leaves the room.")]
    [SerializeField] private Bounds roomBounds = new Bounds(new Vector3(0f, 2.0f, -10.5f), new Vector3(18f, 6f, 15f));
    public Bounds RoomBounds { get => roomBounds; set => roomBounds = value; }

    public bool IsInsideRoom(Vector3 pos)
    {
        // Room 2 bounds (Z: -18.0 to -2.0, X: -9.0 to +9.0)
        return pos.z >= -17.8f && pos.z <= -2.0f && pos.x >= -9.0f && pos.x <= 9.0f;
    }

    private Vector3 lastSeenPosition;

    private void HandleDisturbance(Vector3 disturbancePos)
    {
        // First guard (Varun) is visual detection only!
        // Footsteps, gunshots, and radio alerts do NOT independently reveal the player or trigger pursuit/investigation.
    }

    private void UpdatePatrolState()
    {
        // If player spotted inside forward FOV and unobstructed LOS inside Room 2, transition to pursue
        if (canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position)))
        {
            if (playerTransform != null) lastSeenPosition = playerTransform.position;
            TransitionToPursue();
            return;
        }

        if (waypoints == null || waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            // Perform inspection scanning (sweep facing direction left & right)
            float sweepAngle = Mathf.Sin(Time.time * 2.2f) * 35f;
            Vector3 sweepDir = Quaternion.Euler(0f, sweepAngle, 0f) * transform.forward;
            enemyMovement.SetAimTarget(transform.position + sweepDir * 4.0f);

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                enemyMovement.ClearAimTarget();
                AdvanceWaypoint();
            }
            return;
        }

        if (enemyMovement.HasReachedDestination(arrivalThreshold))
        {
            isWaiting = true;
            // Inspection points (odd indices) pause 2.0s with scanning
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
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                enemyMovement.SetDestination(hit.position);
                enemyMovement.Resume();
            }
            else
            {
                enemyMovement.SetDestination(targetPos);
                enemyMovement.Resume();
            }
        }
    }

    public void TransitionToPatrol()
    {
        currentState = PatrolGuardState.Patrol;
        Debug.Log($"[{gameObject.name}] State -> Patrol");
        isWaiting = false;
        enemyMovement.ClearAimTarget();
        if (perceptionMemory != null) perceptionMemory.ResetPerception();
        DispatchCurrentWaypoint();
    }

    public void TransitionToPursue()
    {
        currentState = PatrolGuardState.Pursue;
        Debug.Log($"[{gameObject.name}] State -> Pursue (Intruder detected in Room 2)");
        enemyMovement.ClearAimTarget();
        repathTimer = 0f;
        if (playerTransform != null) lastSeenPosition = playerTransform.position;
        DispatchPursuitDestination();
    }

    private void UpdatePursueState()
    {
        // Once sight is lost or player/guard leaves Room 2, investigate ONLY the last seen position,
        // then return to patrol. Do NOT track hidden live player coordinates!
        if (!canSeePlayer || (playerTransform != null && !IsInsideRoom(playerTransform.position)) || !IsInsideRoom(transform.position))
        {
            Debug.Log($"[{gameObject.name}] Sight lost or player left Room 2 -> Investigating last seen position at {lastSeenPosition:F1}");
            TransitionToInvestigate(lastSeenPosition);
            return;
        }

        if (playerTransform != null)
        {
            lastSeenPosition = playerTransform.position;
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

    public void TransitionToInvestigate(Vector3 disturbancePos)
    {
        currentState = PatrolGuardState.Investigate;
        investigatePosition = disturbancePos;
        investigateTimer = investigateDuration;
        isInvestigatingArrival = false;
        Debug.Log($"[{gameObject.name}] State -> Investigate disturbance at {investigatePosition:F1}");

        if (NavMesh.SamplePosition(investigatePosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            enemyMovement.SetDestination(hit.position);
        }
        else
        {
            enemyMovement.SetDestination(investigatePosition);
        }
        enemyMovement.Resume();
    }

    private void UpdateInvestigateState()
    {
        if (canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position)))
        {
            TransitionToPursue();
            return;
        }

        if (!isInvestigatingArrival)
        {
            if (enemyMovement.HasReachedDestination(arrivalThreshold + 0.4f))
            {
                isInvestigatingArrival = true;
                enemyMovement.Stop();
            }
        }
        else
        {
            // Scanning disturbance area
            investigateTimer -= Time.deltaTime;
            float sweepAngle = Mathf.Sin(Time.time * 2.5f) * 45f;
            Vector3 sweepDir = Quaternion.Euler(0f, sweepAngle, 0f) * transform.forward;
            enemyMovement.SetAimTarget(transform.position + sweepDir * 4.0f);

            if (investigateTimer <= 0f)
            {
                enemyMovement.ClearAimTarget();
                TransitionToPatrol();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(eyePos, sightDistance);

        if (waypoints == null) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, arrivalThreshold);
            }
        }
    }
}
