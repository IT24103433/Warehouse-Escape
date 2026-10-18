using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World-space dynamic billboard health bar for enemy guards.
/// Shown only when relevant (alerted, damaged, or targeted within combat distance)
/// and strictly verifies unobstructed line-of-sight so guards are NEVER revealed through walls or cover.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float barHeight = 2.2f;
    [SerializeField] private float maxDisplayDistance = 18f;
    [SerializeField] private LayerMask occlusionMask = ~0;

    private Health health;
    private Camera playerCam;
    private Canvas canvas;
    private Image fillImage;
    private Text nameText;
    private Text hpText;
    private Transform barTransform;
    public bool IsBarVisible => canvas != null && canvas.enabled;

    void Start()
    {
        health = GetComponent<Health>();
        SetupWorldCanvas();
        FindPlayerCam();
    }

    private void FindPlayerCam()
    {
        if (playerCam == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerCam = p.GetComponentInChildren<Camera>();
            if (playerCam == null) playerCam = Camera.main;
        }
    }

    private void SetupWorldCanvas()
    {
        Transform existing = transform.Find("HealthBar_Canvas");
        if (existing != null)
        {
            barTransform = existing;
            canvas = existing.GetComponent<Canvas>();
            return;
        }

        GameObject canvasGo = new GameObject("HealthBar_Canvas");
        canvasGo.transform.SetParent(transform, false);

        canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGo.AddComponent<CanvasScaler>();

        barTransform = canvasGo.transform;
        barTransform.localPosition = new Vector3(0f, barHeight, 0f);

        var rt = canvasGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 40f);
        canvasGo.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);

        // Background
        GameObject bgGo = new GameObject("Background");
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgRt = bgGo.AddComponent<RectTransform>();
        bgRt.sizeDelta = new Vector2(180f, 18f);
        var bgImg = bgGo.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.12f, 0.16f, 0.85f);

        // Fill
        GameObject fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(bgGo.transform, false);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(1f, 1f);
        fillRt.offsetMin = new Vector2(2f, 2f);
        fillRt.offsetMax = new Vector2(-2f, -2f);
        fillImage = fillGo.AddComponent<Image>();
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.color = new Color(0.85f, 0.2f, 0.2f, 1.0f);

        // Name & Numeric Text
        GameObject textGo = new GameObject("Label");
        textGo.transform.SetParent(canvasGo.transform, false);
        var textRt = textGo.AddComponent<RectTransform>();
        textRt.sizeDelta = new Vector2(180f, 20f);
        textRt.anchoredPosition = new Vector2(0f, 18f);
        nameText = textGo.AddComponent<Text>();
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 12;
        nameText.fontStyle = FontStyle.Bold;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.white;
        nameText.text = health != null ? health.CharacterName : gameObject.name;

        GameObject hpGo = new GameObject("HP_Value");
        hpGo.transform.SetParent(canvasGo.transform, false);
        var hpRt = hpGo.AddComponent<RectTransform>();
        hpRt.sizeDelta = new Vector2(180f, 16f);
        hpRt.anchoredPosition = new Vector2(0f, 0f);
        hpText = hpGo.AddComponent<Text>();
        hpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hpText.fontSize = 11;
        hpText.fontStyle = FontStyle.Bold;
        hpText.alignment = TextAnchor.MiddleCenter;
        hpText.color = Color.white;

        canvas.enabled = false;
    }

    public bool DebugIsDamaged { get; private set; }
    public bool DebugIsLookingNear { get; private set; }
    public float DebugAngleBody { get; private set; }
    public float DebugAngleHead { get; private set; }
    public int DebugHitsCount { get; private set; }
    public string DebugOccludedBy { get; private set; } = "None";
    public string DebugExitReason { get; private set; } = "None";

    void LateUpdate()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy)
        {
            DebugExitReason = "Inactive";
            return;
        }

        if (health == null || health.IsDead || (GameManager.Instance != null && GameManager.Instance.IsGameEnded))
        {
            if (canvas != null) canvas.enabled = false;
            DebugExitReason = "DeadOrGameEnded";
            return;
        }

        if (barTransform == null)
        {
            SetupWorldCanvas();
            if (barTransform == null)
            {
                DebugExitReason = "NoBarTransform";
                return;
            }
        }

        if (playerCam == null)
        {
            FindPlayerCam();
        }

        if (playerCam == null)
        {
            DebugExitReason = "NoPlayerCam";
            return;
        }

        Transform camT;
        try
        {
            camT = playerCam.transform;
            if (camT == null)
            {
                DebugExitReason = "NoCamTransform";
                return;
            }
        }
        catch (System.Exception)
        {
            playerCam = null;
            DebugExitReason = "CamException";
            return;
        }

        Vector3 camPos = camT.position;
        Vector3 targetPos = transform.position + Vector3.up * barHeight;
        Vector3 toCam = camPos - targetPos;
        float dist = toCam.magnitude;

        // Hide if beyond maximum display distance
        if (dist > maxDisplayDistance)
        {
            if (canvas != null) canvas.enabled = false;
            DebugExitReason = "TooFar";
            return;
        }

        // Show when relevant: damaged, or player is facing this guard
        bool isDamaged = health.CurrentHealth < health.MaxHealth;
        Vector3 camFwd = camT.forward;
        Vector3 guardCenterPos = transform.position + Vector3.up * 0.4f;
        float angleToBody = Vector3.Angle(camFwd, (guardCenterPos - camPos).normalized);
        float angleToHead = Vector3.Angle(camFwd, (targetPos - camPos).normalized);
        bool isLookingNear = angleToBody < 50f || angleToHead < 50f;

        DebugIsDamaged = isDamaged;
        DebugIsLookingNear = isLookingNear;
        DebugAngleBody = angleToBody;
        DebugAngleHead = angleToHead;

        if (!isDamaged && !isLookingNear)
        {
            if (canvas != null) canvas.enabled = false;
            DebugExitReason = $"NotLookingNear(body={angleToBody:F1},head={angleToHead:F1})";
            return;
        }

        // STRICT LINE-OF-SIGHT CHECK: Raycast between player eye and guard head
        // Never reveal guards through walls, racks, crates, or doors!
        Vector3 toGuard = guardCenterPos - camPos;
        Vector3 rayDir = toGuard.normalized;
        float rayDist = toGuard.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(camPos, rayDir, rayDist, occlusionMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        DebugHitsCount = hits.Length;

        for (int i = 0; i < hits.Length; i++)
        {
            var h = hits[i];
            // Reached this guard or its children -> clear line of sight
            if (h.transform == transform || h.transform.IsChildOf(transform)) break;

            // Ignore player's own body/controller at camera position
            if (h.collider.CompareTag("Player") || h.transform.root == camT.root) continue;

            // Intervening wall, crate, or cover obstacle -> OCCLUDED!
            if (canvas != null) canvas.enabled = false;
            DebugOccludedBy = h.collider.gameObject.name;
            DebugExitReason = "OccludedBy_" + h.collider.gameObject.name;
            return;
        }

        // Unoccluded and relevant -> Display and Billboard toward Player
        if (canvas != null) canvas.enabled = true;
        DebugExitReason = "Visible";
        if (barTransform != null && camT != null)
        {
            barTransform.rotation = camT.rotation;
        }

        float norm = health.HealthNormalized;
        if (fillImage != null)
        {
            fillImage.fillAmount = norm;
            fillImage.color = Color.Lerp(new Color(0.85f, 0.15f, 0.15f), new Color(0.2f, 0.85f, 0.3f), norm);
        }

        if (hpText != null)
        {
            hpText.text = $"{health.CurrentHealth} / {health.MaxHealth}";
        }
    }
}
