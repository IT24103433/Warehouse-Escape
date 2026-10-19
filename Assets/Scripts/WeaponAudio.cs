using UnityEngine;

/// <summary>Shared recorded weapon audio. Source credits are in Documentation/AudioSources.md.</summary>
public static class WeaponAudio
{
    private static AudioClip[] fireClips;
    private static AudioClip[] reloadClips;
    private static AudioClip emptyClick;
    private static AudioClip weaponSwap;
    private static AudioClip hitMarker;

    public static AudioClip GetWeaponFireClip(WeaponType type)
    {
        EnsureClips();
        return fireClips[ValidIndex(type)];
    }

    public static AudioClip GetReloadClip(WeaponType type)
    {
        EnsureClips();
        return reloadClips[ValidIndex(type)];
    }

    public static AudioClip GetReloadClip() => GetReloadClip(WeaponType.MP5_SMG);
    public static AudioClip GetEmptyClickClip() { EnsureClips(); return emptyClick; }
    public static AudioClip GetWeaponSwapClip() { EnsureClips(); return weaponSwap; }
    public static AudioClip GetHitMarkerClip() { EnsureClips(); return hitMarker; }

    private static int ValidIndex(WeaponType type)
    {
        int index = (int)type;
        return index >= 0 && index < fireClips.Length ? index : 0;
    }

    public static void EnsureClips()
    {
        if (fireClips != null) return;
        var types = (WeaponType[])System.Enum.GetValues(typeof(WeaponType));
        fireClips = new AudioClip[types.Length];
        reloadClips = new AudioClip[types.Length];
        foreach (var type in types)
        {
            fireClips[(int)type] = Load(type + "_Fire");
            reloadClips[(int)type] = Load(type + "_Reload");
        }
        emptyClick = Load("EmptyClick");
        weaponSwap = Load("WeaponSwap");
        // The hit confirmation is an interface cue, independent of firearm recordings.
        const int rate = 22050;
        var samples = new float[1323];
        for (int i = 0; i < samples.Length; i++)
        {
            float t = (float)i / rate;
            samples[i] = Mathf.Sin(2f * Mathf.PI * 1850f * t) * Mathf.Exp(-t * 60f) * 0.3f;
        }
        hitMarker = AudioClip.Create("Weapon_HitMarker", samples.Length, 1, rate, false);
        hitMarker.SetData(samples, 0);
    }

    private static AudioClip Load(string name)
    {
        var clip = Resources.Load<AudioClip>("Audio/Weapons/" + name);
        if (clip == null) Debug.LogError("Missing weapon recording: " + name);
        return clip;
    }
}
