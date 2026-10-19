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

