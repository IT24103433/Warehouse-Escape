using System;
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
/// Focuses on systematic patrol, active pursuit, and extended investigation of the player's last seen position.
/// When line of sight is broken, investigates the last confirmed coordinates, spends a configurable search timeout
/// inspecting the area, and resumes its patrol circuit only after the timeout elapses.
/// </summary>
[RequireComponent(typeof(EnemyMovement))]
public class SearchGuardAI : MonoBehaviour
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
    [SerializeField] private float searchTimeout = 3.0f;

    [Tooltip("Arrival distance threshold for investigating last seen position.")]
    [SerializeField] private float searchArrivalThreshold = 1.0f;

    private EnemyMovement enemyMovement;
    private Transform playerTransform;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaitingAtWaypoint = false;
    private float repathTimer = 0f;
    private float searchTimer = 0f;
    private bool isInvestigating = false;

    public SearchGuardState CurrentState => currentState;
    public bool CanSeePlayer => canSeePlayer;
    public Vector3 LastSeenPosition => lastSeenPosition;
    public Transform[] PatrolWaypoints { get => patrolWaypoints; set => patrolWaypoints = value; }
    public float SearchTimeout { get => searchTimeout; set => searchTimeout = value; }
    public void SetSearchTimeout(float timeout)
    {
        searchTimeout = timeout;
        if (currentState == SearchGuardState.Search && searchTimer > timeout) searchTimer = timeout;
    }

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Start()
    {
        FindPlayerReference();
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
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    public void SetPlayer(Transform player) => playerTransform = player;

    private void UpdatePatrolState()
    {
        if (canSeePlayer)
        {
            lastSeenPosition = playerTransform.position;
            TransitionToPursue();
            return;
        }

        // Patrol waypoint traversal
        if (patrolWaypoints == null || patrolWaypoints.Length == 0) return;

        if (isWaitingAtWaypoint)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaitingAtWaypoint = false;
                AdvanceWaypoint();
            }
            return;
        }

        if (enemyMovement.HasReachedDestination(waypointArrivalThreshold))
        {
            isWaitingAtWaypoint = true;
            waitTimer = waypointWaitTime;
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
            enemyMovement.SetDestination(wp.position);
            enemyMovement.Resume();
        }
    }

    public void TransitionToPatrol()
    {
        currentState = SearchGuardState.Patrol;
        isInvestigating = false;
        isWaitingAtWaypoint = false;
        DispatchCurrentWaypoint();
    }

    public void TransitionToPursue()
    {
        currentState = SearchGuardState.Pursue;
        isInvestigating = false;
        repathTimer = 0f;
        DispatchPursuitDestination();
    }

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

    private void DispatchPursuitDestination()
    {
        if (playerTransform == null) return;
        enemyMovement.SetDestination(playerTransform.position);
        enemyMovement.Resume();
    }

    public void TransitionToSearch()
    {
        currentState = SearchGuardState.Search;
        searchTimer = searchTimeout;
        isInvestigating = true;

        bool ok = enemyMovement.SetDestination(lastSeenPosition);
        if (ok)
        {
            enemyMovement.Resume();
        }
        else
        {
            enemyMovement.Stop();
            isInvestigating = false;
        }
    }

    private void UpdateSearchState()
    {
        // If spotted again, immediately resume pursuit
        if (canSeePlayer)
        {
            lastSeenPosition = playerTransform.position;
            TransitionToPursue();
            return;
        }

        // Check arrival at last seen position
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

        // Countdown search timeout at last seen position
        if (!isInvestigating)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0f)
            {
                TransitionToPatrol();
            }
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
        }
    }
}
