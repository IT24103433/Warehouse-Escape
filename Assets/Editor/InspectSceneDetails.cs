using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class InspectSceneDetails
{
    [MenuItem("Warehouse/Inspect Scene Details")]
    public static void Execute()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== SCENE INSPECTION REPORT ===");

        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);

        // 1. Player Inspection
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            sb.AppendLine($"\n[PLAYER] Name: {player.name}, Layer: {LayerMask.LayerToName(player.layer)} ({player.layer}), Pos: {player.transform.position:F2}");
            var cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                sb.AppendLine($"  Camera: {cam.gameObject.name}, Layer: {LayerMask.LayerToName(cam.gameObject.layer)}, CullingMask: {cam.cullingMask:X}, Near: {cam.nearClipPlane}, Far: {cam.farClipPlane}");
            }
            sb.AppendLine("  Components on Player root:");
            foreach (var comp in player.GetComponents<Component>())
            {
                sb.AppendLine($"    - {comp.GetType().Name}");
            }
            sb.AppendLine("  Children of Player:");
            foreach (Transform child in player.transform)
            {
                sb.AppendLine($"    + {child.name} (Layer: {LayerMask.LayerToName(child.gameObject.layer)}, Pos: {child.localPosition:F3}, Active: {child.gameObject.activeSelf})");
                foreach (Transform sub in child)
                {
                    sb.AppendLine($"        * {sub.name} (Layer: {LayerMask.LayerToName(sub.gameObject.layer)}, Pos: {sub.localPosition:F3}, Active: {sub.gameObject.activeSelf})");
                    var mf = sub.GetComponent<MeshFilter>();
                    var mr = sub.GetComponent<MeshRenderer>();
                    sb.AppendLine($"          Mesh: {(mf != null && mf.sharedMesh != null ? mf.sharedMesh.name : "null")}, Renderer: {(mr != null ? mr.enabled.ToString() : "none")}");
                }
                var cmf = child.GetComponent<MeshFilter>();
                var cmr = child.GetComponent<MeshRenderer>();
                if (cmf != null || cmr != null)
                {
                    sb.AppendLine($"      Mesh: {(cmf != null && cmf.sharedMesh != null ? cmf.sharedMesh.name : "null")}, Renderer: {(cmr != null ? cmr.enabled.ToString() : "none")}");
                }
            }
        }
        else
        {
            sb.AppendLine("\n[PLAYER] NOT FOUND!");
        }

        // 2. Guards Inspection
        string[] guardNames = new string[] { "Guard_Varun_Patrol", "Guard_Sanjeevsaran_Search", "Guard_Aqqil_Defensive", "Guard_Jadavan_Boss" };
        foreach (var gn in guardNames)
        {
            var g = GameObject.Find(gn);
            if (g != null)
            {
                sb.AppendLine($"\n[GUARD] {gn} Pos: {g.transform.position:F2}, Fwd: {g.transform.forward:F2}, Layer: {LayerMask.LayerToName(g.layer)} ({g.layer}), Active: {g.activeSelf}");
                sb.AppendLine("  Components:");
                foreach (var c in g.GetComponents<Component>())
                {
                    sb.AppendLine($"    - {c.GetType().Name}");
                }
                sb.AppendLine("  Children:");
                foreach (Transform child in g.transform)
                {
                    var mr = child.GetComponent<MeshRenderer>();
                    var mf = child.GetComponent<MeshFilter>();
                    sb.AppendLine($"    + {child.name} (Active: {child.gameObject.activeSelf}, Layer: {child.gameObject.layer}, Mesh: {(mf != null && mf.sharedMesh != null ? mf.sharedMesh.name : "null")}, Renderer: {(mr != null ? mr.enabled.ToString() : "none")})");
                }
            }
            else
            {
                sb.AppendLine($"\n[GUARD] {gn} NOT FOUND!");
            }
        }

        // 3. Canvas & HUD
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            sb.AppendLine($"\n[CANVAS] Name: {canvas.name}, RenderMode: {canvas.renderMode}, Layer: {canvas.gameObject.layer}");
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/scene_inspection_report.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"Scene inspection written to {outPath}");
    }
}
