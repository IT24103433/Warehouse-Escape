using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public static class InspectNavMeshDataAsset
{
    [MenuItem("Warehouse/Inspect NavMeshData Asset")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== INSPECT NAVMESHDATA ASSET ===");

        string path = "Assets/Scenes/Warehouse/NavMesh-Warehouse_Environment.asset";
        var asset = AssetDatabase.LoadAssetAtPath<NavMeshData>(path);
        sb.AppendLine($"Asset at {path}: {(asset != null ? "Loaded" : "Null")}");
        if (asset != null)
        {
            sb.AppendLine($"  Name: {asset.name}");
            sb.AppendLine($"  sourceBounds: {asset.sourceBounds}");
            sb.AppendLine($"  position: {asset.position}, rotation: {asset.rotation}");
        }

        // Test adding this asset to NavMesh
        NavMesh.RemoveAllNavMeshData();
        sb.AppendLine($"NavMesh tri before adding: {NavMesh.CalculateTriangulation().vertices.Length}");
        if (asset != null)
        {
            var inst = NavMesh.AddNavMeshData(asset);
            sb.AppendLine($"AddNavMeshData returned valid instance: {inst.valid}");
            var tri = NavMesh.CalculateTriangulation();
            sb.AppendLine($"NavMesh tri after adding asset: vertices={tri.vertices.Length}, indices={tri.indices.Length}");

            // Test sampling each guard position
            Vector3[] guardSpawns = new Vector3[]
            {
                new Vector3(-5.0f, 1.08f, -15.0f),  // Varun Room 2
                new Vector3(-12.0f, 1.08f, 2.0f),   // Sanjeev Room 3
                new Vector3(16.0f, 1.08f, 8.0f),    // Aqqil Room 4
                new Vector3(0.0f, 1.08f, 23.0f)     // Jadavan Room 5
            };
            string[] names = new string[] { "Varun", "Sanjeev", "Aqqil", "Jadavan" };

            for (int i = 0; i < guardSpawns.Length; i++)
            {
                NavMeshHit hit;
                bool hitOk = NavMesh.SamplePosition(guardSpawns[i], out hit, 2.0f, NavMesh.AllAreas);
                sb.AppendLine($"  {names[i]} at {guardSpawns[i]}: SamplePosition hit={hitOk}, pos={(hitOk ? hit.position.ToString() : "none")}, dist={(hitOk ? hit.distance.ToString("F3") : "N/A")}");
            }
            inst.Remove();
        }

        // Now test the surfaces in the scene
        var surfaces = Object.FindObjectsByType<NavMeshSurface>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\nSurfaces count: {surfaces.Length}");
        foreach (var s in surfaces)
        {
            sb.AppendLine($"\nSurface '{s.gameObject.name}':");
            sb.AppendLine($"  CollectObjects: {s.collectObjects}");
            sb.AppendLine($"  UseGeometry: {s.useGeometry}");
            sb.AppendLine($"  LayerMask: {s.layerMask.value}");
            sb.AppendLine($"  AgentTypeID: {s.agentTypeID}");
            sb.AppendLine($"  navMeshData: {(s.navMeshData != null ? s.navMeshData.name : "null")}");

            // Check NavMeshModifier components in scene
            var modifiers = Object.FindObjectsByType<NavMeshModifier>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            sb.AppendLine($"  Modifiers in scene: {modifiers.Length}");
            foreach (var m in modifiers)
            {
                sb.AppendLine($"    Modifier on '{m.gameObject.name}': ignoreFromBuild={m.ignoreFromBuild}, overrideArea={m.overrideArea}, area={m.area}");
            }
            break; // only print modifiers once
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/inspect_navmesh_data.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"Report written to {outPath}");
    }
}
