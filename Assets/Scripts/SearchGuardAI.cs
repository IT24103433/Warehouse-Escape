using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Operational states for Sanjeevsaran's Search Guard.
/// </summary>
public enum SearchGuardState
{
    Patrol,
    Pursue,
    Search
}

/// <summary>
/// Sanjeevsaran's Search Guard AI.
/// Focuses on systematic patrol, active pursuit, and systematic multi-point investigation around the Last Known Position.
/// When line of sight is broken, systematically inspects several reachable candidate locations around the last known position,
/// spends a configurable search timeout inspecting the area, and abandons the search after the timeout elapses.
/// Stays strictly inside Room 3 (Search & Card A).
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
public class SearchGuardAI : MonoBehaviour, IGuardPerceptionTracker
{
    [Header("Current State")]
    [Tooltip("Current operational state.")]
    [SerializeField] private SearchGuardState currentState = SearchGuardState.Patrol;

    [Tooltip("Last confirmed coordinates where the intruder was seen.")]
    [SerializeField] private Vector3 lastSeenPosition;

    [Tooltip("Whether the intruder is currently within clear visual confirmation.")]
    [SerializeField] private bool canSeePlayer = false;

    [Header("Patrol Settings")]
    [Tooltip("Assigned waypoint circuit for regular patrol.")]
    [SerializeField] private Transform[] patrolWaypoints;

    [Tooltip("Distance threshold to consider a waypoint reached.")]
    [SerializeField] private float waypointArrivalThreshold = 0.6f;

    [Tooltip("Pause duration in seconds at each patrol waypoint.")]
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

    [Header("Pursuit & Search Settings")]
    [Tooltip("Interval in seconds between destination repaths during pursuit.")]
    [SerializeField] private float repathInterval = 0.25f;

    [Tooltip("Duration in seconds to search at the last seen position before giving up.")]
    [SerializeField] private float searchTimeout = 3.5f;

    [Tooltip("Arrival distance threshold for investigating last seen position.")]
    [SerializeField] private float searchArrivalThreshold = 1.0f;

    private EnemyMovement enemyMovement;
    private GuardPerceptionMemory perceptionMemory;
    private Transform playerTransform;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaitingAtWaypoint = false;

    private float repathTimer = 0f;
    private float searchTimer = 0f;

    // Systematic multi-location search inspection points
    private List<Vector3> searchCandidates = new List<Vector3>();
    private int currentSearchCandidateIndex = 0;
    private float candidateInspectTimer = 0f;
    private bool isWaitingAtCandidate = false;

    public SearchGuardState CurrentState => currentState;
    public bool CanSeePlayer => canSeePlayer;
    public Vector3 LastSeenPosition => lastSeenPosition;
    public Transform[] PatrolWaypoints { get => patrolWaypoints; set => patrolWaypoints = value; }
    public float SearchTimeout { get => searchTimeout; set => searchTimeout = value; }
    public void SetSearchTimeout(float timeout) => searchTimeout = timeout;

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
            health.CharacterName = "Sanjeevsaran [Search]";
            health.SetMaxHealth(60);
        }

        var shooting = GetComponent<GuardShooting>();
        if (shooting == null)
        {
            shooting = gameObject.AddComponent<GuardShooting>();
            shooting.ConfigureWeapon(WeaponType.AK47_Rifle, 18, 1.3f, 16f, 3, 2.4f, 1);
        }

        perceptionMemory = GetComponent<GuardPerceptionMemory>();
        if (perceptionMemory == null)
        {
            perceptionMemory = gameObject.AddComponent<GuardPerceptionMemory>();
        }

        if (GetComponent<EnemyHealthBar>() == null)
        {
            gameObject.AddComponent<EnemyHealthBar>();
        }

        HumanoidMeshBuilder.EnsureRuntimeVisuals(transform, CharacterRole.Search_Sanjeevsaran);
    }

    private static readonly Vector3[] SearchRoomNaturalCircuit = new Vector3[]
    {
        new Vector3(-10.0f, 1.08f,  2.0f), // South main aisle entry
        new Vector3(-19.5f, 1.08f,  2.5f), // Southwest pallet stack inspection
        new Vector3(-20.5f, 1.08f, 10.0f), // Card A desk security approach
        new Vector3(-15.0f, 1.08f, 14.5f), // North rack alcove crossing
        new Vector3(-10.0f, 1.08f, 13.0f), // East central passage
        new Vector3(-10.0f, 1.08f,  6.5f)  // Southeast aisle return
    };

    private void EnsureNaturalRoute()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length <= 4)
        {
            var wpParent = GameObject.Find("Waypoints_Sanjeevsaran");
            if (wpParent == null) wpParent = new GameObject("Waypoints_Sanjeevsaran");
            var list = new List<Transform>();

            for (int i = 0; i < SearchRoomNaturalCircuit.Length; i++)
            {
                string wpName = $"WP_Sanjeev_Natural_{i + 1}";
                Transform existing = wpParent.transform.Find(wpName);
                if (existing == null)
                {
                    GameObject go = new GameObject(wpName);
                    go.transform.SetParent(wpParent.transform);
                    go.transform.position = SearchRoomNaturalCircuit[i];
                    existing = go.transform;
                }
                else
                {
                    existing.position = SearchRoomNaturalCircuit[i];
                }
                list.Add(existing);
            }
            patrolWaypoints = list.ToArray();
        }
    }

    public bool HasConfirmedVisualContact() => canSeePlayer;
    public bool IsGuardActive() => isActiveAndEnabled && gameObject.activeInHierarchy;

    void OnEnable()
    {
        GuardPerception.RegisterTracker(this);
        if (perceptionMemory != null)
        {
            perceptionMemory.OnDisturbanceDetected += HandleDisturbance;
        }
    }

    void OnDisable()
    {
        GuardPerception.UnregisterTracker(this);
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

        canSeePlayer = GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask
        );

        switch (currentState)
        {
            case SearchGuardState.Patrol:
                UpdatePatrolState();
                break;

            case SearchGuardState.Pursue:
                UpdatePursueState();
                break;

            case SearchGuardState.Search:
                UpdateSearchState();
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
    [SerializeField] private Bounds roomBounds = new Bounds(new Vector3(-15.5f, 2.0f, 9.5f), new Vector3(15f, 6f, 19f));
    public Bounds RoomBounds { get => roomBounds; set => roomBounds = value; }

    public bool IsInsideRoom(Vector3 pos)
    {
        // Room 3 bounds (X: -23.0 to -6.5, Z: -1.0 to 18.5)
        return pos.x >= -23.5f && pos.x <= -6.0f && pos.z >= -1.5f && pos.z <= 19.0f;
    }

    private void HandleDisturbance(Vector3 disturbancePos)
    {
        if (IsInsideRoom(disturbancePos))
        {
            lastSeenPosition = disturbancePos;
            if (currentState == SearchGuardState.Patrol)
            {
                TransitionToSearch();
            }
        }
    }

    private void UpdatePatrolState()
    {
        if (canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position)))
        {
            lastSeenPosition = playerTransform.position;
            TransitionToPursue();
            return;
        }

        if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;

        if (isWaitingAtWaypoint)
        {
            waitTimer -= Time.deltaTime;
            // Scan during pause
            float sweepAngle = Mathf.Sin(Time.time * 2.0f) * 30f;
            Vector3 sweepDir = Quaternion.Euler(0f, sweepAngle, 0f) * transform.forward;
            enemyMovement.SetAimTarget(transform.position + sweepDir * 4.0f);

            if (waitTimer <= 0f)
            {
                isWaitingAtWaypoint = false;
                enemyMovement.ClearAimTarget();
                AdvanceWaypoint();
            }
            return;
        }

        if (enemyMovement.HasReachedDestination(waypointArrivalThreshold))
        {
            isWaitingAtWaypoint = true;
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
        if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;
        currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        DispatchCurrentWaypoint();
    }

    private void DispatchCurrentWaypoint()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;
        Transform wp = patrolWaypoints[currentWaypointIndex];
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
        currentState = SearchGuardState.Patrol;
        Debug.Log($"[{gameObject.name}] State -> Patrol (Resuming natural circuit in Room 3)");
        isWaitingAtWaypoint = false;
        enemyMovement.ClearAimTarget();
        if (perceptionMemory != null) perceptionMemory.ResetPerception();
        DispatchCurrentWaypoint();
    }

    public void TransitionToPursue()
    {
        currentState = SearchGuardState.Pursue;
        Debug.Log($"[{gameObject.name}] State -> Pursue (Engaging intruder in Room 3)");
        enemyMovement.ClearAimTarget();
        repathTimer = 0f;
        DispatchPursuitDestination();
    }

    private void UpdatePursueState()
    {
        if (canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position)) && IsInsideRoom(transform.position))
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

    private void DispatchPursuitDestination()
    {
        if (playerTransform == null) return;
        Vector3 target = playerTransform.position;
        target.x = Mathf.Clamp(target.x, -22.5f, -6.5f);
        target.z = Mathf.Clamp(target.z, -1.0f, 18.0f);
        enemyMovement.SetDestination(target);
        enemyMovement.Resume();
    }

    public void TransitionToSearch()
    {
        currentState = SearchGuardState.Search;
        Debug.Log($"[{gameObject.name}] State -> Search (Systematically inspecting reachable points around last seen: {lastSeenPosition:F1})");
        searchTimer = searchTimeout;
        if (perceptionMemory != null) perceptionMemory.SetSearchingState();

        GenerateSearchCandidates(lastSeenPosition);
        currentSearchCandidateIndex = 0;
        isWaitingAtCandidate = false;

        DispatchSearchCandidate(currentSearchCandidateIndex);
    }

    private void GenerateSearchCandidates(Vector3 center)
    {
        searchCandidates.Clear();

        // 1. Primary LKP
        Vector3 c0 = center;
        c0.x = Mathf.Clamp(c0.x, -22.5f, -6.5f);
        c0.z = Mathf.Clamp(c0.z, -1.0f, 18.0f);
        if (NavMesh.SamplePosition(c0, out NavMeshHit h0, 2.5f, NavMesh.AllAreas))
            searchCandidates.Add(h0.position);
        else
            searchCandidates.Add(c0);

        // 2. Flank search points around LKP (offset left and right by 3.5m)
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-3.5f, 0f, 1.5f),
            new Vector3( 3.5f, 0f, 1.5f),
            new Vector3( 0.0f, 0f, -3.0f)
        };

        foreach (var off in offsets)
        {
            Vector3 raw = center + off;
            raw.x = Mathf.Clamp(raw.x, -22.5f, -6.5f);
            raw.z = Mathf.Clamp(raw.z, -1.0f, 18.0f);

            if (NavMesh.SamplePosition(raw, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    searchCandidates.Add(hit.position);
                }
            }
        }
    }

    private void DispatchSearchCandidate(int index)
    {
        if (searchCandidates.Count == 0 || index >= searchCandidates.Count)
        {
            TransitionToPatrol();
            return;
        }

        Vector3 target = searchCandidates[index];
        enemyMovement.ClearAimTarget();
        enemyMovement.SetDestination(target);
        enemyMovement.Resume();
    }

    private void UpdateSearchState()
    {
        // Re-acquire line of sight -> immediately resume pursuit
        if (canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position)))
        {
            lastSeenPosition = playerTransform.position;
            TransitionToPursue();
            return;
        }

        // Search timeout check
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            Debug.Log($"[{gameObject.name}] Search timeout elapsed ({searchTimeout:F1}s). Abandoning search.");
            TransitionToPatrol();
            return;
        }

        if (isWaitingAtCandidate)
        {
            candidateInspectTimer -= Time.deltaTime;

            // Inspection sweep
            float sweepAngle = Mathf.Sin(Time.time * 3f) * 45f;
            Vector3 sweepDir = Quaternion.Euler(0f, sweepAngle, 0f) * transform.forward;
            enemyMovement.SetAimTarget(transform.position + sweepDir * 4.0f);

            if (candidateInspectTimer <= 0f)
            {
                isWaitingAtCandidate = false;
                enemyMovement.ClearAimTarget();
                currentSearchCandidateIndex++;

                if (currentSearchCandidateIndex < searchCandidates.Count)
                {
                    DispatchSearchCandidate(currentSearchCandidateIndex);
                }
                else
                {
                    // Inspected all candidates without finding target
                    TransitionToPatrol();
                }
            }
            return;
        }

        // Moving to current candidate
        if (enemyMovement.HasReachedDestination(searchArrivalThreshold))
        {
            isWaitingAtCandidate = true;
            candidateInspectTimer = 0.85f;
            enemyMovement.Stop();
        }
        else if (!enemyMovement.PathPending && (!enemyMovement.HasPath || enemyMovement.PathStatus == NavMeshPathStatus.PathInvalid))
        {
            // Path lost/blocked -> advance to next candidate
            currentSearchCandidateIndex++;
            DispatchSearchCandidate(currentSearchCandidateIndex);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePos, sightDistance);

        if (currentState == SearchGuardState.Search)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastSeenPosition, searchArrivalThreshold);
            Gizmos.DrawLine(eyePos, lastSeenPosition);

            Gizmos.color = Color.cyan;
            foreach (var sc in searchCandidates)
            {
                Gizmos.DrawWireSphere(sc, 0.4f);
            }
        }
    }
}
