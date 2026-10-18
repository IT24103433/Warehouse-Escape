using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class HumanCharacterUpgradePass
{
    [MenuItem("Warehouse/Execute Human Character Upgrade")]
    public static void Execute()
    {
        Debug.Log("[HumanCharacterUpgradePass] Starting human character visual upgrade pass...");

        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[HumanCharacterUpgradePass] Failed to open scene {scenePath}");
            EditorApplication.Exit(1);
            return;
        }

        // 1. Build and attach humanoid models to all 4 guards
        UpgradeGuard("Guard_Varun_Patrol", CharacterRole.Patrol_Varun);
        UpgradeGuard("Guard_Sanjeevsaran_Search", CharacterRole.Search_Sanjeevsaran);
        UpgradeGuard("Guard_Aqqil_Defensive", CharacterRole.Defensive_Aqqil);
        UpgradeGuard("Guard_Jadavan_Boss", CharacterRole.Boss_Jadavan);

        // 2. Upgrade Player First-Person Body & Arms
        UpgradePlayer();

        // 3. Perfect Sign Fitting
        FinishSignFitting();

        // 4. Save Scene Normally
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[HumanCharacterUpgradePass] Warehouse scene cleanly saved with humanoid character models!");

        // 5. Capture Comprehensive Visual Verification Screenshots
        CaptureVerificationScreenshots();

        Debug.Log("[HumanCharacterUpgradePass] Upgrade pass completed successfully.");
        EditorApplication.Exit(0);
    }

    private static void UpgradeGuard(string guardName, CharacterRole role)
    {
        GameObject guardObj = GameObject.Find(guardName);
        if (guardObj == null)
        {
            Debug.LogError($"[HumanCharacterUpgradePass] Guard {guardName} not found in scene!");
            return;
        }

        Debug.Log($"[HumanCharacterUpgradePass] Upgrading {guardName} ({role})...");
        HumanoidMeshBuilder.BuildHumanoidVisual(guardObj.transform, role);
        Debug.Log($"[HumanCharacterUpgradePass] Successfully upgraded {guardName}.");
    }

    private static void UpgradePlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("[HumanCharacterUpgradePass] Player not found by tag!");
            return;
        }

        Debug.Log("[HumanCharacterUpgradePass] Upgrading Player with humanoid first-person body & arms...");
        HumanoidMeshBuilder.BuildHumanoidVisual(playerObj.transform, CharacterRole.Player);
        Debug.Log("[HumanCharacterUpgradePass] Successfully upgraded Player.");
    }

    private static void FinishSignFitting()
    {
        Debug.Log("[HumanCharacterUpgradePass] Fine-tuning all 6 signs for perfect board fitting...");

        string matPath = "Assets/Materials/Mat_Sign_Text_DepthTested.mat";
        Material depthTextMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        Material matSteel = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Steel_Dark.mat");
        Material matHazard = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Exit_Hazard.mat");

        // 1. Sign_ToStage1
        TuneSign("Sign_ToStage1",
            new Vector3(0.00f, 3.45f, -22.30f),
            new Vector2(4.50f, 0.85f),
            "ROOM 1: SAFE START\n-> TO ROOM 2: PATROL",
            0.062f, depthTextMat, matSteel, matHazard);

        // 2. Sign_Route_A
        TuneSign("Sign_Route_A",
            new Vector3(-4.70f, 3.60f, -4.30f),
            new Vector2(4.30f, 0.85f),
            "ROUTE A: SEARCH SECTOR\n[ ACCESS CARD A ]",
            0.060f, depthTextMat, matSteel, matHazard);

        // 3. Sign_Route_B
        TuneSign("Sign_Route_B",
            new Vector3(4.70f, 3.60f, -4.30f),
            new Vector2(4.30f, 0.85f),
            "ROUTE B: DEFENSE SECTOR\n[ ACCESS CARD B ]",
            0.060f, depthTextMat, matSteel, matHazard);

        // 4. Sign_Room4_W
        TuneSign("Sign_Room4_W",
            new Vector3(-4.00f, 3.60f, 19.30f),
            new Vector2(4.60f, 0.85f),
            "FINAL ENCOUNTER\nROOM 5: BOSS LOADING HALL",
            0.060f, depthTextMat, matSteel, matHazard);

        // 5. Sign_Room4_E
        TuneSign("Sign_Room4_E",
            new Vector3(4.00f, 3.60f, 19.30f),
            new Vector2(4.60f, 0.85f),
            "FINAL ENCOUNTER\nROOM 5: BOSS LOADING HALL",
            0.060f, depthTextMat, matSteel, matHazard);

        // 6. Sign_Exit_Master
        TuneSign("Sign_Exit_Master",
            new Vector3(0.00f, 4.05f, 42.70f),
            new Vector2(4.90f, 0.90f),
            "SECURITY CARD READER\nINSERT ACCESS CARD [E]",
            0.062f, depthTextMat, matSteel, matHazard);
    }

    private static void TuneSign(string signName, Vector3 worldPos, Vector2 boardSize, string text, float textScale, Material textMat, Material boardMat, Material hazardMat)
    {
        GameObject signObj = GameObject.Find(signName);
        if (signObj == null) return;

        signObj.transform.position = worldPos;
        Vector3 parentLossy = signObj.transform.parent != null ? signObj.transform.parent.lossyScale : Vector3.one;
        signObj.transform.localScale = new Vector3(
            Mathf.Approximately(parentLossy.x, 0) ? 1f : 1f / parentLossy.x,
            Mathf.Approximately(parentLossy.y, 0) ? 1f : 1f / parentLossy.y,
            Mathf.Approximately(parentLossy.z, 0) ? 1f : 1f / parentLossy.z
        );

        // Adjust board
        Transform boardT = signObj.transform.Find("SignBoard");
        if (boardT != null)
        {
            boardT.localScale = new Vector3(boardSize.x, boardSize.y, 0.08f);
            boardT.GetComponent<Renderer>().sharedMaterial = boardMat;
        }

        // Adjust border
        Transform borderT = signObj.transform.Find("SignBorder");
        if (borderT != null)
        {
            borderT.localScale = new Vector3(boardSize.x - 0.10f, boardSize.y - 0.10f, 0.02f);
            borderT.GetComponent<Renderer>().sharedMaterial = hazardMat;
        }

        // Adjust text mesh
        Transform tmT = signObj.transform.Find("Sign_TextMesh");
        if (tmT != null)
        {
            tmT.localScale = new Vector3(textScale, textScale, textScale);
            var tm = tmT.GetComponent<TextMesh>();
            if (tm != null)
            {
                tm.text = text;
                tm.fontSize = 28;
                tm.characterSize = 1.0f;
                tm.alignment = TextAlignment.Center;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.color = new Color(1.0f, 0.95f, 0.25f, 1.0f);
            }
            var mr = tmT.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = textMat;
        }
    }

    [MenuItem("Warehouse/Capture Verification Screenshots")]
    public static void CaptureVerificationScreenshots()
    {
        string outDir = Path.Combine(Application.dataPath, "../scratch/screenshots");
        if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

        GameObject camObj = new GameObject("CaptureCam");
        Camera cam = camObj.AddComponent<Camera>();
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 100f;
        cam.fieldOfView = 65f;

        RenderTexture rt = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        // 1. Guard Closeups & Full Bodies
        // Varun at (-5.0, 1.08, -15.0), facing +Z
        CaptureFrame(cam, rt, new Vector3(-5.0f, 1.62f, -13.6f), new Vector3(5f, 180f, 0f), Path.Combine(outDir, "closeup_guard_varun_patrol.png"));
        CaptureFrame(cam, rt, new Vector3(-5.0f, 1.15f, -12.0f), new Vector3(8f, 180f, 0f), Path.Combine(outDir, "fullbody_guard_varun_patrol.png"));

        // Sanjeevsaran at (-12.0, 1.08, 2.0), facing -Z (rot 180)
        CaptureFrame(cam, rt, new Vector3(-12.0f, 1.62f, 0.6f), new Vector3(5f, 0f, 0f), Path.Combine(outDir, "closeup_guard_sanjeevsaran_search.png"));
        CaptureFrame(cam, rt, new Vector3(-12.0f, 1.15f, -1.0f), new Vector3(8f, 0f, 0f), Path.Combine(outDir, "fullbody_guard_sanjeevsaran_search.png"));

        // Aqqil at (16.0, 1.08, 8.0), facing -Z (rot 180)
        CaptureFrame(cam, rt, new Vector3(16.0f, 1.62f, 6.6f), new Vector3(5f, 0f, 0f), Path.Combine(outDir, "closeup_guard_aqqil_defensive.png"));
        CaptureFrame(cam, rt, new Vector3(16.0f, 1.15f, 5.0f), new Vector3(8f, 0f, 0f), Path.Combine(outDir, "fullbody_guard_aqqil_defensive.png"));

        // Jadavan Boss at (0.0, 1.08, 23.0), facing +Z
        CaptureFrame(cam, rt, new Vector3(0.0f, 1.80f, 24.6f), new Vector3(5f, 180f, 0f), Path.Combine(outDir, "closeup_guard_jadavan_boss.png"));
        CaptureFrame(cam, rt, new Vector3(0.0f, 1.25f, 26.2f), new Vector3(8f, 180f, 0f), Path.Combine(outDir, "fullbody_guard_jadavan_boss.png"));

        // 2. Player Views
        // Player looking down at legs/boots
        CaptureFrame(cam, rt, new Vector3(0f, 1.78f, -26.0f), new Vector3(65f, 0f, 0f), Path.Combine(outDir, "player_fp_looking_down_legs.png"));
        // Player looking forward at forearms/hands
        CaptureFrame(cam, rt, new Vector3(0f, 1.78f, -26.0f), new Vector3(0f, 0f, 0f), Path.Combine(outDir, "player_fp_looking_forward_arms.png"));

        // 3. Five Player-View Room Perspectives
        CaptureFrame(cam, rt, new Vector3(0f, 1.6f, -26f), new Vector3(3f, 0f, 0f), Path.Combine(outDir, "after_room1_start.png"));
        CaptureFrame(cam, rt, new Vector3(0f, 1.6f, -10f), new Vector3(2f, 0f, 0f), Path.Combine(outDir, "after_room2_patrol.png"));
        CaptureFrame(cam, rt, new Vector3(-12f, 1.6f, 2f), new Vector3(5f, -40f, 0f), Path.Combine(outDir, "after_room3_search.png"));
        CaptureFrame(cam, rt, new Vector3(12f, 1.6f, 5f), new Vector3(5f, 40f, 0f), Path.Combine(outDir, "after_room4_defensive.png"));
        CaptureFrame(cam, rt, new Vector3(0f, 1.6f, 26f), new Vector3(2f, 0f, 0f), Path.Combine(outDir, "after_room5_boss.png"));

        cam.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camObj);
    }

    private static void CaptureFrame(Camera cam, RenderTexture rt, Vector3 pos, Vector3 rot, string outPath)
    {
        cam.transform.position = pos;
        cam.transform.rotation = Quaternion.Euler(rot);
        cam.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(outPath, bytes);
        Object.DestroyImmediate(tex);
        Debug.Log($"[HumanCharacterUpgradePass] Captured frame: {outPath}");
    }
}
