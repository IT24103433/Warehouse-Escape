using System.Collections;
using UnityEngine;

/// <summary>
/// Guard weapon controller attaching guns to hands, aiming at visible targets,
/// and executing balanced combat shooting with strict line-of-sight and cover occlusion.
/// Walls, shelving, crates, and safe-room boundaries strictly block shots.
/// </summary>
public class GuardShooting : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol,
        Rifle,
        BossRifle
    }

    [Header("Weapon Configuration")]
    [SerializeField] private WeaponType weaponType = WeaponType.Pistol;
    [SerializeField] private int damagePerShot = 12;
    [SerializeField] private float fireCooldown = 1.1f;
    [SerializeField] private float maxFiringRange = 12f;
    [SerializeField] private int burstCount = 1;
    [SerializeField] private float burstInterval = 0.15f;
    [SerializeField] private float accuracySpread = 3.5f;

    [Header("Weapon Prefabs (assign in Inspector for standalone builds)")]
    /// <summary>Assign Tactical_Pistol FBX/Prefab here. Used when weaponType == Pistol.</summary>
    [SerializeField] private GameObject pistolPrefab;
    /// <summary>Assign Security_Rifle FBX/Prefab here. Used when weaponType == Rifle or BossRifle.</summary>
    [SerializeField] private GameObject riflePrefab;

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

    private static AudioClip guardGunshotClip;

    void Awake()
    {
        health = GetComponent<Health>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D positional audio
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 35f;

        EnsureGuardAudioClip();
    }

    void Start()
    {
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
    /// Attaches the Blender gun model to the guard's right hand (Hand_R).
    /// </summary>
    public void AttachWeaponToHand()
    {
        if (gunTransform != null) return;

        // Recursively locate Hand_R in Character_Visuals hierarchy
        Transform handR = FindDeepChild(transform, "Hand_R");
        if (handR == null) handR = transform;

        // Resolve weapon model: Inspector prefab slot → Resources folder → procedural fallback.
        // No AssetDatabase calls — works in both editor and standalone builds.
        bool isPistol = (weaponType == WeaponType.Pistol);
        GameObject resolvedPrefab = isPistol ? pistolPrefab : riflePrefab;
        if (resolvedPrefab == null)
        {
            string resourcePath = isPistol ? "Weapons/Tactical_Pistol" : "Weapons/Security_Rifle";
            resolvedPrefab = Resources.Load<GameObject>(resourcePath);
        }

        GameObject gunGo;
        if (resolvedPrefab != null)
        {
            gunGo = Instantiate(resolvedPrefab, handR, false);
        }
        else
        {
            // Procedural fallback cuboid when no prefab assigned
            gunGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gunGo.transform.SetParent(handR, false);
            gunGo.transform.localScale = isPistol
                ? new Vector3(0.04f, 0.08f, 0.22f)
                : new Vector3(0.05f, 0.10f, 0.65f);
        }

        gunGo.name = "Guard_Weapon";
        gunTransform = gunGo.transform;

        // Position and orient weapon naturally in right hand pointing forward (+Z)
        if (weaponType == WeaponType.Pistol)
        {
            gunTransform.localPosition = new Vector3(0.02f, -0.04f, 0.08f);
            gunTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            gunTransform.localScale = Vector3.one * 1.0f;
        }
        else
        {
            gunTransform.localPosition = new Vector3(0.03f, -0.06f, 0.15f);
            gunTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            gunTransform.localScale = (weaponType == WeaponType.BossRifle) ? Vector3.one * 1.15f : Vector3.one * 1.0f;
        }

        // Disable any child colliders on weapon
        foreach (var col in gunGo.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // Muzzle point and light for flashes
        GameObject muzzleGo = new GameObject("Guard_Muzzle");
        muzzleGo.transform.SetParent(gunTransform, false);
        muzzleGo.transform.localPosition = (weaponType == WeaponType.Pistol)
            ? new Vector3(0f, 0.025f, 0.15f)
            : new Vector3(0f, 0.035f, 0.45f);
        muzzlePoint = muzzleGo.transform;

        muzzleLight = muzzleGo.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.range = 3.0f;
        muzzleLight.color = new Color(1.0f, 0.65f, 0.25f);
        muzzleLight.intensity = 2.5f;
        muzzleLight.enabled = false;
    }

    void Update()
    {
        if (health != null && health.IsDead) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return;

        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        // Safe Room Protection: Never target or shoot into Room 1 (Z < -18.0)
        if (transform.position.z < -18.0f || playerTransform.position.z < -18.0f)
            return;

        // Evaluate target distance
        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distToPlayer > maxFiringRange) return;

        // Check unoccluded line of sight
        if (!HasClearLineOfSight(playerTransform.position))
            return;

        // Aim weapon towards visible target
        AimGunAtTarget(playerTransform.position + Vector3.up * 1.0f);

        // Firing cooldown check
        if (!isShootingBurst && Time.time - lastShotTime >= fireCooldown)
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
                // Smoothly orient weapon toward target direction
                Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized);
                gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, targetRot, Time.deltaTime * 12f);
            }
        }
    }

    /// <summary>
    /// Validates unobstructed physical line of sight between guard eye/muzzle and player.
    /// Walls, racks, crates, and doors block line of sight.
    /// </summary>
    public bool HasClearLineOfSight(Vector3 targetPos)
    {
        Vector3 origin = muzzlePoint != null ? muzzlePoint.position : transform.position + Vector3.up * 1.5f;
        Vector3 dest = targetPos + Vector3.up * 1.0f; // Target player chest
        Vector3 dir = dest - origin;
        float dist = dir.magnitude;

        if (dist < 0.2f) return true;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir.normalized, out hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            // Ignore guard's own colliders
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                return true;

            // Hit target player
            if (hit.transform == playerTransform || hit.transform.IsChildOf(playerTransform) || hit.transform.CompareTag("Player"))
                return true;

            // Intersected warehouse geometry (wall, shelf, crate) -> BLOCKED!
            return false;
        }

        return true;
    }

    private IEnumerator FireBurstRoutine()
    {
        isShootingBurst = true;
        lastShotTime = Time.time;

        for (int i = 0; i < burstCount; i++)
        {
            if (health != null && health.IsDead) yield break;
            if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) yield break;
            if (playerTransform == null || playerTransform.position.z < -18.0f) yield break;

            if (HasClearLineOfSight(playerTransform.position))
            {
                ExecuteSingleShot();
            }

            if (burstCount > 1 && i < burstCount - 1)
            {
                yield return new WaitForSeconds(burstInterval);
            }
        }

        isShootingBurst = false;
    }

    private void ExecuteSingleShot()
    {
        Vector3 origin = muzzlePoint != null ? muzzlePoint.position : transform.position + Vector3.up * 1.5f;
        Vector3 targetPoint = playerTransform.position + Vector3.up * 1.0f;

        // Apply tactical spread
        Vector3 dir = (targetPoint - origin).normalized;
        Vector3 spreadDir = Quaternion.Euler(
            Random.Range(-accuracySpread, accuracySpread),
            Random.Range(-accuracySpread, accuracySpread),
            0f
        ) * dir;

        RaycastHit hit;
        if (Physics.Raycast(origin, spreadDir, out hit, maxFiringRange + 5f, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == playerTransform || hit.transform.IsChildOf(playerTransform) || hit.transform.CompareTag("Player"))
            {
                var playerHealth = playerTransform.GetComponent<Health>();
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damagePerShot, gameObject);
                    Debug.Log($"[GuardShooting] Guard '{gameObject.name}' hit player for {damagePerShot} damage!");
                }
            }
        }

        // Muzzle flash & sound
        if (muzzleLight != null) StartCoroutine(FlashMuzzleLight());
        if (guardGunshotClip != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.92f, 1.08f);
            audioSource.PlayOneShot(guardGunshotClip, 0.75f);
        }
    }

    private IEnumerator FlashMuzzleLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.04f);
        muzzleLight.enabled = false;
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

    public void ConfigureRole(WeaponType type, int dmg, float cooldown, float range, int bursts, float spread)
    {
        weaponType = type;
        damagePerShot = dmg;
        fireCooldown = cooldown;
        maxFiringRange = range;
        burstCount = bursts;
        accuracySpread = spread;
    }

    public void SetWeaponPrefabs(GameObject pistol, GameObject rifle)
    {
        pistolPrefab = pistol;
        riflePrefab = rifle;
    }

    private static void EnsureGuardAudioClip()
    {
        if (guardGunshotClip != null) return;
        int sampleRate = 44100;
        float duration = 0.14f;
        int count = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[count];

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / sampleRate;
            float env = Mathf.Exp(-t * 32f);
            float noise = (Random.value * 2f - 1f) * 0.7f;
            float pop = Mathf.Sin(2f * Mathf.PI * (220f - t * 600f) * t) * 0.45f;
            samples[i] = Mathf.Clamp((noise + pop) * env, -1f, 1f);
        }

        guardGunshotClip = AudioClip.Create("GuardGunshot", count, 1, sampleRate, false);
        guardGunshotClip.SetData(samples, 0);
    }
}
