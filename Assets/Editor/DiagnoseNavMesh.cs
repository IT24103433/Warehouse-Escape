using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public static class DiagnoseNavMesh
{
    [MenuItem("Warehouse/Diagnose NavMesh")]
    public static void Execute()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== DIAGNOSE NAVMESH REPORT ===");
        sb.AppendLine($"Timestamp: {DateTime.Now}");

        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        sb.AppendLine($"Opened Scene: {scene.path} (valid={scene.IsValid()}, isLoaded={scene.isLoaded})");

        // 1. Check NavMesh settings & Agent Types
        int settingsCount = NavMesh.GetSettingsCount();
        sb.AppendLine($"\nNavMesh Settings Count: {settingsCount}");
        for (int i = 0; i < settingsCount; i++)
        {
            var settings = NavMesh.GetSettingsByIndex(i);
            sb.AppendLine($"  Index {i}: ID={settings.agentTypeID}, Name='{NavMesh.GetSettingsNameFromID(settings.agentTypeID)}', Radius={settings.agentRadius}, Height={settings.agentHeight}");
        }

        // 2. Check NavMeshSurfaces in scene
        var surfaces = UnityEngine.Object.FindObjectsByType<NavMeshSurface>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\nNavMeshSurface Count: {surfaces.Length}");
        foreach (var s in surfaces)
        {
            sb.AppendLine($"\nSurface on GameObject: '{s.gameObject.name}', ActiveSelf={s.gameObject.activeSelf}, ActiveInHierarchy={s.gameObject.activeInHierarchy}");
            sb.AppendLine($"  AgentTypeID: {s.agentTypeID} ({NavMesh.GetSettingsNameFromID(s.agentTypeID)})");
            sb.AppendLine($"  CollectObjects: {s.collectObjects}");
            sb.AppendLine($"  UseGeometry: {s.useGeometry}");
            sb.AppendLine($"  DefaultArea: {s.defaultArea}");
            sb.AppendLine($"  LayerMask: {s.layerMask.value}");
            sb.AppendLine($"  NavMeshData: {(s.navMeshData != null ? $"{s.navMeshData.name} (InstanceID={s.navMeshData.GetInstanceID()}, AssetPath={AssetDatabase.GetAssetPath(s.navMeshData)})" : "NULL")}");

            var baker = s.GetComponent<NavMeshRuntimeBaker>();
            sb.AppendLine($"  NavMeshRuntimeBaker on same GO: {(baker != null ? "Present" : "None")}");
        }

        // 3. Check NavMeshRuntimeBaker components anywhere in scene
        var bakers = UnityEngine.Object.FindObjectsByType<NavMeshRuntimeBaker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\nTotal NavMeshRuntimeBaker components: {bakers.Length}");
        foreach (var b in bakers)
        {
            sb.AppendLine($"  Baker on: '{b.gameObject.name}', ActiveSelf={b.gameObject.activeSelf}, ActiveInHierarchy={b.gameObject.activeInHierarchy}");
        }

        // 4. Check existing NavMeshData assets in Project
        string[] guids = AssetDatabase.FindAssets("t:NavMeshData");
        sb.AppendLine($"\nProject NavMeshData Assets Count: {guids.Length}");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<NavMeshData>(path);
            sb.AppendLine($"  Asset: '{path}', GUID={guid}, Name='{asset?.name}', Bounds={asset?.sourceBounds}");
        }

        // 5. Check Guards and NavMeshAgents in scene
        var agents = UnityEngine.Object.FindObjectsByType<NavMeshAgent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\nNavMeshAgent Count: {agents.Length}");
        foreach (var a in agents)
        {
            sb.AppendLine($"\nAgent on GameObject: '{a.gameObject.name}', ActiveSelf={a.gameObject.activeSelf}, ActiveInHierarchy={a.gameObject.activeInHierarchy}");
            sb.AppendLine($"  Position: {a.transform.position}");
            sb.AppendLine($"  AgentTypeID: {a.agentTypeID} ({NavMesh.GetSettingsNameFromID(a.agentTypeID)})");
            sb.AppendLine($"  BaseOffset: {a.baseOffset}, Radius: {a.radius}, Height: {a.height}");
            sb.AppendLine($"  IsOnNavMesh (Editor Mode): {a.isOnNavMesh}");

            // Try sampling NavMesh near this agent
            NavMeshHit hit;
            bool sampled = NavMesh.SamplePosition(a.transform.position, out hit, 5.0f, NavMesh.AllAreas);
            sb.AppendLine($"  SamplePosition (radius 5.0): Hit={sampled}, HitPos={(sampled ? hit.position.ToString() : "N/A")}, Dist={(sampled ? hit.distance.ToString("F3") : "N/A")}, Mask={(sampled ? hit.mask.ToString() : "N/A")}");
        }

        // 6. Check NavMeshTriangulation (is any NavMesh currently registered in Editor?)
        var tri = NavMesh.CalculateTriangulation();
        sb.AppendLine($"\nCurrent NavMesh Triangulation: Vertices={tri.vertices.Length}, Indices={tri.indices.Length}, Areas={tri.areas.Length}");

        string reportPath = Path.Combine(Application.dataPath, "../scratch/diagnose_navmesh_report.txt");
        File.WriteAllText(reportPath, sb.ToString());
        Debug.Log($"[DiagnoseNavMesh] Report written to: {reportPath}\n{sb}");
    }
}
