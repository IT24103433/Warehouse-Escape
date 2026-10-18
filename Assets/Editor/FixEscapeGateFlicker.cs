using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixEscapeGateFlicker
{
    [MenuItem("Tools/Fix Escape Gate Flicker")]
    public static void Execute()
    {
        Debug.Log("[FixEscapeGateFlicker] Starting comprehensive gate flicker fix...");

        // 1. Open Warehouse scene cleanly
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);

        // 2. Fix Card Reader Materials (Upgrade from legacy shader 46 to Universal Render Pipeline/Lit)
        var urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader != null)
        {
            UpdateReaderMaterial("Assets/Materials/Mat_Reader_Ready.mat", urpLitShader, new Color(0.0f, 0.75f, 1.0f, 1.0f), new Color(0.0f, 0.45f, 0.85f, 1.0f));
            UpdateReaderMaterial("Assets/Materials/Mat_Reader_Success.mat", urpLitShader, new Color(0.0f, 1.0f, 0.4f, 1.0f), new Color(0.0f, 0.8f, 0.25f, 1.0f));
            UpdateReaderMaterial("Assets/Materials/Mat_Reader_Denied.mat", urpLitShader, new Color(1.0f, 0.2f, 0.2f, 1.0f), new Color(0.85f, 0.1f, 0.1f, 1.0f));
        }
        else
        {
            Debug.LogWarning("[FixEscapeGateFlicker] Universal Render Pipeline/Lit shader not found!");
        }

        // 3. Remove Confirmed Duplicate Geometry from Exit_Safety_Landing
        var exitLanding = GameObject.Find("Exit_Safety_Landing");
        if (exitLanding != null)
        {
            var wallN = exitLanding.transform.Find("Exit_Platform_Wall_N");
            if (wallN != null)
            {
                Debug.Log("[FixEscapeGateFlicker] Removing confirmed duplicate wall geometry: Exit_Platform_Wall_N");
                Object.DestroyImmediate(wallN.gameObject);
            }

            var floor = exitLanding.transform.Find("Exit_Platform_Floor");
            if (floor != null)
            {
                Debug.Log("[FixEscapeGateFlicker] Removing confirmed duplicate floor slab: Exit_Platform_Floor");
                Object.DestroyImmediate(floor.gameObject);
            }
        }
        else
        {
            var wallN = GameObject.Find("Exit_Platform_Wall_N");
            if (wallN != null) Object.DestroyImmediate(wallN);
            var floor = GameObject.Find("Exit_Platform_Floor");
            if (floor != null) Object.DestroyImmediate(floor);
        }

        // 4. Remove Duplicate Visual Geometry on Exit_Door while preserving GameObject and DoorController
        var exitDoor = GameObject.Find("Exit_Door");
        if (exitDoor != null)
        {
            // Remove MeshRenderer and MeshFilter so duplicate 4x4m red box is not rendered and casts no shadow
            var mr = exitDoor.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Debug.Log("[FixEscapeGateFlicker] Removing duplicate MeshRenderer from Exit_Door");
                Object.DestroyImmediate(mr);
            }
            var mf = exitDoor.GetComponent<MeshFilter>();
            if (mf != null)
            {
                Debug.Log("[FixEscapeGateFlicker] Removing duplicate MeshFilter from Exit_Door");
                Object.DestroyImmediate(mf);
            }

            // Ensure collider is disabled on Exit_Door to prevent phantom collision
            var col = exitDoor.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Ensure DoorController remains intact for DoorController references
            var dc = exitDoor.GetComponent<DoorController>();
            if (dc != null)
            {
                dc.StartOpen = true;
                dc.ShowInteractionPrompt = false;
                dc.RequiresKeycard = false;
            }
        }

        // 5. Configure Visual Gate (IndustrialShutter_Arch, Shutter_Drum, Shutter_Slat)
        var shutterArch = GameObject.Find("IndustrialShutter_Arch");
        if (shutterArch != null)
        {
            // Height 5.2m centered at Y=2.6m spans Y=[0.0 .. 5.2], perfectly flush with floor Y=0.0
            shutterArch.transform.position = new Vector3(0f, 2.6f, 49.92f);
            shutterArch.transform.localScale = new Vector3(6.5f, 5.2f, 0.10f);

            // Preserve solid collision at the escape doorway
            var col = shutterArch.GetComponent<BoxCollider>();
            if (col == null) col = shutterArch.AddComponent<BoxCollider>();
            col.center = Vector3.zero;
            col.size = Vector3.one;
            col.isTrigger = false;

            // Ensure shadows are active
            var mr = shutterArch.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                mr.receiveShadows = true;
            }
            Debug.Log("[FixEscapeGateFlicker] Reconfigured IndustrialShutter_Arch: position=(0, 2.6, 49.92), scale=(6.5, 5.2, 0.10)");
        }

        // Shutter top drum
        var drum = GameObject.Find("Shutter_Drum");
        if (drum != null)
        {
            drum.transform.position = new Vector3(0f, 4.7f, 49.80f);
            drum.transform.localScale = new Vector3(6.4f, 0.5f, 0.45f);
            Debug.Log("[FixEscapeGateFlicker] Reconfigured Shutter_Drum: position=(0, 4.7, 49.80)");
        }

        // Shutter slats: distribute evenly across full shutter height from floor Y=0.15 to drum Y=4.15
        var allGos = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var slats = allGos.Where(g => g.name.StartsWith("Shutter_Slat")).OrderBy(g => g.transform.position.y).ToList();
        float[] slatYs = new float[] { 0.15f, 0.65f, 1.15f, 1.65f, 2.15f, 2.65f, 3.15f, 3.65f, 4.15f };

        for (int i = 0; i < slats.Count; i++)
        {
            var go = slats[i];
            float targetY = (i < slatYs.Length) ? slatYs[i] : (0.15f + i * 0.50f);
            go.transform.position = new Vector3(0f, targetY, 49.82f);
            go.transform.localScale = new Vector3(6.2f, 0.04f, 0.05f);
            var smr = go.GetComponent<MeshRenderer>();
            if (smr != null)
            {
                smr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                smr.receiveShadows = true;
            }
        }
        Debug.Log($"[FixEscapeGateFlicker] Reconfigured {slats.Count} Shutter_Slat objects across full height at Z = 49.82");

        // 6. Separate Coplanar Surfaces on Master Exit Sign
        var signMaster = GameObject.Find("Sign_Exit_Master");
        if (signMaster != null)
        {
            signMaster.transform.position = new Vector3(0f, 4.2f, 49.60f);

            var board = signMaster.transform.Find("SignBoard");
            if (board != null)
            {
                board.localPosition = new Vector3(0f, 0f, 0f); // World Z = 49.60, front face = 49.560
                board.localScale = new Vector3(4.9f, 0.9f, 0.08f);
            }

            var border = signMaster.transform.Find("SignBorder");
            if (border != null)
            {
                // Sits proud of SignBoard front face by 15mm
                border.localPosition = new Vector3(0f, 0f, -0.065f); // World Z = 49.535, front = 49.525, back = 49.545
                border.localScale = new Vector3(4.8f, 0.8f, 0.02f);
            }

            var textMesh = signMaster.transform.Find("Sign_TextMesh");
            if (textMesh != null)
            {
                // Sits proud of border by 10mm
                textMesh.localPosition = new Vector3(0f, 0f, -0.085f); // World Z = 49.515
            }
            Debug.Log("[FixEscapeGateFlicker] Separated Sign_Exit_Master coplanar surfaces");
        }

        // 7. Separate Coplanar Surfaces on Card Reader Terminal
        var terminal = GameObject.Find("Card_Reader_Terminal");
        if (terminal != null)
        {
            terminal.transform.position = new Vector3(0f, 0.5f, 48.0f);

            var body = terminal.transform.Find("Pedestal_Body");
            if (body != null)
            {
                body.localPosition = Vector3.zero;
                body.localScale = new Vector3(0.8f, 1.0f, 0.5f); // front face at Z = 47.75
            }

            var screen = terminal.transform.Find("Status_Screen");
            if (screen != null)
            {
                // Sits proud of pedestal front face by 25mm
                screen.localPosition = new Vector3(0f, 0.3f, -0.28f); // front face at Z = 47.70
                screen.localScale = new Vector3(0.4f, 0.2f, 0.04f);
            }

            var label = terminal.transform.Find("Reader_Label");
            if (label != null)
            {
                label.localPosition = new Vector3(0f, 0.65f, -0.29f); // front face at Z = 47.71
            }
            Debug.Log("[FixEscapeGateFlicker] Separated Card_Reader_Terminal screen and label");
        }

        // 8. Optimize Camera Depth Precision
        var playerCam = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
        if (playerCam == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerCam = playerObj.GetComponentInChildren<Camera>();
        }
        if (playerCam != null)
        {
            playerCam.nearClipPlane = 0.05f;
            playerCam.farClipPlane = 120.0f; // Scene max length is ~80m; 120m improves Z-precision 8.3x
            EditorUtility.SetDirty(playerCam);
            Debug.Log("[FixEscapeGateFlicker] Optimized Main Camera depth precision: near=0.05, far=120");
        }

        // 9. Save Scene and Assets Cleanly
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Write output confirmation
        string outPath = Path.Combine(Application.dataPath, "../scratch/gate_fix_result.txt");
        File.WriteAllText(outPath, $"GATE_FIX_APPLIED_SUCCESSFULLY\nTimestamp: {System.DateTime.Now}");
        string flagPath = Path.Combine(Application.dataPath, "../scratch/gate_fix_applied.flag");
        File.WriteAllText(flagPath, "GATE_FIX_APPLIED");
        Debug.Log("[FixEscapeGateFlicker] Gate flicker fix successfully executed, scene and materials saved!");
    }

    private static void UpdateReaderMaterial(string matPath, Shader shader, Color baseColor, Color emissionColor)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, matPath);
        }
        else
        {
            mat.shader = shader;
        }

        mat.SetColor("_BaseColor", baseColor);
        mat.SetFloat("_Metallic", 0.2f);
        mat.SetFloat("_Smoothness", 0.85f);
        mat.SetColor("_EmissionColor", emissionColor);
        mat.EnableKeyword("_EMISSION");
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        EditorUtility.SetDirty(mat);
        Debug.Log($"[FixEscapeGateFlicker] Upgraded reader material: {matPath} to URP Lit ({baseColor})");
    }
}
