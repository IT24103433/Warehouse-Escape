using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class InspectGuardPositions
{
    [MenuItem("Tools/Inspect Guard Positions")]
    public static void Execute()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== INSPECT GUARD POSITIONS & VISUALS ===");

        string[] guardNames = new string[]
        {
            "Guard_Varun_Patrol",
            "Guard_Sanjeevsaran_Search",
            "Guard_Aqqil_Defensive",
            "Guard_Jadavan_Boss"
        };

        foreach (var name in guardNames)
        {
            var go = GameObject.Find(name);
            if (go == null)
            {
                sb.AppendLine($"[MISSING] {name} NOT FOUND!");
                continue;
            }

            sb.AppendLine($"\nGuard: {name}");
            sb.AppendLine($"  ActiveSelf: {go.activeSelf}, ActiveInHierarchy: {go.activeInHierarchy}, Layer: {go.layer}");
            sb.AppendLine($"  Position: {go.transform.position:F3}, Rotation: {go.transform.rotation.eulerAngles:F1}");

            var visuals = go.transform.Find("Character_Visuals");
            if (visuals == null)
            {
                sb.AppendLine("  [NO VISUALS] Child 'Character_Visuals' not found!");
            }
            else
            {
                sb.AppendLine($"  Character_Visuals: Active: {visuals.gameObject.activeSelf}, Layer: {visuals.gameObject.layer}");
                var renderers = visuals.GetComponentsInChildren<Renderer>(true);
                sb.AppendLine($"  Total Renderers under visuals: {renderers.Length}");
                foreach (var r in renderers)
                {
                    sb.AppendLine($"    - {r.gameObject.name}: Type={r.GetType().Name}, Enabled={r.enabled}, Layer={r.gameObject.layer}, Mat={r.sharedMaterial?.name}, Bounds={r.bounds}");
                }
            }
        }

        sb.AppendLine("\n=== INSPECT UI_CANVAS & CAMERA ===");
        var canvasGo = GameObject.Find("UI_Canvas");
        if (canvasGo != null)
        {
            sb.AppendLine($"UI_Canvas: ActiveSelf={canvasGo.activeSelf}, ActiveInHierarchy={canvasGo.activeInHierarchy}, Layer={canvasGo.layer}");
            var c = canvasGo.GetComponent<Canvas>();
            if (c != null)
            {
                sb.AppendLine($"  Canvas: RenderMode={c.renderMode}, SortingOrder={c.sortingOrder}, PlaneDist={c.planeDistance}");
            }
            var p = canvasGo.GetComponent<PlayerHUD>();
            sb.AppendLine($"  PlayerHUD component on UI_Canvas: {(p != null ? "Present" : "Missing")}");

            // List all children
            foreach (Transform child in canvasGo.transform)
            {
                sb.AppendLine($"  Child: {child.name}, Active={child.gameObject.activeSelf}, Layer={child.gameObject.layer}, Pos={child.localPosition}");
                foreach (Transform sub in child)
                {
                    sb.AppendLine($"    - SubChild: {sub.name}, Active={sub.gameObject.activeSelf}, Layer={sub.gameObject.layer}");
                }
            }
        }
        else
        {
            sb.AppendLine("UI_Canvas NOT FOUND!");
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var cam = playerObj.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                sb.AppendLine($"Player Camera: Near={cam.nearClipPlane}, Far={cam.farClipPlane}, FOV={cam.fieldOfView}, CullingMask={cam.cullingMask}");
            }
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/guards_inspect_output.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"[InspectGuardPositions] Output written to {outPath}");
    }
}
