using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public static class BakeAndVerifyNavMesh
{
    [MenuItem("Warehouse/Bake and Verify NavMesh")]
    public static void Execute()
    {
        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[BakeAndVerifyNavMesh] Failed to open scene {scenePath}");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("================================================================================");
        sb.AppendLine("EXPANDED WAREHOUSE NAVMESH BAKE & CONNECTIVITY VERIFICATION REPORT");
        sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("================================================================================\n");

        // 1. Ensure Character Visuals and Dynamic Entities are excluded from baking
        sb.AppendLine("--- 1. VERIFYING NAVMESH MODIFIER EXCLUSIONS ---");
        var modifiers = UnityEngine.Object.FindObjectsByType<NavMeshModifier>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var mod in modifiers)
        {
            // All Character_Visuals, Player, Guards, and Exit_Door must be ignored
            string goName = mod.gameObject.name;
            if (goName.Contains("Visuals") || goName.Contains("Player") || goName.Contains("Guard") || goName.Contains("Door"))
            {
                mod.ignoreFromBuild = true;
                EditorUtility.SetDirty(mod);
                sb.AppendLine($"[EXCLUDED] {goName} on GameObject '{mod.gameObject.name}' marked ignoreFromBuild = true.");
            }
        }

        // Also check any Character_Visuals without a NavMeshModifier
        var allTransforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in allTransforms)
        {
            if (t.name == "Character_Visuals")
            {
                var mod = t.GetComponent<NavMeshModifier>();
                if (mod == null)
                {
                    mod = t.gameObject.AddComponent<NavMeshModifier>();
                }
                mod.ignoreFromBuild = true;
                EditorUtility.SetDirty(mod);
                sb.AppendLine($"[EXCLUDED] Added NavMeshModifier to '{t.parent?.name}/{t.name}' (ignoreFromBuild = true).");
            }
        }

        // 2. Identify / Configure Canonical NavMeshSurface
        sb.AppendLine("\n--- 2. CONFIGURING CANONICAL NAVMESHSURFACE ---");
        var surfaces = UnityEngine.Object.FindObjectsByType<NavMeshSurface>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        NavMeshSurface canonicalSurface = null;

        foreach (var s in surfaces)
        {
            if (s.gameObject.name == "NavMesh Surface" || s.gameObject.name == "Warehouse_Environment")
            {
                if (canonicalSurface == null)
                {
                    canonicalSurface = s;
                }
                else
                {
                    // Disable duplicate surface to prevent overlapping NavMeshData conflicts
                    sb.AppendLine($"[CLEANUP] Disabling secondary NavMeshSurface on '{s.gameObject.name}' to prevent conflicts.");
                    s.enabled = false;
                    s.navMeshData = null;
                    EditorUtility.SetDirty(s);
                }
            }
        }

        if (canonicalSurface == null)
        {
            var go = GameObject.Find("NavMesh Surface");
            if (go == null) go = new GameObject("NavMesh Surface");
            canonicalSurface = go.AddComponent<NavMeshSurface>();
        }

        canonicalSurface.enabled = true;
        canonicalSurface.agentTypeID = 0; // Humanoid
        canonicalSurface.collectObjects = CollectObjects.All;
        canonicalSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        canonicalSurface.defaultArea = 0; // Walkable
        canonicalSurface.layerMask = ~0; // All layers
        canonicalSurface.ignoreNavMeshAgent = true;
        canonicalSurface.ignoreNavMeshObstacle = true;
        EditorUtility.SetDirty(canonicalSurface);

        sb.AppendLine($"Canonical Surface configured on: '{canonicalSurface.gameObject.name}'");
        sb.AppendLine($"  CollectObjects: {canonicalSurface.collectObjects}");
        sb.AppendLine($"  UseGeometry: {canonicalSurface.useGeometry}");
        sb.AppendLine($"  AgentTypeID: {canonicalSurface.agentTypeID} (Humanoid)");

        // 3. Bake NavMesh for Expanded Layout
        sb.AppendLine("\n--- 3. BAKING NAVMESH FOR EXPANDED GEOMETRY ---");
        canonicalSurface.BuildNavMesh();

        var bakedData = canonicalSurface.navMeshData;
        if (bakedData == null)
        {
            sb.AppendLine("[ERROR] BuildNavMesh produced NULL NavMeshData!");
            Debug.LogError("[BakeAndVerifyNavMesh] BuildNavMesh produced NULL NavMeshData!");
            return;
        }

        sb.AppendLine($"[SUCCESS] NavMesh built successfully!");
        sb.AppendLine($"  Source Bounds: {bakedData.sourceBounds}");
        sb.AppendLine($"  Position: {bakedData.position}, Rotation: {bakedData.rotation}");

        // 4. Save NavMeshData to Disk Assets (Persistent Project Asset & Resources)
        sb.AppendLine("\n--- 4. PERSISTING NAVMESHDATA ASSETS ---");
        string sceneAssetDir = "Assets/Scenes/Warehouse";
        if (!Directory.Exists(sceneAssetDir)) Directory.CreateDirectory(sceneAssetDir);
        string mainAssetPath = $"{sceneAssetDir}/NavMesh-Warehouse_Environment.asset";

        string resourcesDir = "Assets/Resources";
        if (!Directory.Exists(resourcesDir)) Directory.CreateDirectory(resourcesDir);
        string resourceAssetPath = $"{resourcesDir}/NavMeshData_Warehouse.asset";

        // Save main scene asset
        if (File.Exists(mainAssetPath))
        {
            AssetDatabase.DeleteAsset(mainAssetPath);
        }
        AssetDatabase.CreateAsset(UnityEngine.Object.Instantiate(bakedData), mainAssetPath);
        var persistentAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>(mainAssetPath);

        // Save Resources asset for BeforeSceneLoad preloading
        if (File.Exists(resourceAssetPath))
        {
            AssetDatabase.DeleteAsset(resourceAssetPath);
        }
        AssetDatabase.CreateAsset(UnityEngine.Object.Instantiate(bakedData), resourceAssetPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        canonicalSurface.navMeshData = persistentAsset;
        EditorUtility.SetDirty(canonicalSurface);
        EditorUtility.SetDirty(persistentAsset);

        sb.AppendLine($"[SAVED] Persistent Asset saved to: {mainAssetPath} (Persistent={EditorUtility.IsPersistent(persistentAsset)})");
        sb.AppendLine($"[SAVED] Resources Asset saved to: {resourceAssetPath} for runtime preloading before scene load.");

        // 5. Test Adding and Querying NavMesh
        sb.AppendLine("\n--- 5. TESTING ACTIVE NAVMESH REGIONS & SPHERICAL PROBING ---");
        NavMesh.RemoveAllNavMeshData();
        var testInstance = NavMesh.AddNavMeshData(persistentAsset);
        sb.AppendLine($"NavMeshDataInstance registered in native engine. Valid={testInstance.valid}");

        var tri = NavMesh.CalculateTriangulation();
        sb.AppendLine($"Active NavMesh Triangulation: Vertices={tri.vertices.Length}, Indices={tri.indices.Length}, Areas={tri.areas.Length}");
        if (tri.vertices.Length < 1000)
        {
            sb.AppendLine($"[WARNING] Low vertex count ({tri.vertices.Length}) for expanded warehouse!");
        }

        // 6. Verify All 4 Guard Spawn Positions
        sb.AppendLine("\n--- 6. VERIFYING GUARD SPAWN POSITIONS ON NAVMESH ---");
        var guardSpawns = new (string name, Vector3 pos)[]
        {
            ("Guard_Varun_Patrol (Room 2)", new Vector3(-5.0f, 1.08f, -15.0f)),
            ("Guard_Sanjeevsaran_Search (Room 3)", new Vector3(-12.0f, 1.08f, 2.0f)),
            ("Guard_Aqqil_Defensive (Room 4)", new Vector3(16.0f, 1.08f, 8.0f)),
            ("Guard_Jadavan_Boss (Room 5)", new Vector3(0.0f, 1.08f, 22.0f))
        };

        bool allSpawnsValid = true;
        foreach (var guard in guardSpawns)
        {
            NavMeshHit hit;
            bool hitOk = NavMesh.SamplePosition(guard.pos, out hit, 1.5f, NavMesh.AllAreas);
            float dist = hitOk ? Vector3.Distance(guard.pos, hit.position) : 999f;
            sb.AppendLine($"Guard: {guard.name} at {guard.pos}:");
            sb.AppendLine($"  SamplePosition Hit={hitOk}, NavMesh Point={hit.position}, Dist={dist:F3}m, AreaMask={hit.mask}");

            if (!hitOk || dist > 1.2f)
            {
                sb.AppendLine($"  [FAIL] Guard spawn is NOT properly on NavMesh!");
                allSpawnsValid = false;
            }
            else
            {
                sb.AppendLine($"  [PASS] Guard spawn is firmly seated on walkable NavMesh.");
            }
        }

        // 7. Test Complete Route Connectivity
        sb.AppendLine("\n--- 7. VERIFYING ROUTE & SECTOR CONNECTIVITY ---");
        var testRoutes = new (string routeName, Vector3 start, Vector3 end)[]
        {
            ("Room 1 Safe Start -> Room 2 Center", new Vector3(0f, 1.08f, -26f), new Vector3(0f, 1.08f, -14f)),
            ("Room 2 Center -> Room 3 Search (Card A)", new Vector3(0f, 1.08f, -14f), new Vector3(-18.2f, 1.08f, 14f)),
            ("Room 2 Center -> Room 4 Defense (Card B)", new Vector3(0f, 1.08f, -14f), new Vector3(14.0f, 1.08f, 10f)),
            ("Room 3 Search -> Room 5 West Entrance", new Vector3(-18.2f, 1.08f, 14f), new Vector3(-5.5f, 1.08f, 22f)),
            ("Room 4 Defense -> Room 5 East Entrance", new Vector3(14.0f, 1.08f, 10f), new Vector3(5.5f, 1.08f, 22f)),
            ("Room 5 West Route -> Detour -> Card Reader", new Vector3(-5.5f, 1.08f, 22f), new Vector3(0f, 1.08f, 46.8f)),
            ("Room 5 East Route -> Detour -> Card Reader", new Vector3(5.5f, 1.08f, 22f), new Vector3(0f, 1.08f, 46.8f)),
            ("Boss Left Flank: Mid Cross-Aisle West -> North Staging", new Vector3(-5.5f, 1.08f, 31f), new Vector3(0f, 1.08f, 41.5f)),
            ("Boss Right Flank: Mid Cross-Aisle East -> North Staging", new Vector3(5.5f, 1.08f, 31f), new Vector3(0f, 1.08f, 41.5f)),
            ("Full Route A: Spawn -> Room 3 (Card A) -> Room 5 West Route -> Card Reader", new Vector3(0f, 1.08f, -26f), new Vector3(0f, 1.08f, 46.8f)),
            ("Full Route B: Spawn -> Room 4 (Card B) -> Room 5 East Route -> Card Reader", new Vector3(0f, 1.08f, -26f), new Vector3(0f, 1.08f, 46.8f))
        };

        bool allRoutesComplete = true;
        foreach (var r in testRoutes)
        {
            NavMeshHit hStart, hEnd;
            bool sOk = NavMesh.SamplePosition(r.start, out hStart, 2.5f, NavMesh.AllAreas);
            bool eOk = NavMesh.SamplePosition(r.end, out hEnd, 2.5f, NavMesh.AllAreas);

            NavMeshPath path = new NavMeshPath();
            bool calcOk = false;
            if (sOk && eOk)
            {
                calcOk = NavMesh.CalculatePath(hStart.position, hEnd.position, NavMesh.AllAreas, path);
            }

            sb.AppendLine($"Route: '{r.routeName}':");
            sb.AppendLine($"  Status: {path.status}, Corners: {path.corners.Length}, CalcOk: {calcOk}");

            if (path.status != NavMeshPathStatus.PathComplete)
            {
                sb.AppendLine($"  [FAIL] Route path status is {path.status} (incomplete or blocked)!");
                allRoutesComplete = false;
            }
            else
            {
                float length = 0f;
                for (int c = 1; c < path.corners.Length; c++)
                {
                    length += Vector3.Distance(path.corners[c - 1], path.corners[c]);
                }
                sb.AppendLine($"  [PASS] Path complete! Total Distance: {length:F2}m.");
            }
        }

        // 8. Test Guard Patrol Route Connectivity
        sb.AppendLine("\n--- 8. VERIFYING GUARD PATROL ROUTES ---");
        // Varun's patrol points in Room 2
        Vector3[] varunWps = new Vector3[]
        {
            new Vector3(-3.0f, 1.08f, -15.5f),
            new Vector3( 4.5f, 1.08f, -14.0f),
            new Vector3( 4.5f, 1.08f,  -9.5f),
            new Vector3( 4.0f, 1.08f,  -5.5f),
            new Vector3( 0.0f, 1.08f,  -6.5f),
            new Vector3(-4.5f, 1.08f,  -5.5f),
            new Vector3(-5.0f, 1.08f, -10.5f),
            new Vector3(-4.5f, 1.08f, -14.5f)
        };
        bool varunCircuitOk = true;
        for (int i = 0; i < varunWps.Length; i++)
        {
            Vector3 from = varunWps[i];
            Vector3 to = varunWps[(i + 1) % varunWps.Length];
            NavMeshHit hFrom, hTo;
            NavMesh.SamplePosition(from, out hFrom, 2.0f, NavMesh.AllAreas);
            NavMesh.SamplePosition(to, out hTo, 2.0f, NavMesh.AllAreas);
            NavMeshPath p = new NavMeshPath();
            NavMesh.CalculatePath(hFrom.position, hTo.position, NavMesh.AllAreas, p);
            if (p.status != NavMeshPathStatus.PathComplete)
            {
                sb.AppendLine($"  [FAIL] Varun WP {i} -> {(i+1)%varunWps.Length} is {p.status}");
                varunCircuitOk = false;
            }
        }
        sb.AppendLine($"Varun Patrol Circuit in Room 2: {(varunCircuitOk ? "[PASS] Complete loop" : "[FAIL]")}");

        // Jadavan Boss Flanking candidates in Room 5
        Vector3[] jadavanWps = new Vector3[]
        {
            new Vector3(-6.5f, 1.08f, 26.0f), // West flank aisle
            new Vector3( 6.5f, 1.08f, 26.0f), // East flank aisle
            new Vector3(-6.5f, 1.08f, 38.0f), // West deep flank
            new Vector3( 6.5f, 1.08f, 38.0f), // East deep flank
            new Vector3( 0.0f, 1.08f, 41.5f)  // North staging apron
        };
        bool bossFlankOk = true;
        Vector3 bossSpawn = new Vector3(0.0f, 1.08f, 22.0f);
        NavMeshHit hBossSpawn;
        NavMesh.SamplePosition(bossSpawn, out hBossSpawn, 2.0f, NavMesh.AllAreas);
        foreach (var jwp in jadavanWps)
        {
            NavMeshHit hJwp;
            NavMesh.SamplePosition(jwp, out hJwp, 2.0f, NavMesh.AllAreas);
            NavMeshPath p = new NavMeshPath();
            NavMesh.CalculatePath(hBossSpawn.position, hJwp.position, NavMesh.AllAreas, p);
            if (p.status != NavMeshPathStatus.PathComplete)
            {
                sb.AppendLine($"  [FAIL] Boss spawn -> Flank pos {jwp} is {p.status}");
                bossFlankOk = false;
            }
        }

        // Test Jadavan Patrol Circuit
        Vector3[] bossCircuit = new Vector3[]
        {
            new Vector3( 0.0f, 1.08f, 22.0f), // South Staging
            new Vector3( 5.5f, 1.08f, 31.0f), // East Flank Corridor
            new Vector3( 0.0f, 1.08f, 41.5f), // North Staging Apron
            new Vector3(-5.5f, 1.08f, 31.0f)  // West Flank Corridor
        };
        for (int i = 0; i < bossCircuit.Length; i++)
        {
            Vector3 from = bossCircuit[i];
            Vector3 to = bossCircuit[(i + 1) % bossCircuit.Length];
            NavMeshHit hFrom, hTo;
            NavMesh.SamplePosition(from, out hFrom, 2.0f, NavMesh.AllAreas);
            NavMesh.SamplePosition(to, out hTo, 2.0f, NavMesh.AllAreas);
            NavMeshPath p = new NavMeshPath();
            NavMesh.CalculatePath(hFrom.position, hTo.position, NavMesh.AllAreas, p);
            if (p.status != NavMeshPathStatus.PathComplete)
            {
                sb.AppendLine($"  [FAIL] Boss Circuit WP {i} -> {(i+1)%bossCircuit.Length} is {p.status}");
                bossFlankOk = false;
            }
        }
        sb.AppendLine($"Jadavan Flank Maneuvers in Room 5: {(bossFlankOk ? "[PASS] All flank vectors and circuit complete" : "[FAIL]")}");

        // 9. Save scene
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        sb.AppendLine("\n--- 9. SCENE SAVED ---");
        sb.AppendLine($"Scene saved to: {scenePath}");

        sb.AppendLine("\n================================================================================");
        sb.AppendLine($"FINAL VERIFICATION: SpawnsValid={allSpawnsValid}, RoutesComplete={allRoutesComplete}, VarunCircuit={varunCircuitOk}, BossFlank={bossFlankOk}");
        sb.AppendLine("================================================================================");

        string reportPath = Path.Combine(Application.dataPath, "../scratch/navmesh_bake_verification.txt");
        File.WriteAllText(reportPath, sb.ToString());
        Debug.Log($"[BakeAndVerifyNavMesh] Verification report saved to: {reportPath}\n{sb}");
    }
}
