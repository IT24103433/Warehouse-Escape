using UnityEngine;

/// <summary>
/// Visible access card reader terminal located at the far end of Room 5 (Boss Room).
/// Accepts either Access Card A or Access Card B when the player interacts via [E].
/// Rejects interaction if the player does not have an access card, continuing gameplay.
/// Upon valid card insertion, triggers the single Victory screen in GameManager exactly once.
/// </summary>
public class CardReader : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance from reader for player interaction.")]
    [SerializeField] private float interactionDistance = 3.0f;

    [Tooltip("Feedback message display duration in seconds.")]
    [SerializeField] private float feedbackDuration = 3.0f;

    [Header("Visual Feedback")]
    [Tooltip("Indicator renderer (e.g. status screen / light).")]
    [SerializeField] private Renderer statusIndicator;

    [Tooltip("Material when idle/ready.")]
    [SerializeField] private Material readyMaterial;

    [Tooltip("Material when card accepted (success/green).")]
    [SerializeField] private Material successMaterial;

    [Tooltip("Material when access denied (red).")]
    [SerializeField] private Material deniedMaterial;

    private Transform playerTransform;
    private bool isCardInserted = false;
    private string statusFeedback = "";
    private float feedbackTimer = 0f;

    private static CardReader instance;
    public static CardReader Instance => instance;

    public static bool IsPlayerNearby(float distance = 3.0f)
    {
        if (instance == null) instance = Object.FindFirstObjectByType<CardReader>();
        if (instance == null) return false;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p == null) return false;
        return Vector3.Distance(p.transform.position, instance.transform.position) <= distance;
    }

    public bool IsCardInserted => isCardInserted;
    public float InteractionDistance => interactionDistance;
    public string StatusFeedback => statusFeedback;

    public bool IsPlayerInRange
    {
        get
        {
            if (playerTransform == null)
            {
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) playerTransform = p.transform;
                if (playerTransform == null) return false;
            }
            return Vector3.Distance(playerTransform.position, transform.position) <= interactionDistance;
        }
    }

    public string GetInteractionPrompt()
    {
        if (!IsPlayerInRange) return "";
        if (WeaponPickup.IsAnyPickupTargeted()) return ""; // Yield prompt to targeted weapon pickup
        if (isCardInserted) return "ACCESS GRANTED";
        return "[E] Insert Access Card";
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ResetReader();
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f)
            {
                statusFeedback = "";
            }
        }

        if (playerTransform == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
            if (playerTransform == null) return;
        }

        if (IsPlayerInRange && !isCardInserted)
        {
            // Resolve E-key priority: if player is looking at a weapon pickup, do not activate card reader
            if (Input.GetKeyDown(KeyCode.E) && !WeaponPickup.IsAnyPickupTargeted())
            {
                TryInsertCard();
            }
        }
    }

    /// <summary>
    /// Attempts card insertion. Accepts either Card A or Card B (AccessCard.HasKeycard).
    /// Returns true if card was accepted and victory triggered; false otherwise.
    /// </summary>
    public bool TryInsertCard()
    {
        if (isCardInserted) return false;

        if (AccessCard.HasKeycard)
        {
            isCardInserted = true;
            string cardDesc = !string.IsNullOrEmpty(AccessCard.CollectedCardName) 
                ? AccessCard.CollectedCardName 
                : "Security Clearance";
            statusFeedback = $"Access Granted! Evacuation Confirmed. [{cardDesc}]";
            feedbackTimer = feedbackDuration;

            if (statusIndicator != null && successMaterial != null)
            {
                statusIndicator.material = successMaterial;
            }

            Debug.Log($"[CardReader] {cardDesc} verified and accepted! Triggering victory.", this);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerVictory($"Warehouse Evacuation Successful! {cardDesc} Verified.");
            }
            return true;
        }
        else
        {
            statusFeedback = "Access card required";
            feedbackTimer = feedbackDuration;

            if (statusIndicator != null && deniedMaterial != null)
            {
                statusIndicator.material = deniedMaterial;
            }

            Debug.LogWarning("[CardReader] Interaction rejected: Access card required.", this);
            return false;
        }
    }

    /// <summary>
    /// Resets the reader state for scene reload/restart.
    /// </summary>
    public void ResetReader()
    {
        isCardInserted = false;
        statusFeedback = "";
        feedbackTimer = 0f;
        if (statusIndicator != null && readyMaterial != null)
        {
            statusIndicator.material = readyMaterial;
        }
    }
}
