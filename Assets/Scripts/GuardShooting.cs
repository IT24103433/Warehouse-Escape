using System.Collections;
using UnityEngine;

/// <summary>
/// Guard weapon controller attaching distinct firearms to hands, aiming at visible targets,
/// and executing balanced combat shooting with strict line-of-sight and cover occlusion.
/// Features:
/// - Distinct weapon assignments: UMP-45 (Varun), AK-47 (Sanjeevsaran), Shotgun (Aqqil), SCAR (Jadavan).
/// - Correct forward barrel alignment in hand without upward skew.
/// - Reaction delay and visible aim preparation before opening fire.
/// - Shotgun bounded pellet dispersion with balanced total damage.
/// - Drops physical WeaponPickup matching assigned weapon on defeat.
/// </summary>
public class GuardShooting : MonoBehaviour
{
    [Header("Weapon Configuration")]
    [SerializeField] private WeaponType assignedWeapon = WeaponType.UMP_SMG;
    [SerializeField] private int damagePerShot = 14;
    [SerializeField] private float fireCooldown = 1.1f;
    [SerializeField] private float maxFiringRange = 14f;
    [SerializeField] private int burstCount = 2;
    [SerializeField] private float burstInterval = 0.14f;
    [SerializeField] private float accuracySpread = 2.5f;
    [SerializeField] private int pelletCount = 1;

    [Header("Combat Reaction & Reload Settings")]
    [Tooltip("Delay in seconds between visually acquiring the target and pulling the trigger (aim preparation).")]
    [SerializeField] private float reactionDelay = 0.35f;

    [Tooltip("Maximum burst cycles before initiating a tactical reload.")]
    [SerializeField] private int magazineBursts = 4;

    [Tooltip("Duration in seconds of tactical reload.")]
    [SerializeField] private float reloadDuration = 1.4f;

    [Header("Weapon Prefab")]
    [SerializeField] private GameObject weaponPrefab;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask obstacleMask = ~0;

    private Transform playerTransform;
    private Health health;
    private AudioSource audioSource;
    private Transform gunTransform;
    private Transform muzzlePoint;
    private Light muzzleLight;

    private float lastShotTime = -99f;
    private bool isShootingBurst = false;
    private bool isReloading = false;
    private float targetVisibleDuration = 0f;
    private int burstsRemaining = 4;
    private bool hasDroppedWeapon = false;
    private IGuardPerceptionTracker perceptionTracker;
    private PatrolGuardAI patrolController;

    public WeaponType AssignedWeapon => assignedWeapon;
    public int DamagePerShot => damagePerShot;
    public int PelletCount => pelletCount;
    public int BurstCount => burstCount;
    public float FireCooldown => fireCooldown;
    public float ReactionDelay => reactionDelay;
    public bool HasVisualTarget => perceptionTracker != null && perceptionTracker.IsGuardActive()
        && perceptionTracker.HasConfirmedVisualContact()
        && (patrolController == null || patrolController.CurrentState == PatrolGuardState.Pursue);
    public bool IsAiming => targetVisibleDuration > 0.05f;
    public bool IsReloading => isReloading;
    public Transform GunTransform => gunTransform;

    void Awake()
    {
        health = GetComponent<Health>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D positional audio
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 3.0f;
        audioSource.maxDistance = 35.0f;
        audioSource.dopplerLevel = 0f;

        burstsRemaining = magazineBursts;
        WeaponAudio.EnsureClips();
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += HandleDeath;
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.OnDied -= HandleDeath;
        }
    }

    void Start()
    {
        patrolController = GetComponent<PatrolGuardAI>();
        foreach (var behaviour in GetComponents<MonoBehaviour>())
            if (behaviour is IGuardPerceptionTracker tracker) { perceptionTracker = tracker; break; }
        FindPlayer();
        AttachWeaponToHand();
    }

    private void FindPlayer()
    {
        if (playerTransform == null)
        {
            var pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) playerTransform = pObj.transform;
        }
    }

    /// <summary>
    /// Attaches the original Blender weapon model to the guard's right hand (Hand_R).
    /// Aligns weapon barrel pointing straight forward (+Z).
    /// </summary>
    public void AttachWeaponToHand()
    {
        if (gunTransform != null) return;

        // Locate Hand_R in Character_Visuals hierarchy
        Transform handR = FindDeepChild(transform, "Hand_R");
        if (handR == null) handR = transform;

        // Resolve weapon prefab: serialized slot -> Resources folder -> procedural fallback
        GameObject resolvedPrefab = weaponPrefab;
        if (resolvedPrefab == null)
        {
            string prefabPath = WeaponData.GetConfig(assignedWeapon).prefabPath;
            resolvedPrefab = Resources.Load<GameObject>(prefabPath);
        }

        GameObject gunGo;
        if (resolvedPrefab != null)
        {
            gunGo = Instantiate(resolvedPrefab, handR, false);
        }
        else
        {
            gunGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gunGo.transform.SetParent(handR, false);
            gunGo.transform.localScale = new Vector3(0.04f, 0.06f, 0.35f);
        }

        gunGo.name = $"Guard_{assignedWeapon}";
        gunTransform = gunGo.transform;

        // Position and orient weapon naturally in right hand pointing forward (+Z relative to guard hand)
        // With Blender FBX exported (-Z forward, Y up), identity or subtle grip alignment points directly forward!
        gunTransform.localPosition = new Vector3(0.02f, -0.04f, 0.12f);
        gunTransform.localRotation = Quaternion.identity;
        gunTransform.localScale = Vector3.one * 0.95f;

        // Disable colliders on held weapon
        foreach (var col in gunGo.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // Muzzle point anchor at barrel front
        Transform existingMuzzle = gunGo.transform.Find("MuzzlePoint");
        if (existingMuzzle != null)
        {
            muzzlePoint = existingMuzzle;
        }
        else
        {
            GameObject muzzleGo = new GameObject("Guard_Muzzle");
            muzzleGo.transform.SetParent(gunTransform, false);
            float fwd = (assignedWeapon == WeaponType.Combat_Shotgun || assignedWeapon == WeaponType.AK47_Rifle) ? 0.52f : 0.32f;
            muzzleGo.transform.localPosition = new Vector3(0f, 0.03f, fwd);
            muzzlePoint = muzzleGo.transform;
        }

        // Muzzle light
        GameObject lightGo = new GameObject("Muzzle_Light");
        lightGo.transform.SetParent(muzzlePoint, false);
        lightGo.transform.localPosition = Vector3.zero;
        muzzleLight = lightGo.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.range = 3.0f;
        muzzleLight.color = new Color(1.0f, 0.70f, 0.30f);
        muzzleLight.intensity = 2.5f;
        muzzleLight.enabled = false;
    }

    void Update()
    {
        if (health != null && health.IsDead) return;
        if (GameManager.Instance != null && (GameManager.Instance.IsGameEnded || GameManager.Instance.IsPaused)) return;

        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        // Safe Room Protection: Never target or shoot into Room 1 (Z < -18.0)
        if (transform.position.z < -18.0f || playerTransform.position.z < -18.0f)
        {
            targetVisibleDuration = 0f;
            return;
        }

        // Hearing supplies investigation targets; only confirmed vision permits combat.
        if (!HasVisualTarget)
        {
            targetVisibleDuration = 0f;
            return;
        }

        // Evaluate target distance
        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distToPlayer > maxFiringRange)
        {
            targetVisibleDuration = 0f;
            return;
        }

        // Check unoccluded line of sight
        if (!HasClearLineOfSight(playerTransform.position))
        {
            targetVisibleDuration = Mathf.Max(0f, targetVisibleDuration - Time.deltaTime * 2f);
            return;
        }

        // Aim weapon towards visible target
        AimGunAtTarget(playerTransform.position + Vector3.up * 1.0f);

        // Accumulate visible aim preparation time
        targetVisibleDuration += Time.deltaTime;

        // Must satisfy reaction delay (aim preparation) before opening fire
        if (targetVisibleDuration < reactionDelay)
            return;

        // Reload check
        if (burstsRemaining <= 0 && !isReloading && !isShootingBurst)
        {
            StartCoroutine(ReloadRoutine());
            return;
        }

        // Firing cooldown check
        if (!isShootingBurst && !isReloading && Time.time - lastShotTime >= fireCooldown)
        {
            StartCoroutine(FireBurstRoutine());
        }
    }

    private void AimGunAtTarget(Vector3 targetWorldPos)
    {
        if (gunTransform != null)
        {
            Vector3 toTarget = targetWorldPos - gunTransform.position;
            if (toTarget.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized);
                gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, targetRot, Time.deltaTime * 12f);
            }
        }
    }

    public bool HasClearLineOfSight(Vector3 targetPos)
    {
        Vector3 origin = muzzlePoint != null ? muzzlePoint.position : transform.position + Vector3.up * 1.5f;
        Vector3 dest = targetPos + Vector3.up * 1.0f;
        Vector3 dir = dest - origin;
        float dist = dir.magnitude;

        if (dist < 0.2f) return true;

        RaycastHit[] hits = Physics.RaycastAll(new Ray(origin, dir.normalized), dist, obstacleMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var h in hits)
        {
            if (h.transform == transform || h.transform.IsChildOf(transform))
                continue;

            if (h.transform == playerTransform || h.transform.IsChildOf(playerTransform) || h.collider.CompareTag("Player"))
                return true;

            return false;
        }

        return true;
    }

    private IEnumerator FireBurstRoutine()
    {
        isShootingBurst = true;
        lastShotTime = Time.time;
        burstsRemaining--;

        try
        {
            for (int i = 0; i < burstCount; i++)
            {
                if (health != null && health.IsDead) yield break;
                if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) yield break;
                if (playerTransform == null || playerTransform.position.z < -18.0f) yield break;

                if (!HasVisualTarget) yield break;
                if (HasClearLineOfSight(playerTransform.position))
                {
                    if (assignedWeapon == WeaponType.Combat_Shotgun)
                    {
                        ExecuteShotgunShot();
                    }
                    else
                    {
                        ExecuteSingleShot();
                    }
                }

                if (burstCount > 1 && i < burstCount - 1)
                {
                    yield return new WaitForSeconds(burstInterval);
                }
            }

        }
        finally { isShootingBurst = false; }
    }

    private void ExecuteSingleShot()
    {
        Vector3 origin = muzzlePoint != null ? muzzlePoint.position : transform.position + Vector3.up * 1.5f;
        Vector3 targetPoint = playerTransform.position + Vector3.up * 1.0f;

        Vector3 dir = (targetPoint - origin).normalized;
        Vector3 spreadDir = Quaternion.Euler(
            Random.Range(-accuracySpread, accuracySpread),
            Random.Range(-accuracySpread, accuracySpread),
            0f
        ) * dir;

        GuardPerception.ReportSound(origin, 32f, SoundType.Gunshot, gameObject);

        RaycastHit[] hits = Physics.RaycastAll(new Ray(origin, spreadDir), maxFiringRange + 5f, obstacleMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var h in hits)
        {
            if (h.transform == transform || h.transform.IsChildOf(transform))
                continue;

            if (h.transform == playerTransform || h.transform.IsChildOf(playerTransform) || h.collider.CompareTag("Player"))
            {
                var playerHealth = playerTransform.GetComponent<Health>();
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damagePerShot, gameObject);
                    Debug.Log($"[GuardShooting] Guard '{gameObject.name}' ({assignedWeapon}) hit player for {damagePerShot} damage! (Player HP: {playerHealth.CurrentHealth})");
                }
            }
            else
            {
                Debug.Log($"[GuardShooting] Guard '{gameObject.name}' shot blocked by cover: '{h.collider.gameObject.name}' at distance {h.distance:F1}m.");
            }
            break;
        }

        if (muzzleLight != null) StartCoroutine(FlashMuzzleLight());
        if (audioSource != null)
        {
            AudioClip clip = WeaponAudio.GetWeaponFireClip(assignedWeapon);
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clip, 0.80f);
        }
    }

    private void ExecuteShotgunShot()
    {
        Vector3 origin = muzzlePoint != null ? muzzlePoint.position : transform.position + Vector3.up * 1.5f;
        Vector3 targetPoint = playerTransform.position + Vector3.up * 1.0f;
        Vector3 dir = (targetPoint - origin).normalized;

        GuardPerception.ReportSound(origin, 34f, SoundType.Gunshot, gameObject);

        int pellets = 6;
        int dmgPerPellet = 7; // Total max 42 damage point blank
        float pelletSpread = 4.5f;

        for (int p = 0; p < pellets; p++)
        {
            Vector3 spreadDir = Quaternion.Euler(
                Random.Range(-pelletSpread, pelletSpread),
                Random.Range(-pelletSpread, pelletSpread),
                0f
            ) * dir;

            RaycastHit[] hits = Physics.RaycastAll(new Ray(origin, spreadDir), maxFiringRange + 2f, obstacleMask, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var h in hits)
            {
                if (h.transform == transform || h.transform.IsChildOf(transform))
                    continue;

                if (h.transform == playerTransform || h.transform.IsChildOf(playerTransform) || h.collider.CompareTag("Player"))
                {
                    var playerHealth = playerTransform.GetComponent<Health>();
                    if (playerHealth != null && !playerHealth.IsDead)
                    {
                        playerHealth.TakeDamage(dmgPerPellet, gameObject);
                    }
                }
                break;
            }
        }

        if (muzzleLight != null) StartCoroutine(FlashMuzzleLight());
        if (audioSource != null)
        {
            AudioClip clip = WeaponAudio.GetWeaponFireClip(assignedWeapon);
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clip, 0.90f);
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (audioSource != null)
        {
            AudioClip clip = WeaponAudio.GetReloadClip(assignedWeapon);
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(clip, 0.75f);
        }
        yield return new WaitForSeconds(reloadDuration);
        burstsRemaining = magazineBursts;
        isReloading = false;
    }

    private IEnumerator FlashMuzzleLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.04f);
        muzzleLight.enabled = false;
    }

    private void HandleDeath(GameObject killer)
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (hasDroppedWeapon) return;
        hasDroppedWeapon = true;

        // Cease combat immediately
        StopAllCoroutines();
        isShootingBurst = false;
        isReloading = false;
        enabled = false;

        // Hide held weapon on corpse
        if (gunTransform != null)
        {
            gunTransform.gameObject.SetActive(false);
        }

        // Drop physical weapon pickup for player
        var cfg = WeaponData.GetConfig(assignedWeapon);
        int mag = cfg.magazineCapacity;
        int res = cfg.defaultReserve / 2;
        WeaponPickup.SpawnDroppedWeapon(assignedWeapon, transform.position + Vector3.up * 0.25f, mag, res);
        Debug.Log($"[GuardShooting] Guard '{gameObject.name}' dropped {assignedWeapon} pickup on death.");
    }

    public void ResetCombat()
    {
        hasDroppedWeapon = false;
        burstsRemaining = magazineBursts;
        isShootingBurst = false;
        isReloading = false;
        targetVisibleDuration = 0f;
        if (gunTransform != null)
        {
            gunTransform.gameObject.SetActive(true);
            gunTransform.localPosition = new Vector3(0.02f, -0.04f, 0.12f);
            gunTransform.localRotation = Quaternion.identity;
        }
    }

    public void ConfigureWeapon(WeaponType type, int dmg, float cooldown, float range, int bursts, float spread, int pellets = 1, GameObject prefab = null)
    {
        assignedWeapon = type;
        damagePerShot = dmg;
        fireCooldown = cooldown;
        maxFiringRange = range;
        burstCount = bursts;
        accuracySpread = spread;
        pelletCount = pellets;
        magazineBursts = (type == WeaponType.SCAR_Rifle) ? 5 : (type == WeaponType.AK47_Rifle ? 4 : (type == WeaponType.Combat_Shotgun ? 4 : 3));
        burstsRemaining = magazineBursts;
        if (prefab != null)
        {
            weaponPrefab = prefab;
            if (gunTransform != null)
            {
                Destroy(gunTransform.gameObject);
                gunTransform = null;
                AttachWeaponToHand();
            }
        }
    }

    public void ConfigureRole(WeaponType type, int dmg, float cooldown, float range, int bursts, float spread, int pellets = 1, GameObject prefab = null)
    {
        ConfigureWeapon(type, dmg, cooldown, range, bursts, spread, pellets, prefab);
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;
            Transform result = FindDeepChild(child, childName);
            if (result != null) return result;
        }
        return null;
    }
}
