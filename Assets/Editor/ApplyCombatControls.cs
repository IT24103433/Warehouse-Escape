using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class ApplyCombatControls
{
    public static void Execute()
    {
        System.IO.Directory.CreateDirectory("Assets/Resources/Effects");
        AssetDatabase.Refresh();
        if (AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Effects/BloodParticles.mat") == null)
            AssetDatabase.CreateAsset(new Material(Shader.Find("Warehouse/BloodParticles")), "Assets/Resources/Effects/BloodParticles.mat");
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        foreach (var health in Object.FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (health.IsPlayer || health.CompareTag("Player")) continue;
            int target = health.GetComponent<FlankingGuardAI>() != null ? 360 :
                health.GetComponent<DefensiveGuardAI>() != null ? 220 :
                health.GetComponent<SearchGuardAI>() != null ? 180 : 120;
            health.SetMaxHealth(target);
            EditorUtility.SetDirty(health);
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[CombatControls] Saved guard health 120/180/220/360; player unchanged.");
    }
}
