using UnityEngine;
using UnityEngine.AI;

/// <summary>Inspectable evidence for one decision. Scores are utilities: higher is better.</summary>
[System.Serializable]
public struct GuardDecisionCandidate
{
    public Vector3 position;
    public float pathLength, freshness, visibility, belief, cover, objective, weaponRange, score;
    public bool reachable;
    public string reason;
}

/// <summary>Shared world queries, independent of the three agents' decision policies.</summary>
public static class GuardDecisionQueries
{
    private static readonly RaycastHit[] hits = new RaycastHit[32];

    public static bool TryReachable(EnemyMovement movement, Vector3 raw, out Vector3 position, out float length)
    {
        position = raw;
        length = 0;
        if (movement == null || !movement.IsOnNavMesh) return false;
        var filter = new NavMeshQueryFilter { agentTypeID = movement.Agent.agentTypeID, areaMask = movement.Agent.areaMask };
        if (!NavMesh.SamplePosition(raw, out var hit, 1.5f, filter)) return false;
        position = hit.position;
        var path = new NavMeshPath();
        if (!movement.CalculatePath(position, path) || path.status != NavMeshPathStatus.PathComplete) return false;
        var corners = path.corners;
        for (int i = 1; i < corners.Length; i++) length += Vector3.Distance(corners[i - 1], corners[i]);
        return true;
    }

    public static bool Clear(Vector3 from, Vector3 to, LayerMask mask, Transform owner, Transform ignoredTarget = null)
    {
        var delta = to - from;
        float distance = delta.magnitude;
        if (distance < .05f) return true;
        int count = Physics.RaycastNonAlloc(from, delta / distance, hits, distance, mask, QueryTriggerInteraction.Ignore);
        // A saturated buffer is conservatively occluded rather than falsely clear.
        if (count == hits.Length) return false;
        for (int i = 0; i < count; i++)
        {
            var hit = hits[i].transform;
            if (owner != null && (hit == owner || hit.IsChildOf(owner))) continue;
            if (ignoredTarget != null && (hit == ignoredTarget || hit.IsChildOf(ignoredTarget))) continue;
            return false;
        }
        return true;
    }

    public static float Visibility(Vector3 floor, LayerMask mask, Transform owner)
    {
        int clear = 0;
        var eye = floor + Vector3.up * 1.5f;
        for (int i = 0; i < 8; i++)
        {
            var direction = Quaternion.Euler(0, i * 45, 0) * Vector3.forward;
            if (Clear(eye, eye + direction * 6, mask, owner)) clear++;
        }
        return clear / 8f;
    }
}
