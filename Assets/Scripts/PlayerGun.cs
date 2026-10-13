using System.Collections;
using UnityEngine;

/// <summary>
/// First-person player gun with hitscan shooting, muzzle-to-impact cover validation,
/// ammunition tracking, reload mechanics, recoil, restrained muzzle flash,
/// procedural audio, and HUD feedback.
/// </summary>
public class PlayerGun : MonoBehaviour
{
    [Header("Gun Specifications")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float fireRate = 0.22f;
    [SerializeField] private float maxRange = 50f;
    [SerializeField] private int magazineCapacity = 12;
    [SerializeField] private int currentAmmo = 12;
    [SerializeField] private int reserveAmmo = 36;
    [SerializeField] private float reloadDuration = 1.4f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask shootableLayers = ~0;

    [Header("Visual Mount & Muzzle")]
    [SerializeField] private Transform gunVisualHolder;
    [SerializeField] private Transform muzzlePoint;
    /// <summary>
    /// Assign the Tactical_Pistol FBX/Prefab here in the Inspector.
    /// Replaces AssetDatabase loading so the weapon works in standalone builds.
    /// </summary>
    [SerializeField] private GameObject weaponPrefab;

    private Camera playerCamera;
    private AudioSource audioSource;
    private float lastFireTime = -99f;
    private bool isReloading = false;

    // Procedural Audio Clips
    private AudioClip gunshotClip;
    private AudioClip reloadClip;
    private AudioClip emptyClickClip;
    private AudioClip hitConfirmClip;

    // Recoil spring state
    private Vector3 initialGunLocalPos;
    private Quaternion initialGunLocalRot;
    private float recoilCamPitch = 0f;
    private Light muzzleLight;

    public int CurrentAmmo => currentAmmo;
    public int MagazineCapacity => magazineCapacity;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;
    public bool IsFullMag => currentAmmo >= magazineCapacity;

    public static PlayerGun Instance { get; private set; }

    public void SetWeaponPrefab(GameObject prefab)
    {
        weaponPrefab = prefab;
    }

    void Awake()
    {
        Instance = this;
        playerCamera = GetComponentInParent<Camera>();
        if (playerCamera == null) playerCamera = Camera.main;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D player audio

        CreateProceduralAudioClips();
        SetupGunVisual();
    }

    void Start()
    {
        ResetGun();
    }

    private void SetupGunVisual()
    {
        if (gunVisualHolder == null)
        {
            GameObject holder = new GameObject("Player_Weapon_Holder");
            holder.transform.SetParent(playerCamera.transform, false);
            // Low-ready right-hand position
            holder.transform.localPosition = new Vector3(0.18f, -0.16f, 0.38f);
            holder.transform.localRotation = Quaternion.Euler(2f, -3f, 0f);
            gunVisualHolder = holder.transform;
            initialGunLocalPos = gunVisualHolder.localPosition;
            initialGunLocalRot = gunVisualHolder.localRotation;

            // Resolve weapon: Inspector prefab slot → Resources folder → procedural fallback.
            // No AssetDatabase calls; works equally in editor and standalone builds.
            GameObject resolvedPrefab = weaponPrefab;
            if (resolvedPrefab == null)
                resolvedPrefab = Resources.Load<GameObject>("Weapons/Tactical_Pistol");

            if (resolvedPrefab != null)
            {
                GameObject gunObj = Instantiate(resolvedPrefab, gunVisualHolder, false);
                gunObj.name = "Pistol_Visual";
                gunObj.transform.localPosition = Vector3.zero;
                gunObj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                gunObj.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);

                // Disable colliders on first-person gun visual
                foreach (var c in gunObj.GetComponentsInChildren<Collider>())
                    c.enabled = false;
            }
            else
            {
                // Procedural fallback: grey cuboid stands-in for pistol when no prefab assigned
                GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = "Pistol_Block";
                block.transform.SetParent(gunVisualHolder, false);
                block.transform.localPosition = new Vector3(0f, 0.02f, 0.08f);
                block.transform.localScale = new Vector3(0.04f, 0.06f, 0.22f);
                block.GetComponent<Collider>().enabled = false;
            }

            // Muzzle light & flash anchor
            GameObject muzzleGo = new GameObject("Muzzle_Point");
            muzzleGo.transform.SetParent(gunVisualHolder, false);
            muzzleGo.transform.localPosition = new Vector3(0f, 0.025f, 0.22f);
            muzzlePoint = muzzleGo.transform;

            muzzleLight = muzzleGo.AddComponent<Light>();
            muzzleLight.type = LightType.Point;
            muzzleLight.range = 2.5f;
            muzzleLight.color = new Color(1.0f, 0.75f, 0.35f);
            muzzleLight.intensity = 2.0f;
            muzzleLight.enabled = false;
        }
        else
        {
            initialGunLocalPos = gunVisualHolder.localPosition;
            initialGunLocalRot = gunVisualHolder.localRotation;
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        // Player movement pitches camera; recover recoil spring
        UpdateRecoilRecovery();

        // Fire on Left Mouse Button
        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }

        // Reload on R
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryReload();
        }
    }

    private void UpdateRecoilRecovery()
    {
        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = Vector3.Lerp(gunVisualHolder.localPosition, initialGunLocalPos, Time.deltaTime * 14f);
            gunVisualHolder.localRotation = Quaternion.Slerp(gunVisualHolder.localRotation, initialGunLocalRot, Time.deltaTime * 14f);
        }

        if (recoilCamPitch > 0.001f)
        {
            float recovery = recoilCamPitch * Time.deltaTime * 10f;
            recoilCamPitch -= recovery;
            var pm = GetComponentInParent<PlayerMovement>();
            if (pm != null)
            {
                pm.CameraPitch -= recovery;
            }
        }
    }

    public bool TryShoot()
    {
        if (isReloading) return false;
        if (Time.time - lastFireTime < fireRate) return false;

        if (currentAmmo <= 0)
        {
            PlaySound(emptyClickClip, 0.7f);
            if (reserveAmmo > 0) TryReload();
            return false;
        }

        lastFireTime = Time.time;
        currentAmmo--;

        ExecuteHitscanShot();
        ApplyRecoilAndFlash();
        PlaySound(gunshotClip, 0.85f);

        return true;
    }

    /// <summary>
    /// Executes hitscan from camera crosshair, then validates muzzle-to-impact path
    /// so shots cannot pass through nearby cover or geometry when hugging a wall.
    /// </summary>
    private void ExecuteHitscanShot()
    {
        if (playerCamera == null) return;

        // 1. Crosshair ray from camera center
        Ray crosshairRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Vector3 intendedTarget;
        RaycastHit camHit;

        if (Physics.Raycast(crosshairRay, out camHit, maxRange, shootableLayers, QueryTriggerInteraction.Ignore))
        {
            intendedTarget = camHit.point;
        }
        else
        {
            intendedTarget = crosshairRay.GetPoint(maxRange);
        }

        // 2. Muzzle-to-impact path validation
        Vector3 muzzlePos = muzzlePoint != null ? muzzlePoint.position : playerCamera.transform.position;
        Vector3 toTarget = intendedTarget - muzzlePos;
        float totalDist = toTarget.magnitude;

        RaycastHit actualHit;
        bool hasHit = Physics.Raycast(muzzlePos, toTarget.normalized, out actualHit, totalDist + 0.1f, shootableLayers, QueryTriggerInteraction.Ignore);

        if (hasHit)
        {
            ProcessHit(actualHit);
        }
        else if (camHit.collider != null)
        {
            ProcessHit(camHit);
        }
    }

    private void ProcessHit(RaycastHit hit)
    {
        // Check for Health on hit target
        var health = hit.collider.GetComponentInParent<Health>();
        if (health != null && !health.IsPlayer && !health.IsDead)
        {
            health.TakeDamage(damage, gameObject);
            PlaySound(hitConfirmClip, 0.9f);
            Debug.Log($"[PlayerGun] HIT guard '{health.gameObject.name}' for {damage} damage! Remaining HP: {health.CurrentHealth}");

            // Notify HUD hit marker pulse
            PlayerHUD.TriggerHitMarker();
        }
        else
        {
            // Environment impact
            SpawnImpactEffect(hit.point, hit.normal);
        }
    }

    private void ApplyRecoilAndFlash()
    {
        // Gun kickback and upward pitch
        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = initialGunLocalPos + new Vector3(0f, 0.015f, -0.035f);
            gunVisualHolder.localRotation = initialGunLocalRot * Quaternion.Euler(-3.5f, 0f, 0f);
        }

        // Camera kick
        var pm = GetComponentInParent<PlayerMovement>();
        if (pm != null)
        {
            float kick = 1.1f;
            pm.CameraPitch += kick;
            recoilCamPitch += kick;
        }

        // Restrained muzzle flash
        if (muzzleLight != null)
        {
            StartCoroutine(FlashMuzzleLight());
        }
    }

    private IEnumerator FlashMuzzleLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.045f);
        muzzleLight.enabled = false;
    }

    private void SpawnImpactEffect(Vector3 pos, Vector3 normal)
    {
        // Ephemeral impact spark marker
        GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spark.name = "Impact_Spark";
        spark.transform.position = pos + normal * 0.02f;
        spark.transform.localScale = Vector3.one * 0.08f;
        spark.GetComponent<Collider>().enabled = false;
        var r = spark.GetComponent<Renderer>();
        r.material.color = new Color(1f, 0.9f, 0.4f);
        Destroy(spark, 0.25f);
    }

    public bool TryReload()
    {
        if (isReloading || currentAmmo >= magazineCapacity || reserveAmmo <= 0)
            return false;

        StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        PlaySound(reloadClip, 0.75f);

        // Lower gun slightly during reload
        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = initialGunLocalPos + new Vector3(0.02f, -0.06f, -0.04f);
            gunVisualHolder.localRotation = initialGunLocalRot * Quaternion.Euler(8f, -10f, 0f);
        }

        yield return new WaitForSeconds(reloadDuration);

        int needed = magazineCapacity - currentAmmo;
        int toLoad = Mathf.Min(needed, reserveAmmo);
        currentAmmo += toLoad;
        reserveAmmo -= toLoad;

        isReloading = false;
        Debug.Log($"[PlayerGun] Reloaded! Ammo: {currentAmmo} / {reserveAmmo}");
    }

    public void ResetGun()
    {
        isReloading = false;
        currentAmmo = magazineCapacity;
        reserveAmmo = 36;
        lastFireTime = -99f;
        recoilCamPitch = 0f;
        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = initialGunLocalPos;
            gunVisualHolder.localRotation = initialGunLocalRot;
        }
        if (muzzleLight != null) muzzleLight.enabled = false;
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    private void CreateProceduralAudioClips()
    {
        gunshotClip = CreateSynthesizedClip("Gunshot", 0.16f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 28f);
            float noise = (Random.value * 2f - 1f) * 0.75f;
            float kick = Mathf.Sin(2f * Mathf.PI * (160f - t * 400f) * t) * 0.4f;
            return (noise + kick) * env;
        });

        reloadClip = CreateSynthesizedClip("Reload", 0.35f, (t, dur) =>
        {
            float click1 = (t > 0.05f && t < 0.12f) ? (Random.value * 2f - 1f) * Mathf.Sin((t - 0.05f) * 1200f) : 0f;
            float click2 = (t > 0.22f && t < 0.30f) ? (Random.value * 2f - 1f) * Mathf.Sin((t - 0.22f) * 1600f) : 0f;
            return click1 + click2;
        });

        emptyClickClip = CreateSynthesizedClip("EmptyClick", 0.05f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 80f);
            return (Random.value * 2f - 1f) * env * 0.6f;
        });

        hitConfirmClip = CreateSynthesizedClip("HitMarker", 0.06f, (t, dur) =>
        {
            float env = Mathf.Exp(-t * 60f);
            return Mathf.Sin(2f * Mathf.PI * 1800f * t) * env * 0.5f;
        });
    }

    private AudioClip CreateSynthesizedClip(string name, float duration, System.Func<float, float, float> generator)
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
