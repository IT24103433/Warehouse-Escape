using UnityEngine;

/// <summary>Varun: prefer neglected, observable areas without excessive travel.</summary>
public static class PatrolDecisionPolicy
{
    public static float Score(float secondsUnvisited, float visibility, float pathLength)
        => .65f * Mathf.Clamp01(secondsUnvisited / 30f) + .35f * Mathf.Clamp01(visibility)
            - .25f * Mathf.Clamp01(pathLength / 30f);
}
