using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DiagnoseGuardPerception
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== TARGETED DIAGNOSTIC: GUARD PERCEPTION & DETECTION ===");

        // 1. Guard Varun & Vision Settings
        var varunGo = GameObject.Find("Guard_Varun_Patrol");
        var playerGo = GameObject.FindGameObjectWithTag("Player");

        if (varunGo == null)
        {
            sb.AppendLine("[ERROR] Guard_Varun_Patrol not found in scene!");
            File.WriteAllText("scratch/GuardPerceptionDiagnosticReport.txt", sb.ToString());
            return;
        }

        if (playerGo == null)
        {
            sb.AppendLine("[ERROR] Player not found in scene!");
            File.WriteAllText("scratch/GuardPerceptionDiagnosticReport.txt", sb.ToString());
            return;
        }

        var patrolAI = varunGo.GetComponent<PatrolGuardAI>();
        var perceptionMem = varunGo.GetComponent<GuardPerceptionMemory>();
        var shooting = varunGo.GetComponent<GuardShooting>();
        var anim = varunGo.GetComponentInChildren<HumanoidAnimator>();

        sb.AppendLine("\n--- 1. VISION SETTINGS DISCREPANCY ---");
        if (patrolAI != null)
        {
            var pSo = new SerializedObject(patrolAI);
            float pSight = pSo.FindProperty("sightDistance")?.floatValue ?? -1;
            float pFov = pSo.FindProperty("fieldOfView")?.floatValue ?? -1;
            float pEyeH = pSo.FindProperty("eyeHeight")?.floatValue ?? -1;
            float pTargetH = pSo.FindProperty("playerTargetHeight")?.floatValue ?? -1;
            sb.AppendLine($"PatrolGuardAI serialized settings: sightDistance={pSight}m, fieldOfView={pFov}deg, eyeHeight={pEyeH}m, playerTargetHeight={pTargetH}m");
        }
        if (perceptionMem != null)
        {
            var mSo = new SerializedObject(perceptionMem);
            float mSight = mSo.FindProperty("sightDistance")?.floatValue ?? -1;
            float mFov = mSo.FindProperty("fieldOfView")?.floatValue ?? -1;
            float mEyeH = mSo.FindProperty("eyeHeight")?.floatValue ?? -1;
            float mTargetH = mSo.FindProperty("playerTargetHeight")?.floatValue ?? -1;
            sb.AppendLine($"GuardPerceptionMemory serialized settings: sightDistance={mSight}m, fieldOfView={mFov}deg, eyeHeight={mEyeH}m, playerTargetHeight={mTargetH}m");
        }

        // 2. Head vs Body Transform & Forward Alignment
        sb.AppendLine("\n--- 2. EYE/HEAD FORWARD DIRECTION VERSUS BODY FORWARD ---");
        Transform headTransform = null;
        if (anim != null && anim.head != null)
        {
            headTransform = anim.head;
        }
        else
        {
            var headFound = varunGo.transform.Find("Character_Visuals/Pelvis/Spine/Neck/Head");
            if (headFound != null) headTransform = headFound;
        }

        sb.AppendLine($"Guard Root Position: {varunGo.transform.position:F3}, Body Forward: {varunGo.transform.forward:F3}");
        if (headTransform != null)
        {
            sb.AppendLine($"Guard Head Transform found: '{headTransform.name}' at World Pos: {headTransform.position:F3}");
            sb.AppendLine($"Guard Head Forward: {headTransform.forward:F3}, Local Rotation: {headTransform.localEulerAngles:F1}");
            float bodyHeadAngle = Vector3.Angle(varunGo.transform.forward, headTransform.forward);
            sb.AppendLine($"Angle between Body Forward and Head Forward (Resting): {bodyHeadAngle:F2} deg");
        }
        else
        {
            sb.AppendLine("Guard Head Transform: NOT FOUND under hierarchy (will be created by HumanoidMeshBuilder at runtime)");
        }

        // 3. Player Location vs Room 2 Bounds
        sb.AppendLine("\n--- 3. ROOM 2 BOUNDS VS PHYSICAL GEOMETRY & DOORWAYS ---");
        var r2 = GameObject.Find("Room_2_Patrol");
        if (r2 != null)
        {
            var floor = r2.transform.Find("Floor");
            if (floor != null)
            {
                Vector3 fPos = floor.position;
                Vector3 fScale = floor.lossyScale;
                sb.AppendLine($"Room 2 Floor Position: {fPos:F2}, Size: {fScale:F2}");
                sb.AppendLine($"Room 2 Physical Extents: X=[{fPos.x - fScale.x/2f:F2}, {fPos.x + fScale.x/2f:F2}], Z=[{fPos.z - fScale.z/2f:F2}, {fPos.z + fScale.z/2f:F2}]");
            }
        }

        if (patrolAI != null)
        {
            sb.AppendLine($"PatrolGuardAI.IsInsideRoom(-19.5f Z [Doorway]): {patrolAI.IsInsideRoom(new Vector3(0f, 1.08f, -19.5f))}");
            sb.AppendLine($"PatrolGuardAI.IsInsideRoom(-19.0f Z [Room 2 Entrance]): {patrolAI.IsInsideRoom(new Vector3(0f, 1.08f, -19.0f))}");
            sb.AppendLine($"PatrolGuardAI.IsInsideRoom(-18.5f Z [Inside Room 2]): {patrolAI.IsInsideRoom(new Vector3(0f, 1.08f, -18.5f))}");
            sb.AppendLine($"PatrolGuardAI.IsInsideRoom(X=-10.0f, Z=-11.0f [West Aisle]): {patrolAI.IsInsideRoom(new Vector3(-10.0f, 1.08f, -11.0f))}");
            sb.AppendLine($"PatrolGuardAI.IsInsideRoom(X=+10.0f, Z=-11.0f [East Aisle]): {patrolAI.IsInsideRoom(new Vector3(10.0f, 1.08f, -11.0f))}");
        }

        // 4. Player Target Heights & Colliders Blocking Rays
        sb.AppendLine("\n--- 4. VISIBLE PLAYER TARGET POINTS & RAYCAST OCCLUSION ---");
        Vector3 playerPos = playerGo.transform.position;
        var pCc = playerGo.GetComponent<CharacterController>();
        var pCam = playerGo.GetComponentInChildren<Camera>();

        sb.AppendLine($"Player Root Pos: {playerPos:F3}");
        if (pCc != null)
        {
            sb.AppendLine($"Player CharacterController: Center={pCc.center:F3}, Height={pCc.height:F2}, Radius={pCc.radius:F2}");
            float bottomY = playerPos.y + pCc.center.y - pCc.height / 2f;
            float topY = playerPos.y + pCc.center.y + pCc.height / 2f;
            sb.AppendLine($"Player CC Vertical Bounds: Y=[{bottomY:F2}, {topY:F2}]");
        }
        if (pCam != null)
        {
            sb.AppendLine($"Player Camera World Pos: {pCam.transform.position:F3} (Eye Level Y = {pCam.transform.position.y:F2})");
        }

        Vector3 hardcodedHead = playerPos + Vector3.up * 0.55f;
        Vector3 hardcodedChest = playerPos + Vector3.up * 0.20f;
        Vector3 actualEye = pCam != null ? pCam.transform.position : playerPos + Vector3.up * 0.70f;
        Vector3 actualChest = playerPos + Vector3.up * (pCc != null ? pCc.center.y : 0.0f);

        sb.AppendLine($"Current Hardcoded Head Target: {hardcodedHead:F3} (Y = {hardcodedHead.y:F2})");
        sb.AppendLine($"Current Hardcoded Chest Target: {hardcodedChest:F3} (Y = {hardcodedChest.y:F2})");
        sb.AppendLine($"Actual Eye Target: {actualEye:F3} (Y = {actualEye.y:F2})");
        sb.AppendLine($"Actual Chest Target: {actualChest:F3} (Y = {actualChest.y:F2})");

        // Raycast test across low cover
        Vector3 guardEyePos = varunGo.transform.position + Vector3.up * 0.6f;
        sb.AppendLine($"Guard Eye Position: {guardEyePos:F3}");

        // Test low cover raycast: Place a mock barrier at 1.1m height between guard and player
        Vector3 mockGuardPos = new Vector3(0f, 1.08f, -10f);
        Vector3 mockPlayerPos = new Vector3(0f, 1.08f, -15f);
        Vector3 testEye = mockGuardPos + Vector3.up * 0.6f; // Y = 1.68

        // Test with actual scene barriers
        var barrier = GameObject.Find("Cover_Barrier_West");
        if (barrier != null)
        {
            var bCol = barrier.GetComponent<Collider>();
            if (bCol != null)
            {
                sb.AppendLine($"Sample Barrier '{barrier.name}' Bounds: Min={bCol.bounds.min:F2}, Max={bCol.bounds.max:F2}, Height={bCol.bounds.size.y:F2}");
            }
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/GuardPerceptionDiagnosticReport.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"[DiagnoseGuardPerception] Diagnostic report written to {outPath}");
    }
}
