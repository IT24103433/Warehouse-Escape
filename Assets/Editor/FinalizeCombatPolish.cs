using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FinalizeCombatPolish
{
    [MenuItem("Warehouse/Finalize Combat Difficulty")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        Apply("Guard_Varun_Patrol", typeof(PatrolGuardAI), 60, 16, 90, 12, 1.35f, 13, 2, 3.0f, .45f);
        Apply("Guard_Sanjeevsaran_Search", typeof(SearchGuardAI), 90, 18, 100, 16, 1.1f, 17, 3, 2.4f, .32f);
        Apply("Guard_Aqqil_Defensive", typeof(DefensiveGuardAI), 110, 18, 100, 7, 1.5f, 11, 1, 3.8f, .30f);
        Apply("Guard_Jadavan_Boss", typeof(FlankingGuardAI), 180, 22, 110, 18, .85f, 21, 4, 1.8f, .22f);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[FinalCombatPolish] Saved difficulty progression: 60 / 90 / 110 / 180 HP.");
    }

    private static void Apply(string name, System.Type controllerType, int hp, float sight, float fov,
        int damage, float cooldown, float range, int burst, float spread, float reaction)
    {
        var guard = GameObject.Find(name);
        if (guard == null) throw new System.InvalidOperationException("Missing guard: " + name);
        var health = guard.GetComponent<Health>() ?? guard.AddComponent<Health>();
        health.CharacterName = name.Replace("Guard_", "").Replace('_', ' ');
        health.SetMaxHealth(hp);
        EditorUtility.SetDirty(health);
        Set(guard.GetComponent(controllerType), "sightDistance", sight);
        Set(guard.GetComponent(controllerType), "fieldOfView", fov);
        var memory = guard.GetComponent<GuardPerceptionMemory>() ?? guard.AddComponent<GuardPerceptionMemory>();
        Set(memory, "sightDistance", sight);
        Set(memory, "fieldOfView", fov);
        var shooting = guard.GetComponent<GuardShooting>() ?? guard.AddComponent<GuardShooting>();
        var weapon = controllerType == typeof(PatrolGuardAI) ? WeaponType.UMP_SMG
            : controllerType == typeof(SearchGuardAI) ? WeaponType.AK47_Rifle
            : controllerType == typeof(DefensiveGuardAI) ? WeaponType.Combat_Shotgun : WeaponType.SCAR_Rifle;
        shooting.ConfigureWeapon(weapon, damage, cooldown, range, burst, spread,
            weapon == WeaponType.Combat_Shotgun ? 6 : 1);
        Set(shooting, "reactionDelay", reaction);
        EditorUtility.SetDirty(shooting);
    }

    private static void Set(Object target, string field, float value)
    {
        if (target == null) throw new System.InvalidOperationException("Missing component: " + field);
        var so = new SerializedObject(target);
        so.FindProperty(field).floatValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
