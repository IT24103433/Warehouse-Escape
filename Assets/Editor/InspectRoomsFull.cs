using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class InspectRoomsFull
{
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== ROOMS FULL INSPECTION ===");

        var roomsGo = GameObject.Find("Rooms");
        if (roomsGo != null)
        {
            foreach (Transform r in roomsGo.transform)
            {
                sb.AppendLine($"\nROOM: {r.name} at {r.position}");
                DumpHierarchy(r, sb, 1);
            }
        }
        else
        {
            sb.AppendLine("Rooms parent not found! Checking root objects:");
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name.Contains("Room"))
                {
                    sb.AppendLine($"Root Room: {go.name}");
                    DumpHierarchy(go.transform, sb, 1);
                }
            }
        }

        // Also check Waypoints
        var wpRoot = GameObject.Find("[Waypoints]");
        if (wpRoot != null)
        {
            sb.AppendLine("\n=== WAYPOINTS ROOT ===");
            DumpHierarchy(wpRoot.transform, sb, 1);
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/rooms_full_inspect.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"[InspectRoomsFull] Wrote to {outPath}");
        EditorApplication.Exit(0);
    }

    private static void DumpHierarchy(Transform t, StringBuilder sb, int depth)
    {
        string indent = new string(' ', depth * 2);
        for (int i = 0; i < t.childCount; i++)
        {
            var c = t.GetChild(i);
            var col = c.GetComponent<Collider>();
            string colInfo = col != null ? $" [Col bounds={col.bounds.min:F1}..{col.bounds.max:F1}, isTrigger={col.isTrigger}]" : "";
            sb.AppendLine($"{indent}+ {c.name} pos={c.position:F2}, rot={c.eulerAngles:F1}, scale={c.lossyScale:F2}{colInfo}");
            if (c.childCount > 0 && depth < 3)
            {
                DumpHierarchy(c, sb, depth + 1);
            }
        }
    }
}
