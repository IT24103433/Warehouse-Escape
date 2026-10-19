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

public interface IGuardPerceptionTracker
{
    bool HasConfirmedVisualContact();
    bool IsGuardActive();
}

/// <summary>
/// Shared perception utility for all guard AI implementations.
/// Provides consistent, physically realistic evaluation of sight distance,
/// horizontal field of view, obstacle occlusion through warehouse walls and shelves,
/// sound disturbance propagation with obstacle attenuation, dynamic suspicion mechanics,
/// and centralized zero-allocation tracking of active guard visual contact.
/// </summary>
public static class GuardPerception
{
    public static event Action<SoundDisturbance> OnSoundReported;

    private static readonly System.Collections.Generic.List<IGuardPerceptionTracker> activeTrackers =
        new System.Collections.Generic.List<IGuardPerceptionTracker>(8);

    public static void RegisterTracker(IGuardPerceptionTracker tracker)
    {
        if (tracker != null && !activeTrackers.Contains(tracker))
        {
            activeTrackers.Add(tracker);
        }
    }

    public static void UnregisterTracker(IGuardPerceptionTracker tracker)
    {
        if (tracker != null)
        {
            activeTrackers.Remove(tracker);
        }
    }

    public static void ClearTrackers()
    {
        activeTrackers.Clear();
    }

    /// <summary>
    /// Returns true if any active, living guard has confirmed visual contact with the player.
    /// Used by the retreating health regeneration system for immediate interruption.
    /// </summary>
    public static bool AnyLivingGuardHasVisualContact()
    {
        for (int i = 0; i < activeTrackers.Count; i++)
        {
            var t = activeTrackers[i];
            if (t != null && t.IsGuardActive() && t.HasConfirmedVisualContact())
            {
                return true;
            }
        }
        return false;
    }

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
    /// <summary>
    /// Evaluates whether the guard has clear line of sight to the player.
    /// Checks sight distance, horizontal field of view angle, and raycast occlusion.
    /// Supports cached eye/gaze transform for animated head facing and actual player camera/collider targets.
    /// Intervening walls, shelves, crates, and doors block line of sight.
    /// </summary>
    public static bool CanSeePlayer(
        Transform guardTransform,
        Transform playerTransform,
        float sightDistance,
        float fieldOfView,
        float eyeHeight,
        float playerTargetHeight,
        LayerMask obstacleMask,
        Transform eyeTransform = null,
        Transform playerCameraTransform = null,
        Collider playerCollider = null)
    {
        if (guardTransform == null || playerTransform == null) return false;

        // 1. Resolve eye position and gaze forward direction:
        // Use cached, correctly oriented eye/head transform if available; fallback to body-forward.
        Vector3 eyePos;
        Vector3 forward;
        if (eyeTransform != null)
        {
            eyePos = eyeTransform.position;
            forward = eyeTransform.forward;
        }
        else
        {
            eyePos = guardTransform.position + Vector3.up * eyeHeight;
            forward = guardTransform.forward;
        }
        forward.y = 0f;

        float distToPlayer = Vector3.Distance(guardTransform.position, playerTransform.position);

        // 2. Maximum sight distance check
        if (distToPlayer > sightDistance) return false;

        // 3. Horizontal Field of View check in horizontal plane
        Vector3 toPlayerHorizontal = playerTransform.position - guardTransform.position;
        toPlayerHorizontal.y = 0f;

        if (forward.sqrMagnitude < 0.001f || toPlayerHorizontal.sqrMagnitude < 0.001f)
            return false;

        // Effective FOV: Standard 90 deg, with modest peripheral awareness (110 deg) at close quarters (< 2.5m)
        float effectiveFov = distToPlayer < 2.5f ? Mathf.Max(fieldOfView, 110f) : fieldOfView;
        float horizontalAngle = Vector3.Angle(forward.normalized, toPlayerHorizontal.normalized);
        if (horizontalAngle > effectiveFov * 0.5f) return false;

        // 4. Derive visible head/chest targets from player's actual camera and collider
        Vector3 headTarget;
        if (playerCameraTransform != null)
        {
            headTarget = playerCameraTransform.position;
        }
        else
        {
            float headOffset = playerTargetHeight > 0f ? playerTargetHeight : 0.70f;
            headTarget = playerTransform.position + Vector3.up * headOffset;
        }

        Vector3 chestTarget;
        if (playerCollider is CharacterController cc)
        {
            chestTarget = playerTransform.position + cc.center;
        }
        else if (playerCollider != null)
        {
            chestTarget = playerCollider.bounds.center;
        }
        else
        {
            chestTarget = playerTransform.position + Vector3.up * 0.10f;
        }

        // Multi-point visibility check: test both head/eyes and chest/torso
        if (CheckRayVisibility(eyePos, headTarget, guardTransform, playerTransform, obstacleMask))
            return true;

        if (CheckRayVisibility(eyePos, chestTarget, guardTransform, playerTransform, obstacleMask))
            return true;

        // Both head and chest occluded by solid cover/walls
        return false;
    }

    private static bool CheckRayVisibility(
        Vector3 eyePos,
        Vector3 targetPos,
        Transform guardTransform,
        Transform playerTransform,
        LayerMask obstacleMask)
    {
        Vector3 toTarget = targetPos - eyePos;
        float dist = toTarget.magnitude;
        float startOffset = 0.40f;
        if (dist <= startOffset) return true;

        Vector3 rayOrigin = eyePos + toTarget.normalized * startOffset;
        float rayDist = dist - startOffset;

        Ray ray = new Ray(rayOrigin, toTarget.normalized);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDist, obstacleMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return true; // Unobstructed

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            // Ignore guard's own colliders
            if (hit.transform == guardTransform || hit.transform.IsChildOf(guardTransform))
                continue;

            // First non-self object hit is the player or child of player -> clear line of sight
            if (hit.transform == playerTransform || hit.transform.IsChildOf(playerTransform) || hit.collider.CompareTag("Player"))
                return true;

            // Intervening solid obstacle (wall, cover crate, barrier, door) blocks this ray
            return false;
        }

        return true;
    }
}
