using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ApplyInformedGuardSettings
{
    [MenuItem("Warehouse/Apply Informed Guard Settings")]
    public static void Execute()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new System.InvalidOperationException("Apply informed settings in Edit Mode.");
        var search = Object.FindFirstObjectByType<SearchGuardAI>();
        if (search == null) throw new System.InvalidOperationException("Open the Warehouse scene first.");
        search.SearchTimeout = 12f;
        EditorUtility.SetDirty(search);
        EditorSceneManager.MarkSceneDirty(search.gameObject.scene);
        EditorSceneManager.SaveScene(search.gameObject.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[InformedGuardSettings] Saved 12-second search budget for multi-view investigation.");
    }
}
