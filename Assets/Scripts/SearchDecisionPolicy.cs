using UnityEngine;

/// <summary>Sanjeevsaran: a finite spatial belief over last-known-position hypotheses.</summary>
public static class SearchDecisionPolicy
{
    public static float Prior(Vector3 offset, Vector3 observedHeading)
    {
        float heading = observedHeading.sqrMagnitude > .01f && offset.sqrMagnitude > .01f
            ? Mathf.Max(0, Vector3.Dot(offset.normalized, observedHeading.normalized)) : 0;
        return Mathf.Exp(-offset.sqrMagnitude / 32f) * (1f + heading);
    }

    public static float Score(float observableProbability, float pathLength)
        => Mathf.Clamp01(observableProbability) / (1f + .15f * Mathf.Max(0, pathLength));

    // An unsuccessful inspection is evidence against the hypothesis, not proof of absence.
    public static float NegativeObservation(float prior) => prior * .08f;
}
