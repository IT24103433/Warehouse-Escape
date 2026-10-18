using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Physical weapon pickup dropped by guards upon defeat or by the player when swapping.
/// Features:
/// - Distinct 3D weapon model representing the dropped firearm.
/// - Bobbing and rotation animation.
/// - Interaction range and look-at validation.
/// - Preserves exact ammunition counts (magazine and reserve).
/// - E-key priority resolution against CardReader interactions.
/// - Swaps with player's current weapon without duplicating pickups.
/// </summary>
public class WeaponPickup : MonoBehaviour
{
    private static readonly List<WeaponPickup> activePickups = new List<WeaponPickup>();
    public static IReadOnlyList<WeaponPickup> ActivePickups => activePickups;

    [Header("Weapon Pickup Data")]
    [SerializeField] private WeaponType weaponType = WeaponType.UMP_SMG;
    [SerializeField] private int currentAmmo = 25;
    [SerializeField] private int reserveAmmo = 50;
    [SerializeField] private float interactionRange = 2.4f;

    private Transform visualChild;
    private float initialY;
    private bool isPlayerLooking = false;
    private SphereCollider triggerCollider;

    public WeaponType Type => weaponType;
    public WeaponType PickupWeaponType => weaponType;
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public bool IsPlayerLooking => isPlayerLooking;

    public static void ClearAllPickups()
    {
        for (int i = activePickups.Count - 1; i >= 0; i--)
        {
            if (activePickups[i] != null && activePickups[i].gameObject != null)
            {
                Destroy(activePickups[i].gameObject);
            }
        }
        activePickups.Clear();
    }

    public static bool IsAnyPickupTargeted()
    {
        for (int i = 0; i < activePickups.Count; i++)
        {
            if (activePickups[i] != null && activePickups[i].isPlayerLooking)
                return true;
        }
        return false;
    }

    void OnEnable()
    {
        if (!activePickups.Contains(this))
            activePickups.Add(this);
    }

    void OnDisable()
    {
        activePickups.Remove(this);
    }

    void Awake()
    {
        triggerCollider = GetComponent<SphereCollider>();
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = interactionRange;
        }
        initialY = transform.position.y;
    }

    void Start()
    {
        SetupVisualModel();
    }

    public void Initialize(WeaponType type, int curAmmo, int resAmmo)
    {
        weaponType = type;
        currentAmmo = curAmmo;
        reserveAmmo = resAmmo;
        SetupVisualModel();
    }

    public void SetupVisualModel()
    {
        // Remove old visual if any
        if (visualChild != null)
        {
            Destroy(visualChild.gameObject);
            visualChild = null;
        }

        string prefabPath = WeaponData.GetConfig(weaponType).prefabPath;
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, transform, false);
            obj.name = "Pickup_Visual";
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(-90f, 180f, 0f);
            obj.transform.localScale = Vector3.one * 1.0f;

            // Disable any colliders on visual model
            foreach (var col in obj.GetComponentsInChildren<Collider>())
                col.enabled = false;

            visualChild = obj.transform;
        }
        else
        {
            // Procedural fallback
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Fallback_Visual";
            cube.transform.SetParent(transform, false);
            cube.transform.localScale = new Vector3(0.08f, 0.12f, 0.5f);
            cube.GetComponent<Collider>().enabled = false;
            visualChild = cube.transform;
        }
    }

    void Update()
    {
        // Gentle bob and slow rotation
        float bob = Mathf.Sin(Time.time * 2.5f) * 0.05f;
        transform.position = new Vector3(transform.position.x, initialY + bob, transform.position.z);
        if (visualChild != null)
        {
            visualChild.Rotate(Vector3.up, 40f * Time.deltaTime, Space.World);
        }

        CheckPlayerLook();
    }

    private void CheckPlayerLook()
    {
        isPlayerLooking = false;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        Camera cam = playerObj.GetComponentInChildren<Camera>();
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        float dist = Vector3.Distance(cam.transform.position, transform.position);
        if (dist > interactionRange) return;

        // Check if player is looking towards this pickup (angle between camera forward and pickup direction < 35 degrees)
        Vector3 toPickup = (transform.position - cam.transform.position);
        float angle = Vector3.Angle(cam.transform.forward, toPickup.normalized);
        if (angle < 32f)
        {
            // Raycast check: make sure no solid wall blocks line from camera to pickup
            if (!Physics.Raycast(cam.transform.position, toPickup.normalized, out RaycastHit hit, dist, ~0, QueryTriggerInteraction.Ignore) ||
                hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                isPlayerLooking = true;
            }
        }
    }

    public static string GetShortName(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.MP5_SMG: return "MP5";
            case WeaponType.UMP_SMG: return "UMP";
            case WeaponType.AK47_Rifle: return "AK47";
            case WeaponType.Combat_Shotgun: return "Shotgun";
            case WeaponType.SCAR_Rifle: return "SCAR";
            default: return "Weapon";
        }
    }

    public string GetPrompt()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        PlayerGun gun = playerObj != null ? playerObj.GetComponentInChildren<PlayerGun>() : null;
        return GetPrompt(gun);
    }

    public string GetPrompt(PlayerGun playerGun)
    {
        string droppedName = GetShortName(weaponType);
        if (playerGun != null)
        {
            string currentName = GetShortName(playerGun.CurrentWeaponType);
            return $"[E] Swap {currentName} for {droppedName} ({currentAmmo}/{reserveAmmo})";
        }
        return $"[E] Pick up {droppedName} ({currentAmmo}/{reserveAmmo})";
    }

    /// <summary>
    /// Executes swap with player. Player receives this weapon and drops their current weapon here.
    /// </summary>
    public bool TrySwap(PlayerGun playerGun)
    {
        if (playerGun == null) return false;

        WeaponType oldPlayerWeapon = playerGun.CurrentWeaponType;
        int oldCurrentAmmo = playerGun.CurrentAmmo;
        int oldReserveAmmo = playerGun.ReserveAmmo;

        // Equip new weapon onto player
        playerGun.EquipWeapon(weaponType, currentAmmo, reserveAmmo);

        // Update this pickup with the old player weapon
        Initialize(oldPlayerWeapon, oldCurrentAmmo, oldReserveAmmo);
        Debug.Log($"[WeaponPickup] Swapped to {weaponType}. Dropped {oldPlayerWeapon} with {oldCurrentAmmo}/{oldReserveAmmo}.");
        return true;
    }

    /// <summary>
    /// Factory helper to spawn a pickup when a guard dies.
    /// </summary>
    public static WeaponPickup SpawnDroppedWeapon(WeaponType type, Vector3 position, int currentAmmo, int reserveAmmo)
    {
        GameObject go = new GameObject($"Pickup_{type}");
        go.transform.position = position;

        var pickup = go.AddComponent<WeaponPickup>();
        pickup.Initialize(type, currentAmmo, reserveAmmo);
        return pickup;
    }
}
