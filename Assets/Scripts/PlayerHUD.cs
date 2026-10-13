using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// First-person HUD providing health bar and numeric readout, ammunition counters,
/// reloading feedback, center crosshair with reactive hit-marker pulse,
/// red damage vignette, and a selectable 720p / 1080p resolution toggle.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    private static PlayerHUD instance;

    private Health playerHealth;
    private PlayerGun playerGun;

    // HUD Elements
    private Image healthFillImage;
    private Text healthNumericText;
    private Text ammoText;
    private Text reloadText;
    private Image damageVignette;
    private Image crosshairHitMarker;
    private Text resolutionText;

    private float hitMarkerTimer = 0f;
    private float vignetteAlpha = 0f;
    private bool is1080p = false;
    private bool isLowQuality = false;
    private float fpsUpdateTimer = 0f;
    private int fpsFrameCount = 0;
    private float currentFps = 60f;
    private float performanceLogTimer = 0f;

    public static void TriggerHitMarker()
    {
        if (instance != null)
        {
            instance.hitMarkerTimer = 0.18f;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        FindPlayerReferences();
        BuildHUDLayout();

        if (playerHealth != null)
        {
            playerHealth.OnDamaged += HandlePlayerDamaged;
        }

        // Default target is 720p as requested
        SetResolution(false);
    }

    private void FindPlayerReferences()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerHealth = p.GetComponent<Health>();
            playerGun = p.GetComponentInChildren<PlayerGun>();
        }
    }

    private void BuildHUDLayout()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        Transform hudPanel = canvas.transform.Find("HUD_Panel");
        if (hudPanel == null) hudPanel = canvas.transform;

        // ========================================================
        // 1. DAMAGE VIGNETTE (Screen-edge red flash on damage)
        // ========================================================
        GameObject vigGo = new GameObject("Damage_Vignette");
        vigGo.transform.SetParent(hudPanel, false);
        var vigRt = vigGo.AddComponent<RectTransform>();
        vigRt.anchorMin = Vector2.zero;
        vigRt.anchorMax = Vector2.one;
        vigRt.offsetMin = Vector2.zero;
        vigRt.offsetMax = Vector2.zero;
        damageVignette = vigGo.AddComponent<Image>();
        damageVignette.color = new Color(0.85f, 0.05f, 0.05f, 0.0f);
        damageVignette.raycastTarget = false;

        // ========================================================
        // 2. HEALTH PANEL (Bottom-Left HUD)
        // ========================================================
        GameObject hpPanel = new GameObject("Health_Status_Panel");
        hpPanel.transform.SetParent(hudPanel, false);
        var hpRt = hpPanel.AddComponent<RectTransform>();
        hpRt.anchorMin = new Vector2(0f, 0f);
        hpRt.anchorMax = new Vector2(0f, 0f);
        hpRt.pivot = new Vector2(0f, 0f);
        hpRt.anchoredPosition = new Vector2(24f, 24f);
        hpRt.sizeDelta = new Vector2(280f, 75f);

        var hpPanelImg = hpPanel.AddComponent<Image>();
        hpPanelImg.color = new Color(0.06f, 0.08f, 0.12f, 0.88f);

        // Header label
        GameObject hpLabelGo = new GameObject("Health_Label");
        hpLabelGo.transform.SetParent(hpPanel.transform, false);
        var labelRt = hpLabelGo.AddComponent<RectTransform>();
        labelRt.anchoredPosition = new Vector2(16f, 18f);
        labelRt.sizeDelta = new Vector2(120f, 20f);
        labelRt.pivot = new Vector2(0f, 0.5f);
        var lText = hpLabelGo.AddComponent<Text>();
        lText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lText.fontSize = 12;
        lText.fontStyle = FontStyle.Bold;
        lText.color = new Color(0.7f, 0.75f, 0.8f);
        lText.text = "OPERATIVE VITALITY";

        // Numeric HP
        GameObject hpNumGo = new GameObject("Health_Numeric");
        hpNumGo.transform.SetParent(hpPanel.transform, false);
        var numRt = hpNumGo.AddComponent<RectTransform>();
        numRt.anchoredPosition = new Vector2(264f, 18f);
        numRt.sizeDelta = new Vector2(110f, 20f);
        numRt.pivot = new Vector2(1f, 0.5f);
        healthNumericText = hpNumGo.AddComponent<Text>();
        healthNumericText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        healthNumericText.fontSize = 15;
        healthNumericText.fontStyle = FontStyle.Bold;
        healthNumericText.alignment = TextAnchor.MiddleRight;
        healthNumericText.color = Color.white;
        healthNumericText.text = "100 / 100";

        // Health Bar BG
        GameObject barBgGo = new GameObject("Health_Bar_BG");
        barBgGo.transform.SetParent(hpPanel.transform, false);
        var barBgRt = barBgGo.AddComponent<RectTransform>();
        barBgRt.anchoredPosition = new Vector2(16f, -14f);
        barBgRt.sizeDelta = new Vector2(248f, 16f);
        barBgRt.pivot = new Vector2(0f, 0.5f);
        var barBgImg = barBgGo.AddComponent<Image>();
        barBgImg.color = new Color(0.18f, 0.20f, 0.24f, 1.0f);

        // Health Bar Fill
        GameObject barFillGo = new GameObject("Health_Bar_Fill");
        barFillGo.transform.SetParent(barBgGo.transform, false);
        var barFillRt = barFillGo.AddComponent<RectTransform>();
        barFillRt.anchorMin = Vector2.zero;
        barFillRt.anchorMax = Vector2.one;
        barFillRt.offsetMin = new Vector2(2f, 2f);
        barFillRt.offsetMax = new Vector2(-2f, -2f);
        healthFillImage = barFillGo.AddComponent<Image>();
        healthFillImage.type = Image.Type.Filled;
        healthFillImage.fillMethod = Image.FillMethod.Horizontal;
        healthFillImage.color = new Color(0.2f, 0.85f, 0.35f);

        // ========================================================
        // 3. WEAPON & AMMO PANEL (Bottom-Right HUD)
        // ========================================================
        GameObject ammoPanel = new GameObject("Weapon_Ammo_Panel");
        ammoPanel.transform.SetParent(hudPanel, false);
        var ammoRt = ammoPanel.AddComponent<RectTransform>();
        ammoRt.anchorMin = new Vector2(1f, 0f);
        ammoRt.anchorMax = new Vector2(1f, 0f);
        ammoRt.pivot = new Vector2(1f, 0f);
        ammoRt.anchoredPosition = new Vector2(-24f, 24f);
        ammoRt.sizeDelta = new Vector2(240f, 75f);

        var ammoPanelImg = ammoPanel.AddComponent<Image>();
        ammoPanelImg.color = new Color(0.06f, 0.08f, 0.12f, 0.88f);

        // Weapon Name
        GameObject wNameGo = new GameObject("Weapon_Name");
        wNameGo.transform.SetParent(ammoPanel.transform, false);
        var wNameRt = wNameGo.AddComponent<RectTransform>();
        wNameRt.anchoredPosition = new Vector2(16f, 18f);
        wNameRt.sizeDelta = new Vector2(160f, 20f);
        wNameRt.pivot = new Vector2(0f, 0.5f);
        var wText = wNameGo.AddComponent<Text>();
        wText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        wText.fontSize = 12;
        wText.fontStyle = FontStyle.Bold;
        wText.color = new Color(0.92f, 0.76f, 0.26f);
        wText.text = "TACTICAL 9MM";

        // Ammo Display
        GameObject ammoValGo = new GameObject("Ammo_Value");
        ammoValGo.transform.SetParent(ammoPanel.transform, false);
        var valRt = ammoValGo.AddComponent<RectTransform>();
        valRt.anchoredPosition = new Vector2(224f, -14f);
        valRt.sizeDelta = new Vector2(208f, 28f);
        valRt.pivot = new Vector2(1f, 0.5f);
        ammoText = ammoValGo.AddComponent<Text>();
        ammoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ammoText.fontSize = 20;
        ammoText.fontStyle = FontStyle.Bold;
        ammoText.alignment = TextAnchor.MiddleRight;
        ammoText.color = Color.white;
        ammoText.text = "12 / 36";

        // Reload prompt
        GameObject reloadGo = new GameObject("Reload_Prompt");
        reloadGo.transform.SetParent(ammoPanel.transform, false);
        var relRt = reloadGo.AddComponent<RectTransform>();
        relRt.anchoredPosition = new Vector2(16f, -14f);
        relRt.sizeDelta = new Vector2(120f, 20f);
        relRt.pivot = new Vector2(0f, 0.5f);
        reloadText = reloadGo.AddComponent<Text>();
        reloadText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        reloadText.fontSize = 11;
        reloadText.fontStyle = FontStyle.Bold;
        reloadText.color = new Color(0.9f, 0.4f, 0.2f);
        reloadText.text = "";

        // ========================================================
        // 4. CENTER CROSSHAIR & HIT MARKER
        // ========================================================
        GameObject crosshairGo = new GameObject("Crosshair_Center");
        crosshairGo.transform.SetParent(hudPanel, false);
        var cRt = crosshairGo.AddComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0.5f, 0.5f);
        cRt.anchorMax = new Vector2(0.5f, 0.5f);
        cRt.pivot = new Vector2(0.5f, 0.5f);
        cRt.anchoredPosition = Vector2.zero;
        cRt.sizeDelta = new Vector2(6f, 6f);
        var cImg = crosshairGo.AddComponent<Image>();
        cImg.color = new Color(1f, 1f, 1f, 0.75f);

        // Hit marker brackets
        GameObject hitGo = new GameObject("Crosshair_HitMarker");
        hitGo.transform.SetParent(crosshairGo.transform, false);
        var hitRt = hitGo.AddComponent<RectTransform>();
        hitRt.sizeDelta = new Vector2(24f, 24f);
        crosshairHitMarker = hitGo.AddComponent<Image>();
        crosshairHitMarker.color = new Color(1f, 0.2f, 0.2f, 0f);

        // ========================================================
        // 5. RESOLUTION TOGGLE INDICATOR (Top-Right HUD)
        // ========================================================
        GameObject resGo = new GameObject("Resolution_Indicator");
        resGo.transform.SetParent(hudPanel, false);
        var resRt = resGo.AddComponent<RectTransform>();
        resRt.anchorMin = new Vector2(1f, 1f);
        resRt.anchorMax = new Vector2(1f, 1f);
        resRt.pivot = new Vector2(1f, 1f);
        resRt.anchoredPosition = new Vector2(-24f, -24f);
        resRt.sizeDelta = new Vector2(280f, 30f);
        resolutionText = resGo.AddComponent<Text>();
        resolutionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resolutionText.fontSize = 11;
        resolutionText.fontStyle = FontStyle.Bold;
        resolutionText.alignment = TextAnchor.MiddleRight;
        resolutionText.color = new Color(0.7f, 0.8f, 0.9f, 0.75f);
        resolutionText.text = "[F1] 720p | [F2] Q:MED | 60 FPS (16.6ms)";
    }

    private void HandlePlayerDamaged(int dmg, GameObject attacker)
    {
        vignetteAlpha = 0.45f;
    }

    void Update()
    {
        if (playerHealth == null || playerGun == null)
        {
            FindPlayerReferences();
        }

        UpdateHealthDisplay();
        UpdateWeaponDisplay();
        UpdateHitMarker();
        UpdateVignette();

        // Resolution Toggle via F1 key
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleResolution();
        }

        // Quality Toggle via F2 key
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleQuality();
        }

        UpdatePerformanceTracking();
    }

    private void UpdateHealthDisplay()
    {
        if (playerHealth != null)
        {
            float norm = playerHealth.HealthNormalized;
            if (healthFillImage != null)
            {
                healthFillImage.fillAmount = norm;
                healthFillImage.color = Color.Lerp(
                    new Color(0.85f, 0.15f, 0.15f),
                    new Color(0.2f, 0.85f, 0.35f),
                    norm
                );
            }

            if (healthNumericText != null)
            {
                healthNumericText.text = $"{playerHealth.CurrentHealth} / {playerHealth.MaxHealth}";
            }
        }
    }

    private void UpdateWeaponDisplay()
    {
        if (playerGun != null)
        {
            if (ammoText != null)
            {
                ammoText.text = $"{playerGun.CurrentAmmo} / {playerGun.ReserveAmmo}";
                ammoText.color = playerGun.CurrentAmmo == 0 ? new Color(0.9f, 0.2f, 0.2f) : Color.white;
            }

            if (reloadText != null)
            {
                if (playerGun.IsReloading)
                {
                    reloadText.text = "RELOADING...";
                    reloadText.color = new Color(0.95f, 0.55f, 0.15f);
                }
                else if (playerGun.CurrentAmmo == 0 && playerGun.ReserveAmmo > 0)
                {
                    reloadText.text = "[R] RELOAD";
                    reloadText.color = new Color(0.9f, 0.2f, 0.2f);
                }
                else
                {
                    reloadText.text = "";
                }
            }
        }
    }

    private void UpdateHitMarker()
    {
        if (crosshairHitMarker != null)
        {
            if (hitMarkerTimer > 0f)
            {
                hitMarkerTimer -= Time.deltaTime;
                crosshairHitMarker.color = new Color(1f, 0.2f, 0.2f, Mathf.Clamp01(hitMarkerTimer * 5f));
            }
            else
            {
                crosshairHitMarker.color = new Color(1f, 0.2f, 0.2f, 0f);
            }
        }
    }

    private void UpdateVignette()
    {
        if (damageVignette != null)
        {
            if (vignetteAlpha > 0.001f)
            {
                vignetteAlpha -= Time.deltaTime * 1.5f;
                damageVignette.color = new Color(0.85f, 0.05f, 0.05f, Mathf.Clamp01(vignetteAlpha));
            }
            else
            {
                damageVignette.color = new Color(0.85f, 0.05f, 0.05f, 0f);
            }
        }
    }

    public void ToggleResolution()
    {
        SetResolution(!is1080p);
    }

    public void SetResolution(bool to1080p)
    {
        is1080p = to1080p;
        int width = is1080p ? 1920 : 1280;
        int height = is1080p ? 1080 : 720;
        Screen.SetResolution(width, height, FullScreenMode.Windowed);

        UpdateResolutionText();
        Debug.Log($"[PlayerHUD] Target resolution set to {(is1080p ? "1080p (1920x1080)" : "720p (1280x720)")} | FPS: {currentFps:F1}");
    }

    public void ToggleQuality()
    {
        SetQuality(!isLowQuality);
    }

    public void SetQuality(bool low)
    {
        isLowQuality = low;
        var pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
        if (pipeline != null)
        {
            pipeline.shadowDistance = isLowQuality ? 25f : 45f;
            pipeline.renderScale = isLowQuality ? 0.9f : 1.0f;
        }
        UpdateResolutionText();
        Debug.Log($"[PlayerHUD] Quality set to {(isLowQuality ? "LOW (Shadows: 25m, Scale: 0.9)" : "MEDIUM (Shadows: 45m, Scale: 1.0)")}");
    }

    private void UpdatePerformanceTracking()
    {
        fpsFrameCount++;
        fpsUpdateTimer += Time.unscaledDeltaTime;
        if (fpsUpdateTimer >= 0.5f)
        {
            currentFps = fpsFrameCount / fpsUpdateTimer;
            fpsFrameCount = 0;
            fpsUpdateTimer = 0f;
            UpdateResolutionText();
        }

        performanceLogTimer += Time.unscaledDeltaTime;
        if (performanceLogTimer >= 5.0f)
        {
            performanceLogTimer = 0f;
            float frameTimeMs = 1000f / Mathf.Max(currentFps, 1f);
            Debug.Log($"[Performance Metrics] Resolution: {(is1080p ? "1080p" : "720p")} | Quality: {(isLowQuality ? "LOW" : "MED")} | Average FPS: {currentFps:F1} | FrameTime: {frameTimeMs:F1} ms");
        }
    }

    private void UpdateResolutionText()
    {
        if (resolutionText != null)
        {
            float frameTimeMs = 1000f / Mathf.Max(currentFps, 1f);
            string resStr = is1080p ? "1080p" : "720p";
            string qStr = isLowQuality ? "LOW" : "MED";
            resolutionText.text = $"[F1] {resStr} | [F2] Q:{qStr} | {Mathf.RoundToInt(currentFps)} FPS ({frameTimeMs:F1}ms)";
        }
    }
}
