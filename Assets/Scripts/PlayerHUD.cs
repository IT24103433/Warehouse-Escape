using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// First-person HUD providing:
/// - Top-Left high-contrast tactical minimap showing room boundaries, corridors, and player position/facing.
/// - Top-Left health bar with numeric readout and low-health pulsing warning immediately below the minimap without overlap.
/// - Center crosshair with bold dark contrast outline, hidden on pause, defeat, and victory.
/// - Bottom-Right ammunition counter, weapon name, and reloading feedback.
/// - Center-Bottom weapon pickup instructions formatted as '[E] Swap MP5 for AK47 (30/90)' or '[E] Pick up AK47 (30/90)'.
/// - Red damage vignette.
/// - Selectable 720p / 1080p resolution and quality toggles.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    private static PlayerHUD instance;
    public static PlayerHUD Instance => instance;

    private Health playerHealth;
    private PlayerGun playerGun;

    // HUD Elements
    private GameObject minimapPanelObject;
    private RectTransform playerMarkerRt;
    private Image minimapFloorPlanImage;

    private GameObject hpPanelObject;
    private Image healthFillImage;
    private Text healthNumericText;
    private Text healthRegenText;
    private Text healthWarningText;

    private Text weaponNameText;
    private Text ammoText;
    private Text reloadText;
    private Image damageVignette;

    private GameObject crosshairRootObject;
    private Image crosshairHitMarker;

    private Text resolutionText;
    private GameObject weaponPickupPanel;
    private Text weaponPickupText;

    // Minimap World Bounds (Rooms 1 through 5)
    private const float MinWorldX = -30.0f;
    private const float MaxWorldX = 30.0f;
    private const float MinWorldZ = -32.0f;
    private const float MaxWorldZ = 52.0f;
    private const float MinimapWidth = 180.0f;
    private const float MinimapHeight = 150.0f;

    private static Sprite cachedMinimapSprite;

    private float hitMarkerTimer = 0f;
    private float vignetteAlpha = 0f;
    private bool is1080p = false;
    private bool isLowQuality = false;
    private float fpsUpdateTimer = 0f;
    private int fpsFrameCount = 0;
    private float currentFps = 60f;
    private float performanceLogTimer = 0f;
    private float peakWorstGameplayFrameTime = 0f;
    private readonly System.Collections.Generic.List<float> recentFrameTimes = new System.Collections.Generic.List<float>(50000);
    private readonly System.Collections.Generic.List<float> periodicLogScratch = new System.Collections.Generic.List<float>(600);
    private int excludedStartupFrames = 0;
    private int excludedCaptureFrames = 0;
    private int captureCooldownFrames = 0;
    public static bool IsExplicitCaptureActive { get; set; } = false;
    [SerializeField] private float crosshairFootprint = 28f;

    public GameObject CrosshairRoot => crosshairRootObject;
    public GameObject MinimapPanel => minimapPanelObject;
    public Image HealthFillImage => healthFillImage;
    public Text HealthNumericText => healthNumericText;
    public Text HealthRegenText => healthRegenText;
    public GameObject WeaponPickupPanel => weaponPickupPanel;
    public Text WeaponPickupText => weaponPickupText;
    public Text AmmoText => ammoText;
    public Text ReloadText => reloadText;

    public float CrosshairSize
    {
        get => crosshairFootprint;
        set => SetCrosshairSize(value);
    }

    public void SetCrosshairSize(float sizeInPixels)
    {
        crosshairFootprint = Mathf.Clamp(sizeInPixels, 14f, 80f);
        if (crosshairRootObject != null)
        {
            float scale = crosshairFootprint / 28f;
            crosshairRootObject.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

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

        // Default target is 720p as requested, unless started with 1080p parameters
        bool startAt1080p = false;
        string[] cmdArgs = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < cmdArgs.Length; i++)
        {
            if (cmdArgs[i].IndexOf("1080", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                startAt1080p = true;
                break;
            }
        }
        if (Screen.width >= 1600) startAt1080p = true;
        SetResolution(startAt1080p);

        CheckForVerificationTrigger();
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
        if (canvas == null)
        {
            var uiCanvasGo = GameObject.Find("UI_Canvas");
            if (uiCanvasGo != null) canvas = uiCanvasGo.GetComponent<Canvas>();
        }
        if (canvas == null) canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // Ensure canvas scaler is consistently configured for 1280x720 baseline
        CanvasScaler mainScaler = canvas.GetComponent<CanvasScaler>();
        if (mainScaler == null) mainScaler = canvas.gameObject.AddComponent<CanvasScaler>();
        mainScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        mainScaler.referenceResolution = new Vector2(1280f, 720f);
        mainScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        mainScaler.matchWidthOrHeight = 0.5f;

        // Ensure canvas sorting and camera settings
        canvas.sortingOrder = 500;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 0.5f;
        }

        Transform hudPanel = canvas.transform.Find("HUD_Panel");
        if (hudPanel == null) hudPanel = canvas.transform;

        // Clean up previously created dynamic HUD objects if recreating
        Transform existingCrosshair = hudPanel.Find("Crosshair_Center");
        if (existingCrosshair != null) Destroy(existingCrosshair.gameObject);
        Transform existingMinimap = hudPanel.Find("Minimap_Panel");
        if (existingMinimap != null) Destroy(existingMinimap.gameObject);
        Transform existingHp = hudPanel.Find("Health_Status_Panel");
        if (existingHp != null) Destroy(existingHp.gameObject);
        Transform existingAmmo = hudPanel.Find("Weapon_Ammo_Panel");
        if (existingAmmo != null) Destroy(existingAmmo.gameObject);
        Transform existingPickup = hudPanel.Find("Weapon_Pickup_Prompt_Panel");
        if (existingPickup != null) Destroy(existingPickup.gameObject);

        // ========================================================
        // 1. DAMAGE VIGNETTE (Screen-edge red flash on damage)
        // ========================================================
        Transform existingVig = hudPanel.Find("Damage_Vignette");
        if (existingVig != null) Destroy(existingVig.gameObject);
        GameObject vigGo = new GameObject("Damage_Vignette", typeof(RectTransform), typeof(Image));
        vigGo.transform.SetParent(hudPanel, false);
        var vigRt = vigGo.GetComponent<RectTransform>();
        vigRt.anchorMin = Vector2.zero;
        vigRt.anchorMax = Vector2.one;
        vigRt.offsetMin = Vector2.zero;
        vigRt.offsetMax = Vector2.zero;
        damageVignette = vigGo.GetComponent<Image>();
        damageVignette.color = new Color(0.85f, 0.05f, 0.05f, 0.0f);
        damageVignette.raycastTarget = false;

        // ========================================================
        // 2. TOP-LEFT MINIMAP (Static floor plan + Moving Player Marker)
        // Bounds: Y[-20 to -200], Width: 200, Height: 180
        // ========================================================
        GameObject mapPanelGo = new GameObject("Minimap_Panel", typeof(RectTransform), typeof(Image));
        mapPanelGo.transform.SetParent(hudPanel, false);
        var mapRt = mapPanelGo.GetComponent<RectTransform>();
        mapRt.anchorMin = new Vector2(0f, 1f);
        mapRt.anchorMax = new Vector2(0f, 1f);
        mapRt.pivot = new Vector2(0f, 1f);
        mapRt.anchoredPosition = new Vector2(24f, -20f);
        mapRt.sizeDelta = new Vector2(200f, 180f);

        var mapBgImg = mapPanelGo.GetComponent<Image>();
        mapBgImg.color = new Color(0.06f, 0.08f, 0.12f, 0.94f);
        minimapPanelObject = mapPanelGo;

        // Header label
        GameObject mapHeaderGo = new GameObject("Map_Title", typeof(RectTransform), typeof(Text));
        mapHeaderGo.transform.SetParent(mapPanelGo.transform, false);
        var mhRt = mapHeaderGo.GetComponent<RectTransform>();
        mhRt.anchorMin = new Vector2(0f, 1f);
        mhRt.anchorMax = new Vector2(1f, 1f);
        mhRt.pivot = new Vector2(0.5f, 1f);
        mhRt.anchoredPosition = new Vector2(0f, -4f);
        mhRt.sizeDelta = new Vector2(190f, 16f);
        var mhText = mapHeaderGo.GetComponent<Text>();
        mhText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        mhText.fontSize = 10;
        mhText.fontStyle = FontStyle.Bold;
        mhText.alignment = TextAnchor.MiddleCenter;
        mhText.color = new Color(0.55f, 0.70f, 0.85f, 0.90f);
        mhText.text = "TACTICAL FLOOR PLAN";

        // Floor Plan Graphic Image
        GameObject floorPlanGo = new GameObject("Floor_Plan_Display", typeof(RectTransform), typeof(Image));
        floorPlanGo.transform.SetParent(mapPanelGo.transform, false);
        var fpRt = floorPlanGo.GetComponent<RectTransform>();
        fpRt.anchorMin = new Vector2(0.5f, 0.5f);
        fpRt.anchorMax = new Vector2(0.5f, 0.5f);
        fpRt.pivot = new Vector2(0.5f, 0.5f);
        fpRt.anchoredPosition = new Vector2(0f, -8f);
        fpRt.sizeDelta = new Vector2(MinimapWidth, MinimapHeight);
        minimapFloorPlanImage = floorPlanGo.GetComponent<Image>();
        minimapFloorPlanImage.sprite = GetOrCreateMinimapSprite();
        minimapFloorPlanImage.color = Color.white;
        minimapFloorPlanImage.raycastTarget = false;

        // Player Marker on Minimap: bright tactical chevron with forward sightline
        GameObject markerGo = new GameObject("Player_Marker", typeof(RectTransform));
        markerGo.transform.SetParent(floorPlanGo.transform, false);
        playerMarkerRt = markerGo.GetComponent<RectTransform>();
        playerMarkerRt.sizeDelta = new Vector2(16f, 16f);
        playerMarkerRt.anchorMin = playerMarkerRt.anchorMax = playerMarkerRt.pivot = new Vector2(0.5f, 0.5f);
        playerMarkerRt.anchoredPosition = Vector2.zero;

        // Marker central dot (black backing + bright cyan center)
        CreateCrosshairTick(markerGo.transform, Vector2.zero, new Vector2(8f, 8f), new Color(0f, 0f, 0f, 0.95f));
        CreateCrosshairTick(markerGo.transform, Vector2.zero, new Vector2(5f, 5f), new Color(0.2f, 0.95f, 1.0f, 1.0f));

        // Forward sightline arrow (indicates actual camera facing direction)
        CreateCrosshairTick(markerGo.transform, new Vector2(0f, 7f), new Vector2(4f, 8f), new Color(0f, 0f, 0f, 0.95f));
        CreateCrosshairTick(markerGo.transform, new Vector2(0f, 7f), new Vector2(2f, 6f), new Color(0.95f, 0.85f, 0.25f, 1.0f));

        // ========================================================
        // 3. HEALTH STATUS PANEL (Top-Left HUD immediately below Minimap)
        // Minimap: Y[-20 to -200]. 12px gap -> Health Panel: Y[-212 to -274], Width: 360, Height: 62
        // All children strictly contained inside panel bounds with 10px top / 12px bottom padding
        // ========================================================
        GameObject hpPanel = new GameObject("Health_Status_Panel", typeof(RectTransform), typeof(Image));
        hpPanel.transform.SetParent(hudPanel, false);
        var hpRt = hpPanel.GetComponent<RectTransform>();
        hpRt.anchorMin = new Vector2(0f, 1f);
        hpRt.anchorMax = new Vector2(0f, 1f);
        hpRt.pivot = new Vector2(0f, 1f);
        hpRt.anchoredPosition = new Vector2(24f, -212f);
        hpRt.sizeDelta = new Vector2(360f, 62f);

        var hpPanelImg = hpPanel.GetComponent<Image>();
        hpPanelImg.color = new Color(0.04f, 0.06f, 0.09f, 0.94f);
        hpPanelObject = hpPanel;

        // Header label (anchored Top-Left inside panel)
        GameObject hpLabelGo = new GameObject("Health_Label", typeof(RectTransform), typeof(Text));
        hpLabelGo.transform.SetParent(hpPanel.transform, false);
        var labelRt = hpLabelGo.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 1f);
        labelRt.anchorMax = new Vector2(0f, 1f);
        labelRt.pivot = new Vector2(0f, 1f);
        labelRt.anchoredPosition = new Vector2(16f, -10f);
        labelRt.sizeDelta = new Vector2(160f, 18f);
        var lText = hpLabelGo.GetComponent<Text>();
        lText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lText.fontSize = 11;
        lText.fontStyle = FontStyle.Bold;
        lText.alignment = TextAnchor.MiddleLeft;
        lText.color = new Color(0.65f, 0.78f, 0.90f);
        lText.text = "OPERATIVE VITALITY";

        // Numeric HP & Low HP Warning (anchored Top-Right inside panel)
        GameObject hpNumGo = new GameObject("Health_Numeric", typeof(RectTransform), typeof(Text));
        hpNumGo.transform.SetParent(hpPanel.transform, false);
        var numRt = hpNumGo.GetComponent<RectTransform>();
        numRt.anchorMin = new Vector2(1f, 1f);
        numRt.anchorMax = new Vector2(1f, 1f);
        numRt.pivot = new Vector2(1f, 1f);
        numRt.anchoredPosition = new Vector2(-16f, -10f);
        numRt.sizeDelta = new Vector2(160f, 18f);
        healthNumericText = hpNumGo.GetComponent<Text>();
        healthNumericText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        healthNumericText.fontSize = 13;
        healthNumericText.fontStyle = FontStyle.Bold;
        healthNumericText.alignment = TextAnchor.MiddleRight;
        healthNumericText.supportRichText = true;
        healthNumericText.color = Color.white;
        healthNumericText.text = "100 / 100";

        // Subtle Regeneration Indicator (anchored Top-Center inside panel)
        GameObject regenGo = new GameObject("Health_Regen_Indicator", typeof(RectTransform), typeof(Text));
        regenGo.transform.SetParent(hpPanel.transform, false);
        var regenRt = regenGo.GetComponent<RectTransform>();
        regenRt.anchorMin = new Vector2(0.5f, 1f);
        regenRt.anchorMax = new Vector2(0.5f, 1f);
        regenRt.pivot = new Vector2(0.5f, 1f);
        regenRt.anchoredPosition = new Vector2(10f, -10f);
        regenRt.sizeDelta = new Vector2(160f, 18f);
        healthRegenText = regenGo.GetComponent<Text>();
        healthRegenText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        healthRegenText.fontSize = 11;
        healthRegenText.fontStyle = FontStyle.Bold;
        healthRegenText.alignment = TextAnchor.MiddleCenter;
        healthRegenText.color = new Color(0.35f, 1.0f, 0.65f, 0.95f);
        healthRegenText.text = "";

        // Health Bar BG (anchored Top-Left inside panel, strictly inside Y[-34 to -50], 12px above bottom)
        GameObject barBgGo = new GameObject("Health_Bar_BG", typeof(RectTransform), typeof(Image));
        barBgGo.transform.SetParent(hpPanel.transform, false);
        var barBgRt = barBgGo.GetComponent<RectTransform>();
        barBgRt.anchorMin = new Vector2(0f, 1f);
        barBgRt.anchorMax = new Vector2(0f, 1f);
        barBgRt.pivot = new Vector2(0f, 1f);
        barBgRt.anchoredPosition = new Vector2(16f, -34f);
        barBgRt.sizeDelta = new Vector2(328f, 16f);
        var barBgImg = barBgGo.GetComponent<Image>();
        barBgImg.color = new Color(0.12f, 0.16f, 0.22f, 1.0f);

        // Health Bar Fill
        GameObject barFillGo = new GameObject("Health_Bar_Fill", typeof(RectTransform), typeof(Image));
        barFillGo.transform.SetParent(barBgGo.transform, false);
        var barFillRt = barFillGo.GetComponent<RectTransform>();
        barFillRt.anchorMin = Vector2.zero;
        barFillRt.anchorMax = Vector2.one;
        barFillRt.pivot = new Vector2(0f, 0.5f);
        barFillRt.offsetMin = new Vector2(2f, 2f);
        barFillRt.offsetMax = new Vector2(-2f, -2f);
        healthFillImage = barFillGo.GetComponent<Image>();
        healthFillImage.type = Image.Type.Filled;
        healthFillImage.fillMethod = Image.FillMethod.Horizontal;
        healthFillImage.color = new Color(0.20f, 0.85f, 0.40f);

        // ========================================================
        // 4. WEAPON & AMMO PANEL (Bottom-Right HUD)
        // ========================================================
        GameObject ammoPanel = new GameObject("Weapon_Ammo_Panel", typeof(RectTransform), typeof(Image));
        ammoPanel.transform.SetParent(hudPanel, false);
        var ammoRt = ammoPanel.GetComponent<RectTransform>();
        ammoRt.anchorMin = new Vector2(1f, 0f);
        ammoRt.anchorMax = new Vector2(1f, 0f);
        ammoRt.pivot = new Vector2(1f, 0f);
        ammoRt.anchoredPosition = new Vector2(-24f, 24f);
        ammoRt.sizeDelta = new Vector2(280f, 96f);

        var ammoPanelImg = ammoPanel.GetComponent<Image>();
        ammoPanelImg.color = new Color(0.06f, 0.08f, 0.12f, 0.90f);

        // Weapon Name
        GameObject wNameGo = new GameObject("Weapon_Name", typeof(RectTransform), typeof(Text));
        wNameGo.transform.SetParent(ammoPanel.transform, false);
        var wNameRt = wNameGo.GetComponent<RectTransform>();
        wNameRt.anchorMin = wNameRt.anchorMax = new Vector2(0f, 0.5f);
        wNameRt.anchoredPosition = new Vector2(16f, 28f);
        wNameRt.sizeDelta = new Vector2(248f, 20f);
        wNameRt.pivot = new Vector2(0f, 0.5f);
        weaponNameText = wNameGo.GetComponent<Text>();
        weaponNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        weaponNameText.fontSize = 12;
        weaponNameText.fontStyle = FontStyle.Bold;
        weaponNameText.color = new Color(0.92f, 0.76f, 0.26f);
        weaponNameText.text = "MP5-A3 SMG";

        // Ammo Display
        GameObject ammoValGo = new GameObject("Ammo_Value", typeof(RectTransform), typeof(Text));
        ammoValGo.transform.SetParent(ammoPanel.transform, false);
        var valRt = ammoValGo.GetComponent<RectTransform>();
        valRt.anchorMin = valRt.anchorMax = new Vector2(1f, 0.5f);
        valRt.anchoredPosition = new Vector2(-16f, 2f);
        valRt.sizeDelta = new Vector2(248f, 28f);
        valRt.pivot = new Vector2(1f, 0.5f);
        ammoText = ammoValGo.GetComponent<Text>();
        ammoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ammoText.fontSize = 20;
        ammoText.fontStyle = FontStyle.Bold;
        ammoText.alignment = TextAnchor.MiddleRight;
        ammoText.color = Color.white;
        ammoText.text = "30 / 90";

        // Reload prompt
        GameObject reloadGo = new GameObject("Reload_Prompt", typeof(RectTransform), typeof(Text));
        reloadGo.transform.SetParent(ammoPanel.transform, false);
        var relRt = reloadGo.GetComponent<RectTransform>();
        relRt.anchorMin = relRt.anchorMax = new Vector2(0f, 0.5f);
        relRt.anchoredPosition = new Vector2(16f, -24f);
        relRt.sizeDelta = new Vector2(248f, 20f);
        relRt.pivot = new Vector2(0f, 0.5f);
        reloadText = reloadGo.GetComponent<Text>();
        reloadText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        reloadText.fontSize = 11;
        reloadText.fontStyle = FontStyle.Bold;
        reloadText.color = new Color(0.9f, 0.4f, 0.2f);
        reloadText.text = "";

        // ========================================================
        // 5. CENTER CROSSHAIR WITH READABLE CONTRAST & HIT MARKER
        // Rendered on a persistent Screen Space Overlay Canvas above other gameplay UI (sortingOrder 1000)
        // Four white ticks with thick black outline, small center dot, and clear center gap
        // Roughly 24–30px footprint at 720p (29px footprint) with adjustable size
        // ========================================================
        GameObject crosshairCanvasGo = GameObject.Find("Crosshair_Canvas");
        Canvas crosshairCanvas;
        if (crosshairCanvasGo == null)
        {
            crosshairCanvasGo = new GameObject("Crosshair_Canvas");
            crosshairCanvas = crosshairCanvasGo.AddComponent<Canvas>();
        }
        else
        {
            crosshairCanvas = crosshairCanvasGo.GetComponent<Canvas>();
            if (crosshairCanvas == null) crosshairCanvas = crosshairCanvasGo.AddComponent<Canvas>();
        }

        crosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        crosshairCanvas.sortingOrder = 1000; // Persistent above other gameplay UI

        CanvasScaler crosshairScaler = crosshairCanvasGo.GetComponent<CanvasScaler>();
        if (crosshairScaler == null) crosshairScaler = crosshairCanvasGo.AddComponent<CanvasScaler>();
        crosshairScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        crosshairScaler.referenceResolution = new Vector2(1280f, 720f);
        crosshairScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        crosshairScaler.matchWidthOrHeight = 0.5f;

        Transform existingCrosshairOnCanvas = crosshairCanvasGo.transform.Find("Crosshair_Center");
        if (existingCrosshairOnCanvas != null) Destroy(existingCrosshairOnCanvas.gameObject);

        GameObject crosshairRoot = new GameObject("Crosshair_Center", typeof(RectTransform));
        crosshairRoot.transform.SetParent(crosshairCanvasGo.transform, false);
        var cRt = crosshairRoot.GetComponent<RectTransform>();
        cRt.anchorMin = new Vector2(0.5f, 0.5f);
        cRt.anchorMax = new Vector2(0.5f, 0.5f);
        cRt.pivot = new Vector2(0.5f, 0.5f);
        cRt.anchoredPosition = Vector2.zero;
        cRt.sizeDelta = new Vector2(28f, 28f);
        crosshairRootObject = crosshairRoot;

        // Small Center Aim Dot: 5x5 thick black outline + 3x3 bright pure white center
        CreateCrosshairTick(crosshairRoot.transform, Vector2.zero, new Vector2(5f, 5f), new Color(0f, 0f, 0f, 0.98f));
        CreateCrosshairTick(crosshairRoot.transform, Vector2.zero, new Vector2(3f, 3f), Color.white);

        // 4 High-Contrast Directional Ticks with clear center gap (gap radius = 5.5px, tick extent = 14.5px, total footprint = 29px)
        // Each tick features a thick black outline (5px wide, 9px long) and crisp pure white core (3px wide, 7px long)
        // Top Tick
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(0f, 10f), new Vector2(5f, 9f), new Color(0f, 0f, 0f, 0.98f));
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(0f, 10f), new Vector2(3f, 7f), Color.white);

        // Bottom Tick
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(0f, -10f), new Vector2(5f, 9f), new Color(0f, 0f, 0f, 0.98f));
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(0f, -10f), new Vector2(3f, 7f), Color.white);

        // Left Tick
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(-10f, 0f), new Vector2(9f, 5f), new Color(0f, 0f, 0f, 0.98f));
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(-10f, 0f), new Vector2(7f, 3f), Color.white);

        // Right Tick
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(10f, 0f), new Vector2(9f, 5f), new Color(0f, 0f, 0f, 0.98f));
        CreateCrosshairTick(crosshairRoot.transform, new Vector2(10f, 0f), new Vector2(7f, 3f), Color.white);

        // Hit marker brackets
        GameObject hitGo = new GameObject("Crosshair_HitMarker", typeof(RectTransform), typeof(Image));
        hitGo.transform.SetParent(crosshairRoot.transform, false);
        var hitRt = hitGo.GetComponent<RectTransform>();
        hitRt.sizeDelta = new Vector2(28f, 28f);
        hitRt.anchorMin = hitRt.anchorMax = hitRt.pivot = new Vector2(0.5f, 0.5f);
        hitRt.anchoredPosition = Vector2.zero;
        crosshairHitMarker = hitGo.GetComponent<Image>();
        crosshairHitMarker.color = new Color(1f, 0.2f, 0.2f, 0f);
        crosshairHitMarker.raycastTarget = false;

        // Apply scale if customized
        SetCrosshairSize(crosshairFootprint);

        // ========================================================
        // 6. WEAPON PICKUP PROMPT PANEL (Lower Center Screen)
        // ========================================================
        GameObject pickupGo = new GameObject("Weapon_Pickup_Prompt_Panel", typeof(RectTransform), typeof(Image));
        pickupGo.transform.SetParent(hudPanel, false);
        var pRt = pickupGo.GetComponent<RectTransform>();
        pRt.anchorMin = new Vector2(0.5f, 0.22f);
        pRt.anchorMax = new Vector2(0.5f, 0.22f);
        pRt.pivot = new Vector2(0.5f, 0.5f);
        pRt.anchoredPosition = Vector2.zero;
        pRt.sizeDelta = new Vector2(480f, 48f);
        var pImg = pickupGo.GetComponent<Image>();
        pImg.color = new Color(0.04f, 0.06f, 0.09f, 0.94f);

        GameObject pTextGo = new GameObject("Pickup_Text", typeof(RectTransform), typeof(Text));
        pTextGo.transform.SetParent(pickupGo.transform, false);
        var pTextRt = pTextGo.GetComponent<RectTransform>();
        pTextRt.anchorMin = Vector2.zero;
        pTextRt.anchorMax = Vector2.one;
        pTextRt.offsetMin = new Vector2(14f, 4f);
        pTextRt.offsetMax = new Vector2(-14f, -4f);
        weaponPickupText = pTextGo.GetComponent<Text>();
        weaponPickupText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        weaponPickupText.fontSize = 20;
        weaponPickupText.fontStyle = FontStyle.Bold;
        weaponPickupText.alignment = TextAnchor.MiddleCenter;
        weaponPickupText.color = new Color(1.0f, 0.92f, 0.35f);
        weaponPickupText.text = "";
        weaponPickupPanel = pickupGo;
        weaponPickupPanel.SetActive(false);

        // ========================================================
        // 7. RESOLUTION TOGGLE INDICATOR (Top-Right HUD)
        // ========================================================
        GameObject resGo = new GameObject("Resolution_Indicator", typeof(RectTransform), typeof(Text));
        resGo.transform.SetParent(hudPanel, false);
        var resRt = resGo.GetComponent<RectTransform>();
        resRt.anchorMin = new Vector2(1f, 1f);
        resRt.anchorMax = new Vector2(1f, 1f);
        resRt.pivot = new Vector2(1f, 1f);
        resRt.anchoredPosition = new Vector2(-24f, -24f);
        resRt.sizeDelta = new Vector2(340f, 30f);
        resolutionText = resGo.GetComponent<Text>();
        resolutionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resolutionText.fontSize = 13;
        resolutionText.fontStyle = FontStyle.Bold;
        resolutionText.alignment = TextAnchor.MiddleRight;
        resolutionText.color = new Color(0.7f, 0.8f, 0.9f, 0.85f);
        resolutionText.text = "[F1] 720p | [F2] Q:MED | 60 FPS (16.6ms)";
    }

    private static void CreateCrosshairTick(Transform parent, Vector2 pos, Vector2 size, Color color)
    {
        GameObject tick = new GameObject("Tick", typeof(RectTransform), typeof(Image));
        tick.transform.SetParent(parent, false);
        var rt = tick.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = tick.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
    }

    private static Sprite GetOrCreateMinimapSprite()
    {
        if (cachedMinimapSprite != null) return cachedMinimapSprite;

        int w = 256;
        int h = 256;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        Color bg = new Color(0.05f, 0.07f, 0.10f, 0.95f);
        Color grid = new Color(0.09f, 0.13f, 0.18f, 0.55f);
        Color r1Floor = new Color(0.08f, 0.18f, 0.15f, 1.0f);
        Color r1Wall = new Color(0.18f, 0.48f, 0.38f, 1.0f);
        Color rFloor = new Color(0.11f, 0.17f, 0.23f, 1.0f);
        Color rWall = new Color(0.22f, 0.48f, 0.62f, 1.0f);
        Color r5Floor = new Color(0.18f, 0.12f, 0.15f, 1.0f);
        Color r5Wall = new Color(0.62f, 0.28f, 0.34f, 1.0f);
        Color exitCol = new Color(0.95f, 0.75f, 0.15f, 1.0f);

        Color[] pixels = new Color[w * h];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = bg;

        // Grid lines every 32 px
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (x % 32 == 0 || y % 32 == 0)
                {
                    pixels[y * w + x] = grid;
                }
            }
        }

        // Helper to convert world (wx, wz) to pixel (px, py)
        System.Func<float, float, Vector2Int> w2p = (wx, wz) =>
        {
            int px = Mathf.Clamp(Mathf.RoundToInt(((wx - MinWorldX) / (MaxWorldX - MinWorldX)) * (w - 1)), 0, w - 1);
            int py = Mathf.Clamp(Mathf.RoundToInt(((wz - MinWorldZ) / (MaxWorldZ - MinWorldZ)) * (h - 1)), 0, h - 1);
            return new Vector2Int(px, py);
        };

        // Draw filled boxes with border outlines
        System.Action<float, float, float, float, Color, Color, int> drawRoom = (x1, z1, x2, z2, fill, wall, borderWidth) =>
        {
            Vector2Int p1 = w2p(x1, z1);
            Vector2Int p2 = w2p(x2, z2);
            int minX = Mathf.Min(p1.x, p2.x);
            int maxX = Mathf.Max(p1.x, p2.x);
            int minY = Mathf.Min(p1.y, p2.y);
            int maxY = Mathf.Max(p1.y, p2.y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    bool isBorder = (x - minX < borderWidth || maxX - x < borderWidth ||
                                     y - minY < borderWidth || maxY - y < borderWidth);
                    pixels[y * w + x] = isBorder ? wall : fill;
                }
            }
        };

        // Helper to punch corridor openings through walls
        System.Action<float, float, float, float, Color> drawCorridor = (x1, z1, x2, z2, fill) =>
        {
            Vector2Int p1 = w2p(x1, z1);
            Vector2Int p2 = w2p(x2, z2);
            int minX = Mathf.Min(p1.x, p2.x);
            int maxX = Mathf.Max(p1.x, p2.x);
            int minY = Mathf.Min(p1.y, p2.y);
            int maxY = Mathf.Max(p1.y, p2.y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    pixels[y * w + x] = fill;
                }
            }
        };

        // 1. Room 1: Safe Start [-6, -32] to [6, -20]
        drawRoom(-6f, -32f, 6f, -20f, r1Floor, r1Wall, 2);

        // 2. Room 2: Patrol Corridor [-11, -20] to [11, -2]
        drawRoom(-11f, -20f, 11f, -2f, rFloor, rWall, 2);

        // 3. Room 3: Search Sector [-28, -2] to [-4, 20]
        drawRoom(-28f, -2f, -4f, 20f, rFloor, rWall, 2);

        // 4. Room 4: Defensive Sector [4, -2] to [28, 20]
        drawRoom(4f, -2f, 28f, 20f, rFloor, rWall, 2);

        // 5. Room 5: Boss Hall [-14, 18] to [14, 50]
        drawRoom(-14f, 18f, 14f, 50f, r5Floor, r5Wall, 2);

        // Room 5 Harder Layout Features:
        Color r5Obstacle = new Color(0.42f, 0.22f, 0.25f, 1.0f);
        Color r5Barrier = new Color(0.95f, 0.45f, 0.15f, 1.0f);
        Color r5Terminal = new Color(0.0f, 0.85f, 1.0f, 1.0f);

        // Central Staggered Stack 1 (Z 24.5 to 28.0)
        drawRoom(-1.8f, 24.5f, 1.8f, 28.0f, r5Obstacle, r5Wall, 1);
        // Central Staggered Stack 2 (Z 34.5 to 38.0, staggered east)
        drawRoom(-1.2f, 34.5f, 2.2f, 38.0f, r5Obstacle, r5Wall, 1);

        // West Outer Racks & Cover (Z 24 to 37)
        drawRoom(-12.0f, 24.0f, -8.5f, 37.0f, r5Obstacle, r5Wall, 1);
        // East Outer Racks & Cover (Z 24 to 37)
        drawRoom(8.5f, 24.0f, 12.0f, 37.0f, r5Obstacle, r5Wall, 1);

        // Security Checkpoint Barrier across center (Z 43.5 to 44.5)
        drawRoom(-2.7f, 43.5f, 2.7f, 44.5f, r5Barrier, r5Barrier, 1);

        // Card Reader Terminal Marker at (0, 48)
        drawRoom(-0.8f, 47.4f, 0.8f, 48.6f, r5Terminal, r5Terminal, 1);

        // Final Exit Doorway [ -2.5, 49.5] to [2.5, 51.5]
        drawRoom(-2.5f, 49.5f, 2.5f, 51.5f, exitCol, exitCol, 1);

        // Punch connecting corridor openings
        // Corridor 1->2
        drawCorridor(-2.0f, -21.0f, 2.0f, -19.0f, rFloor);
        // Corridor A (Room 2 -> Room 3)
        drawCorridor(-6.5f, -3.0f, -3.5f, 1.0f, rFloor);
        drawCorridor(-7.0f, -0.5f, -3.5f, 2.0f, rFloor);
        // Corridor B (Room 2 -> Room 4)
        drawCorridor(3.5f, -3.0f, 6.5f, 1.0f, rFloor);
        drawCorridor(3.5f, -0.5f, 7.0f, 2.0f, rFloor);
        // Corridor 3->5 (Room 3 -> Room 5)
        drawCorridor(-7.0f, 17.5f, -3.5f, 20.5f, r5Floor);
        // Corridor 4->5 (Room 4 -> Room 5)
        drawCorridor(3.5f, 17.5f, 7.0f, 20.5f, r5Floor);

        // Border around entire texture
        for (int i = 0; i < w; i++)
        {
            pixels[i] = rWall;
            pixels[(h - 1) * w + i] = rWall;
        }
        for (int i = 0; i < h; i++)
        {
            pixels[i * w] = rWall;
            pixels[i * w + (w - 1)] = rWall;
        }

        tex.SetPixels(pixels);
        tex.Apply(false, true);

        cachedMinimapSprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
        return cachedMinimapSprite;
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

        UpdateMinimap();
        UpdateHealthDisplay();
        UpdateWeaponDisplay();
        UpdatePickupDisplay();
        UpdateCrosshairVisibility();
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

        // Native Screenshot manual capture via F9 key
        if (Input.GetKeyDown(KeyCode.F9))
        {
            StartCoroutine(CaptureFrameToDisk("gameplay_manual_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png"));
        }

        UpdatePerformanceTracking();
    }

    private void UpdateMinimap()
    {
        if (playerMarkerRt == null) return;

        Transform playerTf = playerHealth != null ? playerHealth.transform : null;
        if (playerTf == null)
        {
            var pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) playerTf = pObj.transform;
        }

        if (playerTf != null)
        {
            Vector3 pos = playerTf.position;
            float u = Mathf.Clamp01((pos.x - MinWorldX) / (MaxWorldX - MinWorldX));
            float v = Mathf.Clamp01((pos.z - MinWorldZ) / (MaxWorldZ - MinWorldZ));

            playerMarkerRt.anchoredPosition = new Vector2(
                (u - 0.5f) * MinimapWidth,
                (v - 0.5f) * MinimapHeight
            );

            Camera cam = playerTf.GetComponentInChildren<Camera>();
            float yaw = cam != null ? cam.transform.eulerAngles.y : playerTf.eulerAngles.y;
            playerMarkerRt.localRotation = Quaternion.Euler(0f, 0f, -yaw);
        }
    }

    private void UpdateCrosshairVisibility()
    {
        bool showCrosshair = true;
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver || GameManager.Instance.IsGameWon)
            {
                showCrosshair = false;
            }
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            showCrosshair = false;
        }

        if (crosshairRootObject != null && crosshairRootObject.activeSelf != showCrosshair)
        {
            crosshairRootObject.SetActive(showCrosshair);
        }
    }

    private void UpdatePickupDisplay()
    {
        // Avoid duplicate pickup prompt panel if GameManager has its interaction prompt active
        if (GameManager.Instance != null && GameManager.Instance.InteractionPromptText != null &&
            GameManager.Instance.HudPanel != null)
        {
            if (weaponPickupPanel != null && weaponPickupPanel.activeSelf)
                weaponPickupPanel.SetActive(false);
            return;
        }

        if (weaponPickupPanel != null && weaponPickupText != null)
        {
            string prompt = playerGun != null ? playerGun.GetNearbyPickupPrompt() : "";
            if (!string.IsNullOrEmpty(prompt))
            {
                weaponPickupPanel.SetActive(true);
                weaponPickupText.text = prompt;
            }
            else
            {
                weaponPickupPanel.SetActive(false);
            }
        }
    }

    private void UpdateHealthDisplay()
    {
        if (playerHealth != null)
        {
            float norm = playerHealth.HealthNormalized;
            int hp = playerHealth.CurrentHealth;
            int maxHp = playerHealth.MaxHealth;

            if (healthFillImage != null)
            {
                healthFillImage.fillAmount = norm;
                healthFillImage.color = Color.Lerp(
                    new Color(0.90f, 0.15f, 0.15f),
                    new Color(0.20f, 0.85f, 0.35f),
                    norm
                );
            }

            if (healthNumericText != null)
            {
                if (hp <= 30 && hp > 0)
                {
                    // Pulsing red warning for low health without premature defeat
                    float pulse = Mathf.PingPong(Time.unscaledTime * 4f, 1f);
                    Color warnCol = Color.Lerp(Color.white, new Color(1f, 0.2f, 0.2f), pulse);
                    healthNumericText.text = $"{hp} / {maxHp}  <color=#FF2222>CRITICAL</color>";
                    healthNumericText.color = warnCol;
                }
                else
                {
                    healthNumericText.text = $"{hp} / {maxHp}";
                    healthNumericText.color = Color.white;
                }
            }

            if (healthRegenText != null)
            {
                if (playerHealth != null && playerHealth.IsRegenerating)
                {
                    float pulse = Mathf.PingPong(Time.unscaledTime * 3f, 1f);
                    Color regenCol = Color.Lerp(new Color(0.25f, 0.85f, 0.45f, 0.85f), new Color(0.50f, 1.0f, 0.70f, 1.0f), pulse);
                    healthRegenText.text = "Recovering +10 HP/s";
                    healthRegenText.color = regenCol;
                }
                else
                {
                    healthRegenText.text = "";
                }
            }
        }
    }

    private void UpdateWeaponDisplay()
    {
        if (playerGun != null)
        {
            if (weaponNameText != null)
            {
                weaponNameText.text = playerGun.CurrentWeaponName.ToUpper();
            }

            if (ammoText != null)
            {
                ammoText.text = $"{playerGun.CurrentAmmo} / \u221E";
                ammoText.color = playerGun.CurrentAmmo == 0 ? new Color(0.9f, 0.2f, 0.2f) : Color.white;
            }

            if (reloadText != null)
            {
                if (playerGun.IsReloading)
                {
                    reloadText.text = "RELOADING...";
                    reloadText.color = new Color(0.95f, 0.55f, 0.15f);
                }
                else if (playerGun.CurrentAmmo == 0)
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
        // Keep the Unity editor in its Game view; standalone players fill the monitor.
        Screen.SetResolution(width, height, Application.isEditor ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow);
        if (!Application.isEditor) StartCoroutine(LogDisplayState());

        UpdateResolutionText();
        Debug.Log($"[PlayerHUD] Target resolution set to {(is1080p ? "1080p (1920x1080)" : "720p (1280x720)")} | FPS: {currentFps:F1}");
    }

    private System.Collections.IEnumerator LogDisplayState()
    {
        // Resolution/mode changes take effect after the current frame.
        yield return null;
        yield return null;
        Debug.Log($"[Display] Mode={Screen.fullScreenMode} Fullscreen={Screen.fullScreen} Size={Screen.width}x{Screen.height}");
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
        float dt = Time.unscaledDeltaTime;
        if (dt <= 0.00001f) return;

        // 1. Separate startup / loading warmup period
        if (Time.unscaledTime <= 3.0f)
        {
            excludedStartupFrames++;
        }
        // 2. Explicitly marked capture operations and immediate post-capture disk I/O
        else if (IsExplicitCaptureActive || captureCooldownFrames > 0)
        {
            excludedCaptureFrames++;
            if (captureCooldownFrames > 0) captureCooldownFrames--;
        }
        // 3. Normal gameplay frames: ALL gameplay frames included (genuine stalls >=200ms are NOT filtered)
        else
        {
            peakWorstGameplayFrameTime = Mathf.Max(peakWorstGameplayFrameTime, dt);
            if (recentFrameTimes.Count < 50000)
            {
                recentFrameTimes.Add(dt);
            }
            else
            {
                recentFrameTimes[recentFrameTimes.Count % 50000] = dt;
            }
        }

        // On-screen rolling FPS display (kept separate from benchmark measurements):
        // Only ignore artificial capture operations from disrupting the visual readout
        if (!IsExplicitCaptureActive && captureCooldownFrames == 0)
        {
            fpsFrameCount++;
            fpsUpdateTimer += dt;
            if (fpsUpdateTimer >= 0.5f)
            {
                currentFps = fpsFrameCount / fpsUpdateTimer;
                fpsFrameCount = 0;
                fpsUpdateTimer = 0f;
                UpdateResolutionText();
            }
        }

        performanceLogTimer += dt;
        if (performanceLogTimer >= 5.0f)
        {
            performanceLogTimer = 0f;
            float frameTimeMs = 1000f / Mathf.Max(currentFps, 1f);

            float onePercentLowFps = currentFps;
            float worstFrameTimeMs = peakWorstGameplayFrameTime * 1000f;
            if (recentFrameTimes.Count >= 30)
            {
                // Reuse pre-allocated scratch list of the last <= 300 frames to compute rolling 1% low without allocating or sorting 50k items
                periodicLogScratch.Clear();
                int sampleCount = Mathf.Min(recentFrameTimes.Count, 300);
                int startIdx = recentFrameTimes.Count - sampleCount;
                for (int i = 0; i < sampleCount; i++) periodicLogScratch.Add(recentFrameTimes[startIdx + i]);
                periodicLogScratch.Sort();
                int idx99 = Mathf.Clamp((int)(periodicLogScratch.Count * 0.99f), 0, periodicLogScratch.Count - 1);
                float ft99 = periodicLogScratch[idx99];
                if (ft99 > 0.00001f) onePercentLowFps = 1f / ft99;
            }

            long totalMemMb = System.GC.GetTotalMemory(false) / (1024 * 1024);
            long unityAllocMb = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
            long unityReservedMb = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024);

            Debug.Log($"[Performance Metrics] Resolution: {(is1080p ? "1080p" : "720p")} | Quality: {(isLowQuality ? "LOW" : "MED")} | Average FPS: {currentFps:F1} ({frameTimeMs:F1}ms) | 1% Low FPS: {onePercentLowFps:F1} | Worst Frame Time: {worstFrameTimeMs:F1}ms | Excluded Startup: {excludedStartupFrames} (Startup <= 3.0s) | Excluded Capture: {excludedCaptureFrames} (Explicit capture/screenshot operations) | Unity Allocated: {unityAllocMb}MB | Reserved: {unityReservedMb}MB | GC Heap: {totalMemMb}MB | Sampled Normal Frames: {recentFrameTimes.Count}");
        }
    }

    void OnApplicationQuit()
    {
        if (recentFrameTimes.Count >= 30)
        {
            var sorted = new System.Collections.Generic.List<float>(recentFrameTimes);
            sorted.Sort();
            float sumFt = 0f;
            for (int i = 0; i < sorted.Count; i++) sumFt += sorted[i];
            float avgFt = sumFt / sorted.Count;
            float avgFps = avgFt > 0.00001f ? 1f / avgFt : 0f;
            int idx99 = Mathf.Clamp((int)(sorted.Count * 0.99f), 0, sorted.Count - 1);
            float ft99 = sorted[idx99];
            float onePercentLow = ft99 > 0.00001f ? 1f / ft99 : 0f;
            float worstFrameTimeMs = sorted[sorted.Count - 1] * 1000f;

            int spikes50 = 0, spikes100 = 0, spikes200 = 0;
            for (int i = 0; i < sorted.Count; i++)
            {
                float ftMs = sorted[i] * 1000f;
                if (ftMs >= 50f) spikes50++;
                if (ftMs >= 100f) spikes100++;
                if (ftMs >= 200f) spikes200++;
            }

            string summary = $"STANDALONE_BENCHMARK_SUMMARY\n" +
                             $"Resolution: {(is1080p ? "1080p" : "720p")} (Target: {(is1080p ? "1920x1080" : "1280x720")}, Actual: {Screen.width}x{Screen.height})\n" +
                             $"Quality: {(isLowQuality ? "LOW" : "MEDIUM")}\n" +
                             $"Total Sampled Normal Gameplay Frames: {sorted.Count}\n" +
                             $"Total Sampled Frames: {sorted.Count}\n" +
                             $"Average FPS: {avgFps:F2}\n" +
                             $"Average Frame Time: {avgFt * 1000f:F2} ms\n" +
                             $"1% Low FPS: {onePercentLow:F2} (99th percentile frame time: {ft99 * 1000f:F2} ms)\n" +
                             $"Worst Frame Time: {worstFrameTimeMs:F2} ms\n" +
                             $"Max Frame Spike: {worstFrameTimeMs:F2} ms\n" +
                             $"Spikes > 50ms: {spikes50}\n" +
                             $"Spikes > 100ms: {spikes100}\n" +
                             $"Spikes > 200ms: {spikes200}\n" +
                             $"Excluded Startup Frames: {excludedStartupFrames} (Reason: Initial startup/loading warmup <= 3.0s)\n" +
                             $"Excluded Capture Frames: {excludedCaptureFrames} (Reason: Explicitly marked capture/screenshot operations)\n" +
                             $"Unity Allocated Memory: {UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)} MB\n" +
                             $"Unity Reserved Memory: {UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)} MB\n" +
                             $"GC Total Memory: {System.GC.GetTotalMemory(false) / (1024 * 1024)} MB\n" +
                             $"Caution: This benchmark represents a short-duration test. Short runs cannot establish zero memory leaks or stutter-free performance across prolonged gameplay.\n";

            string outPath1 = System.IO.Path.Combine(Application.dataPath, $"../scratch/standalone_benchmark_{(is1080p ? "1080p" : "720p")}.txt");
            string outPath2 = System.IO.Path.Combine(Application.dataPath, $"../../../scratch/standalone_benchmark_{(is1080p ? "1080p" : "720p")}.txt");
            try { System.IO.File.WriteAllText(outPath1, summary); } catch {}
            try { System.IO.File.WriteAllText(outPath2, summary); } catch {}
            Debug.Log($"[PlayerHUD] Wrote performance benchmark summary to: {outPath1}");
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

    private void CheckForVerificationTrigger()
    {
        bool hasArg = System.Linq.Enumerable.Any(System.Environment.GetCommandLineArgs(), a => a.Equals("-verify-display", System.StringComparison.OrdinalIgnoreCase));
        string flag1 = System.IO.Path.Combine(Application.dataPath, "../scratch/run_gameplay_capture.flag");
        string flag2 = System.IO.Path.Combine(Application.dataPath, "../../scratch/run_gameplay_capture.flag");
        string flag3 = System.IO.Path.Combine(Application.dataPath, "../../../scratch/run_gameplay_capture.flag");

        if (hasArg || System.IO.File.Exists(flag1) || System.IO.File.Exists(flag2) || System.IO.File.Exists(flag3))
        {
            if (System.IO.File.Exists(flag1)) try { System.IO.File.Delete(flag1); } catch {}
            if (System.IO.File.Exists(flag2)) try { System.IO.File.Delete(flag2); } catch {}
            if (System.IO.File.Exists(flag3)) try { System.IO.File.Delete(flag3); } catch {}
            StartCoroutine(RunNativeVerificationCaptureSequence());
        }
    }

    private System.Collections.IEnumerator CaptureFrameToDisk(string filename)
    {
        IsExplicitCaptureActive = true;
        yield return new WaitForEndOfFrame();
        Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture();
        if (shot != null)
        {
            byte[] png = shot.EncodeToPNG();
            int w = shot.width;
            int h = shot.height;
            Destroy(shot);

            string path1 = System.IO.Path.Combine(Application.dataPath, "../scratch/screenshots", filename);
            string path2 = System.IO.Path.Combine(Application.dataPath, "../../scratch/screenshots", filename);
            string path3 = System.IO.Path.Combine(Application.dataPath, "../../../scratch/screenshots", filename);

            System.Action<string> saveSafe = (p) =>
            {
                try
                {
                    string d = System.IO.Path.GetDirectoryName(p);
                    if (!System.IO.Directory.Exists(d)) System.IO.Directory.CreateDirectory(d);
                    System.IO.File.WriteAllBytes(p, png);
                    Debug.Log($"[PlayerHUD] Saved frame to: {p} ({w}x{h})");
                }
                catch {}
            };

            saveSafe(path1);
            saveSafe(path2);
            saveSafe(path3);
        }
        captureCooldownFrames = 1;
        IsExplicitCaptureActive = false;
        fpsFrameCount = 0;
        fpsUpdateTimer = 0f;
    }

    private System.Collections.IEnumerator RunNativeVerificationCaptureSequence()
    {
        Debug.Log("[PlayerHUD] Starting native gameplay display verification capture sequence...");
        yield return new WaitForSecondsRealtime(0.8f);

        // Frame 1: Spawn room dark area (720p native)
        yield return StartCoroutine(CaptureFrameToDisk("gameplay_actual_720p_spawn_dark.png"));

        // Frame 2: Move player to illuminated doorway facing Room 2
        var pm = Object.FindFirstObjectByType<PlayerMovement>();
        if (pm != null)
        {
            var cc = pm.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            pm.transform.position = new Vector3(0f, 1.08f, -19.5f);
            pm.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            pm.CameraPitch = 0f;
            if (cc != null) cc.enabled = true;
            Physics.SyncTransforms();
        }
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(CaptureFrameToDisk("gameplay_actual_720p_light.png"));

        // Frame 3: Tactical Pause (verifies crosshair hiding & pause panel)
        if (GameManager.Instance != null && !GameManager.Instance.IsPaused)
        {
            GameManager.Instance.TogglePause();
        }
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(CaptureFrameToDisk("gameplay_actual_720p_paused.png"));

        // Frame 4: Resume (verifies crosshair restoration)
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
        {
            GameManager.Instance.TogglePause();
        }
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(CaptureFrameToDisk("gameplay_actual_720p_resumed.png"));

        // Frame 5: Toggle Help Panel (verifies collapsed help panel)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ToggleHelpPanel();
        }
        yield return new WaitForSecondsRealtime(0.4f);
        yield return StartCoroutine(CaptureFrameToDisk("gameplay_actual_720p_help_toggled.png"));
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ToggleHelpPanel(); // restore
        }

        // Frame 6: 1080p Resolution
        SetResolution(true);
        yield return new WaitForSecondsRealtime(0.6f);
        yield return StartCoroutine(CaptureFrameToDisk("gameplay_actual_1080p_spawn.png"));

        // Restore 720p
        SetResolution(false);
        yield return new WaitForSecondsRealtime(0.4f);

        // Reset player position to start
        if (pm != null)
        {
            var cc = pm.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            pm.transform.position = new Vector3(0f, 1.08f, -26f);
            pm.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            if (cc != null) cc.enabled = true;
            Physics.SyncTransforms();
        }

        string flagDone1 = System.IO.Path.Combine(Application.dataPath, "../scratch/gameplay_capture_done.flag");
        string flagDone2 = System.IO.Path.Combine(Application.dataPath, "../../scratch/gameplay_capture_done.flag");
        string flagDone3 = System.IO.Path.Combine(Application.dataPath, "../../../scratch/gameplay_capture_done.flag");
        try { System.IO.File.WriteAllText(flagDone1, "CAPTURES_COMPLETED"); } catch {}
        try { System.IO.File.WriteAllText(flagDone2, "CAPTURES_COMPLETED"); } catch {}
        try { System.IO.File.WriteAllText(flagDone3, "CAPTURES_COMPLETED"); } catch {}

        Debug.Log("[PlayerHUD] Native gameplay display verification capture sequence completed successfully!");

        bool quitAfter = System.Linq.Enumerable.Any(System.Environment.GetCommandLineArgs(), a => a.Equals("-quit-after-verify", System.StringComparison.OrdinalIgnoreCase));
        if (quitAfter)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
