using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneInspector
{
    public static void Inspect()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== SCENE INSPECTOR REPORT ===");

        // Player
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            sb.AppendLine($"\nPLAYER: {player.name} at {player.transform.position}, lossyScale: {player.transform.lossyScale}");
            foreach (var comp in player.GetComponents<Component>())
            {
                sb.AppendLine($"  Component: {comp.GetType().Name}");
            }
            sb.AppendLine("  Children:");
            for (int i = 0; i < player.transform.childCount; i++)
            {
                var child = player.transform.GetChild(i);
                sb.AppendLine($"    - {child.name} at localPos {child.localPosition}, localScale {child.localScale}");
                foreach (var cComp in child.GetComponents<Component>())
                {
                    sb.AppendLine($"        ChildComp: {cComp.GetType().Name}");
                }
            }
        }
        else
        {
            sb.AppendLine("PLAYER NOT FOUND BY TAG!");
        }

        // Guards
        var guards = Object.FindObjectsByType<GuardCapture>(FindObjectsSortMode.None);
        sb.AppendLine($"\nGUARDS COUNT: {guards.Length}");
        foreach (var g in guards)
        {
            var go = g.gameObject;
            sb.AppendLine($"\nGUARD: {go.name} at {go.transform.position}, rot {go.transform.eulerAngles}, lossyScale {go.transform.lossyScale}");
            sb.AppendLine($"  GuardCapture: DisplayName='{g.GuardDisplayName}', Range={g.CaptureRange}, EyeHeight={g.EyeHeight}");
            foreach (var comp in go.GetComponents<Component>())
            {
                sb.AppendLine($"  Component: {comp.GetType().Name}");
            }
            sb.AppendLine("  Children:");
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                sb.AppendLine($"    - {child.name} (active={child.gameObject.activeSelf}) at localPos {child.localPosition}, localScale {child.localScale}");
                foreach (var cComp in child.GetComponents<Component>())
                {
                    sb.AppendLine($"        ChildComp: {cComp.GetType().Name}");
                }
            }
        }

        // Signs
        string[] signNames = new string[]
        {
            "Sign_ToStage1", "Sign_Route_A", "Sign_Route_B", "Sign_Room4_W", "Sign_Room4_E", "Sign_Exit_Master"
        };
        sb.AppendLine("\nSIGNS:");
        foreach (var sName in signNames)
        {
            var s = GameObject.Find(sName);
            if (s != null)
            {
                var board = s.transform.Find("SignBoard");
                var tm = s.transform.Find("Sign_TextMesh");
                TextMesh textMesh = tm != null ? tm.GetComponent<TextMesh>() : null;
                sb.AppendLine($"  Sign {sName}: pos={s.transform.position}, parent={s.transform.parent?.name}, boardScale={(board != null ? board.localScale.ToString() : "none")}, text='{textMesh?.text.Replace('\n', ' ')}', tmScale={(tm != null ? tm.localScale.ToString() : "none")}, fontSize={textMesh?.fontSize}");
            }
            else
            {
                sb.AppendLine($"  Sign {sName}: NOT FOUND");
            }
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/scene_inspect.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"[SceneInspector] Report written to: {outPath}");
        EditorApplication.Exit(0);
    }
}
