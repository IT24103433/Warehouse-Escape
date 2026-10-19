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
    private readonly List<float> searchBeliefs = new List<float>();
    private readonly List<bool> inspectedCandidates = new List<bool>();
    [SerializeField] private List<GuardDecisionCandidate> decisionCandidates = new List<GuardDecisionCandidate>();
    private Vector3 observedHeading, previousObservation;
    private bool hasPreviousObservation;
    private float nextSoundDecisionAt;
    public IReadOnlyList<GuardDecisionCandidate> DecisionCandidates => decisionCandidates;
    public Vector3 SelectedDecisionTarget { get; private set; }
    public int DecisionCount { get; private set; }
    public float RemainingBeliefMass { get; private set; } = 1;
    private Vector3 baseInspectionForward = Vector3.forward;

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

    public bool HasConfirmedVisualContact() => canSeePlayer && (playerTransform == null || IsInsideRoom(playerTransform.position));
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

    private Transform cachedEyeTransform;
    private Transform cachedPlayerCamera;
    private Collider cachedPlayerCollider;

    void Start()
    {
        FindPlayerReference();
        EnsureEyeTransform();
        EnsureNaturalRoute();
        TransitionToPatrol();
    }

    void Update()
    {
        EnsureEyeTransform();
        if (playerTransform == null)
        {
            FindPlayerReference();
        }

        canSeePlayer = GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask,
            cachedEyeTransform, cachedPlayerCamera, cachedPlayerCollider
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
    [SerializeField] private Bounds roomBounds = new Bounds(new Vector3(-15.5f, 2.0f, 9.5f), new Vector3(15f, 6f, 19f));
    public Bounds RoomBounds { get => roomBounds; set => roomBounds = value; }

    public bool IsInsideRoom(Vector3 pos)
    {
        // Room 3 bounds (X: -23.0 to -6.5, Z: -1.0 to 18.5)
        return pos.x >= -23.5f && pos.x <= -6.0f && pos.z >= -1.5f && pos.z <= 19.0f;
    }

    private void HandleDisturbance(Vector3 disturbancePos)
    {
        if (canSeePlayer) return;
        if (IsInsideRoom(disturbancePos))
        {
            // Automatic fire must not rebuild the entire belief search for every bullet.
            if (Time.time < nextSoundDecisionAt) return;
            nextSoundDecisionAt = Time.time + .75f;
            lastSeenPosition = disturbancePos;
            observedHeading = Vector3.zero;
            hasPreviousObservation = false;
            if (currentState != SearchGuardState.Pursue)
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
            // Scan during pause relative to arrival facing
            float sweepAngle = Mathf.Sin(Time.time * 2.0f) * 30f;
            Vector3 sweepDir = Quaternion.Euler(0f, sweepAngle, 0f) * baseInspectionForward;
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
            baseInspectionForward = transform.forward;
            waitTimer = (currentWaypointIndex % 2 == 1) ? 2.0f : waypointWaitTime;
            enemyMovement.Stop();
            return;
        }

        if (!enemyMovement.PathPending && (!enemyMovement.HasPath || enemyMovement.PathStatus != NavMeshPathStatus.PathComplete))
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
        hasPreviousObservation = false;
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
            if (hasPreviousObservation)
            {
                var displacement = playerTransform.position - previousObservation;
                displacement.y = 0;
                if (displacement.sqrMagnitude > .0001f) observedHeading = displacement.normalized;
            }
            previousObservation = playerTransform.position;
            hasPreviousObservation = true;
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
        searchBeliefs.Clear();
        inspectedCandidates.Clear();
        RemainingBeliefMass = 1;
        center.x = Mathf.Clamp(center.x, -22.5f, -6.5f);
        center.z = Mathf.Clamp(center.z, -1f, 18f);
        for (int i = 0; i < 17; i++)
        {
            Vector3 raw = center;
            if (i > 0) raw += Quaternion.Euler(0, ((i - 1) % 8) * 45, 0) * Vector3.forward * (i <= 8 ? 2.5f : 5f);
            if (!IsInsideRoom(raw)) continue;
            if (!GuardDecisionQueries.TryReachable(enemyMovement, raw, out var point, out _) || !IsInsideRoom(point)) continue;
            bool duplicate = false;
            foreach (var previous in searchCandidates)
                if (Vector3.Distance(previous, point) < .75f) { duplicate = true; break; }
            if (duplicate) continue;
            searchCandidates.Add(point);
            searchBeliefs.Add(SearchDecisionPolicy.Prior(point - center, observedHeading));
            inspectedCandidates.Add(false);
        }
        float total = 0;
        foreach (var value in searchBeliefs) total += value;
        for (int i = 0; i < searchBeliefs.Count; i++) searchBeliefs[i] /= Mathf.Max(.0001f, total);
    }

    private bool ObservableFrom(Vector3 from, Vector3 point)
    {
        var direction = point - from;
        if (direction.magnitude > 6f) return false;
        var facing = lastSeenPosition - from;
        facing.y = 0;
        if (facing.sqrMagnitude < .01f) facing = observedHeading.sqrMagnitude > .01f ? observedHeading : transform.forward;
        return Vector3.Angle(facing, direction) <= fieldOfView * .5f + 45f
            && GuardDecisionQueries.Clear(from + Vector3.up * 1.5f, point + Vector3.up * 1.5f, obstacleMask, transform, playerTransform);
    }

    private void DispatchSearchCandidate(int unusedIndex)
    {
        decisionCandidates.Clear();
        DecisionCount++;
        int selected = -1;
        float best = float.NegativeInfinity;
        for (int i = 0; i < searchCandidates.Count; i++)
        {
            var c = new GuardDecisionCandidate { position = searchCandidates[i], belief = searchBeliefs[i], reason = "Already inspected" };
            if (!inspectedCandidates[i])
            {
                c.reachable = GuardDecisionQueries.TryReachable(enemyMovement, c.position, out c.position, out c.pathLength)
                    && IsInsideRoom(c.position);
                c.reason = "No complete path";
                if (c.reachable)
                {
                    float information = searchBeliefs[i];
                    for (int j = 0; j < searchCandidates.Count; j++)
                        if (j != i && ObservableFrom(c.position, searchCandidates[j])) information += searchBeliefs[j];
                    c.visibility = information;
                    c.score = SearchDecisionPolicy.Score(information, c.pathLength);
                    c.reason = "Observable belief / travel effort";
                    if (c.score > best) { best = c.score; selected = i; }
                }
            }
            decisionCandidates.Add(c);
        }
        if (selected < 0 || RemainingBeliefMass < .05f)
        {
            TransitionToPatrol();
            return;
        }
        currentSearchCandidateIndex = selected;
        SelectedDecisionTarget = searchCandidates[selected];
        enemyMovement.ClearAimTarget();
        if (enemyMovement.SetDestination(SelectedDecisionTarget)) enemyMovement.Resume();
        else inspectedCandidates[selected] = true;
    }

    private void ApplyNegativeObservation()
    {
        int current = currentSearchCandidateIndex;
        if (current < 0 || current >= searchCandidates.Count) return;
        inspectedCandidates[current] = true;
        for (int i = 0; i < searchCandidates.Count; i++)
            if (i == current || ObservableFrom(transform.position, searchCandidates[i]))
                searchBeliefs[i] = SearchDecisionPolicy.NegativeObservation(searchBeliefs[i]);
        RemainingBeliefMass = 0;
        foreach (var probability in searchBeliefs) RemainingBeliefMass += probability;
        // Keep residual mass unnormalised: it represents evidence the player left the searched region.
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

            // Inspection sweep relative to arrival facing
            float sweepAngle = Mathf.Sin(Time.time * 3f) * 45f;
            Vector3 sweepDir = Quaternion.Euler(0f, sweepAngle, 0f) * baseInspectionForward;
            enemyMovement.SetAimTarget(transform.position + sweepDir * 4.0f);

            if (candidateInspectTimer <= 0f)
            {
                isWaitingAtCandidate = false;
                enemyMovement.ClearAimTarget();
                ApplyNegativeObservation();
                DispatchSearchCandidate(0);
            }
            return;
        }

        // Moving to current candidate
        Vector3 remaining = SelectedDecisionTarget - transform.position;
        remaining.y = 0;
        if (enemyMovement.HasReachedDestination(searchArrivalThreshold) || remaining.sqrMagnitude <= searchArrivalThreshold * searchArrivalThreshold)
        {
            isWaitingAtCandidate = true;
            baseInspectionForward = lastSeenPosition - transform.position;
            baseInspectionForward.y = 0;
            if (baseInspectionForward.sqrMagnitude < .01f) baseInspectionForward = transform.forward;
            baseInspectionForward.Normalize();
            candidateInspectTimer = 1.1f;
            enemyMovement.Stop();
        }
        else if (!enemyMovement.PathPending && (!enemyMovement.HasPath || enemyMovement.PathStatus != NavMeshPathStatus.PathComplete))
        {
            // Path lost/blocked -> advance to next candidate
            inspectedCandidates[currentSearchCandidateIndex] = true;
            DispatchSearchCandidate(0);
        }
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
