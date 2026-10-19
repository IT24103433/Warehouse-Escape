using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SetupWeaponsAndAccessCards
{
    [MenuItem("Warehouse/Setup Weapons And Access Cards")]
    public static void Execute()
    {
        Debug.Log("[SetupWeaponsAndAccessCards] Starting comprehensive asset & scene setup...");

        // 1. Setup Textures
        SetupTexture("Assets/Textures/Tex_Card_Normal.png", true);
        SetupTexture("Assets/Textures/Tex_Card_A.png", false);
        SetupTexture("Assets/Textures/Tex_Card_B.png", false);
        AssetDatabase.Refresh();

        // 2. Setup Materials
        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLitShader == null) urpLitShader = Shader.Find("Standard");

        var texCardA = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Card_A.png");
        var texCardB = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Card_B.png");
        var texCardNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Card_Normal.png");

        Material matCardA = GetOrCreateMaterial("Assets/Materials/Mat_AccessCard_A.mat", urpLitShader);
        matCardA.SetTexture("_BaseMap", texCardA);
        matCardA.SetTexture("_BumpMap", texCardNormal);
        matCardA.SetFloat("_Smoothness", 0.65f);
        matCardA.SetFloat("_Metallic", 0.15f);
        matCardA.SetColor("_BaseColor", Color.white);
        matCardA.EnableKeyword("_NORMALMAP");
        EditorUtility.SetDirty(matCardA);

        Material matCardB = GetOrCreateMaterial("Assets/Materials/Mat_AccessCard_B.mat", urpLitShader);
        matCardB.SetTexture("_BaseMap", texCardB);
        matCardB.SetTexture("_BumpMap", texCardNormal);
        matCardB.SetFloat("_Smoothness", 0.65f);
        matCardB.SetFloat("_Metallic", 0.15f);
        matCardB.SetColor("_BaseColor", Color.white);
        matCardB.EnableKeyword("_NORMALMAP");
        EditorUtility.SetDirty(matCardB);

        AssetDatabase.SaveAssets();

        // 3. Setup Weapon Prefabs in Resources/Weapons
        string resWeaponsDir = "Assets/Resources/Weapons";
        if (!Directory.Exists(resWeaponsDir)) Directory.CreateDirectory(resWeaponsDir);

        var mp5Asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/MP5_SMG.fbx");
        var umpAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/UMP_SMG.fbx");
        var ak47Asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/AK47_Rifle.fbx");
        var shotgunAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/Combat_Shotgun.fbx");
        var scarAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Weapons/SCAR_Rifle.fbx");

        GameObject mp5Prefab = GetOrCreatePrefab(mp5Asset, "Assets/Resources/Weapons/MP5_SMG.prefab");
        GameObject umpPrefab = GetOrCreatePrefab(umpAsset, "Assets/Resources/Weapons/UMP_SMG.prefab");
        GameObject ak47Prefab = GetOrCreatePrefab(ak47Asset, "Assets/Resources/Weapons/AK47_Rifle.prefab");
        GameObject shotgunPrefab = GetOrCreatePrefab(shotgunAsset, "Assets/Resources/Weapons/Combat_Shotgun.prefab");
        GameObject scarPrefab = GetOrCreatePrefab(scarAsset, "Assets/Resources/Weapons/SCAR_Rifle.prefab");

        AssetDatabase.SaveAssets();

        // 4. Open Scene and Configure Pickups and Guards
        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[SetupWeaponsAndAccessCards] Failed to open {scenePath}");
            return;
        }

        // Find Card FBX
        var cardFbx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/Access_Card.fbx");
        Mesh cardMesh = null;
        if (cardFbx != null)
        {
            var mf = cardFbx.GetComponentInChildren<MeshFilter>();
            if (mf != null) cardMesh = mf.sharedMesh;
        }

        // Configure Card A Pickup (Search Room)
        var cardAObj = GameObject.Find("AccessCard_Pickup_Search");
        if (cardAObj != null)
        {
            if (cardMesh != null)
            {
                var mf = cardAObj.GetComponent<MeshFilter>();
                if (mf == null) mf = cardAObj.AddComponent<MeshFilter>();
                mf.sharedMesh = cardMesh;
            }
            var mr = cardAObj.GetComponent<MeshRenderer>();
            if (mr == null) mr = cardAObj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = matCardA;

            var accessCard = cardAObj.GetComponent<AccessCard>();
            if (accessCard == null) accessCard = cardAObj.AddComponent<AccessCard>();
            accessCard.ConfigureIdentity("Access Card A (Sector 03)", "Sector 03 - Search Logistics", "Level 3");

            var col = cardAObj.GetComponent<BoxCollider>();
            if (col == null) col = cardAObj.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.center = Vector3.zero;
            col.size = new Vector3(1.2f, 1.2f, 1.2f); // 3.0m in world space with 2.5 scale
            cardAObj.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f); // Clearly visible, floating card
            Debug.Log("[SetupWeaponsAndAccessCards] Configured AccessCard_Pickup_Search with Card A mesh & material.");
        }

        // Configure Card B Pickup (Defensive Room)
        var cardBObj = GameObject.Find("AccessCard_Pickup_Defensive");
        if (cardBObj != null)
        {
            if (cardMesh != null)
            {
                var mf = cardBObj.GetComponent<MeshFilter>();
                if (mf == null) mf = cardBObj.AddComponent<MeshFilter>();
                mf.sharedMesh = cardMesh;
            }
            var mr = cardBObj.GetComponent<MeshRenderer>();
            if (mr == null) mr = cardBObj.AddComponent<MeshRenderer>();
            mr.sharedMaterial = matCardB;

            var accessCard = cardBObj.GetComponent<AccessCard>();
            if (accessCard == null) accessCard = cardBObj.AddComponent<AccessCard>();
            accessCard.ConfigureIdentity("Access Card B (Sector 04)", "Sector 04 - Defensive Armory", "Level 3");

            var col = cardBObj.GetComponent<BoxCollider>();
            if (col == null) col = cardBObj.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.center = Vector3.zero;
            col.size = new Vector3(1.2f, 1.2f, 1.2f); // 3.0m in world space with 2.5 scale
            cardBObj.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            Debug.Log("[SetupWeaponsAndAccessCards] Configured AccessCard_Pickup_Defensive with Card B mesh & material.");
        }

        // Configure PlayerGun with MP5 Prefab
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var playerGun = playerObj.GetComponentInChildren<PlayerGun>();
            if (playerGun != null)
            {
                SerializedObject so = new SerializedObject(playerGun);
                var prop = so.FindProperty("weaponPrefab");
                if (prop != null)
                {
                    prop.objectReferenceValue = mp5Prefab;
                    so.ApplyModifiedProperties();
                }
            }
        }

        // Configure Guards
        SetupGuardShooting("Guard_Varun_Patrol", WeaponType.UMP_SMG, 14, 1.1f, 13f, 2, 2.5f, 1, umpPrefab);
        SetupGuardShooting("Guard_Sanjeevsaran_Search", WeaponType.AK47_Rifle, 18, 1.3f, 16f, 3, 2.4f, 1, ak47Prefab);
        SetupGuardShooting("Guard_Aqqil_Defensive", WeaponType.Combat_Shotgun, 7, 1.4f, 10f, 1, 3.8f, 6, shotgunPrefab);
        SetupGuardShooting("Guard_Jadavan_Boss", WeaponType.SCAR_Rifle, 20, 1.1f, 18f, 3, 2.0f, 1, scarPrefab);

        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene);
        Debug.Log($"[SetupWeaponsAndAccessCards] Scene saved cleanly! Saved={saved}");

        File.WriteAllText(Path.Combine(Application.dataPath, "../scratch/setup_weapons_and_cards_done.flag"), "DONE");
    }

    private static void SetupTexture(string path, bool isNormal)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = isNormal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            importer.anisoLevel = 4;
            importer.mipmapEnabled = true;
            importer.SaveAndReimport();
        }
    }

    private static Material GetOrCreateMaterial(string path, Shader shader)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            mat.shader = shader;
        }
        return mat;
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
        return prefab;
    }

    private static void SetupGuardShooting(string guardName, WeaponType type, int dmg, float cooldown, float range, int bursts, float spread, int pellets, GameObject weaponPrefab)
    {
        var guardObj = GameObject.Find(guardName);
        if (guardObj == null) return;

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
    }
}
