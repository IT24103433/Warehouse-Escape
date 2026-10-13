using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game loop manager handling Win/Lose states, objective tracking,
/// gameplay freezing, cursor state, and Canvas-based UI screens.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        GameOver,
        Victory
    }

    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Playing;
    [SerializeField] private string resultReason = "";

    [Header("UI Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("HUD Elements")]
    [SerializeField] private Text objectiveText;
    [SerializeField] private GameObject interactionPromptPanel;
    [SerializeField] private Text interactionPromptText;
    [SerializeField] private Text feedbackText;

    [Header("Game Over UI Elements")]
    [SerializeField] private Text gameOverTitleText;
    [SerializeField] private Text gameOverReasonText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Victory UI Elements")]
    [SerializeField] private Text victoryTitleText;
    [SerializeField] private Text victoryReasonText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button victoryQuitButton;

    [Header("Scene References")]
    [SerializeField] private DoorController doorController;
    [SerializeField] private CardReader cardReader;

    public GameState CurrentState => currentState;
    public bool IsGameEnded => currentState != GameState.Playing;
    public bool IsGameOver => currentState == GameState.GameOver;
    public bool IsGameWon => currentState == GameState.Victory;
    public string ResultReason => resultReason;
    public CardReader CardReaderInstance { get => cardReader; set => cardReader = value; }

    // Public getters for testing and verification
    public GameObject HudPanel => hudPanel;
    public GameObject GameOverPanel => gameOverPanel;
    public GameObject VictoryPanel => victoryPanel;
    public Button RestartButton => restartButton;
    public Button QuitButton => quitButton;
    public Button PlayAgainButton => playAgainButton;
    public Button VictoryQuitButton => victoryQuitButton;
    public Text ObjectiveText => objectiveText;
    public Text InteractionPromptText => interactionPromptText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Reset state on start
        currentState = GameState.Playing;
        resultReason = "";
        Time.timeScale = 1f;

        // Wire button listeners safely
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.RemoveListener(RestartGame);
            playAgainButton.onClick.AddListener(RestartGame);
        }

        if (victoryQuitButton != null)
        {
            victoryQuitButton.onClick.RemoveListener(QuitGame);
            victoryQuitButton.onClick.AddListener(QuitGame);
        }
    }

    public enum ObjectiveStep
    {
        LeaveSafeRoom = 0,
        PassPatrolRoom = 1,
        FindAccessCard = 2,
        ReachSecurityTerminal = 3,
        EscapeTerminal = 4
    }

    private ObjectiveStep currentObjectiveStep = ObjectiveStep.LeaveSafeRoom;
    private Transform playerTransformCached;

    public ObjectiveStep CurrentObjectiveStep => currentObjectiveStep;

    private static readonly string[] ObjectiveDescriptions = new string[]
    {
        "Leave the safe room.",
        "Pass the patrol room and choose either route.",
        "Find an access card in either sector.",
        "Reach the final security terminal.",
        "Press E at the terminal to insert your card and escape."
    };

    void Start()
    {
        if (doorController == null)
        {
            doorController = FindFirstObjectByType<DoorController>();
        }

        ConfigureHUDPanelStyling();
        LockCursor();
        UpdateUI();
    }

    /// <summary>
    /// Configures the objective HUD panel styling, anchors, and contrast background
    /// to ensure optimal readability and zero obstruction of the crosshair or interaction prompt.
    /// </summary>
    private void ConfigureHUDPanelStyling()
    {
        if (objectiveText != null)
        {
            objectiveText.supportRichText = true;
            objectiveText.fontSize = 13;
            objectiveText.lineSpacing = 1.15f;
            objectiveText.alignment = TextAnchor.UpperLeft;
            objectiveText.horizontalOverflow = HorizontalWrapMode.Wrap;
            objectiveText.verticalOverflow = VerticalWrapMode.Overflow;

            Transform panelTransform = objectiveText.transform.parent;
            if (panelTransform != null)
            {
                RectTransform panelRt = panelTransform.GetComponent<RectTransform>();
                if (panelRt != null)
                {
                    // Anchor to Top-Left
                    panelRt.anchorMin = new Vector2(0f, 1f);
                    panelRt.anchorMax = new Vector2(0f, 1f);
                    panelRt.pivot = new Vector2(0f, 1f);
                    panelRt.anchoredPosition = new Vector2(24f, -24f);
                    panelRt.sizeDelta = new Vector2(430f, 215f);

                    // Translucent high-contrast dark panel
                    var img = panelTransform.GetComponent<Image>();
                    if (img == null) img = panelTransform.gameObject.AddComponent<Image>();
                    img.color = new Color(0.06f, 0.08f, 0.12f, 0.88f);
                }

                RectTransform textRt = objectiveText.GetComponent<RectTransform>();
                if (textRt != null)
                {
                    textRt.anchorMin = Vector2.zero;
                    textRt.anchorMax = Vector2.one;
                    textRt.offsetMin = new Vector2(16f, 12f);
                    textRt.offsetMax = new Vector2(-16f, -12f);
                }
            }
        }
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            UpdateHUD();
        }
    }

    /// <summary>
    /// Evaluates objective progression monotonically based on real player traversal and state.
    /// Either card satisfies the objective; both routes are never required.
    /// </summary>
    private void EvaluateObjectiveProgression()
    {
        if (playerTransformCached == null)
        {
            var pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) playerTransformCached = pObj.transform;
        }

        Vector3 pPos = playerTransformCached != null ? playerTransformCached.position : Vector3.zero;

        // Step 0 -> Step 1: Left safe room (Room 1 Z is < -18.0)
        bool hasLeftSafeRoom = pPos.z >= -17.5f;

        // Step 1 -> Step 2: Passed patrol room and entered either branch sector
        // Sector A (Search Sector): X <= -4.5f and Z >= -3.5f
        // Sector B (Defensive Sector): X >= 4.5f and Z >= -3.5f
        // Or reached Corridor/Boss Room (Z >= 13.0f)
        bool hasChosenRoute = (pPos.x <= -4.5f && pPos.z >= -3.5f) ||
                              (pPos.x >= 4.5f && pPos.z >= -3.5f) ||
                              (pPos.z >= 13.0f);

        // Step 2 -> Step 3: Collected either access card (Card A or Card B)
        bool hasCard = AccessCard.HasKeycard;

        // Step 3 -> Step 4: Reached final security terminal in Room 5
        bool isAtTerminal = cardReader != null && cardReader.IsPlayerInRange;

        if (!hasLeftSafeRoom)
        {
            currentObjectiveStep = ObjectiveStep.LeaveSafeRoom;
        }
        else if (!hasChosenRoute && !hasCard)
        {
            currentObjectiveStep = ObjectiveStep.PassPatrolRoom;
        }
        else if (!hasCard)
        {
            currentObjectiveStep = ObjectiveStep.FindAccessCard;
        }
        else if (!isAtTerminal)
        {
            currentObjectiveStep = ObjectiveStep.ReachSecurityTerminal;
        }
        else
        {
            currentObjectiveStep = ObjectiveStep.EscapeTerminal;
        }
    }

    /// <summary>
    /// Updates the dynamic Canvas HUD objectives and terminal interaction prompt.
    /// Displays a prominent "Current Objective" plus single instruction, followed by
    /// a clear progression breakdown distinguishing completed [✓], current [►], and upcoming [○].
    /// </summary>
    private void UpdateHUD()
    {
        if (cardReader == null)
        {
            cardReader = Object.FindFirstObjectByType<CardReader>();
        }

        EvaluateObjectiveProgression();

        if (objectiveText != null)
        {
            int currentStepIdx = (int)currentObjectiveStep;
            string shortInstruction = ObjectiveDescriptions[currentStepIdx];

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("<b><color=#F39C12>CURRENT OBJECTIVE</color></b>");
            sb.AppendLine($"<b><size=16><color=#FFFFFF>{shortInstruction}</color></size></b>");
            sb.AppendLine("<color=#34495E>──────────────────────────────────────</color>");
            sb.AppendLine("<b><size=11><color=#BDC3C7>MISSION CHECKLIST</color></size></b>");

            for (int i = 0; i < ObjectiveDescriptions.Length; i++)
            {
                if (i < currentStepIdx)
                {
                    sb.AppendLine($"<color=#2ECC71>[✓] {ObjectiveDescriptions[i]}</color>");
                }
                else if (i == currentStepIdx)
                {
                    sb.AppendLine($"<color=#F1C40F><b>[►] {ObjectiveDescriptions[i]}</b></color>");
                }
                else
                {
                    sb.AppendLine($"<color=#7F8C8D>[○] {ObjectiveDescriptions[i]}</color>");
                }
            }

            objectiveText.text = sb.ToString();
        }

        if (interactionPromptPanel != null)
        {
            if (cardReader != null)
            {
                bool inRange = cardReader.IsPlayerInRange;
                bool hasFeedback = !string.IsNullOrEmpty(cardReader.StatusFeedback);
                interactionPromptPanel.SetActive(inRange || hasFeedback);

                if (interactionPromptText != null)
                {
                    if (hasFeedback)
                    {
                        interactionPromptText.text = cardReader.StatusFeedback;
                    }
                    else if (inRange)
                    {
                        interactionPromptText.text = cardReader.GetInteractionPrompt();
                    }
                }
            }
            else if (doorController != null)
            {
                bool inRange = doorController.IsPlayerInInteractionRange;
                string prompt = doorController.GetInteractionPrompt();
                bool show = inRange && !string.IsNullOrEmpty(prompt);
                interactionPromptPanel.SetActive(show);

                if (show && interactionPromptText != null)
                {
                    interactionPromptText.text = prompt;
                }
            }
        }
    }

    /// <summary>
    /// Locks cursor for first-person gameplay.
    /// </summary>
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Unlocks cursor for result screen UI buttons.
    /// </summary>
    public void ReleaseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Triggers the Game Over condition when caught by a guard.
    /// Rejected if already ended (cannot overwrite Victory or duplicate).
    /// </summary>
    public bool TriggerGameOver(string reason)
    {
        if (IsGameEnded)
        {
            Debug.LogWarning($"[GameManager] Ignored TriggerGameOver ('{reason}') because game is already in state: {currentState}");
            return false;
        }

        currentState = GameState.GameOver;
        resultReason = reason;
        Time.timeScale = 0f;
        ReleaseCursor();

        Debug.Log($"[GameManager] GAME OVER triggered: {reason}");

        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = string.IsNullOrEmpty(reason) ? "You were captured by warehouse security!" : reason;
        }

        UpdateUI();
        return true;
    }

    /// <summary>
    /// Triggers the Victory condition when crossing the exit trigger beyond the unlocked door.
    /// Rejected if already ended (cannot overwrite Game Over or duplicate).
    /// </summary>
    public bool TriggerVictory(string message = "Warehouse Evacuation Successful!")
    {
        if (IsGameEnded)
        {
            Debug.LogWarning($"[GameManager] Ignored TriggerVictory because game is already in state: {currentState}");
            return false;
        }

        currentState = GameState.Victory;
        resultReason = message;
        Time.timeScale = 0f;
        ReleaseCursor();

        Debug.Log($"[GameManager] VICTORY triggered: {message}");

        if (victoryReasonText != null)
        {
            victoryReasonText.text = string.IsNullOrEmpty(message) ? "Warehouse Evacuation Successful!" : message;
        }

        UpdateUI();
        return true;
    }

    /// <summary>
    /// Restarts the game, restoring time scale, resetting keycard inventory and reloading the scene.
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("[GameManager] RestartGame invoked: Restoring time scale, resetting inventory, and reloading Warehouse scene...");
        Time.timeScale = 1f;
        AccessCard.HasKeycard = false;
        currentState = GameState.Playing;
        resultReason = "";
        currentObjectiveStep = ObjectiveStep.LeaveSafeRoom;
        LockCursor();

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex >= 0)
        {
            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            SceneManager.LoadScene("Warehouse");
        }
    }

    /// <summary>
    /// Quits the game, exiting built executable or stopping Play Mode in Editor.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[GameManager] QuitGame invoked: Quitting application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UpdateUI()
    {
        if (hudPanel != null) hudPanel.SetActive(currentState == GameState.Playing);
        if (gameOverPanel != null) gameOverPanel.SetActive(currentState == GameState.GameOver);
        if (victoryPanel != null) victoryPanel.SetActive(currentState == GameState.Victory);
    }
}
