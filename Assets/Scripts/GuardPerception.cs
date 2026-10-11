using System;
using UnityEngine;

/// <summary>
/// Shared perception utility for all guard AI implementations.
/// Provides consistent, physically realistic evaluation of sight distance,
/// horizontal field of view, and obstacle occlusion through warehouse walls and shelves.
/// </summary>
public static class GuardPerception
{
    /// <summary>
    /// Evaluates whether the guard has clear line of sight to the player.
    /// Checks sight distance, horizontal field of view angle, and raycast occlusion.
    /// Intervening walls, shelves, crates, and doors block line of sight.
    /// </summary>
    public static bool CanSeePlayer(
        Transform guardTransform,
        Transform playerTransform,
        float sightDistance,
        float fieldOfView,
        float eyeHeight,
        float playerTargetHeight,
        LayerMask obstacleMask)
    {
        if (guardTransform == null || playerTransform == null) return false;

        Vector3 eyePos = guardTransform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = playerTransform.position + Vector3.up * playerTargetHeight;
        Vector3 toTarget = targetPos - eyePos;
        float dist = toTarget.magnitude;

        // 1. Maximum sight distance check
        if (dist > sightDistance) return false;

        // 2. Horizontal Field of View check in horizontal plane
        Vector3 forward = guardTransform.forward;
        forward.y = 0f;
        Vector3 horizontalDir = toTarget;
        horizontalDir.y = 0f;

        if (forward.sqrMagnitude < 0.001f || horizontalDir.sqrMagnitude < 0.001f)
            return false;

        float horizontalAngle = Vector3.Angle(forward.normalized, horizontalDir.normalized);
        if (horizontalAngle > fieldOfView * 0.5f) return false;

        // 3. Clear line-of-sight raycast against warehouse geometry
        float startOffset = 0.55f;
        if (dist <= startOffset) return true;

        Vector3 rayOrigin = eyePos + toTarget.normalized * startOffset;
        float rayDist = dist - startOffset;

        Ray ray = new Ray(rayOrigin, toTarget.normalized);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDist, obstacleMask, QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            // Ignore own guard colliders
            if (hit.transform == guardTransform || hit.transform.IsChildOf(guardTransform))
                continue;

            // First non-self hit is player -> clear line of sight
            if (hit.transform == playerTransform || hit.transform.IsChildOf(playerTransform) || hit.collider.CompareTag("Player"))
                return true;

            // Intervening geometry (wall, shelf, crate, door) occludes sight
            return false;
        }

        // Unobstructed path
        return true;
    }
}
