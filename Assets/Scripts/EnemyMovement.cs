using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls enemy movement and provides smooth rotation decoupling from NavMeshAgent.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Smooth angular rotation speed in degrees per second.")]
    [SerializeField] private float rotationSpeed = 360f;

    [Tooltip("Minimum desired velocity magnitude required to apply rotation.")]
    [SerializeField] private float minTurnVelocity = 0.05f;

    private NavMeshAgent agent;

    public NavMeshAgent Agent => agent;
    public bool IsOnNavMesh => agent != null && agent.isOnNavMesh;
    public float RemainingDistance => agent != null && agent.isOnNavMesh ? agent.remainingDistance : Mathf.Infinity;
    public bool PathPending => agent != null && agent.isOnNavMesh && agent.pathPending;
    public bool HasPath => agent != null && agent.isOnNavMesh && agent.hasPath;
    public NavMeshPathStatus PathStatus => agent != null && agent.isOnNavMesh ? agent.pathStatus : NavMeshPathStatus.PathInvalid;
    public bool HasCompletePath => agent != null && agent.isOnNavMesh && !agent.pathPending && agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disable automatic agent angular rotation snapping to allow custom smooth rotation
        agent.updateRotation = false;

        EnsureAgentOnNavMesh();
    }

    void Start()
    {
        EnsureAgentOnNavMesh();
    }

    /// <summary>
    /// Snaps agent onto the nearest active NavMesh region if not already seated.
    /// </summary>
    public void EnsureAgentOnNavMesh()
    {
        if (agent == null) return;
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    void Update()
    {
        SmoothRotate();
    }

    /// <summary>
    /// Smoothly rotates the transform towards the agent's desired velocity direction.
    /// </summary>
    private void SmoothRotate()
    {
        if (!agent.isOnNavMesh) return;

        Vector3 desiredVel = agent.desiredVelocity;
        desiredVel.y = 0f; // Constrain yaw rotation to horizontal plane

        if (desiredVel.sqrMagnitude > minTurnVelocity * minTurnVelocity)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredVel.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Sets a new destination for the NavMeshAgent.
    /// Returns true if destination was accepted; false if unable to set destination.
    /// </summary>
    public bool SetDestination(Vector3 destination)
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"[EnemyMovement] Cannot set destination on '{gameObject.name}': Agent is not on NavMesh.", this);
            return false;
        }

        bool success = agent.SetDestination(destination);
        if (!success)
        {
            Debug.LogWarning($"[EnemyMovement] SetDestination returned false on '{gameObject.name}' for destination {destination}.", this);
        }

        return success;
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
    /// Halts agent movement.
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
}
