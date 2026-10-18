using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class InspectCanvasTree
{
    public static void Execute()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Debug.Log($"[InspectCanvasTree] Found {canvases.Length} Canvases in scene.");

        foreach (var c in canvases)
        {
            Debug.Log($"=== CANVAS: {c.gameObject.name} (RenderMode: {c.renderMode}, SortingOrder: {c.sortingOrder}) ===");
            PrintHierarchy(c.transform, 0);
        }

        // Also check if any duplicate PlayerHUD or UI components exist
        var huds = Object.FindObjectsByType<PlayerHUD>(FindObjectsSortMode.None);
        Debug.Log($"[InspectCanvasTree] Found {huds.Length} PlayerHUD instances in scene.");
        foreach (var h in huds)
        {
            Debug.Log($"  PlayerHUD on: {h.gameObject.name} (Parent: {h.transform.parent?.name})");
        }
    }

    static void PrintHierarchy(Transform tf, int indent)
    {
        string ind = new string(' ', indent * 2);
        var rt = tf.GetComponent<RectTransform>();
        string rectStr = rt != null ? $"pos={rt.anchoredPosition}, size={rt.sizeDelta}, pivot={rt.pivot}, anchorMin={rt.anchorMin}, anchorMax={rt.anchorMax}" : "no RectTransform";
        var txt = tf.GetComponent<Text>();
        string txtStr = txt != null ? $" [Text: \"{txt.text.Replace("\n", "\\n")}\", size={txt.fontSize}, color={txt.color}]" : "";
        var img = tf.GetComponent<Image>();
        string imgStr = img != null ? $" [Image: color={img.color}]" : "";

        Debug.Log($"{ind}- {tf.gameObject.name}: {rectStr}{txtStr}{imgStr}");

        for (int i = 0; i < tf.childCount; i++)
        {
            PrintHierarchy(tf.GetChild(i), indent + 1);
        }
    }
}
