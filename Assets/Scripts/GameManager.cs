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

    private bool isHelpExpanded = true;
    public bool IsHelpExpanded => isHelpExpanded;
    private Text helpTextComponent;
    private RectTransform helpPanelRt;

    public void ToggleHelpPanel()
    {
        isHelpExpanded = !isHelpExpanded;
        UpdateControlsHelpText();
    }

    private void UpdateControlsHelpText()
    {
        if (helpPanelRt != null)
        {
            helpPanelRt.sizeDelta = new Vector2(460f, isHelpExpanded ? 82f : 30f);
        }
        if (helpTextComponent != null)
        {
            if (isHelpExpanded)
            {
                helpTextComponent.text = "<size=16><b><color=#FFFFFF>[WASD] Move   [LMB] Fire   [RMB] Aim\n[Space] Jump  [C] Crouch  [R] Reload  [E] Interact</color></b></size>\n<size=14><color=#7E94A8><b>[H]</b> Toggle Help   •   <b>[ESC]</b> Pause</color></size>";
            }
            else
            {
                helpTextComponent.text = "<size=16><color=#DCE4EC><b>CONTROLS:</b> Press <color=#FFD166><b>[H]</b></color> for Keybindings</color></size>";
            }
        }
    }

    /// <summary>
    /// Configures the objective HUD panel styling, anchors, and contrast background
    /// to ensure optimal readability directly below the Top-Left Health status panel without overlap.
    /// Also establishes the compact Controls Help panel.
    /// </summary>
    private void ConfigureHUDPanelStyling()
    {
        if (objectiveText != null)
        {
            objectiveText.supportRichText = true;
            objectiveText.fontSize = 20;
            objectiveText.lineSpacing = 1.15f;
            objectiveText.alignment = TextAnchor.MiddleLeft;
            objectiveText.horizontalOverflow = HorizontalWrapMode.Wrap;
            objectiveText.verticalOverflow = VerticalWrapMode.Overflow;

            Transform panelTransform = objectiveText.transform.parent;
            if (panelTransform != null)
            {
                RectTransform panelRt = panelTransform.GetComponent<RectTransform>();
                if (panelRt != null)
                {
                    // Anchor to Top-Left, cleanly positioned directly below Health panel (which sits below Minimap)
                    // Minimap: Y[-20 to -200]. 12px gap -> Health: Y[-212 to -274]. 12px gap -> Objectives: Y[-286 to -370]
                    panelRt.anchorMin = new Vector2(0f, 1f);
                    panelRt.anchorMax = new Vector2(0f, 1f);
                    panelRt.pivot = new Vector2(0f, 1f);
                    panelRt.anchoredPosition = new Vector2(24f, -286f);
                    panelRt.sizeDelta = new Vector2(460f, 84f);

                    // Translucent high-contrast dark panel
                    var img = panelTransform.GetComponent<Image>();
                    if (img == null) img = panelTransform.gameObject.AddComponent<Image>();
                    img.color = new Color(0.04f, 0.06f, 0.09f, 0.94f);
                }

                RectTransform textRt = objectiveText.GetComponent<RectTransform>();
                if (textRt != null)
                {
                    textRt.anchorMin = Vector2.zero;
                    textRt.anchorMax = Vector2.one;
                    textRt.offsetMin = new Vector2(16f, 8f);
                    textRt.offsetMax = new Vector2(-16f, -8f);
                }

                // Ensure CanvasScaler on UI_Canvas is configured for 1280x720 baseline
                Transform hudParent = panelTransform.parent;
                if (hudParent != null && hudParent.parent != null)
                {
                    CanvasScaler scaler = hudParent.parent.GetComponent<CanvasScaler>();
                    if (scaler != null)
                    {
                        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        scaler.referenceResolution = new Vector2(1280f, 720f);
                        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                        scaler.matchWidthOrHeight = 0.5f;
                    }
                }

                // Create or configure compact Controls Help Panel directly below Objective panel
                // Objectives: Y[-286 to -370]. 12px gap -> Controls Help: Y[-382 to -440]
                if (hudParent != null)
                {
                    Transform existingHelp = hudParent.Find("Controls_Help_Panel");
                    GameObject helpGo;
                    if (existingHelp == null)
                    {
                        helpGo = new GameObject("Controls_Help_Panel");
                        helpGo.transform.SetParent(hudParent, false);
                    }
                    else
                    {
                        helpGo = existingHelp.gameObject;
                    }

                    RectTransform helpRt = helpGo.GetComponent<RectTransform>();
                    if (helpRt == null) helpRt = helpGo.AddComponent<RectTransform>();
                    helpRt.anchorMin = new Vector2(0f, 1f);
                    helpRt.anchorMax = new Vector2(0f, 1f);
                    helpRt.pivot = new Vector2(0f, 1f);
                    helpRt.anchoredPosition = new Vector2(24f, -382f);
                    helpRt.sizeDelta = new Vector2(460f, isHelpExpanded ? 82f : 30f);
                    helpPanelRt = helpRt;

                    var helpImg = helpGo.GetComponent<Image>();
                    if (helpImg == null) helpImg = helpGo.AddComponent<Image>();
                    helpImg.color = new Color(0.04f, 0.06f, 0.09f, 0.94f);

                    Transform existingHelpText = helpGo.transform.Find("Help_Text");
                    GameObject helpTextGo;
                    if (existingHelpText == null)
                    {
                        helpTextGo = new GameObject("Help_Text");
                        helpTextGo.transform.SetParent(helpGo.transform, false);
                    }
                    else
                    {
                        helpTextGo = existingHelpText.gameObject;
                    }

                    RectTransform htRt = helpTextGo.GetComponent<RectTransform>();
                    if (htRt == null) htRt = helpTextGo.AddComponent<RectTransform>();
                    htRt.anchorMin = Vector2.zero;
                    htRt.anchorMax = Vector2.one;
                    htRt.offsetMin = new Vector2(14f, 5f);
                    htRt.offsetMax = new Vector2(-14f, -5f);

                    var ht = helpTextGo.GetComponent<Text>();
                    if (ht == null) ht = helpTextGo.AddComponent<Text>();
                    ht.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    ht.fontSize = 16;
                    ht.supportRichText = true;
                    ht.lineSpacing = 1.15f;
                    ht.color = Color.white;
                    helpTextComponent = ht;
                    UpdateControlsHelpText();
                }

                // Build Pause Banner Panel if not existing
                if (hudParent != null && hudParent.parent != null && hudParent.parent.Find("Pause_Panel") == null)
                {
                    GameObject pauseGo = new GameObject("Pause_Panel");
                    pauseGo.transform.SetParent(hudParent.parent, false);
                    RectTransform pRt = pauseGo.AddComponent<RectTransform>();
                    pRt.anchorMin = Vector2.zero;
                    pRt.anchorMax = Vector2.one;
                    pRt.offsetMin = Vector2.zero;
                    pRt.offsetMax = Vector2.zero;

                    var pBg = pauseGo.AddComponent<Image>();
                    pBg.color = new Color(0.02f, 0.04f, 0.07f, 0.75f);

                    GameObject pBox = new GameObject("Pause_Box");
                    pBox.transform.SetParent(pauseGo.transform, false);
                    RectTransform bRt = pBox.AddComponent<RectTransform>();
                    bRt.anchorMin = bRt.anchorMax = bRt.pivot = new Vector2(0.5f, 0.5f);
                    bRt.sizeDelta = new Vector2(360f, 120f);
                    var bImg = pBox.AddComponent<Image>();
                    bImg.color = new Color(0.08f, 0.10f, 0.15f, 0.95f);

                    GameObject pTitle = new GameObject("Title");
                    pTitle.transform.SetParent(pBox.transform, false);
                    RectTransform tRt = pTitle.AddComponent<RectTransform>();
                    tRt.anchorMin = tRt.anchorMax = new Vector2(0.5f, 0.65f);
                    tRt.sizeDelta = new Vector2(320f, 40f);
                    var tText = pTitle.AddComponent<Text>();
                    tText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    tText.fontSize = 22;
                    tText.fontStyle = FontStyle.Bold;
                    tText.alignment = TextAnchor.MiddleCenter;
                    tText.color = new Color(0.95f, 0.85f, 0.35f);
                    tText.text = "TACTICAL PAUSE";

                    GameObject pSub = new GameObject("Subtext");
                    pSub.transform.SetParent(pBox.transform, false);
                    RectTransform sRt = pSub.AddComponent<RectTransform>();
                    sRt.anchorMin = sRt.anchorMax = new Vector2(0.5f, 0.32f);
                    sRt.sizeDelta = new Vector2(320f, 30f);
                    var sText = pSub.AddComponent<Text>();
                    sText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    sText.fontSize = 13;
                    sText.alignment = TextAnchor.MiddleCenter;
                    sText.color = Color.white;
                    sText.text = "Press [ESC] to Resume";

                    pausePanel = pauseGo;
                    pausePanel.SetActive(false);
                }

                // Configure Interaction Prompt Panel styling
                if (interactionPromptPanel != null)
                {
                    var ipRt = interactionPromptPanel.GetComponent<RectTransform>();
                    if (ipRt != null)
                    {
                        ipRt.anchorMin = new Vector2(0.5f, 0.22f);
                        ipRt.anchorMax = new Vector2(0.5f, 0.22f);
                        ipRt.pivot = new Vector2(0.5f, 0.5f);
                        ipRt.anchoredPosition = Vector2.zero;
                        ipRt.sizeDelta = new Vector2(480f, 48f);
                    }
                    var ipImg = interactionPromptPanel.GetComponent<Image>();
                    if (ipImg == null) ipImg = interactionPromptPanel.AddComponent<Image>();
                    ipImg.color = new Color(0.04f, 0.06f, 0.09f, 0.95f);

                    if (interactionPromptText != null)
                    {
                        interactionPromptText.alignment = TextAnchor.MiddleCenter;
                        interactionPromptText.fontSize = 20;
                        interactionPromptText.fontStyle = FontStyle.Bold;
                        interactionPromptText.color = new Color(1.0f, 0.92f, 0.35f);
                    }
                }
            }
        }
    }

    private bool isPaused = false;
    private GameObject pausePanel;

    public bool IsPaused => isPaused;

    public void TogglePause()
    {
        if (IsGameEnded) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        if (isPaused)
        {
            ReleaseCursor();
        }
        else
        {
            LockCursor();
        }

        Debug.Log($"[GameManager] Pause toggled: {(isPaused ? "PAUSED" : "RESUMED")}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHelpPanel();
        }

        if (currentState == GameState.Playing && !isPaused)
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
    /// a clear progression breakdown.
    /// </summary>
    public void UpdateHUD()
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

            // Determine current sector from player coordinates
            string sectorLabel = "ROOM 1: SAFE START";
            Vector3 pPos = playerTransformCached != null ? playerTransformCached.position : Vector3.zero;
            if (pPos.z < -17.5f) sectorLabel = "ROOM 1: SAFE START";
            else if (pPos.z < -3.5f) sectorLabel = "ROOM 2: PATROL CORRIDOR";
            else if (pPos.z < 13.0f)
            {
                sectorLabel = (pPos.x < 0f) ? "ROOM 3: SEARCH SECTOR" : "ROOM 4: DEFENSE SECTOR";
            }
            else sectorLabel = "ROOM 5: BOSS LOADING HALL";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"<size=13><color=#45AAF2><b>CURRENT OBJECTIVE:</b> {sectorLabel}</color></size>");
            sb.AppendLine($"<size=21><b><color=#FFFFFF>{shortInstruction}</color></b></size>");
            sb.AppendLine($"<size=13><color=#9FB3C8>Progress: Step {currentStepIdx + 1} of 5</color></size>");

            objectiveText.text = sb.ToString();
        }

        if (interactionPromptPanel != null)
        {
            // 1. Weapon pickup prompt takes first priority when player is looking at an eligible dropped weapon
            string weaponPrompt = "";
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                var gun = playerObj.GetComponentInChildren<PlayerGun>();
                if (gun != null) weaponPrompt = gun.GetNearbyPickupPrompt();
            }

            if (!string.IsNullOrEmpty(weaponPrompt))
            {
                interactionPromptPanel.SetActive(true);
                if (interactionPromptText != null)
                    interactionPromptText.text = weaponPrompt;
            }
            else if (cardReader != null && (cardReader.IsPlayerInRange || !string.IsNullOrEmpty(cardReader.StatusFeedback)))
            {
                bool inRange = cardReader.IsPlayerInRange;
                bool hasFeedback = !string.IsNullOrEmpty(cardReader.StatusFeedback);
                string readerPrompt = hasFeedback ? cardReader.StatusFeedback : cardReader.GetInteractionPrompt();
                bool show = !string.IsNullOrEmpty(readerPrompt);
                interactionPromptPanel.SetActive(show);

                if (show && interactionPromptText != null)
                {
                    interactionPromptText.text = readerPrompt;
                }
            }
            else if (doorController != null && doorController.IsPlayerInInteractionRange)
            {
                string prompt = doorController.GetInteractionPrompt();
                bool show = !string.IsNullOrEmpty(prompt);
                interactionPromptPanel.SetActive(show);

                if (show && interactionPromptText != null)
                {
                    interactionPromptText.text = prompt;
                }
            }
            else
            {
                interactionPromptPanel.SetActive(false);
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
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        AccessCard.HasKeycard = false;
        WeaponPickup.ClearAllPickups();
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
    /// Resets game state, UI panels, time scale and cursor in-place without reloading the scene.
    /// </summary>
    public void ResetStateInPlace()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        currentState = GameState.Playing;
        resultReason = "";
        LockCursor();
        UpdateUI();
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

    public void UpdateUI()
    {
        if (hudPanel != null) hudPanel.SetActive(currentState == GameState.Playing);
        if (gameOverPanel != null) gameOverPanel.SetActive(currentState == GameState.GameOver);
        if (victoryPanel != null) victoryPanel.SetActive(currentState == GameState.Victory);
        UpdateHUD();
    }
}
