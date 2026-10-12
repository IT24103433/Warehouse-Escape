using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Executes in Play Mode to capture authentic GPU-rendered frames with full URP lighting,
/// materials, shadows, grounded characters, articulated locomotion, and environment details.
/// Captures:
/// - All four guard closeups and full bodies (Varun, Sanjeevsaran, Aqqil, Jadavan Boss).
/// - Player first-person perspectives: Forward (pitch 0 deg), Moderate-Down (pitch 40 deg),
///   Maximum-Down (pitch 75 deg showing thighs, knees, and grounded boots), and walking stride evidence.
/// - Five room perspectives (Room 1 Start through Room 5 Boss).
/// Operates as a transient runner that self-destructs without contaminating shipping scene data.
/// </summary>
public class PlayModeCaptureRunner : MonoBehaviour
{
    private struct GuardCapturePoint
    {
        public string filename;
        public Vector3 position;
        public Vector3 rotation;
        public float fov;

        public GuardCapturePoint(string file, Vector3 pos, Vector3 rot, float fov = 55f)
        {
            this.filename = file;
            this.position = pos;
            this.rotation = rot;
            this.fov = fov;
        }
    }

    void Start()
    {
        StartCoroutine(CaptureRoutine());
    }

    private IEnumerator CaptureRoutine()
    {
        Debug.Log("[PlayModeCaptureRunner] Starting authentic Play Mode visual verification capture...");

        // Wait for URP shaders, lighting, NavMesh, and character animation to settle
        yield return new WaitForSeconds(1.0f);

        string outDir = Path.Combine(Application.dataPath, "../scratch/screenshots");
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        RenderTexture rt = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;

        // ========================================================
        // 1. CAPTURE PLAYER FIRST-PERSON VIEWS USING ACTUAL PLAYER CAMERA
        // ========================================================
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Camera playerCam = playerObj.GetComponentInChildren<Camera>();
            if (playerCam != null)
            {
                playerCam.targetTexture = rt;
                var pm = playerObj.GetComponent<PlayerMovement>();
                float initialPitch = pm != null ? pm.CameraPitch : 0f;
                var cc = playerObj.GetComponent<CharacterController>();

                // A. Forward View (Pitch 0 deg)
                if (pm != null) pm.CameraPitch = 0f;
                yield return null;
                yield return new WaitForEndOfFrame();
                playerCam.Render();
                SaveRenderTexture(rt, Path.Combine(outDir, "player_fp_looking_forward_arms.png"));
                Debug.Log("[PlayModeCaptureRunner] Captured player_fp_looking_forward_arms.png (pitch 0 deg)");

                // B. Moderate-Down View (Pitch 40 deg)
                if (pm != null) pm.CameraPitch = 40f;
                yield return null;
                yield return new WaitForEndOfFrame();
                playerCam.Render();
                SaveRenderTexture(rt, Path.Combine(outDir, "player_fp_looking_moderate_down.png"));
                Debug.Log("[PlayModeCaptureRunner] Captured player_fp_looking_moderate_down.png (pitch 40 deg)");

                // C. Maximum-Down View (Pitch 75 deg - legs, knees, and boots clearly framed)
                if (pm != null) pm.CameraPitch = 75f;
                yield return null;
                yield return new WaitForEndOfFrame();
                playerCam.Render();
                SaveRenderTexture(rt, Path.Combine(outDir, "player_fp_looking_down_legs.png"));
                Debug.Log("[PlayModeCaptureRunner] Captured player_fp_looking_down_legs.png (pitch 75 deg)");

                // D. Walk Movement Stride Evidence
                // Simulate forward movement over several frames to animate the leg stride and arm swing
                if (pm != null) pm.CameraPitch = 55f;
                for (int f = 0; f < 18; f++)
                {
                    if (cc != null) cc.Move(playerObj.transform.forward * (5f * Time.deltaTime));
                    yield return null;
                }
                yield return new WaitForEndOfFrame();
                playerCam.Render();
                SaveRenderTexture(rt, Path.Combine(outDir, "player_fp_movement_stride.png"));
                Debug.Log("[PlayModeCaptureRunner] Captured player_fp_movement_stride.png (walking locomotion evidence)");

                // Restore player camera
                if (pm != null) pm.CameraPitch = initialPitch;
                playerCam.targetTexture = null;
            }
        }

        // ========================================================
        // 2. CAPTURE GUARD CLOSEUPS, FULL BODIES, AND ROOM VIEWS
        // ========================================================
        GameObject camObj = new GameObject("PlayMode_CaptureCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 120f;
        cam.cullingMask = ~0;

        var urpData = camObj.AddComponent<UniversalAdditionalCameraData>();
        urpData.renderType = CameraRenderType.Base;
        urpData.renderPostProcessing = true;
        urpData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        urpData.antialiasingQuality = AntialiasingQuality.High;
        cam.targetTexture = rt;

        GuardCapturePoint[] points = new GuardCapturePoint[]
        {
            // Varun (Patrol Guard): at (-5.0, 1.08, -15.0), facing +Z
            new GuardCapturePoint("closeup_guard_varun_patrol.png", new Vector3(-5.0f, 1.62f, -13.7f), new Vector3(4f, 180f, 0f), 50f),
            new GuardCapturePoint("fullbody_guard_varun_patrol.png", new Vector3(-5.0f, 1.15f, -11.8f), new Vector3(6f, 180f, 0f), 60f),

            // Sanjeevsaran (Search Guard): at (-12.0, 1.08, 2.0), facing -Z
            new GuardCapturePoint("closeup_guard_sanjeevsaran_search.png", new Vector3(-12.0f, 1.62f, 0.7f), new Vector3(4f, 0f, 0f), 50f),
            new GuardCapturePoint("fullbody_guard_sanjeevsaran_search.png", new Vector3(-12.0f, 1.15f, -1.2f), new Vector3(6f, 0f, 0f), 60f),

            // Aqqil (Defensive Guard): at (16.0, 1.08, 8.0), facing -Z
            new GuardCapturePoint("closeup_guard_aqqil_defensive.png", new Vector3(16.0f, 1.62f, 6.7f), new Vector3(4f, 0f, 0f), 50f),
            new GuardCapturePoint("fullbody_guard_aqqil_defensive.png", new Vector3(16.0f, 1.15f, 4.8f), new Vector3(6f, 0f, 0f), 60f),

            // Jadavan Boss: at (0.0, 1.08, 23.0), facing +Z, scale 1.15
            new GuardCapturePoint("closeup_guard_jadavan_boss.png", new Vector3(0.0f, 1.82f, 24.5f), new Vector3(4f, 180f, 0f), 50f),
            new GuardCapturePoint("fullbody_guard_jadavan_boss.png", new Vector3(0.0f, 1.30f, 26.6f), new Vector3(6f, 180f, 0f), 60f),

            // Five Room Perspectives
            // Room 1: Safe Start
            new GuardCapturePoint("after_room1_start.png", new Vector3(0f, 1.65f, -26.0f), new Vector3(2f, 0f, 0f), 65f),
            // Room 2: Patrol Encounter with Dual Branch Exits
            new GuardCapturePoint("after_room2_patrol.png", new Vector3(0f, 1.65f, -13.0f), new Vector3(2f, 0f, 0f), 65f),
            // Room 3: Search Sector
            new GuardCapturePoint("after_room3_search.png", new Vector3(-12.0f, 1.65f, -1.0f), new Vector3(2f, -15f, 0f), 65f),
            // Room 4: Defensive Sector
            new GuardCapturePoint("after_room4_defensive.png", new Vector3(10.0f, 1.65f, 1.5f), new Vector3(2f, 35f, 0f), 65f),
            // Room 5: Boss Room & Final Exit
            new GuardCapturePoint("after_room5_boss.png", new Vector3(0.0f, 1.65f, 19.5f), new Vector3(2f, 0f, 0f), 65f)
        };

        foreach (var pt in points)
        {
            cam.transform.position = pt.position;
            cam.transform.rotation = Quaternion.Euler(pt.rotation);
            cam.fieldOfView = pt.fov;

            yield return new WaitForEndOfFrame();
            cam.Render();

            SaveRenderTexture(rt, Path.Combine(outDir, pt.filename));
            Debug.Log($"[PlayModeCaptureRunner] Captured frame: {pt.filename}");
        }

        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        Destroy(camObj);

        string doneFlag = Path.Combine(Application.dataPath, "../scratch/screenshots_done.flag");
        File.WriteAllText(doneFlag, "CAPTURES_COMPLETED_SUCCESSFULLY");
        Debug.Log("[PlayModeCaptureRunner] All authentic Play Mode verification screenshots captured!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Destroy(gameObject);
    }

    private void SaveRenderTexture(RenderTexture rt, string filePath)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        File.WriteAllBytes(filePath, tex.EncodeToPNG());
        Destroy(tex);
    }
}
