using UnityEngine;

/// <summary>
/// Trigger volume placed on the exit landing platform beyond the final exit door.
/// When the player crosses this boundary after opening the exit, triggers the single Victory screen.
/// Supports both physical trigger collisions and bounding volume proximity.
/// </summary>
public class EscapeTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Optional reference to DoorController to verify door is open before escape.")]
    [SerializeField] private DoorController exitDoor;

    [Tooltip("Victory message to display on results screen.")]
    [SerializeField] private string victoryMessage = "Warehouse Evacuation Successful!";

    [Tooltip("Whether this legacy escape trigger is active. Disabled in favor of the Room 5 Card Reader terminal.")]
    [SerializeField] private bool isEscapeTriggerActive = false;

    private bool hasTriggered = false;
    private Collider triggerCol;
    private Transform playerTransform;

    public bool IsEscapeTriggerActive { get => isEscapeTriggerActive; set => isEscapeTriggerActive = value; }

    void Start()
    {
        triggerCol = GetComponent<Collider>();
        if (exitDoor == null)
        {
            exitDoor = FindFirstObjectByType<DoorController>();
        }
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (!isEscapeTriggerActive || hasTriggered) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return;

        if (playerTransform == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
            if (playerTransform == null) return;
        }

        if (triggerCol != null && triggerCol.bounds.Contains(playerTransform.position))
        {
            TryTriggerEscape();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isEscapeTriggerActive || hasTriggered) return;

        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            TryTriggerEscape();
        }
    }

    public bool TryTriggerEscape()
    {
        if (!isEscapeTriggerActive || hasTriggered) return false;

        // Requirement: Require a valid exit-door reference
        if (exitDoor == null)
        {
            Debug.LogWarning("[EscapeTrigger] Escape rejected: Missing valid exit-door reference. DoorController reference is strictly required.", this);
            return false;
        }

        // Requirement: Require door to be fully open and not locked
        if (!exitDoor.IsFullyOpen || exitDoor.IsLocked)
        {
            Debug.LogWarning($"[EscapeTrigger] Escape rejected: Exit door is not fully open or remains locked. (IsFullyOpen: {exitDoor.IsFullyOpen}, IsOpen: {exitDoor.IsOpen}, IsLocked: {exitDoor.IsLocked})", this);
            return false;
        }

        hasTriggered = true;
        Debug.Log("[EscapeTrigger] Player successfully crossed the exit threshold with exit door fully open!");

        if (GameManager.Instance != null)
        {
            return GameManager.Instance.TriggerVictory(victoryMessage);
        }

        return false;
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    public DoorController ExitDoor
    {
        get => exitDoor;
        set => exitDoor = value;
    }
}
