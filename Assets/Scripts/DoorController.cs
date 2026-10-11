using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls the warehouse exit door and synchronizes a stationary NavMesh obstacle at the doorway.
/// Keeps navigation blocked at the passage while the visible door moves, releasing it only when fully open,
/// and restoring it immediately upon closing.
/// Uses the stationary doorway position for player interaction distance so the door can be operated from ground level.
/// </summary>
public class DoorController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public float openHeight = 4f;
    public float doorSpeed = 3f;

    [Header("Navigation Blocker")]
    [Tooltip("Stationary NavMeshObstacle anchored at the doorway.")]
    [SerializeField] private NavMeshObstacle stationaryObstacle;

    [Tooltip("Tolerance distance to open position before the doorway is considered physically clear for passage.")]
    [SerializeField] private float clearThreshold = 0.05f;

    [Header("Security Settings")]
    [Tooltip("Whether opening the exit door requires the security access card.")]
    [SerializeField] private bool requiresKeycard = true;

    private Transform player;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;
    private string statusFeedback = "";
    private float feedbackTimer = 0f;

    public bool IsOpen => isOpen;
    public bool IsFullyOpen => isOpen && Vector3.Distance(transform.position, openPosition) <= clearThreshold;
    public NavMeshObstacle StationaryObstacle => stationaryObstacle;
    public Vector3 ClosedPosition => closedPosition;
    public Vector3 OpenPosition => openPosition;
    public bool RequiresKeycard { get => requiresKeycard; set => requiresKeycard = value; }
    public bool IsLocked => requiresKeycard && !AccessCard.HasKeycard;
    public string StatusFeedback => statusFeedback;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;

        EnsureStationaryObstacle();

        // Initially closed: ensure navigation blocker is active and stationary at doorway
        UpdateNavObstacle();
    }

    void Update()
    {
        if (feedbackTimer > 0f)
        {
            feedbackTimer -= Time.deltaTime;
            if (feedbackTimer <= 0f) statusFeedback = "";
        }

        // Calculate distance between player and stationary doorway position
        // Using closedPosition prevents the raised door from becoming unreachable from ground level
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, closedPosition);

            // Press E when close to the doorway
            if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
            {
                ToggleDoor();
            }
        }

        // Decide where the door should move
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        // Smoothly move the visible door
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            doorSpeed * Time.deltaTime
        );

        // Keep blocker stationary at doorway; release only when fully open; restore when closing
        UpdateNavObstacle();
    }

    /// <summary>
    /// Toggles the open/closed state of the door if unlocked.
    /// Returns true if toggled; false if locked.
    /// </summary>
    public bool ToggleDoor()
    {
        if (IsLocked)
        {
            statusFeedback = "Access Denied: Security Keycard Required!";
            feedbackTimer = 2.5f;
            Debug.LogWarning("[DoorController] Exit is locked! Player needs the Access Card.", this);
            return false;
        }

        isOpen = !isOpen;
        statusFeedback = isOpen ? "Door Opening..." : "Door Closing...";
        feedbackTimer = 2.0f;
        UpdateNavObstacle();
        return true;
    }

    void OnGUI()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, closedPosition);
        if (distance <= interactionDistance)
        {
            string prompt = IsLocked
                ? "<color=#E74C3C>[E] EXIT LOCKED - KEYCARD REQUIRED</color>"
                : (isOpen ? "<color=#2ECC71>[E] CLOSE EXIT DOOR</color>" : "<color=#2ECC71>[E] OPEN EXIT DOOR</color>");

            GUIStyle promptStyle = new GUIStyle(GUI.skin.box);
            promptStyle.fontSize = 15;
            promptStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Box(new Rect(Screen.width * 0.5f - 180, Screen.height * 0.72f, 360, 36), prompt, promptStyle);
        }

        if (!string.IsNullOrEmpty(statusFeedback))
        {
            GUIStyle fbStyle = new GUIStyle(GUI.skin.box);
            fbStyle.fontSize = 13;
            fbStyle.normal.textColor = Color.yellow;
            fbStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Box(new Rect(Screen.width * 0.5f - 200, Screen.height * 0.78f, 400, 30), statusFeedback, fbStyle);
        }
    }

    /// <summary>
    /// Programmatically sets door state.
    /// </summary>
    public void SetOpen(bool open)
    {
        if (isOpen != open)
        {
            isOpen = open;
            UpdateNavObstacle();
        }
    }

    /// <summary>
    /// Finds or creates a dedicated stationary NavMeshObstacle anchored at the closed doorway position.
    /// </summary>
    private void EnsureStationaryObstacle()
    {
        if (stationaryObstacle == null)
        {
            // Search for existing blocker GameObject in scene
            var existingBlocker = GameObject.Find("Exit_Door_NavBlocker");
            if (existingBlocker != null)
            {
                stationaryObstacle = existingBlocker.GetComponent<NavMeshObstacle>();
            }
        }

        if (stationaryObstacle == null)
        {
            // Create a dedicated stationary blocker at the closed doorway position
            GameObject blocker = new GameObject("Exit_Door_NavBlocker");
            blocker.transform.position = closedPosition;
            blocker.transform.rotation = transform.rotation;
            stationaryObstacle = blocker.AddComponent<NavMeshObstacle>();
            stationaryObstacle.shape = NavMeshObstacleShape.Box;
            stationaryObstacle.center = Vector3.zero;
            stationaryObstacle.size = new Vector3(2f, 3f, 0.2f);
            stationaryObstacle.carving = true;
            stationaryObstacle.carveOnlyStationary = false;
        }

        // Ensure the blocker remains anchored at closed doorway position
        stationaryObstacle.transform.position = closedPosition;
    }

    /// <summary>
    /// Updates the stationary obstacle state:
    /// - Stays anchored at the doorway closed position.
    /// - Released (carving disabled) ONLY when the door is commanded open and has physically reached open position.
    /// - Restored (carving enabled) immediately when closing begins or while closed/opening.
    /// </summary>
    private void UpdateNavObstacle()
    {
        if (stationaryObstacle == null) return;

        // Ensure the blocker is stationary at the doorway
        stationaryObstacle.transform.position = closedPosition;

        // The passage is physically clear ONLY when commanded open and within clear threshold of open position
        bool isPhysicallyClear = isOpen && Vector3.Distance(transform.position, openPosition) <= clearThreshold;
        bool shouldBlock = !isPhysicallyClear;

        if (stationaryObstacle.enabled != shouldBlock || stationaryObstacle.carving != shouldBlock)
        {
            stationaryObstacle.enabled = shouldBlock;
            stationaryObstacle.carving = shouldBlock;
        }
    }
}