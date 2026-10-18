using System;
using UnityEngine;

public enum GuardPerceptionState
{
    Unaware,
    Suspicious,
    Alerted,
    Searching,
    Returning
}

/// <summary>
/// Believable perception and memory controller for autonomous guards.
/// Features:
/// - Progressive suspicion buildup with sustained line-of-sight and smooth decay after losing sight.
/// - Distance-dependent alert buildup (close range alerts much faster; walls strictly block vision).
/// - Gunshot and running-footstep hearing with distance limits and physical obstacle attenuation.
/// - Last Known Position (LKP) memory with decaying confidence and expiration timestamps.
/// - Safe radio broadcast integration.
/// - Readable overhead status indicator (?, !, SEARCH, RETURN) occluded by walls.
/// </summary>
public class GuardPerceptionMemory : MonoBehaviour
{
    [Header("Perception State")]
    [SerializeField] private GuardPerceptionState perceptionState = GuardPerceptionState.Unaware;
    [Range(0f, 1f)]
    [SerializeField] private float suspicionLevel = 0f;
    [SerializeField] private bool canSeePlayer = false;
    [SerializeField] private bool visualOnlyDetection = false;

    [Header("Memory Tracking")]
    [SerializeField] private Vector3 lastKnownPosition;
    [SerializeField] private float lkpTimestamp = -999f;
    [Range(0f, 1f)]
    [SerializeField] private float lkpConfidence = 0f;
    [SerializeField] private float memoryDuration = 7.5f;

    [Header("Hearing Settings")]
    [SerializeField] private float maxHearingRange = 26f;
    [SerializeField] private LayerMask obstacleMask = ~0;

    [Header("Vision Parameters")]
    [SerializeField] private float sightDistance = 12f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private float eyeHeight = 0.6f;
    [SerializeField] private float playerTargetHeight = 0f;

    private Transform playerTransform;
    private Health health;
    private TextMesh indicatorTextMesh;
    private GameObject indicatorObj;
    private Camera mainCamera;

    public GuardPerceptionState PerceptionState => perceptionState;
    public float SuspicionLevel { get => suspicionLevel; set => suspicionLevel = Mathf.Clamp01(value); }
    public bool CanSeePlayer => canSeePlayer;
    public bool VisualOnlyDetection { get => visualOnlyDetection; set => visualOnlyDetection = value; }
    public Vector3 LastKnownPosition => lastKnownPosition;
    public float LkpTimestamp => lkpTimestamp;
    public float LkpConfidence => lkpConfidence;
    public float MemoryDuration { get => memoryDuration; set => memoryDuration = value; }
    public bool HasValidLKP => lkpConfidence > 0.05f && (Time.time - lkpTimestamp) < memoryDuration;

    public event Action<Vector3> OnDisturbanceDetected;
    public event Action<GuardPerceptionState> OnPerceptionStateChanged;

    void Awake()
    {
        health = GetComponent<Health>();
        SetupIndicator();
    }

    void OnEnable()
    {
        GuardPerception.OnSoundReported += HandleSoundDisturbance;
        GuardRadioManager.OnRadioAlertReceived += HandleRadioMessage;
    }

    void OnDisable()
    {
        GuardPerception.OnSoundReported -= HandleSoundDisturbance;
        GuardRadioManager.OnRadioAlertReceived -= HandleRadioMessage;
    }

    void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (playerTransform == null)
        {
            var pObj = GameObject.FindGameObjectWithTag("Player");
            if (pObj != null) playerTransform = pObj.transform;
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (health != null && health.IsDead)
        {
            SetIndicatorActive(false);
            return;
        }

        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        // 1. Line-of-sight evaluation
        canSeePlayer = GuardPerception.CanSeePlayer(
            transform, playerTransform, sightDistance, fieldOfView, eyeHeight, playerTargetHeight, obstacleMask
        );

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 2. Suspicion buildup / decay
        float prevSuspicion = suspicionLevel;
        suspicionLevel = GuardPerception.UpdateSuspicion(suspicionLevel, canSeePlayer, distToPlayer, Time.deltaTime);

        // 3. Update LKP when actively seeing player
        if (canSeePlayer)
        {
            lastKnownPosition = playerTransform.position;
            lkpTimestamp = Time.time;
            lkpConfidence = 1.0f;
        }
        else
        {
            // Confidence decays smoothly over memoryDuration
            if (lkpTimestamp > 0f)
            {
                float elapsed = Time.time - lkpTimestamp;
                lkpConfidence = Mathf.Clamp01(1.0f - (elapsed / memoryDuration));
            }
        }

        // 4. Update Perception State
        UpdatePerceptionState(prevSuspicion);

        // 5. Update overhead visual indicator
        UpdateIndicatorPresentation();
    }

    private void UpdatePerceptionState(float prevSuspicion)
    {
        GuardPerceptionState oldState = perceptionState;

        if (suspicionLevel >= 1.0f)
        {
            if (canSeePlayer)
            {
                perceptionState = GuardPerceptionState.Alerted;
            }
            else if (HasValidLKP)
            {
                perceptionState = GuardPerceptionState.Searching;
            }
            else
            {
                perceptionState = GuardPerceptionState.Returning;
            }
        }
        else if (suspicionLevel >= 0.25f)
        {
            perceptionState = GuardPerceptionState.Suspicious;
        }
        else
        {
            if (HasValidLKP && perceptionState == GuardPerceptionState.Searching)
            {
                // Maintain search until LKP expires or timeout
            }
            else
            {
                perceptionState = GuardPerceptionState.Unaware;
            }
        }

        // Broadcast radio when transitioning to full alert
        if (oldState != GuardPerceptionState.Alerted && perceptionState == GuardPerceptionState.Alerted)
        {
            GuardRadioManager.BroadcastAlert(gameObject.name, lastKnownPosition);
        }

        if (oldState != perceptionState)
        {
            OnPerceptionStateChanged?.Invoke(perceptionState);
        }
    }

    private void HandleSoundDisturbance(SoundDisturbance sound)
    {
        if (health != null && health.IsDead) return;
        if (visualOnlyDetection) return; // Visual-only guards like Varun ignore all audio disturbances

        // Check if guard can hear this sound with distance & wall attenuation
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        if (GuardPerception.CanHearSound(eyePos, sound, obstacleMask, out float audibility))
        {
            if (sound.soundType == SoundType.Gunshot)
            {
                // Gunshots provoke immediate alert & investigation
                suspicionLevel = 1.0f;
                lastKnownPosition = sound.position;
                lkpTimestamp = Time.time;
                lkpConfidence = 1.0f;
                perceptionState = GuardPerceptionState.Alerted;

                Debug.Log($"[GuardPerceptionMemory] '{gameObject.name}' heard GUNSHOT at {sound.position:F1} (audibility: {audibility:F2}). Investigating!");
                OnDisturbanceDetected?.Invoke(sound.position);
            }
            else if (sound.soundType == SoundType.Footstep)
            {
                // Running footsteps provoke suspicion
                suspicionLevel = Mathf.Max(suspicionLevel, 0.45f);
                lastKnownPosition = sound.position;
                lkpTimestamp = Time.time;
                lkpConfidence = 0.8f;

                Debug.Log($"[GuardPerceptionMemory] '{gameObject.name}' heard FOOTSTEPS at {sound.position:F1} (audibility: {audibility:F2}).");
                OnDisturbanceDetected?.Invoke(sound.position);
            }
        }
    }

    private void HandleRadioMessage(RadioMessage msg)
    {
        if (health != null && health.IsDead) return;
        if (visualOnlyDetection) return; // Visual-only guards like Varun ignore radio alerts
        if (msg.reportingGuard == gameObject.name) return; // Ignore own broadcast

        // Check if the reported LKP is close enough or relevant to this guard
        float distToLkp = Vector3.Distance(transform.position, msg.lastKnownPosition);
        if (distToLkp < 35.0f && !canSeePlayer)
        {
            // Enter heightened alertness
            suspicionLevel = Mathf.Max(suspicionLevel, 0.5f);
            if (!HasValidLKP)
            {
                lastKnownPosition = msg.lastKnownPosition;
                lkpTimestamp = msg.timestamp;
                lkpConfidence = 0.70f;
            }
            Debug.Log($"[GuardPerceptionMemory] '{gameObject.name}' received radio dispatch from '{msg.reportingGuard}'. Heightened alert.");
        }
    }

    private void SetupIndicator()
    {
        if (indicatorObj != null) return;

        indicatorObj = new GameObject("Perception_Status_Indicator");
        indicatorObj.transform.SetParent(transform, false);
        indicatorObj.transform.localPosition = new Vector3(0f, 2.25f, 0f);

        indicatorTextMesh = indicatorObj.AddComponent<TextMesh>();
        indicatorTextMesh.text = "";
        indicatorTextMesh.fontSize = 24;
        indicatorTextMesh.characterSize = 0.12f;
        indicatorTextMesh.alignment = TextAlignment.Center;
        indicatorTextMesh.anchor = TextAnchor.MiddleCenter;
        indicatorTextMesh.fontStyle = FontStyle.Bold;

        // Apply depth-tested shader so indicator is strictly occluded by walls
        Shader dtShader = Shader.Find("Custom/DepthTestedText");
        if (dtShader != null)
        {
            Material mat = new Material(dtShader);
            indicatorObj.GetComponent<MeshRenderer>().material = mat;
        }

        indicatorObj.SetActive(false);
    }

    private void UpdateIndicatorPresentation()
    {
        if (indicatorObj == null || indicatorTextMesh == null) return;

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null)
        {
            SetIndicatorActive(false);
            return;
        }

        // Strict wall occlusion: If intervening geometry blocks line from camera to indicator, hide it!
        Vector3 camPos = mainCamera.transform.position;
        Vector3 indPos = indicatorObj.transform.position;
        Vector3 toCam = camPos - indPos;
        float distToCam = toCam.magnitude;

        if (Physics.Raycast(indPos, toCam.normalized, distToCam - 0.2f, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            // Wall or obstacle is between player camera and guard indicator -> DO NOT REVEAL
            SetIndicatorActive(false);
            return;
        }

        // Billboard towards camera
        indicatorObj.transform.rotation = Quaternion.LookRotation(indicatorObj.transform.position - camPos);

        switch (perceptionState)
        {
            case GuardPerceptionState.Suspicious:
                indicatorTextMesh.text = "?";
                indicatorTextMesh.color = new Color(1f, 0.85f, 0.2f, 1f); // Yellow
                SetIndicatorActive(true);
                break;

            case GuardPerceptionState.Alerted:
                indicatorTextMesh.text = "!";
                indicatorTextMesh.color = new Color(1f, 0.2f, 0.2f, 1f); // Red
                SetIndicatorActive(true);
                break;

            case GuardPerceptionState.Searching:
                indicatorTextMesh.text = "SEARCH";
                indicatorTextMesh.color = new Color(1f, 0.55f, 0.1f, 1f); // Amber
                SetIndicatorActive(true);
                break;

            case GuardPerceptionState.Returning:
                indicatorTextMesh.text = "RETURN";
                indicatorTextMesh.color = new Color(0.3f, 0.8f, 1f, 1f); // Cyan
                SetIndicatorActive(true);
                break;

            case GuardPerceptionState.Unaware:
            default:
                SetIndicatorActive(false);
                break;
        }
    }

    private void SetIndicatorActive(bool active)
    {
        if (indicatorObj != null && indicatorObj.activeSelf != active)
        {
            indicatorObj.SetActive(active);
        }
    }

    public void SetSearchingState()
    {
        perceptionState = GuardPerceptionState.Searching;
    }

    public void SetReturningState()
    {
        perceptionState = GuardPerceptionState.Returning;
    }

    public void ResetPerception()
    {
        suspicionLevel = 0f;
        perceptionState = GuardPerceptionState.Unaware;
        canSeePlayer = false;
        lkpConfidence = 0f;
        lkpTimestamp = -999f;
        SetIndicatorActive(false);
    }
}
