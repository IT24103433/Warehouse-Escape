using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public static class DeepDiagnoseNavMesh
{
    [MenuItem("Warehouse/Deep Diagnose")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== DEEP DIAGNOSE NAVMESH ===");

        var go1 = GameObject.Find("Warehouse_Environment");
        var go2 = GameObject.Find("NavMesh Surface");

        sb.AppendLine($"\nGO 1: 'Warehouse_Environment' found: {go1 != null}");
        if (go1 != null)
        {
            var s1 = go1.GetComponent<NavMeshSurface>();
            sb.AppendLine($"  NavMeshSurface: {s1 != null}");
            if (s1 != null)
            {
                sb.AppendLine($"    navMeshData: {s1.navMeshData != null}");
                if (s1.navMeshData != null)
                {
                    sb.AppendLine($"      name: '{s1.navMeshData.name}', sourceBounds: {s1.navMeshData.sourceBounds}");
                    sb.AppendLine($"      assetPath: '{AssetDatabase.GetAssetPath(s1.navMeshData)}'");
                    sb.AppendLine($"      isPersistent: {EditorUtility.IsPersistent(s1.navMeshData)}");
                }
            }
        }

        sb.AppendLine($"\nGO 2: 'NavMesh Surface' found: {go2 != null}");
        if (go2 != null)
        {
            var s2 = go2.GetComponent<NavMeshSurface>();
            sb.AppendLine($"  NavMeshSurface: {s2 != null}");
            if (s2 != null)
            {
                sb.AppendLine($"    navMeshData: {s2.navMeshData != null}");
                if (s2.navMeshData != null)
                {
                    sb.AppendLine($"      name: '{s2.navMeshData.name}', sourceBounds: {s2.navMeshData.sourceBounds}");
                    sb.AppendLine($"      assetPath: '{AssetDatabase.GetAssetPath(s2.navMeshData)}'");
                    sb.AppendLine($"      isPersistent: {EditorUtility.IsPersistent(s2.navMeshData)}");
                }
            }
        }

        // Check file on disk: Assets/Scenes/Warehouse/NavMesh-Warehouse_Environment.asset
        var diskAsset = AssetDatabase.LoadAssetAtPath<NavMeshData>("Assets/Scenes/Warehouse/NavMesh-Warehouse_Environment.asset");
        sb.AppendLine($"\nDisk Asset: {diskAsset != null}");
        if (diskAsset != null)
        {
            sb.AppendLine($"  Name: '{diskAsset.name}', sourceBounds: {diskAsset.sourceBounds}");
            sb.AppendLine($"  Matches GO1 navMeshData: {go1 != null && go1.GetComponent<NavMeshSurface>()?.navMeshData == diskAsset}");
            sb.AppendLine($"  Matches GO2 navMeshData: {go2 != null && go2.GetComponent<NavMeshSurface>()?.navMeshData == diskAsset}");
        }

        // Check guard spawn positions against both bounds
        Vector3[] guardSpawns = new Vector3[]
        {
            new Vector3(-5.0f, 1.08f, -15.0f),  // Varun Room 2
            new Vector3(-12.0f, 1.08f, 2.0f),   // Sanjeev Room 3
            new Vector3(16.0f, 1.08f, 8.0f),    // Aqqil Room 4
            new Vector3(0.0f, 1.08f, 23.0f)     // Jadavan Room 5
        };

        sb.AppendLine("\nGuard Spawns check:");
        for (int i = 0; i < guardSpawns.Length; i++)
        {
            var pos = guardSpawns[i];
            sb.AppendLine($"  Guard {i} pos: {pos}");
            if (diskAsset != null)
            {
                sb.AppendLine($"    Inside diskAsset bounds: {diskAsset.sourceBounds.Contains(pos)}");
            }
        }

        // Check NavMesh areas
        sb.AppendLine("\nNavMesh.GetAreaFromName:");
        string[] areaNames = new string[] { "Walkable", "Not Walkable", "Jump" };
        foreach (var an in areaNames)
        {
            sb.AppendLine($"  Area '{an}': {NavMesh.GetAreaFromName(an)}, Cost: {NavMesh.GetAreaCost(NavMesh.GetAreaFromName(an))}");
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/deep_diagnose.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"[DeepDiagnoseNavMesh] Written to {outPath}");
    }
}
