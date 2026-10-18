using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class InspectRoom4
{
    public static void Inspect()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== ROOM 4 & DEFENSIVE INSPECTION ===");

        // Search all GameObjects in scene
        var allGos = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        sb.AppendLine($"Total GameObjects in scene: {allGos.Length}");

        foreach (var go in allGos)
        {
            // Check if in Room 4 area (X > 4, Z between -5 and 16) or has guard/defensive/mannequin/capsule in name
            bool inRoom4Area = go.transform.position.x > 3f && go.transform.position.z > -4f && go.transform.position.z < 18f;
            string lowerName = go.name.ToLower();
            bool isGuardOrMesh = lowerName.Contains("guard") || lowerName.Contains("aqqil") || lowerName.Contains("defens") || 
                                lowerName.Contains("mannequin") || lowerName.Contains("capsule") || lowerName.Contains("human");

            if (inRoom4Area && (go.GetComponent<Renderer>() != null || go.GetComponent<Collider>() != null || isGuardOrMesh))
            {
                // Print only root or notable objects
                sb.AppendLine($"\nGO: '{go.name}' at pos {go.transform.position}, rot {go.transform.eulerAngles}, scale {go.transform.localScale}, parent='{go.transform.parent?.name}', active={go.activeSelf}");
                foreach (var c in go.GetComponents<Component>())
                {
                    if (c == null) continue;
                    sb.Append($"   [{c.GetType().Name}]");
                    if (c is MeshFilter mf && mf.sharedMesh != null) sb.Append($" mesh='{mf.sharedMesh.name}'");
                    if (c is Renderer r && r.sharedMaterial != null) sb.Append($" mat='{r.sharedMaterial.name}' enabled={r.enabled}");
                    if (c is Collider col) sb.Append($" isTrigger={col.isTrigger} enabled={col.enabled}");
                    if (c is DefensiveGuardAI ai) sb.Append($" postPos={ai.PostPosition}");
                }
                sb.AppendLine();
            }
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/room4_deep_inspect.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"[InspectRoom4] Saved deep inspection to {outPath}");
        EditorApplication.Exit(0);
    }
}
