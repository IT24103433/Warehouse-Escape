using UnityEngine;

/// <summary>
/// Security access-card pickup foundation for Sanjeevsaran's systems engineering.
/// Placed in the warehouse terminal area and defended by Aqqil's Defensive Guard.
/// Upon collection, unlocks the warehouse exit door and provides user feedback.
/// </summary>
public class AccessCard : MonoBehaviour
{
    private static bool hasKeycard = false;
    public static bool HasKeycard
    {
        get => hasKeycard;
        set => hasKeycard = value;
    }

    public static string CollectedCardName { get; set; } = "";
    public static string CollectedSector { get; set; } = "";

    [Header("Card Identification")]
    [Tooltip("Readable identity of this security access card.")]
    [SerializeField] private string cardName = "Access Card A";

    [Tooltip("Facility sector assigned to this card.")]
    [SerializeField] private string sectorCode = "Sector 03 - Search Logistics";

    [Tooltip("Security clearance level.")]
    [SerializeField] private string clearanceLevel = "Level 3";

    [Header("Visual Feedback Settings")]
    [Tooltip("Bobbing oscillation speed.")]
    [SerializeField] private float bobSpeed = 3f;

    [Tooltip("Bobbing oscillation amplitude in meters.")]
    [SerializeField] private float bobHeight = 0.12f;

    [Tooltip("Rotation speed in degrees per second.")]
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 initialPosition;
    private bool isCollected = false;

    public bool IsCollected => isCollected;
    public string CardName => cardName;
    public string SectorCode => sectorCode;
    public string ClearanceLevel => clearanceLevel;

    public void ConfigureIdentity(string name, string sector, string clearance = "Level 3")
    {
        cardName = name;
        sectorCode = sector;
        clearanceLevel = clearance;
    }

    void Awake()
    {
        hasKeycard = false; // Reset static state for scene loads
        CollectedCardName = "";
        CollectedSector = "";
    }

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // Idle floating and rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        transform.position = initialPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            Collect();
        }
    }

    /// <summary>
    /// Collects the keycard, granting access to unlock the exit door.
    /// </summary>
    public void Collect()
    {
        isCollected = true;
        hasKeycard = true;
        CollectedCardName = cardName;
        CollectedSector = sectorCode;
        Debug.Log($"[AccessCard] {cardName} ({sectorCode} - {clearanceLevel}) collected! Exit terminal is now unlocked.", this);

        // Hide visual card mesh and disable trigger
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) r.enabled = false;

        var colliders = GetComponentsInChildren<Collider>();
        foreach (var c in colliders) c.enabled = false;
    }

    /// <summary>
    /// Resets the card state (for testing and scene resets).
    /// </summary>
    public void ResetCard()
    {
        isCollected = false;
        hasKeycard = false;
        CollectedCardName = "";
        CollectedSector = "";
        transform.position = initialPosition;

        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) r.enabled = true;

        var colliders = GetComponentsInChildren<Collider>();
        foreach (var c in colliders) c.enabled = true;
    }
}
