using System;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

/// <summary>
/// Kinematic procedural humanoid animator.
/// Drives articulated hips, knees, shoulders, elbows, feet, spine, and head
/// based on real horizontal locomotion states with smooth idle breathing and velocity-driven gait.
/// Operates strictly as a visual presentation controller without root motion or gameplay interference.
/// 
/// Key Capabilities:
/// - Robust speed derivation from actual horizontal ground displacement and CharacterController velocity,
///   explicitly filtering vertical gravity / Move() jitter and enforcing an idle deadzone to eliminate foot sliding.
/// - Distance-coupled stride phase progression matching ground displacement to prevent foot slippage.
/// - Camera pitch tracking (-80 to +80 deg) with smooth first-person arms retraction when looking down,
///   guaranteeing a completely clear, natural view of thighs, knees, and grounded combat boots.
/// </summary>
public class HumanoidAnimator : MonoBehaviour
{
    [Header("Articulated Transforms")]
    public Transform pelvis;
    public Transform spine;
    public Transform head;
    public Transform upperArmL;
    public Transform forearmL;
    public Transform upperArmR;
    public Transform forearmR;
    public Transform thighL;
    public Transform shinL;
    public Transform footL;
    public Transform thighR;
    public Transform shinR;
    public Transform footR;

    [Header("Locomotion Configuration")]
    public float walkFrequency = 4.2f;
    public float maxStrideAngle = 26f;
    public float maxArmSwingAngle = 22f;
    public float kneeBendAngle = 44f;
    public float idleBreathingSpeed = 1.6f;
    public float idleBreathingAngle = 1.4f;

    [Header("First-Person Player Mode")]
    public bool isFirstPersonPlayer = false;
    public Transform fpArmsRoot;

    // References to physics/navigation components
    private NavMeshAgent agent;
    private CharacterController controller;
    private Camera playerCam;

    // Rest pose caches
    private Vector3 pelvisBasePos;
    private Quaternion pelvisBaseRot;
    private Quaternion spineBaseRot;
    private Quaternion headBaseRot;
    private Quaternion thighLBaseRot;
    private Quaternion shinLBaseRot;
    private Quaternion footLBaseRot;
    private Quaternion thighRBaseRot;
    private Quaternion shinRBaseRot;
    private Quaternion footRBaseRot;
    private Quaternion upperArmLBaseRot;
    private Quaternion forearmLBaseRot;
    private Quaternion upperArmRBaseRot;
    private Quaternion forearmRBaseRot;
    private Vector3 fpArmsBasePos;
    private Quaternion fpArmsBaseRot;

    // Runtime animation state
    private float gaitPhase = 0f;
    private float smoothSpeed = 0f;
    private float moveWeight = 0f;
    private Vector3 lastWorldPos;
    private float lastPitch = 0f;

    void Awake()
    {
        // Enforce collider-free and nav-excluded visual presentation
        foreach (var col in GetComponentsInChildren<Collider>(true))
        {
            col.enabled = false;
            Destroy(col);
        }

        var navMod = GetComponent<NavMeshModifier>();
        if (navMod == null) navMod = gameObject.AddComponent<NavMeshModifier>();
        navMod.overrideArea = true;
        navMod.ignoreFromBuild = true;

        EnsureMeshIntegrity();
        CacheRestPoses();
        ResolveMovementSources();
    }

    private void EnsureMeshIntegrity()
    {
        if (transform.parent != null)
        {
            string pName = transform.parent.name;
            CharacterRole role = CharacterRole.Player;
            if (pName.Contains("Varun")) role = CharacterRole.Patrol_Varun;
            else if (pName.Contains("Sanjeevsaran")) role = CharacterRole.Search_Sanjeevsaran;
            else if (pName.Contains("Aqqil")) role = CharacterRole.Defensive_Aqqil;
            else if (pName.Contains("Jadavan")) role = CharacterRole.Boss_Jadavan;
            else if (transform.parent.CompareTag("Player") || pName.Contains("Player")) role = CharacterRole.Player;

            HumanoidMeshBuilder.EnsureRuntimeVisuals(transform.parent, role);
        }
    }

    void OnEnable()
    {
        CacheRestPoses();
        ResolveMovementSources();
        lastWorldPos = transform.position;
    }

    public void CacheRestPoses()
    {
        if (pelvis != null)
        {
            pelvisBasePos = pelvis.localPosition;
            pelvisBaseRot = pelvis.localRotation;
        }
        if (spine != null) spineBaseRot = spine.localRotation;
        if (head != null) headBaseRot = head.localRotation;
        if (thighL != null) thighLBaseRot = thighL.localRotation;
        if (shinL != null) shinLBaseRot = shinL.localRotation;
        if (footL != null) footLBaseRot = footL.localRotation;
        if (thighR != null) thighRBaseRot = thighR.localRotation;
        if (shinR != null) shinRBaseRot = shinR.localRotation;
        if (footR != null) footRBaseRot = footR.localRotation;
        if (upperArmL != null) upperArmLBaseRot = upperArmL.localRotation;
        if (forearmL != null) forearmLBaseRot = forearmL.localRotation;
        if (upperArmR != null) upperArmRBaseRot = upperArmR.localRotation;
        if (forearmR != null) forearmRBaseRot = forearmR.localRotation;

        if (fpArmsRoot != null)
        {
            fpArmsBasePos = fpArmsRoot.localPosition;
            fpArmsBaseRot = fpArmsRoot.localRotation;
        }
    }

    private void ResolveMovementSources()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        controller = GetComponentInParent<CharacterController>();

        if (isFirstPersonPlayer)
        {
            if (controller != null)
                playerCam = controller.GetComponentInChildren<Camera>();
            if (playerCam == null)
                playerCam = Camera.main;
        }
    }

    void Update()
    {
        if (Time.timeScale <= 0f) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
        {
            ApplyIdleBreathingOnly();
            return;
        }

        float dt = Time.deltaTime;
        if (dt <= 0.0001f) return;

        // 1. Calculate actual horizontal ground displacement
        Vector3 currentWorldPos = transform.position;
        Vector3 horizDelta = currentWorldPos - lastWorldPos;
        horizDelta.y = 0f;
        float displacementSpeed = horizDelta.magnitude / dt;
        lastWorldPos = currentWorldPos;

        // 2. Query physics/controller velocity, explicitly filtering vertical velocity
        float reportedSpeed = 0f;
        if (controller != null && controller.enabled)
        {
            Vector3 cv = controller.velocity;
            cv.y = 0f;
            reportedSpeed = cv.magnitude;
        }
        else if (agent != null && agent.enabled)
        {
            reportedSpeed = agent.velocity.magnitude;
        }

        // 3. Robust speed combination with an idle deadzone to prevent foot sliding / gravity noise
        float rawSpeed = Mathf.Max(displacementSpeed, reportedSpeed);
        if (rawSpeed < 0.08f) rawSpeed = 0f;

        // Smooth speed and blend weight
        smoothSpeed = Mathf.MoveTowards(smoothSpeed, rawSpeed, dt * 6.0f);
        float targetWeight = Mathf.Clamp01(smoothSpeed / 3.0f);
        moveWeight = Mathf.MoveTowards(moveWeight, targetWeight, dt * 5.0f);

        // 4. Stride phase progression tied directly to distance traveled to avoid foot sliding
        if (moveWeight > 0.01f)
        {
            float strideCycleDist = 1.4f;
            float movedDist = horizDelta.magnitude;
            if (movedDist > 0.0001f)
            {
                gaitPhase += (movedDist / strideCycleDist) * (Mathf.PI * 2f);
            }
            else
            {
                gaitPhase += (smoothSpeed / strideCycleDist) * (Mathf.PI * 2f) * dt;
            }
            if (gaitPhase > Mathf.PI * 2f) gaitPhase %= (Mathf.PI * 2f);
        }

        // 5. Apply articulated procedural pose
        ApplyArticulatedPose(dt);
    }

    private void ApplyIdleBreathingOnly()
    {
        float breath = Mathf.Sin(Time.time * idleBreathingSpeed);
        if (spine != null)
            spine.localRotation = spineBaseRot * Quaternion.Euler(breath * idleBreathingAngle, 0f, 0f);
        if (pelvis != null)
            pelvis.localPosition = pelvisBasePos + Vector3.up * (breath * 0.005f);
    }

    private void ApplyArticulatedPose(float dt)
    {
        float breath = Mathf.Sin(Time.time * idleBreathingSpeed);

        // 1. Pelvis & Hips (natural subtle sway and vertical dip during stride)
        if (pelvis != null)
        {
            float verticalDip = -Mathf.Abs(Mathf.Sin(gaitPhase)) * 0.022f * moveWeight;
            float hipSwayY = Mathf.Sin(gaitPhase) * 2.2f * moveWeight;
            float hipRollZ = Mathf.Sin(gaitPhase) * 1.2f * moveWeight;

            pelvis.localPosition = pelvisBasePos + Vector3.up * (verticalDip + breath * 0.005f * (1f - moveWeight));
            pelvis.localRotation = pelvisBaseRot * Quaternion.Euler(0f, hipSwayY, hipRollZ);
        }

        // 2. Spine & Chest (counter-rotation to hips)
        if (spine != null)
        {
            float spineCounterY = -Mathf.Sin(gaitPhase) * 3.0f * moveWeight;
            float spinePitch = (breath * idleBreathingAngle * (1f - moveWeight * 0.5f)) + (moveWeight * 3.0f);
            spine.localRotation = spineBaseRot * Quaternion.Euler(spinePitch, spineCounterY, 0f);
        }

        // 3. Head (stays oriented forward with subtle scanning when idle)
        if (head != null)
        {
            float headScan = (1f - moveWeight) * Mathf.Sin(Time.time * 0.4f) * 3.0f;
            head.localRotation = headBaseRot * Quaternion.Euler(-moveWeight * 2.0f, headScan, 0f);
        }

        // 4. Legs (Grounded foot plant, knee flex on swing phase)
        float sinPhase = Mathf.Sin(gaitPhase);

        // Left Leg
        float strideL = sinPhase * maxStrideAngle * moveWeight;
        float kneeFlexL = Mathf.Clamp(-sinPhase, 0f, 1f) * kneeBendAngle * moveWeight;
        float ankleL = -strideL * 0.42f;

        if (thighL != null)
            thighL.localRotation = thighLBaseRot * Quaternion.Euler(strideL, 0f, 0f);
        if (shinL != null)
            shinL.localRotation = shinLBaseRot * Quaternion.Euler(kneeFlexL, 0f, 0f);
        if (footL != null)
            footL.localRotation = footLBaseRot * Quaternion.Euler(ankleL, 0f, 0f);

        // Right Leg (opposite phase)
        float strideR = -sinPhase * maxStrideAngle * moveWeight;
        float kneeFlexR = Mathf.Clamp(sinPhase, 0f, 1f) * kneeBendAngle * moveWeight;
        float ankleR = -strideR * 0.42f;

        if (thighR != null)
            thighR.localRotation = thighRBaseRot * Quaternion.Euler(strideR, 0f, 0f);
        if (shinR != null)
            shinR.localRotation = shinRBaseRot * Quaternion.Euler(kneeFlexR, 0f, 0f);
        if (footR != null)
            footR.localRotation = footRBaseRot * Quaternion.Euler(ankleR, 0f, 0f);

        // 5. Arms (Oppositional swing for guards)
        float armSwingL = -sinPhase * maxArmSwingAngle * moveWeight;
        float elbowFlexL = (14f + Mathf.Abs(sinPhase) * 18f * moveWeight);

        if (upperArmL != null)
            upperArmL.localRotation = upperArmLBaseRot * Quaternion.Euler(armSwingL, 0f, -armSwingL * 0.15f);
        if (forearmL != null)
            forearmL.localRotation = forearmLBaseRot * Quaternion.Euler(elbowFlexL, 0f, 0f);

        float armSwingR = sinPhase * maxArmSwingAngle * moveWeight;
        float elbowFlexR = (14f + Mathf.Abs(sinPhase) * 18f * moveWeight);

        if (upperArmR != null)
            upperArmR.localRotation = upperArmRBaseRot * Quaternion.Euler(armSwingR, 0f, armSwingR * 0.15f);
        if (forearmR != null)
            forearmR.localRotation = forearmRBaseRot * Quaternion.Euler(elbowFlexR, 0f, 0f);

        // 6. First-Person Camera Pitch Adaptation & Arms Presentation
        if (isFirstPersonPlayer && fpArmsRoot != null)
        {
            float targetPitch = 0f;
            if (playerCam != null)
            {
                targetPitch = playerCam.transform.localEulerAngles.x;
                if (targetPitch > 180f) targetPitch -= 360f;
            }
            lastPitch = Mathf.MoveTowards(lastPitch, targetPitch, dt * 200f);

            // Pitch tucking: when looking down (>10 deg), smoothly retract arms down & back
            float downTuck = Mathf.Clamp01((lastPitch - 10f) / 55f);
            float upTuck = Mathf.Clamp01((-lastPitch - 10f) / 50f);

            Vector3 pitchPosOffset = new Vector3(0f, -0.36f * downTuck - 0.12f * upTuck, -0.26f * downTuck);
            Quaternion pitchRotOffset = Quaternion.Euler(-32f * downTuck, 0f, 0f);

            float armBobY = Mathf.Sin(gaitPhase * 2f) * 0.010f * moveWeight + (breath * 0.003f * (1f - downTuck));
            float armSwayX = Mathf.Sin(gaitPhase) * 0.012f * moveWeight * (1f - downTuck);
            float armRollZ = Mathf.Sin(gaitPhase) * 1.2f * moveWeight * (1f - downTuck);

            fpArmsRoot.localPosition = fpArmsBasePos + pitchPosOffset + new Vector3(armSwayX, armBobY, 0f);
            fpArmsRoot.localRotation = fpArmsBaseRot * pitchRotOffset * Quaternion.Euler(armBobY * 70f, armSwayX * 50f, armRollZ);
        }
    }
}
