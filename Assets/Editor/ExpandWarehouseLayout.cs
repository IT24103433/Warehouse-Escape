using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Stage 1 Layout Expansion pass for Warehouse-Escape.
/// Enlarges encounter rooms (Rooms 2, 3, 4, 5) and connecting corridors while keeping Room 1 safe start simple.
/// Creates navigable warehouse layouts with racks, cross-aisles, cover (including Blender props),
/// side paths, and dead ends.
/// Exits require exploration with consistent clues (signs, lighting, floor markings, landmarks).
/// Room 5 is made substantially harder with multiple connected aisles, alternate routes, misleading side paths,
/// dangerous sightlines, and discoverable Card Reader terminal.
/// Updates patrol routes, territories, spawn positions, and NavMesh.
/// </summary>
public static class ExpandWarehouseLayout
{
    private static Material matFloor;
    private static Material matWall;
    private static Material matCeiling;
    private static Material matSteel;
    private static Material matRackBlue;
    private static Material matRackOrange;
    private static Material matShelf;
    private static Material matPallet;
    private static Material matCrateDark;
    private static Material matCrateWood;
    private static Material matBarrelBlue;
    private static Material matBarrelHazard;
    private static Material matCargoWrapped;
    private static Material matStripeYellow;
    private static Material matStripeWhite;
    private static Material matForklift;
    private static Material matShutter;
    private static Material matExitHazard;

    private static GameObject prefabBlenderCrate;
    private static GameObject prefabBlenderBarrier;
    private static GameObject prefabBlenderRack;

    private static void LoadAssets()
    {
        matFloor = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor.mat");
        matWall = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Wall.mat");
        matCeiling = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Ceiling.mat");
        matSteel = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Steel_Dark.mat");
        matRackBlue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Rack_Blue.mat");
        matRackOrange = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Rack_Orange.mat");
        matShelf = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Shelf_Level.mat");
        matPallet = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Pallet.mat");
        matCrateDark = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Dark.mat");
        matCrateWood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Crate_Wood.mat");
        matBarrelBlue = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Barrel_Blue.mat");
        matBarrelHazard = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Barrel_Hazard.mat");
        matCargoWrapped = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Cargo_Wrapped.mat");
        matStripeYellow = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor_Stripe_Yellow.mat");
        matStripeWhite = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Floor_Stripe_White.mat");
        matForklift = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Forklift_Yellow.mat");
        matShutter = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Shutter_Metal.mat");
        matExitHazard = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Exit_Hazard.mat");

        prefabBlenderCrate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Cargo_Crate.fbx");
        prefabBlenderBarrier = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Security_Barrier.fbx");
        prefabBlenderRack = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Warehouse_Steel_Rack.fbx");
    }

    [MenuItem("Warehouse/Execute Layout Expansion Pass")]
    public static void Execute()
    {
        Debug.Log("[ExpandWarehouseLayout] Starting layout expansion pass...");
        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[ExpandWarehouseLayout] Failed to open {scenePath}");
            return;
        }

        LoadAssets();

        // 1. Expand Room 2 (Patrol Room)
        ExpandRoom2();

        // 2. Expand Room 3 (Search Sector - Route A)
        ExpandRoom3();

        // 3. Expand Room 4 (Defensive Sector - Route B)
        ExpandRoom4();

        // 4. Expand Room 5 (Boss Loading Hall)
        ExpandRoom5();

        // 5. Update Waypoints & AI Territories
        UpdateWaypointsAndTerritories();

        // 6. Rebake NavMesh
        RebakeNavMesh();

        // 7. Save Scene
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[ExpandWarehouseLayout] Scene layout expanded and saved successfully!");

        EditorApplication.Exit(0);
    }

    private static GameObject CreateBox(string name, Transform parent, Vector3 localPos, Vector3 size, Material mat, bool isTrigger = false)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = size;
        if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;
        var col = go.GetComponent<BoxCollider>();
        if (col != null) col.isTrigger = isTrigger;
        return go;
    }

    private static void InstantiateBlenderProp(GameObject prefab, Transform parent, Vector3 localPos, Vector3 localRot, Vector3 localScale, string name)
    {
        if (prefab == null) return;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        instance.transform.SetParent(parent, false);
        instance.transform.localPosition = localPos;
        instance.transform.localEulerAngles = localRot;
        instance.transform.localScale = localScale;

        // Ensure collider for cover based on child renderers
        var renderers = instance.GetComponentsInChildren<Renderer>();
        if (renderers != null && renderers.Length > 0)
        {
            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                b.Encapsulate(renderers[i].bounds);
            }
            var col = instance.GetComponent<BoxCollider>();
            if (col == null) col = instance.AddComponent<BoxCollider>();
            col.center = instance.transform.InverseTransformPoint(b.center);
            Vector3 worldSize = b.size;
            col.size = new Vector3(
                worldSize.x / Mathf.Max(0.001f, instance.transform.lossyScale.x),
                worldSize.y / Mathf.Max(0.001f, instance.transform.lossyScale.y),
                worldSize.z / Mathf.Max(0.001f, instance.transform.lossyScale.z)
            );
        }
        else if (instance.GetComponent<Collider>() == null)
        {
            instance.AddComponent<BoxCollider>();
        }
    }

    private static void ExpandRoom2()
    {
        var r2 = GameObject.Find("Room_2_Patrol");
        if (r2 == null) return;

        // Ensure expanded floor: Width 22m, Length 18m -> bounds X [-11, 11], Z [-20, -2]
        var floor = r2.transform.Find("Floor");
        if (floor != null)
        {
            floor.localPosition = new Vector3(0f, -0.25f, -11.0f);
            floor.localScale = new Vector3(22.0f, 0.5f, 18.0f);
        }

        // Walls
        var wallW = r2.transform.Find("Wall_West");
        if (wallW != null)
        {
            wallW.localPosition = new Vector3(-11.0f, 2.5f, -11.0f);
            wallW.localScale = new Vector3(0.5f, 5.0f, 18.0f);
        }

        var wallE = r2.transform.Find("Wall_East");
        if (wallE != null)
        {
            wallE.localPosition = new Vector3(11.0f, 2.5f, -11.0f);
            wallE.localScale = new Vector3(0.5f, 5.0f, 18.0f);
        }

        var wallSouthL = r2.transform.Find("Wall_South_L");
        if (wallSouthL != null)
        {
            wallSouthL.localPosition = new Vector3(-6.25f, 2.5f, -20.0f);
            wallSouthL.localScale = new Vector3(9.5f, 5.0f, 0.5f);
        }

        var wallSouthR = r2.transform.Find("Wall_South_R");
        if (wallSouthR != null)
        {
            wallSouthR.localPosition = new Vector3(6.25f, 2.5f, -20.0f);
            wallSouthR.localScale = new Vector3(9.5f, 5.0f, 0.5f);
        }

        var wallSouthLintel = r2.transform.Find("Wall_South_Lintel");
        if (wallSouthLintel != null)
        {
            wallSouthLintel.localPosition = new Vector3(0f, 4.25f, -20.0f);
            wallSouthLintel.localScale = new Vector3(3.0f, 1.5f, 0.5f);
        }

        // Connecting Corridor 1->2 floor stretch
        var r1 = GameObject.Find("Room_1_Start");
        if (r1 != null)
        {
            var corr12Floor = r1.transform.Find("Corridor_1_2_Floor");
            if (corr12Floor != null)
            {
                corr12Floor.localPosition = new Vector3(0f, -0.25f, -21.0f);
                corr12Floor.localScale = new Vector3(3.0f, 0.5f, 2.8f);
            }
        }

        // North Walls & Exits
        var wallNorthCenter = r2.transform.Find("Wall_North_Center");
        if (wallNorthCenter != null)
        {
            wallNorthCenter.localPosition = new Vector3(0f, 2.5f, -2.0f);
            wallNorthCenter.localScale = new Vector3(7.5f, 5.0f, 0.5f);
        }

        var wallNorthFarW = r2.transform.Find("Wall_North_FarW");
        if (wallNorthFarW != null)
        {
            wallNorthFarW.localPosition = new Vector3(-8.75f, 2.5f, -2.0f);
            wallNorthFarW.localScale = new Vector3(4.5f, 5.0f, 0.5f);
        }

        var wallNorthFarE = r2.transform.Find("Wall_North_FarE");
        if (wallNorthFarE != null)
        {
            wallNorthFarE.localPosition = new Vector3(8.75f, 2.5f, -2.0f);
            wallNorthFarE.localScale = new Vector3(4.5f, 5.0f, 0.5f);
        }

        var lintelA = r2.transform.Find("Lintel_Exit_A");
        if (lintelA != null)
        {
            lintelA.localPosition = new Vector3(-5.0f, 4.25f, -2.0f);
            lintelA.localScale = new Vector3(3.0f, 1.5f, 0.5f);
        }

        var lintelB = r2.transform.Find("Lintel_Exit_B");
        if (lintelB != null)
        {
            lintelB.localPosition = new Vector3(5.0f, 4.25f, -2.0f);
            lintelB.localScale = new Vector3(3.0f, 1.5f, 0.5f);
        }

        // Adjust corridor branches leading from Room 2 north exits
        var corrA_F1 = r2.transform.Find("Corridor_A_Floor_1");
        if (corrA_F1 != null) { corrA_F1.localPosition = new Vector3(-5.0f, -0.25f, 0.5f); corrA_F1.localScale = new Vector3(3.8f, 0.5f, 5.0f); }
        var corrA_WallE = r2.transform.Find("Corridor_A_Wall_E");
        if (corrA_WallE != null) { corrA_WallE.localPosition = new Vector3(-3.5f, 2.5f, 0.75f); corrA_WallE.localScale = new Vector3(0.5f, 5.0f, 5.5f); }
        var corrA_WallW = r2.transform.Find("Corridor_A_Wall_W");
        if (corrA_WallW != null) { corrA_WallW.localPosition = new Vector3(-6.5f, 2.5f, -1.5f); corrA_WallW.localScale = new Vector3(0.5f, 5.0f, 1.0f); }

        var corrB_F1 = r2.transform.Find("Corridor_B_Floor_1");
        if (corrB_F1 != null) { corrB_F1.localPosition = new Vector3(5.0f, -0.25f, 0.5f); corrB_F1.localScale = new Vector3(3.8f, 0.5f, 5.0f); }
        var corrB_WallW = r2.transform.Find("Corridor_B_Wall_W");
        if (corrB_WallW != null) { corrB_WallW.localPosition = new Vector3(3.5f, 2.5f, 0.75f); corrB_WallW.localScale = new Vector3(0.5f, 5.0f, 5.5f); }
        var corrB_WallE = r2.transform.Find("Corridor_B_Wall_E");
        if (corrB_WallE != null) { corrB_WallE.localPosition = new Vector3(6.5f, 2.5f, -1.5f); corrB_WallE.localScale = new Vector3(0.5f, 5.0f, 1.0f); }

        // Route Signs positioned cleanly
        var signA = r2.transform.Find("Sign_Route_A");
        if (signA != null) signA.localPosition = new Vector3(-5.0f, 3.6f, -2.3f);
        var signB = r2.transform.Find("Sign_Route_B");
        if (signB != null) signB.localPosition = new Vector3(5.0f, 3.6f, -2.3f);

        // Add Expanded Cover & Side Paths to Room 2
        var existingGroup = r2.transform.Find("Expanded_Warehouse_Props");
        if (existingGroup != null) Object.DestroyImmediate(existingGroup.gameObject);
        var eg = new GameObject("Expanded_Warehouse_Props");
        eg.transform.SetParent(r2.transform, false);
        var expandedGroup = eg.transform;

        // West Flank Cover Barricade & Crate Stacks
        InstantiateBlenderProp(prefabBlenderBarrier, expandedGroup, new Vector3(-7.5f, 0f, -11.0f), Vector3.zero, new Vector3(1f, 1f, 1f), "Cover_Barrier_West");
        InstantiateBlenderProp(prefabBlenderCrate, expandedGroup, new Vector3(-8.5f, 0f, -15.5f), Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f), "Cargo_Crate_SW_Corner");
        InstantiateBlenderProp(prefabBlenderCrate, expandedGroup, new Vector3(-8.5f, 0f, -6.5f), Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f), "Cargo_Crate_NW_Corner");

        // East Flank Dead-End Bay & Cover Barricade
        InstantiateBlenderProp(prefabBlenderBarrier, expandedGroup, new Vector3(7.5f, 0f, -11.0f), Vector3.zero, new Vector3(1f, 1f, 1f), "Cover_Barrier_East");
        InstantiateBlenderProp(prefabBlenderCrate, expandedGroup, new Vector3(8.5f, 0f, -15.5f), Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f), "Cargo_Crate_SE_Corner");
        InstantiateBlenderProp(prefabBlenderCrate, expandedGroup, new Vector3(8.5f, 0f, -6.5f), Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f), "Cargo_Crate_NE_Corner");

        // Dead-End storage partition in NE corner creating an exploration dead end
        CreateBox("DeadEnd_Partition_NE", expandedGroup, new Vector3(8.2f, 1.8f, -4.5f), new Vector3(2.5f, 3.6f, 0.4f), matSteel);

        // Guide Floor Lines in Room 2 (Guiding players to Route A & Route B)
        var floorGuideA = CreateBox("FloorGuide_RouteA", expandedGroup, new Vector3(-2.5f, 0.005f, -6.0f), new Vector3(0.2f, 0.01f, 8.0f), matStripeYellow);
        floorGuideA.GetComponent<Collider>().enabled = false;
        var floorGuideB = CreateBox("FloorGuide_RouteB", expandedGroup, new Vector3(2.5f, 0.005f, -6.0f), new Vector3(0.2f, 0.01f, 8.0f), matStripeYellow);
        floorGuideB.GetComponent<Collider>().enabled = false;
    }

    private static void ExpandRoom3()
    {
        var r3 = GameObject.Find("Room_3_Search");
        if (r3 == null) return;

        // Expanded floor: Width 24m, Length 22m -> bounds X [-28, -4], Z [-2, 20]
        var floor = r3.transform.Find("Floor");
        if (floor != null)
        {
            floor.localPosition = new Vector3(-16.0f, -0.25f, 9.0f);
            floor.localScale = new Vector3(24.0f, 0.5f, 22.0f);
        }

        // Outer Walls (Wall_South leaves doorway X: [-6.5, -3.5] clear)
        var wallS = r3.transform.Find("Wall_South");
        if (wallS != null) { wallS.localPosition = new Vector3(-17.25f, 2.5f, -2.0f); wallS.localScale = new Vector3(21.5f, 5.0f, 0.5f); }
        var wallW = r3.transform.Find("Wall_West");
        if (wallW != null) { wallW.localPosition = new Vector3(-28.0f, 2.5f, 9.0f); wallW.localScale = new Vector3(0.5f, 5.0f, 22.0f); }
        var wallN = r3.transform.Find("Wall_North");
        if (wallN != null) { wallN.localPosition = new Vector3(-16.75f, 2.5f, 20.0f); wallN.localScale = new Vector3(22.5f, 5.0f, 0.5f); }

        var wallEMid = r3.transform.Find("Wall_East_Mid");
        if (wallEMid != null) { wallEMid.localPosition = new Vector3(-4.0f, 2.5f, 7.0f); wallEMid.localScale = new Vector3(0.5f, 5.0f, 11.0f); }

        // Expanded Props & Dead Ends in Room 3
        var existingProps = r3.transform.Find("Expanded_Search_Props");
        if (existingProps != null) Object.DestroyImmediate(existingProps.gameObject);
        var pg = new GameObject("Expanded_Search_Props");
        pg.transform.SetParent(r3.transform, false);
        var propsGroup = pg.transform;

        // Cover barricade & dead-end storage bay along far West wall (X <= -25 leaves X = -21 transit clear)
        InstantiateBlenderProp(prefabBlenderBarrier, propsGroup, new Vector3(-25.5f, 0f, 3.5f), Vector3.zero, new Vector3(1f, 1f, 1f), "Search_Cover_Barrier_S");
        InstantiateBlenderProp(prefabBlenderBarrier, propsGroup, new Vector3(-25.5f, 0f, 9.5f), Vector3.zero, new Vector3(1f, 1f, 1f), "Search_Cover_Barrier_N");
        InstantiateBlenderProp(prefabBlenderCrate, propsGroup, new Vector3(-26.0f, 0f, 6.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "DeadEnd_Cargo_Stack_1");
        InstantiateBlenderProp(prefabBlenderCrate, propsGroup, new Vector3(-26.0f, 0f, 7.5f), Vector3.zero, new Vector3(1.0f, 1.0f, 1.0f), "DeadEnd_Cargo_Stack_2");
        InstantiateBlenderProp(prefabBlenderRack, propsGroup, new Vector3(-25.5f, 0f, 13.0f), Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f), "DeadEnd_Blender_Rack");

        // Dead end partition wall forming a deceptive side alcove
        CreateBox("DeadEnd_Partition_West", propsGroup, new Vector3(-24.5f, 2.0f, 1.5f), new Vector3(0.4f, 4.0f, 5.0f), matSteel);

        // Ensure Card A table and trigger are 100% accessible and lit
        var cardTable = r3.transform.Find("Search_Pallet_CardTable");
        if (cardTable != null)
        {
            cardTable.localPosition = new Vector3(-21.0f, 0.4f, 14.0f);
        }

        var cardAObj = GameObject.Find("AccessCard_Pickup_Search");
        if (cardAObj != null)
        {
            cardAObj.transform.position = new Vector3(-21.0f, 1.0f, 14.0f);
            var col = cardAObj.GetComponent<BoxCollider>();
            if (col != null)
            {
                // Generous trigger bounds ensuring physical touch without obstruction
                col.center = Vector3.zero;
                col.size = new Vector3(4.0f, 10.0f, 6.0f);
            }
        }

        // Overhead Corridor Exit Sign
        var exitSign = propsGroup.Find("Sign_Search_To_Exit");
        if (exitSign == null)
        {
            var sGo = CreateBox("Sign_Search_To_Exit", propsGroup, new Vector3(-6.5f, 3.8f, 17.5f), new Vector3(3.2f, 0.7f, 0.08f), matSteel);
            var tmGo = new GameObject("Text");
            tmGo.transform.SetParent(sGo.transform, false);
            tmGo.transform.localPosition = new Vector3(0f, 0f, -0.05f);
            tmGo.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            var tm = tmGo.AddComponent<TextMesh>();
            tm.text = "TO BOSS SECTOR 5 [ DISPATCH ]";
            tm.fontSize = 24;
            tm.color = Color.yellow;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
        }
    }

    private static void ExpandRoom4()
    {
        var r4 = GameObject.Find("Room_4_Defensive");
        if (r4 == null) return;

        // Expanded floor: Width 24m, Length 22m -> bounds X [4, 28], Z [-2, 20]
        var floor = r4.transform.Find("Floor");
        if (floor != null)
        {
            floor.localPosition = new Vector3(16.0f, -0.25f, 9.0f);
            floor.localScale = new Vector3(24.0f, 0.5f, 22.0f);
        }

        // Outer Walls (Wall_South leaves doorway X: [3.5, 6.5] clear)
        var wallS = r4.transform.Find("Wall_South");
        if (wallS != null) { wallS.localPosition = new Vector3(17.25f, 2.5f, -2.0f); wallS.localScale = new Vector3(21.5f, 5.0f, 0.5f); }
        var wallE = r4.transform.Find("Wall_East");
        if (wallE != null) { wallE.localPosition = new Vector3(28.0f, 2.5f, 9.0f); wallE.localScale = new Vector3(0.5f, 5.0f, 22.0f); }
        var wallN = r4.transform.Find("Wall_North");
        if (wallN != null) { wallN.localPosition = new Vector3(16.75f, 2.5f, 20.0f); wallN.localScale = new Vector3(22.5f, 5.0f, 0.5f); }

        var wallWMid = r4.transform.Find("Wall_West_Mid");
        if (wallWMid != null) { wallWMid.localPosition = new Vector3(4.0f, 2.5f, 7.0f); wallWMid.localScale = new Vector3(0.5f, 5.0f, 11.0f); }

        // Expanded Props & Defensive Barricades in Room 4
        var existingDefProps = r4.transform.Find("Expanded_Defensive_Props");
        if (existingDefProps != null) Object.DestroyImmediate(existingDefProps.gameObject);
        var pg = new GameObject("Expanded_Defensive_Props");
        pg.transform.SetParent(r4.transform, false);
        var propsGroup = pg.transform;

        // Heavy concrete security barricades for defensive gunplay cover (positioned clear of X=10 transit aisle)
        InstantiateBlenderProp(prefabBlenderBarrier, propsGroup, new Vector3(13.5f, 0f, 5.0f), Vector3.zero, new Vector3(1f, 1f, 1f), "Defensive_Barrier_West");
        InstantiateBlenderProp(prefabBlenderBarrier, propsGroup, new Vector3(16.0f, 0f, 5.0f), Vector3.zero, new Vector3(1f, 1f, 1f), "Defensive_Barrier_Center");
        InstantiateBlenderProp(prefabBlenderBarrier, propsGroup, new Vector3(20.0f, 0f, 7.5f), new Vector3(0f, 90f, 0f), new Vector3(1f, 1f, 1f), "Defensive_Barrier_East");

        // Dead-End Forklift Staging Alcove on East Wall
        InstantiateBlenderProp(prefabBlenderCrate, propsGroup, new Vector3(25.5f, 0f, 3.5f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "DeadEnd_Crate_E1");
        InstantiateBlenderProp(prefabBlenderCrate, propsGroup, new Vector3(25.5f, 0f, 5.0f), Vector3.zero, new Vector3(1.0f, 1.0f, 1.0f), "DeadEnd_Crate_E2");
        InstantiateBlenderProp(prefabBlenderRack, propsGroup, new Vector3(25.5f, 0f, 7.5f), Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f), "DeadEnd_Blender_Rack_E");
        CreateBox("DeadEnd_Partition_East", propsGroup, new Vector3(24.0f, 2.0f, 1.5f), new Vector3(0.4f, 4.0f, 5.0f), matSteel);

        // Security Console & Card B Pickup verification
        var desk = r4.transform.Find("Terminal_Desk");
        if (desk == null) desk = r4.transform.Find("[Aqqil_Placeholders]/[Placeholder]_SecurityConsole_Terminal/Terminal_Desk");
        if (desk != null) desk.position = new Vector3(16.0f, 0.5f, 10.0f);

        var cardBObj = GameObject.Find("AccessCard_Pickup_Defensive");
        if (cardBObj != null)
        {
            cardBObj.transform.position = new Vector3(16.0f, 1.15f, 10.0f);
            var col = cardBObj.GetComponent<BoxCollider>();
            if (col != null)
            {
                col.center = Vector3.zero;
                col.size = new Vector3(4.0f, 4.0f, 4.0f);
            }
        }

        // Overhead Corridor Exit Sign
        var exitSign = propsGroup.Find("Sign_Defensive_To_Exit");
        if (exitSign == null)
        {
            var sGo = CreateBox("Sign_Defensive_To_Exit", propsGroup, new Vector3(6.5f, 3.8f, 17.5f), new Vector3(3.2f, 0.7f, 0.08f), matSteel);
            var tmGo = new GameObject("Text");
            tmGo.transform.SetParent(sGo.transform, false);
            tmGo.transform.localPosition = new Vector3(0f, 0f, -0.05f);
            tmGo.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            var tm = tmGo.AddComponent<TextMesh>();
            tm.text = "TO BOSS SECTOR 5 [ DISPATCH ]";
            tm.fontSize = 24;
            tm.color = Color.cyan;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
        }
    }

    private static void ExpandRoom5()
    {
        var r5 = GameObject.Find("Room_5_Boss");
        if (r5 == null) return;

        // Expanded floor: Width 28m, Length 32m -> bounds X [-14, 14], Z [18, 50]
        var floor = r5.transform.Find("Floor");
        if (floor != null)
        {
            floor.localPosition = new Vector3(0f, -0.25f, 34.0f);
            floor.localScale = new Vector3(28.0f, 0.5f, 32.0f);
        }

        // Outer Walls
        var wallW = r5.transform.Find("Wall_West");
        if (wallW != null) { wallW.localPosition = new Vector3(-14.0f, 2.5f, 34.0f); wallW.localScale = new Vector3(0.5f, 5.0f, 32.0f); }
        var wallE = r5.transform.Find("Wall_East");
        if (wallE != null) { wallE.localPosition = new Vector3(14.0f, 2.5f, 34.0f); wallE.localScale = new Vector3(0.5f, 5.0f, 32.0f); }

        var wallSouthCenter = r5.transform.Find("Wall_South_Center");
        if (wallSouthCenter != null) { wallSouthCenter.localPosition = new Vector3(0f, 2.5f, 18.0f); wallSouthCenter.localScale = new Vector3(5.0f, 5.0f, 0.5f); }
        var wallSouthFarW = r5.transform.Find("Wall_South_FarW");
        if (wallSouthFarW != null) { wallSouthFarW.localPosition = new Vector3(-9.75f, 2.5f, 18.0f); wallSouthFarW.localScale = new Vector3(8.5f, 5.0f, 0.5f); }
        var wallSouthFarE = r5.transform.Find("Wall_South_FarE");
        if (wallSouthFarE != null) { wallSouthFarE.localPosition = new Vector3(9.75f, 2.5f, 18.0f); wallSouthFarE.localScale = new Vector3(8.5f, 5.0f, 0.5f); }

        var wallNL = r5.transform.Find("Wall_North_L");
        if (wallNL != null) { wallNL.localPosition = new Vector3(-8.25f, 2.5f, 50.0f); wallNL.localScale = new Vector3(11.5f, 5.0f, 0.5f); }
        var wallNR = r5.transform.Find("Wall_North_R");
        if (wallNR != null) { wallNR.localPosition = new Vector3(8.25f, 2.5f, 50.0f); wallNR.localScale = new Vector3(11.5f, 5.0f, 0.5f); }
        var wallNLintel = r5.transform.Find("Wall_North_Lintel");
        if (wallNLintel != null) { wallNLintel.localPosition = new Vector3(0f, 4.6f, 50.0f); wallNLintel.localScale = new Vector3(5.0f, 1.2f, 0.5f); }

        // Entrance Signs
        var signRoom4W = r5.transform.Find("Sign_Room4_W");
        if (signRoom4W != null) signRoom4W.localPosition = new Vector3(-5.0f, 3.6f, 18.3f);
        var signRoom4E = r5.transform.Find("Sign_Room4_E");
        if (signRoom4E != null) signRoom4E.localPosition = new Vector3(5.0f, 3.6f, 18.3f);

        // Substantially harder warehouse geometry:
        // Central Loading thoroughfare (Z 20 to 47), West aisle (X = -6.5), East aisle (X = +6.5), Cross-aisles, Dead-end storage bay
        var existingBossGeom = r5.transform.Find("Expanded_Boss_Geometry");
        if (existingBossGeom != null) Object.DestroyImmediate(existingBossGeom.gameObject);
        var bpg = new GameObject("Expanded_Boss_Geometry");
        bpg.transform.SetParent(r5.transform, false);
        var bossPropsGroup = bpg.transform;

        // West Outer Rack Line (X = -4.2 for barrier, X = -10.5 for outer steel racks, keeping X = -6.5 transit clear)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-4.2f, 0f, 26.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_W1");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(-4.2f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_W2");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(-10.5f, 0f, 25.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_W1");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(-10.5f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_W2");
        InstantiateBlenderProp(prefabBlenderCrate, bossPropsGroup, new Vector3(-12.0f, 0f, 30.0f), Vector3.zero, new Vector3(1.4f, 1.4f, 1.4f), "Boss_Heavy_Crate_W");

        // East Outer Rack Line (X = +4.2 for barrier, X = +10.5 for outer steel racks, keeping X = +6.5 transit clear)
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(4.2f, 0f, 26.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_E1");
        InstantiateBlenderProp(prefabBlenderBarrier, bossPropsGroup, new Vector3(4.2f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.0f, 1.0f), "Boss_Cover_Barrier_E2");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(10.5f, 0f, 25.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_E1");
        InstantiateBlenderProp(prefabBlenderRack, bossPropsGroup, new Vector3(10.5f, 0f, 35.0f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), "Boss_Steel_Rack_E2");

        // Misleading Dead-End Alcove on East Wall
        CreateBox("Boss_DeadEnd_Wall_S", bossPropsGroup, new Vector3(12.5f, 2.0f, 28.0f), new Vector3(3.0f, 4.0f, 0.4f), matSteel);
        CreateBox("Boss_DeadEnd_Wall_N", bossPropsGroup, new Vector3(12.5f, 2.0f, 34.0f), new Vector3(3.0f, 4.0f, 0.4f), matSteel);
        InstantiateBlenderProp(prefabBlenderCrate, bossPropsGroup, new Vector3(12.5f, 0f, 31.0f), Vector3.zero, new Vector3(1.5f, 1.5f, 1.5f), "DeadEnd_Cargo_Container");

        // Clues leading to Card Reader Terminal:
        // Floor Walkway stripes leading up center aisle straight to Card Reader Terminal
        var floorStripes = CreateBox("Boss_Evacuation_Walkway", bossPropsGroup, new Vector3(0f, 0.005f, 33.0f), new Vector3(1.8f, 0.01f, 26.0f), matStripeWhite);
        floorStripes.GetComponent<Collider>().enabled = false;

        // Position Card Reader Terminal at far north wall
        var terminal = r5.transform.Find("Card_Reader_Terminal");
        if (terminal != null)
        {
            terminal.localPosition = new Vector3(0f, 0.5f, 48.0f);
        }

        // Shutter scenery door behind reader
        var dec = r5.transform.Find("Decorations");
        if (dec != null)
        {
            var shutterArch = dec.Find("IndustrialShutter_Arch");
            if (shutterArch != null) shutterArch.localPosition = new Vector3(0f, 2.6f, 49.85f);
            var focalSpot = dec.Find("Reader_FocalSpotlight");
            if (focalSpot != null) focalSpot.localPosition = new Vector3(0f, 4.5f, 46.5f);
        }

        // Master Exit Sign overhead
        var signMaster = r5.transform.Find("Sign_Exit_Master");
        if (signMaster != null)
        {
            signMaster.localPosition = new Vector3(0f, 4.2f, 49.6f);
        }

        // Reposition legacy Exit_Door to the north exit wall behind Card Reader (Z = 49.85) in open position
        var exitDoor = GameObject.Find("Exit_Door");
        if (exitDoor != null)
        {
            exitDoor.transform.position = new Vector3(0f, 4.5f, 49.85f);
            var doorCol = exitDoor.GetComponent<Collider>();
            if (doorCol != null)
            {
                doorCol.enabled = false;
            }
            var doorCtrl = exitDoor.GetComponent<DoorController>();
            if (doorCtrl != null)
            {
                doorCtrl.StartOpen = true;
                doorCtrl.ShowInteractionPrompt = false;
                doorCtrl.RequiresKeycard = false;
            }
        }

        var navBlocker = GameObject.Find("Exit_Door_NavBlocker");
        if (navBlocker != null)
        {
            Object.DestroyImmediate(navBlocker);
        }

        // Align decorative shutter slats and drum with the north exit wall archway at Z = 49.85
        if (dec != null)
        {
            for (int i = 0; i < dec.childCount; i++)
            {
                var child = dec.GetChild(i);
                if (child.name.StartsWith("Shutter_"))
                {
                    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, 49.85f);
                }
            }
        }
    }

    private static void UpdateWaypointsAndTerritories()
    {
        // 0. Remove legacy EnemyGuard placeholder if present
        var legacyEnemy = GameObject.Find("EnemyGuard");
        if (legacyEnemy != null) Object.DestroyImmediate(legacyEnemy);

        // 1. Varun (Patrol Guard - Room 2)
        var varun = GameObject.Find("Guard_Varun_Patrol");
        if (varun != null)
        {
            varun.transform.position = new Vector3(-5.0f, 1.08f, -15.0f);
            var pAI = varun.GetComponent<PatrolGuardAI>();
            // Updated Waypoints for expanded Room 2 circuit
            var wpVarunParent = GameObject.Find("Waypoints_Varun");
            if (wpVarunParent != null)
            {
                var wp1 = wpVarunParent.transform.Find("WP_Varun_1");
                if (wp1 != null) wp1.position = new Vector3(-5.0f, 0.15f, -15.0f);
                var wp2 = wpVarunParent.transform.Find("WP_Varun_2");
                if (wp2 != null) wp2.position = new Vector3(-5.0f, 0.15f, -6.0f);
                var wp3 = wpVarunParent.transform.Find("WP_Varun_3");
                if (wp3 != null) wp3.position = new Vector3(5.0f, 0.15f, -6.0f);
                var wp4 = wpVarunParent.transform.Find("WP_Varun_4");
                if (wp4 != null) wp4.position = new Vector3(5.0f, 0.15f, -15.0f);
            }
        }

        // 2. Sanjeevsaran (Search Guard - Room 3)
        var sanjeev = GameObject.Find("Guard_Sanjeevsaran_Search");
        if (sanjeev != null)
        {
            sanjeev.transform.position = new Vector3(-12.0f, 1.08f, 2.0f);
            var wpSanjeevParent = GameObject.Find("Waypoints_Sanjeevsaran");
            if (wpSanjeevParent != null)
            {
                var wp1 = wpSanjeevParent.transform.Find("WP_Sanjeev_1");
                if (wp1 != null) wp1.position = new Vector3(-10.0f, 0.15f, 2.0f);
                var wp2 = wpSanjeevParent.transform.Find("WP_Sanjeev_2");
                if (wp2 != null) wp2.position = new Vector3(-22.0f, 0.15f, 2.0f);
                var wp3 = wpSanjeevParent.transform.Find("WP_Sanjeev_3");
                if (wp3 != null) wp3.position = new Vector3(-22.0f, 0.15f, 12.0f);
                var wp4 = wpSanjeevParent.transform.Find("WP_Sanjeev_4");
                if (wp4 != null) wp4.position = new Vector3(-10.0f, 0.15f, 12.0f);
            }
        }

        // 3. Aqqil (Defensive Guard - Room 4)
        var aqqil = GameObject.Find("Guard_Aqqil_Defensive");
        if (aqqil != null)
        {
            aqqil.transform.position = new Vector3(16.0f, 1.08f, 8.0f);
            var dAI = aqqil.GetComponent<DefensiveGuardAI>();
            if (dAI != null)
            {
                dAI.PostPosition = new Vector3(16.0f, 1.08f, 8.0f);
                dAI.DefenseRadius = 6.0f;
                dAI.LeashDistance = 9.0f;
            }
        }

        // 4. Jadavan (Final Boss - Room 5)
        var boss = GameObject.Find("Guard_Jadavan_Boss");
        if (boss != null)
        {
            boss.transform.position = new Vector3(0.0f, 1.08f, 23.0f);
            var wpBossParent = GameObject.Find("Waypoints_Jadavan");
            if (wpBossParent != null)
            {
                var wp1 = wpBossParent.transform.Find("WP_Jadavan_1");
                if (wp1 != null) wp1.position = new Vector3(0.0f, 0.15f, 23.0f);
                var wp2 = wpBossParent.transform.Find("WP_Jadavan_2");
                if (wp2 != null) wp2.position = new Vector3(6.5f, 0.15f, 32.0f);
                var wp3 = wpBossParent.transform.Find("WP_Jadavan_3");
                if (wp3 != null) wp3.position = new Vector3(0.0f, 0.15f, 42.0f);
                var wp4 = wpBossParent.transform.Find("WP_Jadavan_4");
                if (wp4 != null) wp4.position = new Vector3(-6.5f, 0.15f, 32.0f);
            }
        }
    }

    private static void RebakeNavMesh()
    {
        Debug.Log("[ExpandWarehouseLayout] Rebaking NavMeshSurface on updated geometry...");
        var surfaces = Object.FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
        foreach (var surface in surfaces)
        {
            surface.BuildNavMesh();
            Debug.Log($"[ExpandWarehouseLayout] Baked NavMeshSurface on: {surface.gameObject.name}");
        }
    }
}
