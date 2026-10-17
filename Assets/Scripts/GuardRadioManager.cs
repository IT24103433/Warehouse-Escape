using System;
using System.Collections.Generic;
using UnityEngine;

public struct RadioMessage
{
    public string reportingGuard;
    public Vector3 lastKnownPosition;
    public float timestamp;
}

/// <summary>
/// Centralized radio communication bus for security guards.
/// Broadcasts timestamped last known intruder positions with realistic radio delay and cooldown.
/// Messages do not provide continuous tracking, allowing stealth and escape opportunities.
/// </summary>
public static class GuardRadioManager
{
    public static event Action<RadioMessage> OnRadioAlertReceived;

    private static Dictionary<string, float> lastBroadcastTime = new Dictionary<string, float>();
    private const float RADIO_COOLDOWN = 10.0f; // Minimum seconds between broadcasts by the same guard

    public static void BroadcastAlert(string guardName, Vector3 lastKnownPosition)
    {
        if (lastBroadcastTime.TryGetValue(guardName, out float lastTime))
        {
            if (Time.time - lastTime < RADIO_COOLDOWN)
            {
                return; // Guard is on radio cooldown
            }
        }

        lastBroadcastTime[guardName] = Time.time;

        RadioMessage msg = new RadioMessage
        {
            reportingGuard = guardName,
            lastKnownPosition = lastKnownPosition,
            timestamp = Time.time
        };

        Debug.Log($"[GuardRadioManager] RADIO DISPATCH from '{guardName}': Intruder confirmed at {lastKnownPosition:F1}. Broadcasting alert to security network.");
        OnRadioAlertReceived?.Invoke(msg);
    }

    public static void ResetRadio()
    {
        lastBroadcastTime.Clear();
    }
}
