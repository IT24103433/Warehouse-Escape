using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AssignWeaponPrefabsAndSaveScene
{
    [MenuItem("Warehouse/Assign Weapon Prefabs And Save")]
    public static void Execute()
    {
        Debug.Log("[AssignWeaponPrefabsAndSaveScene] Starting weapon and access card prefab assignment pass...");
        SetupWeaponsAndAccessCards.Execute();

        // 1. Ensure Resources/Weapons folder exists and contains prefabs
        string resWeaponsDir = "Assets/Resources/Weapons";
        if (!Directory.Exists(resWeaponsDir))
        {
            Directory.CreateDirectory(resWeaponsDir);
            AssetDatabase.Refresh();
        }

        var mp5Asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/MP5_SMG.fbx");
        var umpAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/UMP_SMG.fbx");
        var ak47Asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/AK47_Rifle.fbx");
        var shotgunAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/Combat_Shotgun.fbx");
        var scarAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/SCAR_Rifle.fbx");

        if (mp5Asset == null || umpAsset == null || ak47Asset == null || shotgunAsset == null || scarAsset == null)
        {
            Debug.LogError($"[AssignWeaponPrefabsAndSaveScene] Missing FBX assets: MP5={mp5Asset != null}, UMP={umpAsset != null}, AK={ak47Asset != null}, Shotgun={shotgunAsset != null}, SCAR={scarAsset != null}");
            return;
        }

        GameObject mp5Prefab = GetOrCreatePrefab(mp5Asset, "Assets/Resources/Weapons/MP5_SMG.prefab");
        GameObject umpPrefab = GetOrCreatePrefab(umpAsset, "Assets/Resources/Weapons/UMP_SMG.prefab");
        GameObject ak47Prefab = GetOrCreatePrefab(ak47Asset, "Assets/Resources/Weapons/AK47_Rifle.prefab");
        GameObject shotgunPrefab = GetOrCreatePrefab(shotgunAsset, "Assets/Resources/Weapons/Combat_Shotgun.prefab");
        GameObject scarPrefab = GetOrCreatePrefab(scarAsset, "Assets/Resources/Weapons/SCAR_Rifle.prefab");

        AssetDatabase.SaveAssets();

        // 2. Open Warehouse Scene
        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[AssignWeaponPrefabsAndSaveScene] Failed to open {scenePath}");
            return;
        }

        // 3. Assign Player Weapon Prefab (MP5)
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Camera playerCam = playerObj.GetComponentInChildren<Camera>();
            if (playerCam != null)
            {
                var playerGun = playerCam.GetComponent<PlayerGun>();
                if (playerGun == null) playerGun = playerCam.gameObject.AddComponent<PlayerGun>();

                SerializedObject so = new SerializedObject(playerGun);
                var prop = so.FindProperty("weaponPrefab");
                if (prop != null)
                {
                    prop.objectReferenceValue = mp5Prefab;
                    so.ApplyModifiedProperties();
                    Debug.Log("[AssignWeaponPrefabsAndSaveScene] Serialized weaponPrefab on PlayerGun assigned to MP5_SMG.");
                }
            }

            // Rebuild Player first-person body & arms
            HumanoidMeshBuilder.BuildHumanoidVisual(playerObj.transform, CharacterRole.Player);
        }

        // 4. Assign Guard Weapon Prefabs & Rebuild Characters with Improved Faces and Face Lighting
        SetupGuard("Guard_Varun_Patrol", CharacterRole.Patrol_Varun, WeaponType.UMP_SMG, 14, 1.1f, 13f, 2, 2.5f, 1, umpPrefab);
        SetupGuard("Guard_Sanjeevsaran_Search", CharacterRole.Search_Sanjeevsaran, WeaponType.AK47_Rifle, 18, 1.3f, 16f, 3, 2.4f, 1, ak47Prefab);
        SetupGuard("Guard_Aqqil_Defensive", CharacterRole.Defensive_Aqqil, WeaponType.Combat_Shotgun, 7, 1.4f, 10f, 1, 3.8f, 6, shotgunPrefab);
        SetupGuard("Guard_Jadavan_Boss", CharacterRole.Boss_Jadavan, WeaponType.SCAR_Rifle, 20, 1.1f, 18f, 3, 2.0f, 1, scarPrefab);

        // 5. Mark Scene Dirty and Save
        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene);
        Debug.Log($"[AssignWeaponPrefabsAndSaveScene] Warehouse scene saved cleanly! Saved={saved}");

        string reportPath = Path.Combine(Application.dataPath, "../scratch/weapon_setup_done.txt");
        File.WriteAllText(reportPath, $"WEAPON_SETUP_COMPLETED at {System.DateTime.Now}\nMP5: {mp5Prefab.name}\nUMP: {umpPrefab.name}\nAK: {ak47Prefab.name}\nShotgun: {shotgunPrefab.name}\nSCAR: {scarPrefab.name}\nSceneSaved: {saved}");
    }

    private static GameObject GetOrCreatePrefab(GameObject asset, string prefabPath)
    {
        if (File.Exists(prefabPath))
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
        Debug.Log($"[AssignWeaponPrefabsAndSaveScene] Saved fresh prefab at {prefabPath}");
        return prefab;
    }

    private static void SetupGuard(string guardName, CharacterRole role, WeaponType type, int dmg, float cooldown, float range, int bursts, float spread, int pellets, GameObject weaponPrefab)
    {
        var guardObj = GameObject.Find(guardName);
        if (guardObj == null)
        {
            Debug.LogWarning($"[AssignWeaponPrefabsAndSaveScene] Guard '{guardName}' not found in scene!");
            return;
        }

        // Rebuild Character Visuals with new face lighting, skin shading, and distinct traits
        HumanoidMeshBuilder.BuildHumanoidVisual(guardObj.transform, role);

        var shooting = guardObj.GetComponent<GuardShooting>();
        if (shooting == null) shooting = guardObj.AddComponent<GuardShooting>();

        SerializedObject so = new SerializedObject(shooting);
        var typeProp = so.FindProperty("assignedWeapon");
        if (typeProp != null) typeProp.enumValueIndex = (int)type;

        var dmgProp = so.FindProperty("damagePerShot");
        if (dmgProp != null) dmgProp.intValue = dmg;

        var cdProp = so.FindProperty("fireCooldown");
        if (cdProp != null) cdProp.floatValue = cooldown;

        var rangeProp = so.FindProperty("maxFiringRange");
        if (rangeProp != null) rangeProp.floatValue = range;

        var burstsProp = so.FindProperty("burstCount");
        if (burstsProp != null) burstsProp.intValue = bursts;

        var spreadProp = so.FindProperty("accuracySpread");
        if (spreadProp != null) spreadProp.floatValue = spread;

        var pelletProp = so.FindProperty("pelletCount");
        if (pelletProp != null) pelletProp.intValue = pellets;

        var prefabProp = so.FindProperty("weaponPrefab");
        if (prefabProp != null) prefabProp.objectReferenceValue = weaponPrefab;

        so.ApplyModifiedProperties();
        Debug.Log($"[AssignWeaponPrefabsAndSaveScene] Guard '{guardName}' ({type}) configured with serialized {weaponPrefab.name}.");
    }
}
