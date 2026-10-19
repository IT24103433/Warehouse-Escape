using UnityEngine;

/// <summary>Aqqil: protect the objective while choosing useful shotgun/cover positions.</summary>
public static class DefenseDecisionPolicy
{
    public static float Score(float objective, float cover, float weaponRange, float visibility,
        float freshness, float pathLength, bool hasThreat)
    {
        float utility = hasThreat
            ? .30f * objective + .25f * cover + .30f * weaponRange + .15f * freshness
            : .45f * objective + .30f * visibility + .25f * freshness;
        return utility - .15f * Mathf.Clamp01(pathLength / 20f);
    }
}
