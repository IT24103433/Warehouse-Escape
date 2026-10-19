using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// First-person player gun with continuous automatic firing, hitscan shooting,
/// muzzle-to-impact cover validation, ammunition tracking, weapon swapping,
/// recoil recovery, procedural audio, and HUD integration.
/// Defaults to original Blender MP5-style SMG.
/// </summary>
public class PlayerGun : MonoBehaviour
{
    public static PlayerGun Instance { get; private set; }

    [Header("Current Weapon State")]
    [SerializeField] private WeaponType currentWeaponType = WeaponType.MP5_SMG;
    [SerializeField] private WeaponData currentWeaponData;
    [SerializeField] private int currentAmmo = 30;
    [SerializeField] private int reserveAmmo = 90;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask shootableLayers = ~0;

    [Header("Visual Mount & Muzzle")]
    [SerializeField] private Transform gunVisualHolder;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject weaponPrefab;

    private Camera playerCamera;
    private Health playerHealth;
    private AudioSource audioSource;
    private AudioSource reloadAudioSource;
    private float lastFireTime = -99f;
    private bool isReloading = false;
    private Coroutine reloadCoroutine;
    private GameObject currentGunVisualObj;

    // Recoil spring state
    private Vector3 initialGunLocalPos = new Vector3(0.18f, -0.16f, 0.38f);
    private Quaternion initialGunLocalRot = Quaternion.identity;
    private float recoilCamPitch = 0f;
    private Light muzzleLight;

    public WeaponType CurrentWeaponType => currentWeaponType;
    public WeaponType CurrentWeapon => currentWeaponType;
    public WeaponData CurrentWeaponData => currentWeaponData;
    public int CurrentAmmo => currentAmmo;
    public int MagazineCapacity => currentWeaponData != null ? currentWeaponData.magazineCapacity : 30;
    public int ReserveAmmo => reserveAmmo;
    public bool HasUnlimitedReserve => true;
    public bool IsReloading => isReloading;
    public bool IsFullMag => currentAmmo >= MagazineCapacity;
    public string CurrentWeaponName => currentWeaponData != null ? currentWeaponData.weaponName : "MP5-A3 SMG";
    public Transform GunTransform => gunVisualHolder;
    public AudioSource ReloadAudioSource => reloadAudioSource;
    public AudioSource ShootAudioSource => audioSource;

    void Awake()
    {
        Instance = this;
        playerCamera = GetComponentInParent<Camera>();
        if (playerCamera == null) playerCamera = Camera.main;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDied += _ => CancelReload();
            }
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D player audio

        reloadAudioSource = gameObject.AddComponent<AudioSource>();
        reloadAudioSource.playOnAwake = false;
        reloadAudioSource.spatialBlend = 0f;

        WeaponAudio.EnsureClips();
        currentWeaponData = WeaponData.CreateMP5();
        currentAmmo = currentWeaponData.magazineCapacity;
        reserveAmmo = int.MaxValue; // Player has unlimited reserve ammunition

        SetupGunVisual();
    }

    void Start()
    {
        ResetGun();
    }

    public void SetWeaponPrefab(GameObject prefab)
    {
        weaponPrefab = prefab;
    }

    private void SetupGunVisual()
    {
        if (gunVisualHolder == null)
        {
            GameObject holder = new GameObject("Player_Weapon_Holder");
            holder.transform.SetParent(playerCamera.transform, false);
            holder.transform.localPosition = initialGunLocalPos;
            holder.transform.localRotation = initialGunLocalRot;
            gunVisualHolder = holder.transform;
        }

        InstantiateWeaponVisual();
    }

    private void InstantiateWeaponVisual()
    {
        if (currentGunVisualObj != null)
        {
            Destroy(currentGunVisualObj);
            currentGunVisualObj = null;
        }

        // Resolve prefab
        GameObject resolved = weaponPrefab;
        if (resolved == null && currentWeaponData != null)
        {
            resolved = Resources.Load<GameObject>(currentWeaponData.prefabPath);
        }

        if (resolved != null)
        {
            currentGunVisualObj = Instantiate(resolved, gunVisualHolder, false);
            currentGunVisualObj.name = $"{currentWeaponType}_Visual";
            // Align model directly forward (+Z) so barrel points straight toward screen center / crosshair
            currentGunVisualObj.transform.localPosition = Vector3.zero;
            currentGunVisualObj.transform.localRotation = Quaternion.identity;
            currentGunVisualObj.transform.localScale = Vector3.one;

            // Strip any colliders from view model
            foreach (var col in currentGunVisualObj.GetComponentsInChildren<Collider>())
                col.enabled = false;
        }
        else
        {
            // Clean procedural placeholder
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = "Placeholder_Gun";
            block.transform.SetParent(gunVisualHolder, false);
            block.transform.localPosition = new Vector3(0f, 0.02f, 0.12f);
            block.transform.localRotation = Quaternion.identity;
            block.transform.localScale = new Vector3(0.04f, 0.06f, 0.28f);
            block.GetComponent<Collider>().enabled = false;
            currentGunVisualObj = block;
        }

        // Setup Muzzle point at barrel front
        Transform existingMuzzle = currentGunVisualObj.transform.Find("MuzzlePoint");
        if (existingMuzzle != null)
        {
            muzzlePoint = existingMuzzle;
        }
        else
        {
            GameObject muzzleGo = new GameObject("Muzzle_Point");
            muzzleGo.transform.SetParent(gunVisualHolder, false);
            // Default barrel front offset
            float forwardOffset = currentWeaponData != null && currentWeaponData.isShotgun ? 0.55f : 0.38f;
            muzzleGo.transform.localPosition = new Vector3(0f, 0.03f, forwardOffset);
            muzzlePoint = muzzleGo.transform;
        }

        // Muzzle light
        if (muzzleLight == null)
        {
            GameObject lightGo = new GameObject("Muzzle_Flash_Light");
            lightGo.transform.SetParent(muzzlePoint, false);
            lightGo.transform.localPosition = Vector3.zero;
            muzzleLight = lightGo.AddComponent<Light>();
            muzzleLight.type = LightType.Point;
            muzzleLight.range = 3.0f;
            muzzleLight.color = new Color(1.0f, 0.80f, 0.40f);
            muzzleLight.intensity = 2.5f;
            muzzleLight.enabled = false;
        }
    }

    void Update()
    {
        if (playerHealth == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerHealth = p.GetComponent<Health>();
        }

        // Stop all firing and cancel pending reload immediately when dead, paused, or game ended
        if (playerHealth != null && playerHealth.IsDead)
        {
            CancelReload();
            return;
        }
        if (Time.timeScale == 0f)
        {
            if (reloadAudioSource != null && reloadAudioSource.isPlaying)
                reloadAudioSource.Pause();
            return;
        }
        else
        {
            if (reloadAudioSource != null && !reloadAudioSource.isPlaying && isReloading)
                reloadAudioSource.UnPause();
        }
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
        {
            CancelReload();
            return;
        }

        UpdateRecoilRecovery();
        HandleWeaponPickupInteraction();

        // Automatic Continuous Firing vs Semi-Automatic
        if (currentWeaponData != null && currentWeaponData.isAutomatic)
        {
            // Continuous firing: hold Left Mouse Button
            if (Input.GetMouseButton(0))
            {
                TryShoot();
            }
        }
        else
        {
            // Semi-automatic / pump shotgun: click Left Mouse Button
            if (Input.GetMouseButtonDown(0))
            {
                TryShoot();
            }
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
            float recovery = recoilCamPitch * Time.deltaTime * 12f;
            recoilCamPitch -= recovery;
            var pm = GetComponentInParent<PlayerMovement>();
            if (pm != null)
            {
                pm.CameraPitch -= recovery;
            }
        }
    }

    private void HandleWeaponPickupInteraction()
    {
        // Check for nearby dropped weapon pickup
        WeaponPickup nearbyPickup = null;
        var pickups = WeaponPickup.ActivePickups;
        for (int i = 0; i < pickups.Count; i++)
        {
            if (pickups[i] != null && pickups[i].IsPlayerLooking)
            {
                nearbyPickup = pickups[i];
                break;
            }
        }

        if (nearbyPickup != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                nearbyPickup.TrySwap(this);
                PlaySound(WeaponAudio.GetWeaponSwapClip(), 0.9f);
            }
        }
    }

    public string GetNearbyPickupPrompt()
    {
        var pickups = WeaponPickup.ActivePickups;
        for (int i = 0; i < pickups.Count; i++)
        {
            if (pickups[i] != null && pickups[i].IsPlayerLooking)
            {
                return pickups[i].GetPrompt(this);
            }
        }
        return "";
    }

    public bool TryShoot()
    {
        if (isReloading) return false;
        if (playerHealth != null && playerHealth.IsDead) return false;
        if (Time.timeScale == 0f) return false;
        if (GameManager.Instance != null && (GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused)) return false;

        float rate = currentWeaponData != null ? currentWeaponData.fireRate : 0.11f;
        if (Time.time - lastFireTime < rate) return false;

        if (currentAmmo <= 0)
        {
            PlaySound(WeaponAudio.GetEmptyClickClip(), 0.75f);
            if (reserveAmmo > 0) TryReload();
            return false;
        }

        lastFireTime = Time.time;
        currentAmmo--;

        if (currentWeaponData != null && currentWeaponData.isShotgun)
        {
            ExecuteShotgunShot();
        }
        else
        {
            ExecuteHitscanShot();
        }

        // Play distinct acoustic profile tailored for current weapon
        PlaySound(WeaponAudio.GetWeaponFireClip(currentWeaponType), 0.85f);

        ApplyRecoilAndFlash();
        return true;
    }

    private void ExecuteHitscanShot()
    {
        if (playerCamera == null) return;

        // Broadcast gunshot audio disturbance to guard perception
        GuardPerception.ReportSound(transform.position, 32f, SoundType.Gunshot, gameObject);

        float range = currentWeaponData != null ? currentWeaponData.maxRange : 50f;
        int dmg = currentWeaponData != null ? currentWeaponData.damage : 18;
        float spr = currentWeaponData != null ? currentWeaponData.spread : 1.5f;

        // Spread vector
        Vector3 aimDir = playerCamera.transform.forward;
        if (spr > 0.01f)
        {
            aimDir = Quaternion.Euler(
                Random.Range(-spr * 0.4f, spr * 0.4f),
                Random.Range(-spr * 0.4f, spr * 0.4f),
                0f
            ) * aimDir;
        }

        // 1. Crosshair ray from camera center
        Ray crosshairRay = new Ray(playerCamera.transform.position, aimDir);
        bool hasCamHit = RaycastNonPlayer(crosshairRay, range, out RaycastHit camHit);

        Vector3 intendedTarget = hasCamHit ? camHit.point : crosshairRay.GetPoint(range);

        // 2. Muzzle-to-impact path validation
        Vector3 muzzlePos = muzzlePoint != null ? muzzlePoint.position : playerCamera.transform.position;
        Vector3 toTarget = intendedTarget - muzzlePos;
        float totalDist = toTarget.magnitude;

        if (totalDist < 0.05f) return;

        bool hasMuzzleHit = RaycastNonPlayer(new Ray(muzzlePos, toTarget.normalized), totalDist + 0.15f, out RaycastHit muzzleHit);

        if (hasMuzzleHit && hasCamHit)
        {
            var muzzleHealth = muzzleHit.collider.GetComponentInParent<Health>();
            if (muzzleHit.distance < totalDist - 0.25f && (muzzleHealth == null || muzzleHealth.IsPlayer))
            {
                Debug.Log($"[PlayerGun] Shot BLOCKED by cover: '{muzzleHit.collider.gameObject.name}' at distance {muzzleHit.distance:F1}m.");
                SpawnImpactEffect(muzzleHit.point, muzzleHit.normal);
                return;
            }
            ProcessHit(muzzleHit, dmg);
        }
        else if (hasMuzzleHit)
        {
            ProcessHit(muzzleHit, dmg);
        }
        else if (hasCamHit)
        {
            ProcessHit(camHit, dmg);
        }
    }

    private void ExecuteShotgunShot()
    {
        if (playerCamera == null) return;
        GuardPerception.ReportSound(transform.position, 34f, SoundType.Gunshot, gameObject);

        int pellets = currentWeaponData != null ? currentWeaponData.pelletCount : 6;
        int dmgPerPellet = currentWeaponData != null ? currentWeaponData.damage : 7;
        float spread = currentWeaponData != null ? currentWeaponData.pelletSpread : 4.5f;
        float range = currentWeaponData != null ? currentWeaponData.maxRange : 28f;

        Vector3 muzzlePos = muzzlePoint != null ? muzzlePoint.position : playerCamera.transform.position;

        for (int p = 0; p < pellets; p++)
        {
            Vector3 pelletDir = Quaternion.Euler(
                Random.Range(-spread, spread),
                Random.Range(-spread, spread),
                0f
            ) * playerCamera.transform.forward;

            Ray camRay = new Ray(playerCamera.transform.position, pelletDir);
            bool hasCamHit = RaycastNonPlayer(camRay, range, out RaycastHit camHit);
            Vector3 targetPt = hasCamHit ? camHit.point : camRay.GetPoint(range);

            Vector3 toTarget = targetPt - muzzlePos;
            if (toTarget.sqrMagnitude < 0.01f) continue;

            if (RaycastNonPlayer(new Ray(muzzlePos, toTarget.normalized), toTarget.magnitude + 0.1f, out RaycastHit muzzleHit))
            {
                ProcessHit(muzzleHit, dmgPerPellet);
            }
            else if (hasCamHit)
            {
                ProcessHit(camHit, dmgPerPellet);
            }
        }
    }

    private bool RaycastNonPlayer(Ray ray, float maxDist, out RaycastHit closestValidHit)
    {
        closestValidHit = default;
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDist, shootableLayers, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0) return false;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Transform playerRoot = transform.root;
        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            if (h.collider == null) continue;
            if (h.transform == playerRoot || h.transform.IsChildOf(playerRoot)) continue;
            var hp = h.collider.GetComponentInParent<Health>();
            if (hp != null && hp.IsPlayer) continue;

            closestValidHit = h;
            return true;
        }

        return false;
    }

    private void ProcessHit(RaycastHit hit, int dmg)
    {
        var health = hit.collider.GetComponentInParent<Health>();
        if (health != null && !health.IsPlayer && !health.IsDead)
        {
            health.TakeDamage(dmg, gameObject);
            PlaySound(WeaponAudio.GetHitMarkerClip(), 0.9f);
            PlayerHUD.TriggerHitMarker();
        }
        else
        {
            SpawnImpactEffect(hit.point, hit.normal);
        }
    }

    private void ApplyRecoilAndFlash()
    {
        float kickPitch = currentWeaponData != null ? currentWeaponData.recoilPitch : 0.75f;

        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = initialGunLocalPos + new Vector3(0f, 0.012f, -0.030f);
            gunVisualHolder.localRotation = initialGunLocalRot * Quaternion.Euler(-2.5f, 0f, 0f);
        }

        var pm = GetComponentInParent<PlayerMovement>();
        if (pm != null)
        {
            pm.CameraPitch += kickPitch;
            recoilCamPitch += kickPitch;
        }

        if (muzzleLight != null)
        {
            StartCoroutine(FlashMuzzleLight());
        }
    }

    private IEnumerator FlashMuzzleLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.04f);
        muzzleLight.enabled = false;
    }

    private void SpawnImpactEffect(Vector3 pos, Vector3 normal)
    {
        GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spark.name = "Impact_Spark";
        spark.transform.position = pos + normal * 0.02f;
        spark.transform.localScale = Vector3.one * 0.07f;
        spark.GetComponent<Collider>().enabled = false;
        var r = spark.GetComponent<Renderer>();
        r.material.color = new Color(1f, 0.85f, 0.35f);
        Destroy(spark, 0.2f);
    }

    public bool TryReload()
    {
        if (isReloading || currentAmmo >= MagazineCapacity)
            return false;

        reloadCoroutine = StartCoroutine(ReloadRoutine());
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (reloadAudioSource != null)
        {
            reloadAudioSource.clip = WeaponAudio.GetReloadClip(currentWeaponType);
            reloadAudioSource.volume = 0.85f;
            reloadAudioSource.Play();
        }

        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = initialGunLocalPos + new Vector3(0.015f, -0.05f, -0.03f);
            gunVisualHolder.localRotation = initialGunLocalRot * Quaternion.Euler(6f, -8f, 0f);
        }

        float dur = currentWeaponData != null ? currentWeaponData.reloadDuration : 1.5f;
        yield return new WaitForSeconds(dur);

        currentAmmo = MagazineCapacity; // Refill full magazine from unlimited reserve pool

        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = initialGunLocalPos;
            gunVisualHolder.localRotation = initialGunLocalRot;
        }

        isReloading = false;
        reloadCoroutine = null;
        if (reloadAudioSource != null && reloadAudioSource.isPlaying)
        {
            reloadAudioSource.Stop();
        }
        Debug.Log($"[PlayerGun] Reload completed: {currentAmmo} / \u221E");
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        if (reloadAudioSource != null && reloadAudioSource.isPlaying)
        {
            reloadAudioSource.Stop();
        }
        isReloading = false;
        if (gunVisualHolder != null)
        {
            gunVisualHolder.localPosition = initialGunLocalPos;
            gunVisualHolder.localRotation = initialGunLocalRot;
        }
    }

    void OnDisable()
    {
        CancelReload();
    }

    /// <summary>
    /// Equips a new weapon onto the player with specified ammo state.
    /// Player always receives unlimited reserve ammo for the equipped weapon.
    /// </summary>
    public void EquipWeapon(WeaponType type, int curAmmo, int resAmmo)
    {
        CancelReload();
        lastFireTime = -99f;
        currentWeaponType = type;
        currentWeaponData = WeaponData.GetConfig(type);
        currentAmmo = curAmmo;
        reserveAmmo = int.MaxValue; // Unlimited player reserve ammunition
        weaponPrefab = Resources.Load<GameObject>(currentWeaponData.prefabPath);

        InstantiateWeaponVisual();
        Debug.Log($"[PlayerGun] Equipped {currentWeaponData.weaponName} with {currentAmmo} mag / \u221E reserve.");
    }

    public void ResetGun()
    {
        CancelReload();
        EquipWeapon(WeaponType.MP5_SMG, 30, int.MaxValue);
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
}
