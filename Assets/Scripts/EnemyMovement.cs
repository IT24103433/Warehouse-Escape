using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Strengthened Agent Movement Controller.
/// Acts as the single authority for physical locomotion, rotation, acceleration, braking,
/// stopping, aiming presentation, and stuck-progress recovery.
/// Higher-level AI decision scripts (Patrol, Search, Defensive, Flanking) specify targets,
/// while EnemyMovement manages physical execution without competing rotation or teleportation.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Locomotion & Rotation Settings")]
    [Tooltip("Smooth angular turning speed in degrees per second.")]
    [SerializeField] private float rotationSpeed = 320f;

    [Tooltip("Minimum velocity required to drive automatic movement rotation.")]
    [SerializeField] private float minTurnVelocity = 0.08f;

    [Tooltip("Braking distance where agent smoothly decelerates to avoid overshoot.")]
    [SerializeField] private float brakingDistance = 1.2f;

    [Header("Stuck Detection & Bounded Recovery")]
    [Tooltip("Duration in seconds of near-zero displacement before flagging genuine lack of progress.")]
    [SerializeField] private float stuckDurationThreshold = 1.8f;

    [Tooltip("Minimum displacement expected over sample window.")]
    [SerializeField] private float minExpectedDisplacement = 0.12f;

    [Tooltip("Maximum bounded recovery attempts before halting safely.")]
    [SerializeField] private int maxStuckRetries = 3;

    private NavMeshAgent agent;
    private Health health;

    // Aiming / facing override
    private bool hasAimOverride = false;
    private Vector3 aimTargetPosition;

    // Stuck progress tracking
    private Vector3 lastProgressSamplePos;
    private float progressSampleTimer = 0f;
    private float stuckTimer = 0f;
    private int stuckRetryCount = 0;
    private float baseAgentSpeed = 3.5f;
    private bool hasLoggedNavMeshWarning = false;

    public NavMeshAgent Agent => agent;
    public bool IsOnNavMesh => agent != null && agent.isOnNavMesh;
    public float RemainingDistance => agent != null && agent.isOnNavMesh ? agent.remainingDistance : Mathf.Infinity;
    public bool PathPending => agent != null && agent.isOnNavMesh && agent.pathPending;
    public bool HasPath => agent != null && agent.isOnNavMesh && agent.hasPath;
    public NavMeshPathStatus PathStatus => agent != null && agent.isOnNavMesh ? agent.pathStatus : NavMeshPathStatus.PathInvalid;
    public bool HasCompletePath => agent != null && agent.isOnNavMesh && !agent.pathPending && agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete;
    public bool IsStuck => stuckTimer >= stuckDurationThreshold;

    public event Action OnStuckRecovered;
    public event Action OnStuckUnresolved;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        // Disable automatic agent angular snapping to give EnemyMovement sole rotation authority
        agent.updateRotation = false;
        baseAgentSpeed = agent.speed;

        EnsureAgentOnNavMesh();
    }

    void Start()
    {
        EnsureAgentOnNavMesh();
        lastProgressSamplePos = transform.position;
    }

    /// <summary>
    /// Snaps agent onto the nearest active NavMesh region if not already seated.
    /// Preserves standalone NavMesh initialization.
    /// </summary>
    public void EnsureAgentOnNavMesh()
    {
        if (agent == null) return;
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3.5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                hasLoggedNavMeshWarning = false;
            }
            else if (!hasLoggedNavMeshWarning)
            {
                Debug.LogWarning($"[EnemyMovement] Agent '{gameObject.name}' could not find NavMesh within 3.5m. Waiting for NavMesh bootstrap.", this);
                hasLoggedNavMeshWarning = true;
            }
        }
    }

    void Update()
    {
        if (health != null && health.IsDead) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return;

        if (!agent.isOnNavMesh)
        {
            EnsureAgentOnNavMesh();
            return;
        }

        // 1. Single authority rotation handling
        ExecuteRotation();

        // 2. Smooth braking near destination to prevent jitter / overshoot
        ExecuteSmoothBraking();

        // 3. Genuine lack of progress detection
        MonitorProgress();
    }

    /// <summary>
    /// Executes smooth rotation towards either the active aim target (combat/facing override)
    /// or the agent's desired velocity direction (locomotion).
    /// </summary>
    private void ExecuteRotation()
    {
        if (hasAimOverride)
        {
            // Rotate smoothly towards specified aim / look target
            Vector3 toAim = aimTargetPosition - transform.position;
            toAim.y = 0f;
            if (toAim.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toAim.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRot, rotationSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            // Rotate smoothly towards movement velocity
            Vector3 desiredVel = agent.desiredVelocity;
            desiredVel.y = 0f;

            if (desiredVel.sqrMagnitude > minTurnVelocity * minTurnVelocity)
            {
                Quaternion targetRot = Quaternion.LookRotation(desiredVel.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRot, rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    /// <summary>
    /// Dynamically modulates agent speed when approaching destination for smooth braking.
    /// </summary>
    private void ExecuteSmoothBraking()
    {
        if (!agent.hasPath || agent.isStopped) return;

        float remDist = agent.remainingDistance;
        if (remDist > 0.05f && remDist < brakingDistance)
        {
            // Decelerate smoothly as agent closes in
            float brakeFactor = Mathf.Clamp01(remDist / brakingDistance);
            agent.speed = Mathf.Max(1.0f, baseAgentSpeed * Mathf.Lerp(0.45f, 1.0f, brakeFactor));
        }
    }

    /// <summary>
    /// Monitors physical displacement over time to detect genuine lack of progress
    /// while an active reachable destination remains active.
    /// </summary>
    private void MonitorProgress()
    {
        if (!agent.hasPath || agent.isStopped || agent.pathPending)
        {
            stuckTimer = 0f;
            stuckRetryCount = 0;
            return;
        }

        progressSampleTimer += Time.deltaTime;
        if (progressSampleTimer >= 1.0f)
        {
            progressSampleTimer = 0f;
            float displacement = Vector3.Distance(transform.position, lastProgressSamplePos);
            lastProgressSamplePos = transform.position;

            // If displacement is less than minimum expected while far from destination
            if (displacement < minExpectedDisplacement && agent.remainingDistance > agent.stoppingDistance + 0.5f)
            {
                stuckTimer += 1.0f;
                if (stuckTimer >= stuckDurationThreshold)
                {
                    HandleStuckCondition();
                }
            }
            else
            {
                stuckTimer = 0f;
                stuckRetryCount = 0;
            }
        }
    }

    /// <summary>
    /// Recovers from lack of progress using bounded retries and nearby reachable positions.
    /// Avoids continuous repathing loops or unnatural teleportation.
    /// </summary>
    private void HandleStuckCondition()
    {
        stuckTimer = 0f;

        if (stuckRetryCount < maxStuckRetries)
        {
            stuckRetryCount++;
            Debug.LogWarning($"[EnemyMovement] Genuine lack of progress detected on '{gameObject.name}'. Attempting bounded recovery {stuckRetryCount}/{maxStuckRetries}...", this);

            // Sample alternative reachable position within 2.5m on NavMesh
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * 2.5f;
            randomOffset.y = 0f;
            Vector3 candidatePos = transform.position + randomOffset;

            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 2.5f, agent.areaMask))
            {
                NavMeshPath testPath = new NavMeshPath();
                if (agent.CalculatePath(hit.position, testPath) && testPath.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetPath(testPath);
                    agent.isStopped = false;
                    OnStuckRecovered?.Invoke();
                    return;
                }
            }
        }
        else
        {
            Debug.LogWarning($"[EnemyMovement] Bounded recovery exhausted on '{gameObject.name}'. Halting safely at current position.", this);
            Stop();
            stuckRetryCount = 0;
            OnStuckUnresolved?.Invoke();
        }
    }

    /// <summary>
    /// Sets an aim target for the agent to smoothly face (e.g. visible intruder or disturbance).
    /// </summary>
    public void SetAimTarget(Vector3 targetWorldPos)
    {
        hasAimOverride = true;
        aimTargetPosition = targetWorldPos;
    }

    /// <summary>
    /// Clears the aim target override, returning rotation control to velocity-driven heading.
    /// </summary>
    public void ClearAimTarget()
    {
        hasAimOverride = false;
    }

    /// <summary>
    /// Sets a new destination for the NavMeshAgent.
    /// Validates path completeness and rejects incomplete or invalid paths.
    /// </summary>
    public bool SetDestination(Vector3 destination)
    {
        if (!agent.isOnNavMesh)
        {
            EnsureAgentOnNavMesh();
            if (!agent.isOnNavMesh)
            {
                Debug.LogWarning($"[EnemyMovement] Cannot set destination on '{gameObject.name}': Agent is not on NavMesh.", this);
                return false;
            }
        }

        // Pre-validate path to reject partial or invalid routes (blocked doors, unreachable alcoves)
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(destination, path))
        {
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                // Incomplete or partial path -> reject destination
                return false;
            }

            bool success = agent.SetPath(path);
            if (success)
            {
                agent.isStopped = false;
                baseAgentSpeed = agent.speed;
                stuckTimer = 0f;
                lastProgressSamplePos = transform.position;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Calculates a path to the target position using the agent's query parameters without moving.
    /// </summary>
    public bool CalculatePath(Vector3 targetPosition, NavMeshPath path)
    {
        if (!agent.isOnNavMesh) return false;
        return agent.CalculatePath(targetPosition, path);
    }

    /// <summary>
    /// Halts agent movement safely.
    /// </summary>
    public void Stop()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Resumes agent movement.
    /// </summary>
    public void Resume()
    {
        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    /// <summary>
    /// Checks whether the agent has reached its current destination within threshold.
    /// Reports arrival ONLY for a valid, complete path.
    /// </summary>
    public bool HasReachedDestination(float arrivalThreshold = 0.5f)
    {
        if (!agent.isOnNavMesh) return false;
        if (agent.pathPending) return false;
        if (!agent.hasPath) return false;
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return false;

        return agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arrivalThreshold);
    }

    /// <summary>
    /// Fully resets movement controller state on respawn / restart.
    /// </summary>
    public void ResetMovement()
    {
        stuckTimer = 0f;
        stuckRetryCount = 0;
        progressSampleTimer = 0f;
        hasAimOverride = false;
        EnsureAgentOnNavMesh();
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = false;
            agent.velocity = Vector3.zero;
        }
    }
}
