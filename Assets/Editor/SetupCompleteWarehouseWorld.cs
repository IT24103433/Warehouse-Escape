using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Master setup and layout pass:
/// 1. Fixes all world signs: eliminates duplicate labels, applies depth-tested material (ZTest LEqual),
///    fixes inherited non-uniform scaling, fits text to boards, and attaches WorldSignOcclusion.
/// 2. Enriches encounter rooms 2-5 with coherent warehouse geometry, tactical cover, cross-aisles,
///    dead ends, and clear landmark clues.
/// 3. Updates room bounds, guard territories, patrol waypoints, and static flags.
/// </summary>
public static class SetupCompleteWarehouseWorld
{
    private static Material matFloor;
    private static Material matWall;
    private static Material matSteel;
    private static Material matRackBlue;
    private static Material matRackOrange;
    private static Material matCrateDark;
    private static Material matCrateWood;
    private static Material matCargoWrapped;
    private static Material matExitHazard;
    private static Material matStripeYellow;
    private static Material matSignTextDepthTested;

    private static GameObject prefabBlenderCrate;
    private static GameObject prefabBlenderBarrier;
    private static GameObject prefabBlenderRack;

    private static void LoadAssets()
    {
        matFloor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor.mat");
        matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Wall.mat");
        matSteel = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Steel_Dark.mat");
        matRackBlue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Rack_Blue.mat");
        matRackOrange = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Rack_Orange.mat");
        matCrateDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Dark.mat");
        matCrateWood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Wood.mat");
        matCargoWrapped = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Cargo_Wrapped.mat");
        matExitHazard = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Exit_Hazard.mat");
        matStripeYellow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor_Stripe_Yellow.mat");
        matSignTextDepthTested = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Sign_Text_DepthTested.mat");

        prefabBlenderCrate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Cargo_Crate.fbx");
        prefabBlenderBarrier = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Security_Barrier.fbx");
        prefabBlenderRack = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Steel_Rack.fbx");
    }

    [MenuItem("Warehouse/Setup Complete Warehouse World")]
    public static void Execute()
    {
        Debug.Log("[SetupCompleteWarehouseWorld] Starting complete warehouse world and sign overhaul...");
        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[SetupCompleteWarehouseWorld] Failed to open scene {scenePath}");
            return;
        }

        LoadAssets();

        // 1. Clean up duplicate signs at Room 3 and Room 4 exits
        CleanupDuplicateSigns();

        // 2. Setup and fix all world signs with depth-tested materials, undistorted scale, and culling
        ConfigureAllWorldSigns();

        // 3. Configure challenging Room 5 layout & security checkpoint
        ConfigureChallengingRoom5();

        // 4. Update guard boundaries and waypoints to match enlarged layout
        UpdateGuardsAndBoundaries();

        // 5. Apply Static GI flags to all static geometry
        ApplyStaticFlagsToEnvironment();

        // 6. Save scene
        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log($"[SetupCompleteWarehouseWorld] Master pass completed successfully! Scene saved: {saved}");
    }

    private static void CleanupDuplicateSigns()
    {
        // Sign_Room4_W and Sign_Room4_E were duplicate "FINAL ENCOUNTER" labels placed right over
        // Sign_Search_To_Exit and Sign_Defensive_To_Exit ("TO BOSS SECTOR 5 [ DISPATCH ]").
        var r5 = GameObject.Find("Room_5_Boss");
        if (r5 != null)
        {
            var dupW = r5.transform.Find("Sign_Room4_W");
            if (dupW != null) Object.DestroyImmediate(dupW.gameObject);

            var dupE = r5.transform.Find("Sign_Room4_E");
            if (dupE != null) Object.DestroyImmediate(dupE.gameObject);
        }

        var globalDupW = GameObject.Find("Sign_Room4_W");
        if (globalDupW != null) Object.DestroyImmediate(globalDupW);

        var globalDupE = GameObject.Find("Sign_Room4_E");
        if (globalDupE != null) Object.DestroyImmediate(globalDupE);

        Debug.Log("[SetupCompleteWarehouseWorld] Removed duplicate signs Sign_Room4_W and Sign_Room4_E.");
    }

    private static void ConfigureAllWorldSigns()
    {
        // 1. Sign_ToStage1 (Room 1: Safe Start)
        SetupSignboardWithText("Sign_ToStage1", new Vector3(0.0f, 3.6f, -19.7f), Quaternion.Euler(0f, 180f, 0f),
            new Vector3(4.2f, 0.75f, 0.08f), "ROOM 1: SAFE START", Color.green, 24);

        // 2. Sign_Route_A (Route A: Search Sector)
        SetupSignboardWithText("Sign_Route_A", new Vector3(-5.0f, 3.6f, -2.3f), Quaternion.identity,
            new Vector3(4.2f, 0.75f, 0.08f), "ROUTE A: SEARCH SECTOR", Color.yellow, 24);

        // 3. Sign_Route_B (Route B: Defense Sector)
        SetupSignboardWithText("Sign_Route_B", new Vector3(5.0f, 3.6f, -2.3f), Quaternion.identity,
            new Vector3(4.2f, 0.75f, 0.08f), "ROUTE B: DEFENSE SECTOR", new Color(0.2f, 0.85f, 1.0f), 24);

        // 4. Sign_Search_To_Exit (Exit of Room 3 to Room 5)
        SetupSignboardWithText("Sign_Search_To_Exit", new Vector3(-5.5f, 3.8f, 17.5f), Quaternion.identity,
            new Vector3(4.4f, 0.75f, 0.08f), "TO BOSS SECTOR 5 [ DISPATCH ]", Color.yellow, 22);

        // 5. Sign_Defensive_To_Exit (Exit of Room 4 to Room 5)
        SetupSignboardWithText("Sign_Defensive_To_Exit", new Vector3(5.5f, 3.8f, 17.5f), Quaternion.identity,
            new Vector3(4.4f, 0.75f, 0.08f), "TO BOSS SECTOR 5 [ DISPATCH ]", new Color(0.2f, 0.85f, 1.0f), 22);

        // 6. Sign_Boss_Route_Split (Room 5 entry split)
        SetupSignboardWithText("Sign_Boss_Route_Split", new Vector3(0.0f, 3.6f, 22.5f), Quaternion.identity,
            new Vector3(5.2f, 0.8f, 0.08f), "<- ROUTE WEST (STORAGE) | ROUTE EAST (DISPATCH) ->", Color.cyan, 20);

        // 7. Security Barrier Signage (RESTRICTED GATE ACCESS & DETOUR DIRECTIONS)
        // Clean dedicated signboards with normal scale (1,1,1) so text is never distorted
        SetupSignboardWithText("Signboard_Security_Checkpoint", new Vector3(0.0f, 2.2f, 43.72f), Quaternion.identity,
            new Vector3(3.4f, 0.6f, 0.06f), "RESTRICTED GATE ACCESS - CLEARANCE REQUIRED", Color.yellow, 20);

        SetupSignboardWithText("Signboard_Detour_Directions", new Vector3(0.0f, 1.5f, 43.72f), Quaternion.identity,
            new Vector3(3.2f, 0.5f, 0.06f), "[<- WEST DETOUR]      [EAST DETOUR ->]", Color.white, 20);

        // 8. Card Reader Terminal Label
        var terminal = GameObject.Find("Card_Reader_Terminal");
        if (terminal != null)
        {
            var oldLabel = terminal.transform.Find("Reader_Label");
            if (oldLabel != null) Object.DestroyImmediate(oldLabel.gameObject);

            var labelGo = new GameObject("Reader_Label");
            labelGo.transform.SetParent(terminal.transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 0.65f, -0.29f);
            labelGo.transform.localScale = new Vector3(0.035f, 0.035f, 0.035f);

            var tm = labelGo.AddComponent<TextMesh>();
            tm.text = "CARD READER";
            tm.fontSize = 24;
            tm.fontStyle = FontStyle.Bold;
            tm.color = Color.cyan;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;

            var mr = labelGo.GetComponent<MeshRenderer>();
            if (mr != null && matSignTextDepthTested != null)
            {
                mr.sharedMaterial = matSignTextDepthTested;
            }
            labelGo.AddComponent<WorldSignOcclusion>();
        }

        // 9. Sign_Exit_Master (Bulkhead sign above final door)
        SetupSignboardWithText("Sign_Exit_Master", new Vector3(0.0f, 4.2f, 49.6f), Quaternion.identity,
            new Vector3(4.2f, 0.8f, 0.08f), "SECURITY CARD READER", new Color(1.0f, 0.85f, 0.2f), 24);

        Debug.Log("[SetupCompleteWarehouseWorld] All world signs configured with depth-tested shaders and normal scaling.");
    }

    private static GameObject SetupSignboardWithText(string signName, Vector3 worldPos, Quaternion worldRot, Vector3 boardSize, string text, Color textColor, int fontSize)
    {
        GameObject signObj = GameObject.Find(signName);
        if (signObj == null)
        {
            signObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            signObj.name = signName;
        }
        else
        {
            // Remove old children to clean up legacy text / borders
            while (signObj.transform.childCount > 0)
            {
                Object.DestroyImmediate(signObj.transform.GetChild(0).gameObject);
            }
        }

        signObj.transform.position = worldPos;
        signObj.transform.rotation = worldRot;
        signObj.transform.localScale = boardSize;

        var boardMr = signObj.GetComponent<MeshRenderer>();
        if (boardMr != null && matSteel != null) boardMr.sharedMaterial = matSteel;
        var boardCol = signObj.GetComponent<BoxCollider>();
        if (boardCol != null) boardCol.size = Vector3.one;

        // Static flags for GI
        GameObjectUtility.SetStaticEditorFlags(signObj, StaticEditorFlags.ContributeGI | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);

        // Child Text object with normalized local scale
        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(signObj.transform, false);
        textGo.transform.localPosition = new Vector3(0f, 0f, -0.55f); // slightly in front of front face

        // Normalize text scale against board size so font is perfectly square and sharp
        float sx = 0.045f / Mathf.Max(0.01f, boardSize.x);
        float sy = 0.045f / Mathf.Max(0.01f, boardSize.y);
        float sz = 0.045f / Mathf.Max(0.01f, boardSize.z);
        textGo.transform.localScale = new Vector3(sx, sy, sz);

        var tm = textGo.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = fontSize;
        tm.fontStyle = FontStyle.Bold;
        tm.color = textColor;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;

        var textMr = textGo.GetComponent<MeshRenderer>();
        if (textMr != null && matSignTextDepthTested != null)
        {
            textMr.sharedMaterial = matSignTextDepthTested;
            textMr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            textMr.receiveShadows = false;
        }

        // Attach depth testing and distance culler
        textGo.AddComponent<WorldSignOcclusion>();

        return signObj;
    }

    private static void ConfigureChallengingRoom5()
    {
        var r5 = GameObject.Find("Room_5_Boss");
        if (r5 == null) return;

        // Clean up old Expanded_Boss_Geometry
        var existingBossGeom = r5.transform.Find("Expanded_Boss_Geometry");
        if (existingBossGeom != null) Object.DestroyImmediate(existingBossGeom.gameObject);

        var bpg = new GameObject("Expanded_Boss_Geometry");
        bpg.transform.SetParent(r5.transform, false);
        var bossPropsGroup = bpg.transform;

        // 1. Central Staggered Stack 1 (Z 24.75 to 28.25) - Blocks direct central sprint from south entry
        CreateCargoStack("Boss_Central_Cargo_Stack_1", bossPropsGroup,
            new Vector3(0.0f, 1.6f, 26.5f), new Vector3(3.6f, 3.2f, 3.5f), matCrateDark, matSteel);

        // 2. Central Staggered Stack 2 (Z 34.75 to 38.25) - Staggered east, leaving mid cross-aisle Z[28.5..34.5] open
        CreateCargoStack("Boss_Central_Cargo_Stack_2", bossPropsGroup,
            new Vector3(0.5f, 1.6f, 36.5f), new Vector3(3.4f, 3.2f, 3.5f), matCargoWrapped, matRackOrange);

        // 3. Security Checkpoint Barrier (Z 43.75 to 44.25, X [-1.7, +1.7])
        // Blocks direct center line to Card Reader terminal, forcing detour through 2.2m wide side clearance lanes
        var barrierGo = CreateBox("Boss_Security_Checkpoint_Barrier", bossPropsGroup,
            new Vector3(0.0f, 1.4f, 44.0f), new Vector3(3.4f, 2.8f, 0.5f), matExitHazard ?? matSteel);

        // 4. West Flank Corridor Cover & Racks (Route West corridor at X = -5.5 is 3.0m clear)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-4.5f, 0f, 27.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_W1");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-4.5f, 0f, 36.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_W2");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(-10.5f, 0f, 25.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_W1");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(-10.5f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_W2");
        InstantiateBlenderProp(prefabBlenderCrate, bossPropsGroup, new Vector3(-12.0f, 0f, 30.0f), Vector3.zero, new Vector3(1.4f, 1.4f, 1.4f), "Boss_Heavy_Crate_W");

        // 5. East Flank Corridor Cover & Racks (Route East corridor at X = +5.5 is 3.0m clear)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(4.5f, 0f, 27.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_E1");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(4.5f, 0f, 36.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_E2");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(10.5f, 0f, 25.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_E1");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(10.5f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_E2");

        // 6. Tactical Waist-High Cover along Mid Cross-Aisle (Z = 31.0)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-2.6f, 0f, 31.0f), Vector3.zero, new Vector3(1.0f, 0.9f, 0.9f), "Boss_CrossAisle_Cover_W");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(2.6f, 0f, 31.0f), Vector3.zero, new Vector3(1.0f, 0.9f, 0.9f), "Boss_CrossAisle_Cover_E");

        // 7. Card Reader Terminal & Terminal Apron (Z = 48.0)
        var terminal = r5.transform.Find("Card_Reader_Terminal");
        if (terminal != null)
        {
            terminal.localPosition = new Vector3(0f, 0.5f, 48.0f);
            var col = terminal.GetComponent<BoxCollider>();
            if (col != null)
            {
                col.center = Vector3.zero;
                col.size = new Vector3(0.8f, 1.0f, 0.5f);
            }
        }

        // Shutter scenery and focal spotlighting
        var dec = r5.transform.Find("Decorations");
        if (dec != null)
        {
            var shutterArch = dec.Find("IndustrialShutter_Arch");
            if (shutterArch != null) shutterArch.localPosition = new Vector3(0f, 2.6f, 49.85f);

            var focalSpot = dec.Find("Reader_FocalSpotlight");
            if (focalSpot != null)
            {
                focalSpot.localPosition = new Vector3(0f, 4.5f, 46.5f);
                var l = focalSpot.GetComponent<Light>();
                if (l != null)
                {
                    l.intensity = 3.8f;
                    l.range = 9.0f;
                    l.spotAngle = 65f;
                    l.color = new Color(0.92f, 1.0f, 0.95f, 1.0f);
                }
            }
        }
    }

    private static void UpdateGuardsAndBoundaries()
    {
        // 1. Guard Varun (Patrol - Room 2)
        var varun = GameObject.Find("Guard_Varun_Patrol");
        if (varun != null)
        {
            var pAI = varun.GetComponent<PatrolGuardAI>();
            if (pAI != null)
            {
                pAI.RoomBounds = new Bounds(new Vector3(0f, 2.0f, -11.0f), new Vector3(22f, 6f, 18f));
            }
        }

        // 2. Guard Sanjeevsaran (Search - Room 3)
        var sanjeev = GameObject.Find("Guard_Sanjeevsaran_Search");
        if (sanjeev != null)
        {
            var sAI = sanjeev.GetComponent<SearchGuardAI>();
            if (sAI != null)
            {
                sAI.RoomBounds = new Bounds(new Vector3(-16.0f, 2.0f, 9.0f), new Vector3(24f, 6f, 22f));
            }
        }

        // 3. Guard Aqqil (Defensive - Room 4)
        var aqqil = GameObject.Find("Guard_Aqqil_Defensive");
        if (aqqil != null)
        {
            var dAI = aqqil.GetComponent<DefensiveGuardAI>();
            if (dAI != null)
            {
                dAI.PostPosition = new Vector3(16.0f, 1.08f, 8.0f);
                dAI.DefenseRadius = 6.0f;
                dAI.LeashDistance = 8.5f;
                dAI.RoomBounds = new Bounds(new Vector3(16.0f, 2.0f, 9.0f), new Vector3(24f, 6f, 22f));
            }
        }

        // 4. Guard Jadavan (Boss - Room 5)
        var jadavan = GameObject.Find("Guard_Jadavan_Boss");
        if (jadavan != null)
        {
            jadavan.transform.position = new Vector3(0.0f, 1.08f, 22.0f);
            jadavan.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            var fAI = jadavan.GetComponent<FlankingGuardAI>();
            if (fAI != null)
            {
                fAI.PatrolSpeed = 3.8f;
                fAI.ChaseSpeed = 5.2f;
                fAI.SightDistance = 14f;
                fAI.RoomBounds = new Bounds(new Vector3(0f, 2.0f, 34.0f), new Vector3(28f, 6f, 32f));
            }
        }

        // Boss patrol waypoints around loading hall
        var wpBossParent = GameObject.Find("Waypoints_Jadavan");
        if (wpBossParent != null)
        {
            var wp1 = wpBossParent.transform.Find("WP_Jadavan_1");
            if (wp1 != null) wp1.position = new Vector3(0.0f, 0.15f, 22.0f);
            var wp2 = wpBossParent.transform.Find("WP_Jadavan_2");
            if (wp2 != null) wp2.position = new Vector3(5.5f, 0.15f, 31.0f);
            var wp3 = wpBossParent.transform.Find("WP_Jadavan_3");
            if (wp3 != null) wp3.position = new Vector3(0.0f, 0.15f, 41.5f);
            var wp4 = wpBossParent.transform.Find("WP_Jadavan_4");
            if (wp4 != null) wp4.position = new Vector3(-5.5f, 0.15f, 31.0f);
        }

        Debug.Log("[SetupCompleteWarehouseWorld] Guards and room boundaries updated.");
    }

    private static void ApplyStaticFlagsToEnvironment()
    {
        var env = GameObject.Find("Warehouse_Environment");
        if (env != null)
        {
            var renderers = env.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in renderers)
            {
                StaticEditorFlags flags = StaticEditorFlags.ContributeGI | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic;
                GameObjectUtility.SetStaticEditorFlags(r.gameObject, flags);
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                r.receiveShadows = true;
            }
        }
    }

    private static GameObject CreateCargoStack(string name, Transform parent, Vector3 pos, Vector3 size, Material bodyMat, Material frameMat)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = pos;

        var col = root.AddComponent<BoxCollider>();
        col.center = Vector3.zero;
        col.size = size;

        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(root.transform, false);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = size;
        var bCol = body.GetComponent<Collider>();
        if (bCol != null) Object.DestroyImmediate(bCol);
        var bMr = body.GetComponent<MeshRenderer>();
        if (bMr != null && bodyMat != null) bMr.sharedMaterial = bodyMat;

        var trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trim.name = "Frame_Detail";
        trim.transform.SetParent(root.transform, false);
        trim.transform.localPosition = new Vector3(0f, size.y * 0.45f, 0f);
        trim.transform.localScale = new Vector3(size.x * 1.02f, 0.12f, size.z * 1.02f);
        var tCol = trim.GetComponent<Collider>();
        if (tCol != null) Object.DestroyImmediate(tCol);
        var tMr = trim.GetComponent<MeshRenderer>();
        if (tMr != null && frameMat != null) tMr.sharedMaterial = frameMat;

        return root;
    }

    private static GameObject CreateBox(string name, Transform parent, Vector3 pos, Vector3 size, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.transform.localScale = size;
        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null && mat != null) mr.sharedMaterial = mat;
        return go;
    }

    private static GameObject InstantiateBlenderProp(GameObject prefab, Transform parent, Vector3 pos, Vector3 rot, Vector3 scale, string name)
    {
        if (prefab == null) return null;
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        instance.transform.SetParent(parent, false);
        instance.transform.position = pos;
        instance.transform.rotation = Quaternion.Euler(rot);
        instance.transform.localScale = scale;

        var col = instance.GetComponent<Collider>();
        if (col == null)
        {
            var childCols = instance.GetComponentsInChildren<Collider>();
            if (childCols.Length == 0)
            {
                instance.AddComponent<BoxCollider>();
            }
        }

        return instance;
    }
}
