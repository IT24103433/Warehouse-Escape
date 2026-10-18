using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class SetupDisplayAndHUD
{
    [MenuItem("Warehouse/Setup Display And Save Scene")]
    public static void Execute()
    {
        Debug.Log("[SetupDisplayAndHUD] Configuring Canvas, HUD, and Crosshair display in Warehouse scene...");

        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[SetupDisplayAndHUD] Could not open scene at {scenePath}");
            return;
        }

        var canvasGo = GameObject.Find("UI_Canvas");
        if (canvasGo == null)
        {
            Debug.LogError("[SetupDisplayAndHUD] Could not find UI_Canvas in scene!");
            return;
        }

        // Configure Canvas
        var canvas = canvasGo.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
        }

        // Configure CanvasScaler for 1280x720 native baseline
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Ensure PlayerHUD component is attached directly to UI_Canvas
        var hud = canvasGo.GetComponent<PlayerHUD>();
        if (hud == null)
        {
            hud = canvasGo.AddComponent<PlayerHUD>();
            Debug.Log("[SetupDisplayAndHUD] Added PlayerHUD to UI_Canvas.");
        }

        // Configure Objective_Panel rect transform
        var objPanel = GameObject.Find("Objective_Panel");
        if (objPanel != null)
        {
            var objRt = objPanel.GetComponent<RectTransform>();
            if (objRt != null)
            {
                objRt.anchorMin = new Vector2(0f, 1f);
                objRt.anchorMax = new Vector2(0f, 1f);
                objRt.pivot = new Vector2(0f, 1f);
                objRt.anchoredPosition = new Vector2(24f, -286f);
                objRt.sizeDelta = new Vector2(460f, 84f);
            }
        }

        var hpPanel = GameObject.Find("Health_Status_Panel");
        if (hpPanel != null)
        {
            var hpRt = hpPanel.GetComponent<RectTransform>();
            if (hpRt != null)
            {
                hpRt.anchorMin = new Vector2(0f, 1f);
                hpRt.anchorMax = new Vector2(0f, 1f);
                hpRt.pivot = new Vector2(0f, 1f);
                hpRt.anchoredPosition = new Vector2(24f, -212f);
                hpRt.sizeDelta = new Vector2(360f, 62f);
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("[SetupDisplayAndHUD] Successfully updated and saved Warehouse scene!");
    }
}
