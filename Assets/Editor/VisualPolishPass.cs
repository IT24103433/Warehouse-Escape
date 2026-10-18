using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class VisualPolishPass
{
    [MenuItem("Warehouse/Execute Visual Polish Pass")]
    public static void Execute()
    {
        Debug.Log("[VisualPolishPass] Starting visual polish pass...");

        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[VisualPolishPass] Failed to open scene {scenePath}");
            return;
        }

        // 1. Configure Texture Importers for Normal Maps
        ConfigureTextures();

        // 2. Apply Procedural Textures & UV Tiling to Materials
        ApplyMaterials();

        // 3. Create Depth-Tested Text Material
        Material depthTextMat = CreateDepthTestedTextMaterial();

        // 4. Overhaul All Signs (Remove non-uniform scale, mount industrial boards, depth-tested text)
        OverhaulSigns(depthTextMat);

        // 5. Save Scene normally
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[VisualPolishPass] Scene saved cleanly with visual polish updates!");

        // 6. Capture Viewpoints
        CaptureAllViewpoints("after");
        Debug.Log("[VisualPolishPass] Completed visual polish pass and captured screenshots.");
    }

    [MenuItem("Warehouse/Save Scene and Exit Cleanly")]
    public static void SaveAndExit()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.IsValid())
        {
            EditorSceneManager.SaveScene(scene);
        }
        EditorApplication.Exit(0);
    }

    private static void ConfigureTextures()
    {
        string[] normalMaps = new string[]
        {
            "Assets/Textures/Tex_Concrete_Floor_Normal.png",
            "Assets/Textures/Tex_Wall_Panels_Normal.png",
            "Assets/Textures/Tex_Wood_Crate_Normal.png",
            "Assets/Textures/Tex_Metal_Plate_Normal.png"
        };

        foreach (var path in normalMaps)
        {
            if (File.Exists(path))
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null && importer.textureType != TextureImporterType.NormalMap)
                {
                    importer.textureType = TextureImporterType.NormalMap;
                    importer.SaveAndReimport();
                    Debug.Log($"[VisualPolishPass] Configured {path} as NormalMap.");
                }
            }
        }
    }

    private static void ApplyMaterials()
    {
        // Concrete Floor
        var matFloor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor.mat");
        if (matFloor != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Concrete_Floor.png");
            var norm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Concrete_Floor_Normal.png");
            if (tex != null)
            {
                matFloor.SetTexture("_BaseMap", tex);
                matFloor.SetTextureScale("_BaseMap", new Vector2(8f, 12f));
            }
            if (norm != null)
            {
                matFloor.SetTexture("_BumpMap", norm);
                matFloor.SetTextureScale("_BumpMap", new Vector2(8f, 12f));
                matFloor.EnableKeyword("_NORMALMAP");
            }
            matFloor.SetFloat("_Metallic", 0.05f);
            matFloor.SetFloat("_Smoothness", 0.35f);
            matFloor.SetColor("_BaseColor", new Color(0.85f, 0.86f, 0.88f));
            EditorUtility.SetDirty(matFloor);
        }

        // Industrial Wall Panels
        var matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Wall.mat");
        if (matWall != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wall_Panels.png");
            var norm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wall_Panels_Normal.png");
            if (tex != null)
            {
                matWall.SetTexture("_BaseMap", tex);
                matWall.SetTextureScale("_BaseMap", new Vector2(6f, 2f));
            }
            if (norm != null)
            {
                matWall.SetTexture("_BumpMap", norm);
                matWall.SetTextureScale("_BumpMap", new Vector2(6f, 2f));
                matWall.EnableKeyword("_NORMALMAP");
            }
            matWall.SetFloat("_Metallic", 0.2f);
            matWall.SetFloat("_Smoothness", 0.3f);
            matWall.SetColor("_BaseColor", new Color(0.9f, 0.92f, 0.95f));
            EditorUtility.SetDirty(matWall);
        }

        // Wood Crates
        var matCrateWood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Wood.mat");
        if (matCrateWood != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wood_Crate.png");
            var norm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wood_Crate_Normal.png");
            if (tex != null)
            {
                matCrateWood.SetTexture("_BaseMap", tex);
                matCrateWood.SetTextureScale("_BaseMap", new Vector2(1f, 1f));
            }
            if (norm != null)
            {
                matCrateWood.SetTexture("_BumpMap", norm);
                matCrateWood.SetTextureScale("_BumpMap", new Vector2(1f, 1f));
                matCrateWood.EnableKeyword("_NORMALMAP");
            }
            matCrateWood.SetFloat("_Smoothness", 0.22f);
            matCrateWood.SetColor("_BaseColor", new Color(0.9f, 0.85f, 0.8f));
            EditorUtility.SetDirty(matCrateWood);
        }

        // Pallets
        var matPallet = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Pallet.mat");
        if (matPallet != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wood_Crate.png");
            var norm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wood_Crate_Normal.png");
            if (tex != null)
            {
                matPallet.SetTexture("_BaseMap", tex);
                matPallet.SetTextureScale("_BaseMap", new Vector2(1.5f, 1.5f));
            }
            if (norm != null)
            {
                matPallet.SetTexture("_BumpMap", norm);
                matPallet.SetTextureScale("_BumpMap", new Vector2(1.5f, 1.5f));
                matPallet.EnableKeyword("_NORMALMAP");
            }
            matPallet.SetFloat("_Smoothness", 0.18f);
            matPallet.SetColor("_BaseColor", new Color(0.85f, 0.8f, 0.72f));
            EditorUtility.SetDirty(matPallet);
        }

        // Cardboard Dark Crates
        var matCrateDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Dark.mat");
        if (matCrateDark != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Cardboard.png");
            if (tex != null)
            {
                matCrateDark.SetTexture("_BaseMap", tex);
                matCrateDark.SetTextureScale("_BaseMap", new Vector2(1f, 1f));
            }
            matCrateDark.SetFloat("_Smoothness", 0.2f);
            matCrateDark.SetColor("_BaseColor", new Color(0.88f, 0.84f, 0.78f));
            EditorUtility.SetDirty(matCrateDark);
        }

        // Dark Structural Steel
        var matSteelDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Steel_Dark.mat");
        if (matSteelDark != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Metal_Plate.png");
            var norm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Metal_Plate_Normal.png");
            if (tex != null)
            {
                matSteelDark.SetTexture("_BaseMap", tex);
                matSteelDark.SetTextureScale("_BaseMap", new Vector2(4f, 4f));
            }
            if (norm != null)
            {
                matSteelDark.SetTexture("_BumpMap", norm);
                matSteelDark.SetTextureScale("_BumpMap", new Vector2(4f, 4f));
                matSteelDark.EnableKeyword("_NORMALMAP");
            }
            matSteelDark.SetFloat("_Metallic", 0.75f);
            matSteelDark.SetFloat("_Smoothness", 0.5f);
            matSteelDark.SetColor("_BaseColor", new Color(0.45f, 0.48f, 0.52f));
            EditorUtility.SetDirty(matSteelDark);
        }

        // Ventilation Ducts
        var matDuct = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Duct_Metal.mat");
        if (matDuct != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Metal_Plate.png");
            if (tex != null)
            {
                matDuct.SetTexture("_BaseMap", tex);
                matDuct.SetTextureScale("_BaseMap", new Vector2(2f, 8f));
            }
            matDuct.SetFloat("_Metallic", 0.8f);
            matDuct.SetFloat("_Smoothness", 0.6f);
            matDuct.SetColor("_BaseColor", new Color(0.85f, 0.88f, 0.92f));
            EditorUtility.SetDirty(matDuct);
        }

        // Roll-up Shutter Metal
        var matShutter = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Shutter_Metal.mat");
        if (matShutter != null)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wall_Panels.png");
            var norm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wall_Panels_Normal.png");
            if (tex != null)
            {
                matShutter.SetTexture("_BaseMap", tex);
                matShutter.SetTextureScale("_BaseMap", new Vector2(2f, 4f));
            }
            if (norm != null)
            {
                matShutter.SetTexture("_BumpMap", norm);
                matShutter.SetTextureScale("_BumpMap", new Vector2(2f, 4f));
                matShutter.EnableKeyword("_NORMALMAP");
            }
            matShutter.SetFloat("_Metallic", 0.65f);
            matShutter.SetFloat("_Smoothness", 0.45f);
            matShutter.SetColor("_BaseColor", new Color(0.6f, 0.65f, 0.7f));
            EditorUtility.SetDirty(matShutter);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[VisualPolishPass] Applied textures, normal maps, and UV tiling to materials.");
    }

    private static Material CreateDepthTestedTextMaterial()
    {
        string matPath = "Assets/Materials/Mat_Sign_Text_DepthTested.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

        Shader depthShader = Shader.Find("Custom/DepthTestedText");
        if (depthShader == null)
        {
            Debug.LogError("[VisualPolishPass] Custom/DepthTestedText shader not found!");
            depthShader = Shader.Find("Universal Render Pipeline/Unlit");
        }

        if (mat == null)
        {
            mat = new Material(depthShader);
            mat.name = "Mat_Sign_Text_DepthTested";
            AssetDatabase.CreateAsset(mat, matPath);
        }
        else
        {
            mat.shader = depthShader;
        }

        // Find font texture from runtime built-in font or Arial
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font != null && font.material != null && font.material.mainTexture != null)
        {
            mat.mainTexture = font.material.mainTexture;
        }

        mat.SetColor("_Color", new Color(1.0f, 0.95f, 0.25f, 1.0f)); // High-visibility warning yellow
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        Debug.Log("[VisualPolishPass] Created/Updated Mat_Sign_Text_DepthTested material.");
        return mat;
    }

    private static void OverhaulSigns(Material depthTextMat)
    {
        var matSteel = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Steel_Dark.mat");
        var matHazard = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Exit_Hazard.mat");

        // 1. Sign_ToStage1 (Room 1 -> Room 2)
        OverhaulSingleSign(
            "Sign_ToStage1",
            new Vector3(0.00f, 3.45f, -22.30f),
            Quaternion.identity,
            new Vector2(4.2f, 0.7f),
            "ROOM 1: SAFE START\n-> TO ROOM 2: PATROL",
            depthTextMat, matSteel, matHazard,
            facingSouth: true
        );

        // 2. Sign_Route_A (Room 2 Exit Left)
        OverhaulSingleSign(
            "Sign_Route_A",
            new Vector3(-4.70f, 3.60f, -4.30f),
            Quaternion.identity,
            new Vector2(3.6f, 0.7f),
            "ROUTE A: SEARCH SECTOR\n[ ACCESS CARD A ]",
            depthTextMat, matSteel, matHazard,
            facingSouth: true
        );

        // 3. Sign_Route_B (Room 2 Exit Right)
        OverhaulSingleSign(
            "Sign_Route_B",
            new Vector3(4.70f, 3.60f, -4.30f),
            Quaternion.identity,
            new Vector2(3.6f, 0.7f),
            "ROUTE B: DEFENSE SECTOR\n[ ACCESS CARD B ]",
            depthTextMat, matSteel, matHazard,
            facingSouth: true
        );

        // 4. Sign_Room4_W (Corridor 3->5 Entrance to Room 5)
        OverhaulSingleSign(
            "Sign_Room4_W",
            new Vector3(-4.00f, 3.60f, 19.30f),
            Quaternion.identity,
            new Vector2(3.8f, 0.7f),
            "FINAL ENCOUNTER\nROOM 5: BOSS LOADING HALL",
            depthTextMat, matSteel, matHazard,
            facingSouth: true
        );

        // 5. Sign_Room4_E (Corridor 4->5 Entrance to Room 5)
        OverhaulSingleSign(
            "Sign_Room4_E",
            new Vector3(4.00f, 3.60f, 19.30f),
            Quaternion.identity,
            new Vector2(3.8f, 0.7f),
            "FINAL ENCOUNTER\nROOM 5: BOSS LOADING HALL",
            depthTextMat, matSteel, matHazard,
            facingSouth: true
        );

        // 6. Sign_Exit_Master (Room 5 Master Exit above Card Reader)
        OverhaulSingleSign(
            "Sign_Exit_Master",
            new Vector3(0.00f, 4.05f, 42.70f),
            Quaternion.identity,
            new Vector2(4.6f, 0.75f),
            "SECURITY CARD READER\nINSERT ACCESS CARD [E]",
            depthTextMat, matSteel, matHazard,
            facingSouth: true
        );
    }

    private static void OverhaulSingleSign(
        string signName,
        Vector3 worldPos,
        Quaternion worldRot,
        Vector2 boardSize,
        string signText,
        Material textMat,
        Material boardMat,
        Material hazardMat,
        bool facingSouth
    )
    {
        GameObject signObj = GameObject.Find(signName);
        if (signObj == null)
        {
            Debug.LogWarning($"[VisualPolishPass] Sign {signName} not found in scene.");
            return;
        }

        // CRITICAL: Compensate parent lossy scale to guarantee uniform 1,1,1 world scale
        signObj.transform.position = worldPos;
        signObj.transform.rotation = worldRot;
        Vector3 parentLossy = signObj.transform.parent != null ? signObj.transform.parent.lossyScale : Vector3.one;
        signObj.transform.localScale = new Vector3(
            Mathf.Approximately(parentLossy.x, 0) ? 1f : 1f / parentLossy.x,
            Mathf.Approximately(parentLossy.y, 0) ? 1f : 1f / parentLossy.y,
            Mathf.Approximately(parentLossy.z, 0) ? 1f : 1f / parentLossy.z
        );

        // Remove old renderer, filter, or collider on root signObj so it does not collide or duplicate
        var existingMr = signObj.GetComponent<MeshRenderer>();
        if (existingMr != null) Object.DestroyImmediate(existingMr);
        var existingMf = signObj.GetComponent<MeshFilter>();
        if (existingMf != null) Object.DestroyImmediate(existingMf);
        var existingCol = signObj.GetComponent<Collider>();
        if (existingCol != null) Object.DestroyImmediate(existingCol);

        // 1. Backing Board (Dark Steel Industrial Backing Plate)
        Transform boardT = signObj.transform.Find("SignBoard");
        GameObject boardObj;
        if (boardT == null)
        {
            boardObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boardObj.name = "SignBoard";
            boardObj.transform.SetParent(signObj.transform, false);
        }
        else
        {
            boardObj = boardT.gameObject;
        }
        boardObj.transform.localPosition = Vector3.zero;
        boardObj.transform.localRotation = Quaternion.identity;
        boardObj.transform.localScale = new Vector3(boardSize.x, boardSize.y, 0.08f);
        boardObj.GetComponent<Renderer>().sharedMaterial = boardMat;
        var bCol = boardObj.GetComponent<Collider>();
        if (bCol != null) Object.DestroyImmediate(bCol);

        // 2. Safety Hazard Border Trim
        Transform borderT = signObj.transform.Find("SignBorder");
        GameObject borderObj;
        if (borderT == null)
        {
            borderObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderObj.name = "SignBorder";
            borderObj.transform.SetParent(signObj.transform, false);
        }
        else
        {
            borderObj = borderT.gameObject;
        }
        float zOffset = facingSouth ? -0.045f : 0.045f;
        borderObj.transform.localPosition = new Vector3(0f, 0f, zOffset);
        borderObj.transform.localRotation = Quaternion.identity;
        borderObj.transform.localScale = new Vector3(boardSize.x - 0.08f, boardSize.y - 0.08f, 0.02f);
        borderObj.GetComponent<Renderer>().sharedMaterial = hazardMat;
        var rCol = borderObj.GetComponent<Collider>();
        if (rCol != null) Object.DestroyImmediate(rCol);

        // 3. Remove old SignLabel if it exists
        Transform oldLabel = signObj.transform.Find("SignLabel");
        if (oldLabel != null)
            Object.DestroyImmediate(oldLabel.gameObject);

        // 4. TextMesh Component (Uniform local scale, depth tested)
        Transform textT = signObj.transform.Find("Sign_TextMesh");
        GameObject textObj;
        if (textT == null)
        {
            textObj = new GameObject("Sign_TextMesh");
            textObj.transform.SetParent(signObj.transform, false);
            textObj.AddComponent<MeshRenderer>();
            textObj.AddComponent<TextMesh>();
        }
        else
        {
            textObj = textT.gameObject;
        }

        float textZ = facingSouth ? -0.065f : 0.065f;
        textObj.transform.localPosition = new Vector3(0f, 0f, textZ);
        textObj.transform.localRotation = facingSouth ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);
        // Uniform scale so text is never stretched
        textObj.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);

        var tm = textObj.GetComponent<TextMesh>();
        if (tm == null)
            tm = textObj.AddComponent<TextMesh>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font != null)
        {
            tm.font = font;
            if (textMat.mainTexture == null && font.material != null)
            {
                textMat.mainTexture = font.material.mainTexture;
            }
        }

        tm.text = signText;
        tm.fontSize = 28;
        tm.characterSize = 1.0f;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = new Color(1.0f, 0.95f, 0.25f, 1.0f);

        var mr = textObj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sharedMaterial = textMat;
        }

        Debug.Log($"[VisualPolishPass] Overhauled {signName}: uniform scale, depth-tested material, clean mounting.");
    }

    private static void CaptureAllViewpoints(string prefix)
    {
        string outDir = Path.Combine(Application.dataPath, "../scratch/screenshots");
        if (!Directory.Exists(outDir))
            Directory.CreateDirectory(outDir);

        Camera cam = Camera.main;
        bool createdCam = false;
        if (cam == null)
        {
            var camObj = new GameObject("CaptureCamera");
            cam = camObj.AddComponent<Camera>();
            createdCam = true;
        }

        var rt = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        var viewpoints = new (string name, Vector3 pos, Vector3 rot)[]
        {
            ("room1_start", new Vector3(0f, 1.6f, -26f), new Vector3(3f, 0f, 0f)),
            ("room2_patrol", new Vector3(0f, 1.6f, -10f), new Vector3(2f, 0f, 0f)),
            ("room3_search", new Vector3(-12f, 1.6f, 2f), new Vector3(5f, -40f, 0f)),
            ("room4_defensive", new Vector3(12f, 1.6f, 2f), new Vector3(5f, 40f, 0f)),
            ("room5_boss", new Vector3(0f, 1.6f, 23f), new Vector3(2f, 0f, 0f))
        };

        var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);

        foreach (var vp in viewpoints)
        {
            cam.transform.position = vp.pos;
            cam.transform.eulerAngles = vp.rot;
            cam.Render();

            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();
            string filename = $"{prefix}_{vp.name}.png";
            string filePath = Path.Combine(outDir, filename);
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"[VisualPolishPass] Saved screenshot {filePath} ({bytes.Length} bytes)");
        }

        cam.targetTexture = null;
        rt.Release();
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(tex);
        if (createdCam)
        {
            Object.DestroyImmediate(cam.gameObject);
        }
    }
}
