using UnityEngine;

public enum WeaponType
{
    MP5_SMG,
    UMP_SMG,
    AK47_Rifle,
    Combat_Shotgun,
    SCAR_Rifle
}

[System.Serializable]
public class WeaponData
{
    public WeaponType type;
    public string weaponName;
    public int damage;
    public float fireRate;
    public int magazineCapacity;
    public int defaultReserve;
    public float reloadDuration;
    public bool isAutomatic;
    public bool isShotgun;
    public int pelletCount;
    public float pelletSpread;
    public float recoilPitch;
    public float spread;
    public float maxRange;
    public string prefabPath;

    public static WeaponData CreateMP5()
    {
        return new WeaponData
        {
            type = WeaponType.MP5_SMG,
            weaponName = "MP5-A3 SMG",
            damage = 18,
            fireRate = 0.11f, // ~545 RPM
            magazineCapacity = 30,
            defaultReserve = 90,
            reloadDuration = 1.5f,
            isAutomatic = true,
            isShotgun = false,
            pelletCount = 1,
            pelletSpread = 0f,
            recoilPitch = 0.75f,
            spread = 1.5f,
            maxRange = 55f,
            prefabPath = "Weapons/MP5_SMG"
        };
    }

    public static WeaponData CreateUMP()
    {
        return new WeaponData
        {
            type = WeaponType.UMP_SMG,
            weaponName = "UMP-45 SMG",
            damage = 16,
            fireRate = 0.14f, // ~430 RPM
            magazineCapacity = 25,
            defaultReserve = 75,
            reloadDuration = 1.4f,
            isAutomatic = true,
            isShotgun = false,
            pelletCount = 1,
            pelletSpread = 0f,
            recoilPitch = 0.90f,
            spread = 2.0f,
            maxRange = 45f,
            prefabPath = "Weapons/UMP_SMG"
        };
    }

    public static WeaponData CreateAK47()
    {
        return new WeaponData
        {
            type = WeaponType.AK47_Rifle,
            weaponName = "AK-47 Rifle",
            damage = 25,
            fireRate = 0.15f, // 400 RPM
            magazineCapacity = 30,
            defaultReserve = 60,
            reloadDuration = 1.8f,
            isAutomatic = true,
            isShotgun = false,
            pelletCount = 1,
            pelletSpread = 0f,
            recoilPitch = 1.35f,
            spread = 2.4f,
            maxRange = 65f,
            prefabPath = "Weapons/AK47_Rifle"
        };
    }

    public static WeaponData CreateShotgun()
    {
        return new WeaponData
        {
            type = WeaponType.Combat_Shotgun,
            weaponName = "Combat Shotgun",
            damage = 7, // 7 per pellet * 6 pellets = 42 max damage point blank
            fireRate = 0.80f,
            magazineCapacity = 8,
            defaultReserve = 24,
            reloadDuration = 2.0f,
            isAutomatic = false,
            isShotgun = true,
            pelletCount = 6,
            pelletSpread = 4.5f,
            recoilPitch = 2.2f,
            spread = 4.5f,
            maxRange = 28f,
            prefabPath = "Weapons/Combat_Shotgun"
        };
    }

    public static WeaponData CreateSCAR()
    {
        return new WeaponData
        {
            type = WeaponType.SCAR_Rifle,
            weaponName = "SCAR Battle Rifle",
            damage = 26,
            fireRate = 0.13f, // ~460 RPM
            magazineCapacity = 20,
            defaultReserve = 60,
            reloadDuration = 1.7f,
            isAutomatic = true,
            isShotgun = false,
            pelletCount = 1,
            pelletSpread = 0f,
            recoilPitch = 1.25f,
            spread = 1.6f,
            maxRange = 70f,
            prefabPath = "Weapons/SCAR_Rifle"
        };
    }

    public static WeaponData GetConfig(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.MP5_SMG: return CreateMP5();
            case WeaponType.UMP_SMG: return CreateUMP();
            case WeaponType.AK47_Rifle: return CreateAK47();
            case WeaponType.Combat_Shotgun: return CreateShotgun();
            case WeaponType.SCAR_Rifle: return CreateSCAR();
            default: return CreateMP5();
        }
    }
}

/// <summary>
/// Centralized audio synthesizer providing distinct, non-overlapping acoustic profiles
/// for MP5, UMP, AK47, SCAR, and Shotgun, as well as mechanical reload and empty-magazine feedback.
/// </summary>
public static class WeaponAudio
{
    private static AudioClip clipMP5;
    private static AudioClip clipUMP;
    private static AudioClip clipAK47;
    private static AudioClip clipSCAR;
    private static AudioClip clipShotgun;
    private static AudioClip clipReload;
    private static AudioClip clipEmptyClick;
    private static AudioClip clipWeaponSwap;
    private static AudioClip clipHitMarker;

    public static AudioClip GetWeaponFireClip(WeaponType type)
    {
        EnsureClips();
        switch (type)
        {
            case WeaponType.MP5_SMG: return clipMP5;
            case WeaponType.UMP_SMG: return clipUMP;
            case WeaponType.AK47_Rifle: return clipAK47;
            case WeaponType.SCAR_Rifle: return clipSCAR;
            case WeaponType.Combat_Shotgun: return clipShotgun;
            default: return clipMP5;
        }
    }

    public static AudioClip GetReloadClip()
    {
        EnsureClips();
        return clipReload;
    }

    public static AudioClip GetEmptyClickClip()
    {
        EnsureClips();
        return clipEmptyClick;
    }

    public static AudioClip GetWeaponSwapClip()
    {
        EnsureClips();
        return clipWeaponSwap;
    }

    public static AudioClip GetHitMarkerClip()
    {
        EnsureClips();
        return clipHitMarker;
    }

    public static void EnsureClips()
    {
        if (clipMP5 != null) return;

        // MP5: 9mm submachine gun. Fast, crisp, high-frequency mechanical snap.
        // Duration: 0.075s matching 0.08s fire rate cadence.
        clipMP5 = CreateSynthesizedClip("Weapon_MP5_Fire", 0.075f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 42f);
            float noise = (Random.value * 2f - 1f) * 0.70f;
            float snap = Mathf.Sin(2f * Mathf.PI * (340f - t * 750f) * t) * 0.45f;
            return (noise + snap) * env;
        });

        // UMP-45: .45 ACP submachine gun. Heavy low-end bass punch and chunkier bolt thud.
        // Duration: 0.095s.
        clipUMP = CreateSynthesizedClip("Weapon_UMP_Fire", 0.095f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 28f);
            float noise = (Random.value * 2f - 1f) * 0.60f;
            float thud = Mathf.Sin(2f * Mathf.PI * (190f - t * 400f) * t) * 0.60f;
            return (noise + thud) * env;
        });

        // AK-47: 7.62x39mm assault rifle. Loud, aggressive, unsuppressed sharp concussive crack with wide metallic bark.
        // Duration: 0.115s.
        clipAK47 = CreateSynthesizedClip("Weapon_AK47_Fire", 0.115f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 24f);
            float noise = (Random.value * 2f - 1f) * 0.85f;
            float crack = Mathf.Sin(2f * Mathf.PI * (480f - t * 900f) * t) * 0.55f;
            return (noise + crack) * env;
        });

        // SCAR: 7.62x51mm heavy rifle. High-pressure modern punch with crisp double-impulse mechanical transient.
        // Duration: 0.10f.
        clipSCAR = CreateSynthesizedClip("Weapon_SCAR_Fire", 0.10f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 30f);
            float noise = (Random.value * 2f - 1f) * 0.75f;
            float pulse1 = Mathf.Sin(2f * Mathf.PI * (380f - t * 650f) * t) * 0.50f;
            float pulse2 = (t > 0.02f) ? Mathf.Sin(2f * Mathf.PI * 180f * (t - 0.02f)) * 0.35f : 0f;
            return (noise + pulse1 + pulse2) * env;
        });

        // Shotgun: 12-gauge buckshot. Deep sub-bass boom with wide multi-pellet spread noise.
        // Duration: 0.18f.
        clipShotgun = CreateSynthesizedClip("Weapon_Shotgun_Blast", 0.18f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 20f);
            float noise = (Random.value * 2f - 1f) * 0.90f;
            float boom = Mathf.Sin(2f * Mathf.PI * (130f - t * 250f) * t) * 0.65f;
            return (noise + boom) * env;
        });

        // Reload: 3-stage mechanical reload (mag release -> insert -> bolt lock/release)
        clipReload = CreateSynthesizedClip("Weapon_Reload", 0.65f, (t, dur) =>
        {
            float click1 = (t > 0.04f && t < 0.12f) ? (Random.value * 2f - 1f) * Mathf.Sin((t - 0.04f) * 1400f) : 0f;
            float slap = (t > 0.26f && t < 0.36f) ? (Random.value * 2f - 1f) * 0.7f + Mathf.Sin((t - 0.26f) * 600f) * 0.5f : 0f;
            float bolt = (t > 0.48f && t < 0.56f) ? (Random.value * 2f - 1f) * Mathf.Sin((t - 0.48f) * 2200f) : 0f;
            return click1 + slap + bolt;
        });

        // Empty click: Sharp trigger sear dry-click
        clipEmptyClick = CreateSynthesizedClip("Weapon_EmptyClick", 0.04f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 90f);
            return (Random.value * 2f - 1f) * env * 0.7f;
        });

        // Weapon swap: equipment rustle & sling clink
        clipWeaponSwap = CreateSynthesizedClip("Weapon_Swap", 0.15f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 25f);
            float clink = Mathf.Sin(2f * Mathf.PI * 950f * t) * 0.4f + (Random.value * 2f - 1f) * 0.3f;
            return clink * env;
        });

        // Hit marker confirm
        clipHitMarker = CreateSynthesizedClip("Weapon_HitMarker", 0.06f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 60f);
            return Mathf.Sin(2f * Mathf.PI * 1850f * t) * env * 0.5f;
        });
    }

    private static AudioClip CreateSynthesizedClip(string name, float duration, System.Func<float, float, float> generator)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            samples[i] = Mathf.Clamp(generator(t, duration), -1f, 1f);
        }

        AudioClip clip = AudioClip.Create(name, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
