using System;
using UnityEngine;

public enum SoundType
{
    Gunshot,
    Footstep
}

public struct SoundDisturbance
{
    public Vector3 position;
    public float range;
    public SoundType soundType;
    public GameObject source;
    public float timestamp;
}

/// <summary>
/// Shared perception utility for all guard AI implementations.
/// Provides consistent, physically realistic evaluation of sight distance,
/// horizontal field of view, obstacle occlusion through warehouse walls and shelves,
/// sound disturbance propagation with obstacle attenuation, and dynamic suspicion mechanics.
/// </summary>
public static class GuardPerception
{
    public static event Action<SoundDisturbance> OnSoundReported;

    /// <summary>
    /// Broadcasts an audio disturbance into the environment (gunshot or footsteps).
    /// Guards listening on this channel evaluate physical reachability and wall attenuation.
    /// </summary>
    public static void ReportSound(Vector3 position, float range, SoundType soundType, GameObject source = null)
    {
        OnSoundReported?.Invoke(new SoundDisturbance
        {
            position = position,
            range = range,
            soundType = soundType,
            source = source,
            timestamp = Time.time
        });
    }

    /// <summary>
    /// Evaluates whether a listener at listenerPos can hear a sound disturbance,
    /// applying distance falloff and physical barrier attenuation through walls/shelves.
    /// </summary>
    public static bool CanHearSound(
        Vector3 listenerPos,
        SoundDisturbance sound,
        LayerMask obstacleMask,
        out float effectiveAudibility)
    {
        effectiveAudibility = 0f;
        float dist = Vector3.Distance(listenerPos, sound.position);
        if (dist > sound.range) return false;

        // Trace line from listener to sound position to detect physical intervening obstacles
        Vector3 dir = (sound.position - listenerPos);
        if (dir.sqrMagnitude < 0.01f)
        {
            effectiveAudibility = 1.0f;
            return true;
        }

        Ray ray = new Ray(listenerPos, dir.normalized);
        RaycastHit[] hits = Physics.RaycastAll(ray, dist, obstacleMask, QueryTriggerInteraction.Ignore);

        int wallCount = 0;
        foreach (var h in hits)
        {
            if (h.collider != null && !h.collider.isTrigger)
            {
                // Count distinct non-agent static obstacles
                if (sound.source == null || (h.transform != sound.source.transform && !h.transform.IsChildOf(sound.source.transform)))
                {
                    wallCount++;
                }
            }
        }

        // Each solid intervening wall reduces effective audible range by 45%
        float attenuationFactor = Mathf.Pow(0.55f, Mathf.Min(wallCount, 5));
        float effectiveMaxRange = sound.range * attenuationFactor;

        if (dist <= effectiveMaxRange)
        {
            effectiveAudibility = Mathf.Clamp01(1f - (dist / effectiveMaxRange));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates progressive suspicion buildup or decay based on sustained line of sight,
    /// distance proximity, and elapsed time.
    /// </summary>
    public static float UpdateSuspicion(
        float currentSuspicion,
        bool hasLineOfSight,
        float distanceToPlayer,
        float deltaTime)
    {
        if (hasLineOfSight)
        {
            // Close range (< 4m): Rapid alert buildup (~0.35s to full alert)
            // Mid range (4-10m): Moderate buildup (~0.85s to full alert)
            // Long range (> 10m): Slow suspicion buildup (~1.8s to full alert)
            float buildRate = 1.0f;
            if (distanceToPlayer < 4.0f)
                buildRate = 3.2f;
            else if (distanceToPlayer <= 10.0f)
                buildRate = 1.4f;
            else
                buildRate = 0.65f;

            return Mathf.Clamp01(currentSuspicion + buildRate * deltaTime);
        }
        else
        {
            // Smooth decay over ~2.5s after losing sight
            float decayRate = 0.40f;
            return Mathf.Clamp01(currentSuspicion - decayRate * deltaTime);
        }
    }

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
