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

    void Start()
    {
        if (doorController == null)
        {
            doorController = FindFirstObjectByType<DoorController>();
        }

        LockCursor();
        UpdateUI();
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            UpdateHUD();
        }
    }

    /// <summary>
    /// Updates the dynamic Canvas HUD objectives and door interaction prompt.
    /// </summary>
    private void UpdateHUD()
    {
        if (objectiveText != null)
        {
            bool hasCard = AccessCard.HasKeycard;
            string cardBadge = hasCard ? "<color=#2ECC71>[ACQUIRED]</color>" : "<color=#E74C3C>[REQUIRED]</color>";

            objectiveText.text = "<b>MISSION OBJECTIVES</b>\n" +
                                 "- Room 1: Safe Start Zone\n" +
                                 "- Room 2: Infiltrate Patrol Sector\n" +
                                 $"- Room 3 or 4: Retrieve Access Card {cardBadge}\n" +
                                 "- Room 5: Infiltrate Boss Room & Insert Card at Reader";
        }

        if (cardReader == null)
        {
            cardReader = Object.FindFirstObjectByType<CardReader>();
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
