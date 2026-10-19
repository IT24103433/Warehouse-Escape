using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class SetupChallengingFinalRoom
{
    private static Material matFloor;
    private static Material matWall;
    private static Material matSteel;
    private static Material matRackOrange;
    private static Material matRackBlue;
    private static Material matCrateDark;
    private static Material matCrateWood;
    private static Material matCargoWrapped;
    private static Material matBarrelHazard;
    private static Material matExitHazard;
    private static Material matStripeYellow;

    private static GameObject prefabBlenderCrate;
    private static GameObject prefabBlenderBarrier;
    private static GameObject prefabBlenderRack;

    private static void LoadAssets()
    {
        matFloor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor.mat");
        matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Wall.mat");
        matSteel = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Steel_Dark.mat");
        matRackOrange = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Rack_Orange.mat");
        matRackBlue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Rack_Blue.mat");
        matCrateDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Dark.mat");
        matCrateWood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Wood.mat");
        matCargoWrapped = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Cargo_Wrapped.mat");
        matBarrelHazard = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Barrel_Hazard.mat");
        matExitHazard = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Exit_Hazard.mat");
        matStripeYellow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor_Stripe_Yellow.mat");

        prefabBlenderCrate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Cargo_Crate.fbx");
        prefabBlenderBarrier = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Security_Barrier.fbx");
        prefabBlenderRack = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Steel_Rack.fbx");
    }

    [MenuItem("Warehouse/Setup Challenging Final Room")]
    public static void Execute()
    {
        Debug.Log("[SetupChallengingFinalRoom] Starting layout update for challenging Room 5...");
        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[SetupChallengingFinalRoom] Failed to open scene {scenePath}");
            return;
        }

        LoadAssets();

        var r5 = GameObject.Find("Room_5_Boss");
        if (r5 == null)
        {
            Debug.LogError("[SetupChallengingFinalRoom] Room_5_Boss not found!");
            return;
        }

        // 1. Recreate Expanded_Boss_Geometry cleanly
        var existingBossGeom = r5.transform.Find("Expanded_Boss_Geometry");
        if (existingBossGeom != null) Object.DestroyImmediate(existingBossGeom.gameObject);

        var bpg = new GameObject("Expanded_Boss_Geometry");
        bpg.transform.SetParent(r5.transform, false);
        var bossPropsGroup = bpg.transform;

        // 2. Central Staggered Stack 1 (Z 24.75 to 28.25) - Blocks direct central sprint from south entry
        CreateCargoStack("Boss_Central_Cargo_Stack_1", bossPropsGroup,
            new Vector3(0.0f, 1.6f, 26.5f), new Vector3(3.6f, 3.2f, 3.5f), matCrateDark, matSteel);

        // 3. Central Staggered Stack 2 (Z 34.75 to 38.25) - Staggered east, leaving mid cross-aisle Z[28.5..34.5] open
        CreateCargoStack("Boss_Central_Cargo_Stack_2", bossPropsGroup,
            new Vector3(0.5f, 1.6f, 36.5f), new Vector3(3.4f, 3.2f, 3.5f), matCargoWrapped, matRackOrange);

        // 4. Security Checkpoint Barrier (Z 43.75 to 44.25, X [-1.6, +1.6])
        // Blocks direct center line to the Card Reader terminal, forcing detour through 2.15m wide side clearance lanes
        var barrierGo = CreateBox("Boss_Security_Checkpoint_Barrier", bossPropsGroup,
            new Vector3(0.0f, 1.4f, 44.0f), new Vector3(3.2f, 2.8f, 0.5f), matExitHazard ?? matSteel);

        // Security barrier signage facing south
        CreateSign("Sign_Security_Checkpoint", barrierGo.transform,
            new Vector3(0.0f, 0.4f, -0.28f), new Vector3(3.0f, 0.7f, 0.05f),
            "RESTRICTED GATE ACCESS - CLEARANCE REQUIRED", Color.yellow);
        CreateSign("Sign_Detour_Directions", barrierGo.transform,
            new Vector3(0.0f, -0.3f, -0.28f), new Vector3(3.0f, 0.6f, 0.05f),
            "[<- WEST DETOUR]      [EAST DETOUR ->]", Color.white);

        // 5. West Outer Rack Line & Tactical Cover (Route West corridor at X = -5.5 is 3.0m clear)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-4.5f, 0f, 27.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_W1");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-4.5f, 0f, 36.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_W2");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(-10.5f, 0f, 25.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_W1");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(-10.5f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_W2");
        InstantiateBlenderProp(prefabBlenderCrate, bossPropsGroup, new Vector3(-12.0f, 0f, 30.0f), Vector3.zero, new Vector3(1.4f, 1.4f, 1.4f), "Boss_Heavy_Crate_W");

        // 6. East Outer Rack Line & Tactical Cover (Route East corridor at X = +5.5 is 3.0m clear)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(4.5f, 0f, 27.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_E1");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(4.5f, 0f, 36.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_E2");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(10.5f, 0f, 25.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_E1");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(10.5f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_E2");

        // 7. Tactical Waist-High Cover along Mid Cross-Aisle (Z = 31.0)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-2.6f, 0f, 31.0f), Vector3.zero, new Vector3(1.0f, 0.9f, 0.9f), "Boss_CrossAisle_Cover_W");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(2.6f, 0f, 31.0f), Vector3.zero, new Vector3(1.0f, 0.9f, 0.9f), "Boss_CrossAisle_Cover_E");

        // 8. Directional Sign at South Staging Split (Z = 23.0)
        var splitSign = CreateBox("Sign_Boss_Route_Split", bossPropsGroup,
            new Vector3(0.0f, 3.5f, 23.0f), new Vector3(4.8f, 0.8f, 0.08f), matSteel);
        CreateSign("Sign_Text", splitSign.transform,
            new Vector3(0.0f, 0.0f, -0.05f), new Vector3(4.6f, 0.7f, 0.02f),
            "<- ROUTE WEST (STORAGE) | ROUTE EAST (DISPATCH) ->", Color.cyan);

        // 9. Card Reader Terminal & Terminal Apron (Z = 48.0, Apron X [-2.25, 2.25], Z [45.5, 48.0])
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

        // Master Exit Sign overhead
        var signMaster = r5.transform.Find("Sign_Exit_Master");
        if (signMaster != null)
        {
            signMaster.localPosition = new Vector3(0f, 4.2f, 49.6f);
        }

        // 10. Configure Static Editor Flags on all new geometry
        var allRenderers = bossPropsGroup.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var r in allRenderers)
        {
            StaticEditorFlags flags = StaticEditorFlags.ContributeGI | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic;
            GameObjectUtility.SetStaticEditorFlags(r.gameObject, flags);
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            r.receiveShadows = true;
        }

        // 11. Update Boss Waypoints & Initial Spawn
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

        var boss = GameObject.Find("Guard_Jadavan_Boss");
        if (boss != null)
        {
            boss.transform.position = new Vector3(0.0f, 1.08f, 22.0f);
            boss.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            var flankAI = boss.GetComponent<FlankingGuardAI>();
            if (flankAI != null)
            {
                // Ensure room bounds cover Room 5
                flankAI.PatrolSpeed = 3.8f;
                flankAI.ChaseSpeed = 5.2f;
                flankAI.SightDistance = 14f;
            }
        }

        // 12. Save scene
        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log($"[SetupChallengingFinalRoom] Room 5 geometry and waypoints updated and saved successfully! Saved={saved}");
    }

    private static GameObject CreateCargoStack(string name, Transform parent, Vector3 pos, Vector3 size, Material bodyMat, Material frameMat)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = pos;

        // Solid collider for entire stack
        var col = root.AddComponent<BoxCollider>();
        col.center = Vector3.zero;
        col.size = size;

        // Visual representation: base pallet, cargo body, top crate
        var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(root.transform, false);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = size;
        var bCol = body.GetComponent<Collider>();
        if (bCol != null) Object.DestroyImmediate(bCol);
        var bMr = body.GetComponent<MeshRenderer>();
        if (bMr != null && bodyMat != null) bMr.sharedMaterial = bodyMat;

        // Edge trim / steel frame detail
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

    private static void CreateSign(string name, Transform parent, Vector3 localPos, Vector3 size, string text, Color textColor)
    {
        var textGo = new GameObject(name);
        textGo.transform.SetParent(parent, false);
        textGo.transform.localPosition = localPos;
        textGo.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);

        var tm = textGo.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 26;
        tm.fontStyle = FontStyle.Bold;
        tm.color = textColor;
        tm.alignment = TextAlignment.Center;
        tm.anchor = TextAnchor.MiddleCenter;
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

        // Ensure collider is active
        var col = instance.GetComponent<Collider>();
        if (col == null)
        {
            var childCols = instance.GetComponentsInChildren<Collider>();
            if (childCols.Length == 0)
            {
                var box = instance.AddComponent<BoxCollider>();
            }
        }

        return instance;
    }
}
