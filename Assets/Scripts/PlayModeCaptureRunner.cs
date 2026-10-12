using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Executes in Play Mode to capture authentic GPU-rendered frames with full URP lighting,
/// materials, shadows, grounded characters, articulated animation, and environment details.
/// </summary>
public class PlayModeCaptureRunner : MonoBehaviour
{
    private struct CapturePoint
    {
        public string filename;
        public Vector3 position;
        public Vector3 rotation;
        public float fov;

        public CapturePoint(string file, Vector3 pos, Vector3 rot, float fov = 65f)
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

        // Wait 1.0 second (multiple frames) for full URP shaders, lighting, NavMesh, and character animation to settle
        yield return new WaitForSeconds(1.0f);

        string outDir = Path.Combine(Application.dataPath, "../scratch/screenshots");
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        // Create dedicated high-quality capture camera configured for URP
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

        RenderTexture rt = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;
        cam.targetTexture = rt;

        CapturePoint[] points = new CapturePoint[]
        {
            // 1. Guard Closeups & Full Bodies
            // Varun: at (-5.0, 1.08, -15.0), facing +Z
            new CapturePoint("closeup_guard_varun_patrol.png", new Vector3(-5.0f, 1.62f, -13.7f), new Vector3(4f, 180f, 0f), 55f),
            new CapturePoint("fullbody_guard_varun_patrol.png", new Vector3(-5.0f, 1.15f, -11.8f), new Vector3(6f, 180f, 0f), 60f),

            // Sanjeevsaran: at (-12.0, 1.08, 2.0), facing -Z
            new CapturePoint("closeup_guard_sanjeevsaran_search.png", new Vector3(-12.0f, 1.62f, 0.7f), new Vector3(4f, 0f, 0f), 55f),
            new CapturePoint("fullbody_guard_sanjeevsaran_search.png", new Vector3(-12.0f, 1.15f, -1.2f), new Vector3(6f, 0f, 0f), 60f),

            // Aqqil: at (16.0, 1.08, 8.0), facing -Z
            new CapturePoint("closeup_guard_aqqil_defensive.png", new Vector3(16.0f, 1.62f, 6.7f), new Vector3(4f, 0f, 0f), 55f),
            new CapturePoint("fullbody_guard_aqqil_defensive.png", new Vector3(16.0f, 1.15f, 4.8f), new Vector3(6f, 0f, 0f), 60f),

            // Jadavan Boss: at (0.0, 1.08, 23.0), facing +Z, scale 1.15
            new CapturePoint("closeup_guard_jadavan_boss.png", new Vector3(0.0f, 1.78f, 24.6f), new Vector3(4f, 180f, 0f), 55f),
            new CapturePoint("fullbody_guard_jadavan_boss.png", new Vector3(0.0f, 1.30f, 26.6f), new Vector3(6f, 180f, 0f), 60f),

            // 2. Player Views
            // Looking down at legs and boots
            new CapturePoint("player_fp_looking_down_legs.png", new Vector3(0f, 1.78f, -26.0f), new Vector3(62f, 0f, 0f), 70f),
            // Looking forward at forearms and hands
            new CapturePoint("player_fp_looking_forward_arms.png", new Vector3(0f, 1.78f, -26.0f), new Vector3(0f, 0f, 0f), 65f),

            // 3. Five Room Perspectives
            // Room 1: Safe Start
            new CapturePoint("after_room1_start.png", new Vector3(0f, 1.65f, -26.0f), new Vector3(2f, 0f, 0f), 65f),
            // Room 2: Patrol Encounter with Dual Branch Exits
            new CapturePoint("after_room2_patrol.png", new Vector3(0f, 1.65f, -13.0f), new Vector3(2f, 0f, 0f), 65f),
            // Room 3: Search Sector
            new CapturePoint("after_room3_search.png", new Vector3(-12.0f, 1.65f, -1.0f), new Vector3(2f, -15f, 0f), 65f),
            // Room 4: Defensive Sector (Exactly ONE active guard)
            new CapturePoint("after_room4_defensive.png", new Vector3(10.0f, 1.65f, 1.5f), new Vector3(2f, 35f, 0f), 65f),
            // Room 5: Boss Room & Final Exit
            new CapturePoint("after_room5_boss.png", new Vector3(0.0f, 1.65f, 19.5f), new Vector3(2f, 0f, 0f), 65f)
        };

        foreach (var pt in points)
        {
            cam.transform.position = pt.position;
            cam.transform.rotation = Quaternion.Euler(pt.rotation);
            cam.fieldOfView = pt.fov;

            // Wait for end of frame to ensure GPU render pipeline completes
            yield return new WaitForEndOfFrame();

            cam.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            string savePath = Path.Combine(outDir, pt.filename);
            File.WriteAllBytes(savePath, tex.EncodeToPNG());
            Destroy(tex);

            Debug.Log($"[PlayModeCaptureRunner] Captured authentic Play Mode frame: {pt.filename}");
        }

        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        Destroy(camObj);

        string doneFlag = Path.Combine(Application.dataPath, "../scratch/screenshots_done.flag");
        File.WriteAllText(doneFlag, "CAPTURES_COMPLETED_SUCCESSFULLY");
        Debug.Log("[PlayModeCaptureRunner] All 15 authentic Play Mode verification screenshots captured!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
