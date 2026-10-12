using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public enum CharacterRole
{
    Patrol_Varun,
    Search_Sanjeevsaran,
    Defensive_Aqqil,
    Boss_Jadavan,
    Player
}

/// <summary>
/// Procedural mesh and humanoid character generator.
/// Constructs proportioned human anatomy (head/face with ears, nose, eyes, brows, cap/helmet;
/// articulated torso, collar, pockets, badge, duty belt with radio/pouches, limbs, laced boots).
/// Assembles collider-free, nav-excluded visual hierarchies with shared materials and procedural animation.
/// </summary>
public static class HumanoidMeshBuilder
{
    // Reusable Materials Cache
    public static Material MatSkin;
    public static Material MatUniformNavy;
    public static Material MatUniformVestSearch;
    public static Material MatUniformVestDefensive;
    public static Material MatUniformBossTactical;
    public static Material MatUniformTrousers;
    public static Material MatLeatherBlack;
    public static Material MatMetalGoldBadge;
    public static Material MatMetalSilverEquipment;
    public static Material MatHairDark;
    public static Material MatPlayerUniform;
    public static Material MatCrimsonAccent;

    public static void InitializeMaterials()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null) litShader = Shader.Find("Standard");

        MatSkin = CreateOrGetMat("Mat_Human_Skin", litShader, new Color(0.88f, 0.70f, 0.58f), 0.0f, 0.25f);
        MatUniformNavy = CreateOrGetMat("Mat_Uniform_Navy", litShader, new Color(0.10f, 0.15f, 0.25f), 0.05f, 0.22f);
        MatUniformVestSearch = CreateOrGetMat("Mat_Uniform_Vest_Search", litShader, new Color(0.24f, 0.28f, 0.24f), 0.10f, 0.25f);
        MatUniformVestDefensive = CreateOrGetMat("Mat_Uniform_Vest_Defensive", litShader, new Color(0.15f, 0.17f, 0.20f), 0.15f, 0.30f);
        MatUniformBossTactical = CreateOrGetMat("Mat_Uniform_Boss_Tactical", litShader, new Color(0.08f, 0.09f, 0.11f), 0.30f, 0.40f);
        MatUniformTrousers = CreateOrGetMat("Mat_Uniform_Trousers", litShader, new Color(0.11f, 0.13f, 0.16f), 0.05f, 0.20f);
        MatLeatherBlack = CreateOrGetMat("Mat_Leather_Black", litShader, new Color(0.07f, 0.08f, 0.09f), 0.05f, 0.45f);
        MatMetalGoldBadge = CreateOrGetMat("Mat_Metal_Gold_Badge", litShader, new Color(0.92f, 0.76f, 0.26f), 0.85f, 0.75f);
        MatMetalSilverEquipment = CreateOrGetMat("Mat_Metal_Silver_Equipment", litShader, new Color(0.70f, 0.73f, 0.78f), 0.80f, 0.65f);
        MatHairDark = CreateOrGetMat("Mat_Hair_Dark", litShader, new Color(0.12f, 0.10f, 0.09f), 0.0f, 0.15f);
        MatPlayerUniform = CreateOrGetMat("Mat_Player_Uniform", litShader, new Color(0.22f, 0.28f, 0.36f), 0.05f, 0.25f);
        MatCrimsonAccent = CreateOrGetMat("Mat_Crimson_Accent", litShader, new Color(0.68f, 0.12f, 0.15f), 0.25f, 0.45f);
    }

    private static Material CreateOrGetMat(string name, Shader shader, Color color, float metallic, float smoothness)
    {
#if UNITY_EDITOR
        string path = $"Assets/Materials/{name}.mat";
        Material existing = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            existing.shader = shader;
            existing.SetColor("_BaseColor", color);
            existing.SetFloat("_Metallic", metallic);
            existing.SetFloat("_Smoothness", smoothness);
            return existing;
        }
#endif
        Material mat = new Material(shader) { name = name };
        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(mat, $"Assets/Materials/{name}.mat");
#endif
        return mat;
    }

    /// <summary>
    /// Builds the complete humanoid character visual hierarchy for a guard or player.
    /// </summary>
    public static GameObject BuildHumanoidVisual(Transform parentRoot, CharacterRole role)
    {
        InitializeMaterials();

        // Remove old visual child if exists
        Transform oldVisual = parentRoot.Find("Character_Visuals");
        if (oldVisual != null)
            Object.DestroyImmediate(oldVisual.gameObject);

        // Remove old capsule renderers on root
        var rootMr = parentRoot.GetComponent<MeshRenderer>();
        if (rootMr != null) Object.DestroyImmediate(rootMr);
        var rootMf = parentRoot.GetComponent<MeshFilter>();
        if (rootMf != null) Object.DestroyImmediate(rootMf);

        GameObject visualRoot = new GameObject("Character_Visuals");
        visualRoot.transform.SetParent(parentRoot, false);

        // Exclude completely from NavMesh baking
        var navMod = visualRoot.AddComponent<NavMeshModifier>();
        navMod.overrideArea = true;
        navMod.ignoreFromBuild = true;

        // Positioning & scale offsets
        // Ground is at world Y = 0.00.
        // Parent root is at Y = 1.08.
        if (role == CharacterRole.Boss_Jadavan)
        {
            // Boss root scale is 1.15.
            // Local Y offset: -1.08 / 1.15 = -0.93913 aligns visual ground (0,0,0) with floor Y = 0.00!
            float invScale = 1.0f / 1.15f;
            visualRoot.transform.localPosition = new Vector3(0f, -1.08f * invScale, 0f);
            visualRoot.transform.localRotation = Quaternion.identity;
            // Slightly broader, intimidating posture
            visualRoot.transform.localScale = Vector3.one;
        }
        else
        {
            visualRoot.transform.localPosition = new Vector3(0f, -1.08f, 0f);
            visualRoot.transform.localRotation = Quaternion.identity;
            visualRoot.transform.localScale = Vector3.one;
        }

        // Materials setup for role
        Material primaryUniformMat = MatUniformNavy;
        Material vestMat = null;
        if (role == CharacterRole.Patrol_Varun)
        {
            primaryUniformMat = MatUniformNavy;
        }
        else if (role == CharacterRole.Search_Sanjeevsaran)
        {
            primaryUniformMat = MatUniformNavy;
            vestMat = MatUniformVestSearch;
        }
        else if (role == CharacterRole.Defensive_Aqqil)
        {
            primaryUniformMat = MatUniformTrousers;
            vestMat = MatUniformVestDefensive;
        }
        else if (role == CharacterRole.Boss_Jadavan)
        {
            primaryUniformMat = MatUniformBossTactical;
            vestMat = MatUniformBossTactical;
        }
        else if (role == CharacterRole.Player)
        {
            primaryUniformMat = MatPlayerUniform;
            vestMat = MatUniformTrousers;
        }

        // 1. Pelvis / Hips Node (Y = 0.94m)
        GameObject pelvisObj = new GameObject("Pelvis");
        pelvisObj.transform.SetParent(visualRoot.transform, false);
        pelvisObj.transform.localPosition = new Vector3(0f, 0.94f, 0f);

        // Pelvis geometry (hips + duty belt + pouches + radio)
        BuildPelvisGeometry(pelvisObj, primaryUniformMat, role);

        // 2. Spine / Torso Node (Y = 0.18m relative to Pelvis -> Y = 1.12m)
        GameObject spineObj = new GameObject("Spine");
        spineObj.transform.SetParent(pelvisObj.transform, false);
        spineObj.transform.localPosition = new Vector3(0f, 0.18f, 0f);

        // Torso geometry (chest + collar + pockets + badge + vest/armor)
        BuildTorsoGeometry(spineObj, primaryUniformMat, vestMat, role);

        // 3. Neck & Head (Only for guards; player head is omitted for first-person camera clear view)
        GameObject headObj = null;
        if (role != CharacterRole.Player)
        {
            GameObject neckObj = new GameObject("Neck");
            neckObj.transform.SetParent(spineObj.transform, false);
            neckObj.transform.localPosition = new Vector3(0f, 0.34f, 0f);
            AttachMesh(neckObj, CreateCylinderMesh(0.065f, 0.08f, 0.10f, 10, new Vector3(0f, 0.05f, 0f)), MatSkin);

            headObj = new GameObject("Head");
            headObj.transform.SetParent(neckObj.transform, false);
            headObj.transform.localPosition = new Vector3(0f, 0.10f, 0f);
            BuildHeadGeometry(headObj, role);
        }

        // 4. Arms (Shoulders pivot at Y = 0.28m on spine, X = +/- 0.24m)
        // Left Arm
        GameObject shoulderL = new GameObject("Shoulder_L");
        shoulderL.transform.SetParent(spineObj.transform, false);
        shoulderL.transform.localPosition = new Vector3(-0.24f, 0.28f, 0f);

        GameObject upperArmL = new GameObject("UpperArm_L");
        upperArmL.transform.SetParent(shoulderL.transform, false);
        BuildLimbSegment(upperArmL, 0.075f, 0.065f, 0.28f, primaryUniformMat);

        GameObject forearmL = new GameObject("Forearm_L");
        forearmL.transform.SetParent(upperArmL.transform, false);
        forearmL.transform.localPosition = new Vector3(0f, -0.28f, 0f);
        BuildForearmAndHand(forearmL, primaryUniformMat, MatSkin, isLeft: true);

        // Right Arm
        GameObject shoulderR = new GameObject("Shoulder_R");
        shoulderR.transform.SetParent(spineObj.transform, false);
        shoulderR.transform.localPosition = new Vector3(0.24f, 0.28f, 0f);

        GameObject upperArmR = new GameObject("UpperArm_R");
        upperArmR.transform.SetParent(shoulderR.transform, false);
        BuildLimbSegment(upperArmR, 0.075f, 0.065f, 0.28f, primaryUniformMat);

        GameObject forearmR = new GameObject("Forearm_R");
        forearmR.transform.SetParent(upperArmR.transform, false);
        forearmR.transform.localPosition = new Vector3(0f, -0.28f, 0f);
        BuildForearmAndHand(forearmR, primaryUniformMat, MatSkin, isLeft: false);

        // 5. Legs (Hips pivot at Y = -0.04m on pelvis, X = +/- 0.12m)
        // Left Leg
        GameObject thighL = new GameObject("Thigh_L");
        thighL.transform.SetParent(pelvisObj.transform, false);
        thighL.transform.localPosition = new Vector3(-0.12f, -0.04f, 0f);
        BuildLimbSegment(thighL, 0.095f, 0.08f, 0.42f, MatUniformTrousers);

        GameObject shinL = new GameObject("Shin_L");
        shinL.transform.SetParent(thighL.transform, false);
        shinL.transform.localPosition = new Vector3(0f, -0.42f, 0f);
        BuildLimbSegment(shinL, 0.078f, 0.07f, 0.38f, MatUniformTrousers);

        GameObject footL = new GameObject("Foot_L");
        footL.transform.SetParent(shinL.transform, false);
        footL.transform.localPosition = new Vector3(0f, -0.38f, 0f);
        BuildBootGeometry(footL);

        // Right Leg
        GameObject thighR = new GameObject("Thigh_R");
        thighR.transform.SetParent(pelvisObj.transform, false);
        thighR.transform.localPosition = new Vector3(0.12f, -0.04f, 0f);
        BuildLimbSegment(thighR, 0.095f, 0.08f, 0.42f, MatUniformTrousers);

        GameObject shinR = new GameObject("Shin_R");
        shinR.transform.SetParent(thighR.transform, false);
        shinR.transform.localPosition = new Vector3(0f, -0.42f, 0f);
        BuildLimbSegment(shinR, 0.078f, 0.07f, 0.38f, MatUniformTrousers);

        GameObject footR = new GameObject("Foot_R");
        footR.transform.SetParent(shinR.transform, false);
        footR.transform.localPosition = new Vector3(0f, -0.38f, 0f);
        BuildBootGeometry(footR);

        // 6. First-Person Forearms (If Player)
        GameObject fpArmsRoot = null;
        if (role == CharacterRole.Player)
        {
            Camera cam = parentRoot.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                // Ensure camera near clip plane is close enough to prevent any close-up clipping
                cam.nearClipPlane = 0.05f;

                Transform oldFpArms = cam.transform.Find("Player_FP_Arms");
                if (oldFpArms != null)
                    Object.DestroyImmediate(oldFpArms.gameObject);

                fpArmsRoot = new GameObject("Player_FP_Arms");
                fpArmsRoot.transform.SetParent(cam.transform, false);
                fpArmsRoot.transform.localPosition = new Vector3(0f, -0.28f, 0.38f);
                fpArmsRoot.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);

                BuildFirstPersonArms(fpArmsRoot, MatPlayerUniform, MatSkin);
            }
        }

        // 7. Attach and wire HumanoidAnimator
        var animator = visualRoot.AddComponent<HumanoidAnimator>();
        animator.pelvis = pelvisObj.transform;
        animator.spine = spineObj.transform;
        animator.head = headObj != null ? headObj.transform : null;
        animator.upperArmL = upperArmL.transform;
        animator.forearmL = forearmL.transform;
        animator.upperArmR = upperArmR.transform;
        animator.forearmR = forearmR.transform;
        animator.thighL = thighL.transform;
        animator.shinL = shinL.transform;
        animator.footL = footL.transform;
        animator.thighR = thighR.transform;
        animator.shinR = shinR.transform;
        animator.footR = footR.transform;

        if (role == CharacterRole.Player)
        {
            animator.isFirstPersonPlayer = true;
            animator.fpArmsRoot = fpArmsRoot != null ? fpArmsRoot.transform : null;
        }

        // CRITICAL: Strip any colliders from the visual hierarchy
        StripAllColliders(visualRoot);
        if (fpArmsRoot != null) StripAllColliders(fpArmsRoot);

        return visualRoot;
    }

    private static void StripAllColliders(GameObject root)
    {
        foreach (var col in root.GetComponentsInChildren<Collider>(true))
        {
            Object.DestroyImmediate(col);
        }
    }

    // ==========================================
    // ANATOMY GEOMETRY BUILDERS
    // ==========================================

    private static void BuildPelvisGeometry(GameObject pelvisObj, Material uniformMat, CharacterRole role)
    {
        // Lower pelvis / hips
        AttachMesh(pelvisObj, CreateBoxMesh(new Vector3(0.32f, 0.16f, 0.22f), Vector3.zero), MatUniformTrousers);

        // Duty Belt (Leather)
        GameObject beltObj = new GameObject("DutyBelt");
        beltObj.transform.SetParent(pelvisObj.transform, false);
        beltObj.transform.localPosition = new Vector3(0f, 0.06f, 0f);
        AttachMesh(beltObj, CreateBoxMesh(new Vector3(0.34f, 0.07f, 0.24f), Vector3.zero), MatLeatherBlack);

        // Belt Buckle (Gold/Silver metal)
        GameObject buckleObj = new GameObject("Buckle");
        buckleObj.transform.SetParent(beltObj.transform, false);
        buckleObj.transform.localPosition = new Vector3(0f, 0f, 0.125f);
        AttachMesh(buckleObj, CreateBoxMesh(new Vector3(0.07f, 0.055f, 0.015f), Vector3.zero), MatMetalGoldBadge);

        // Radio Holster & Radio (Left hip)
        GameObject radioObj = new GameObject("SecurityRadio");
        radioObj.transform.SetParent(beltObj.transform, false);
        radioObj.transform.localPosition = new Vector3(-0.175f, 0.02f, 0.02f);
        radioObj.transform.localRotation = Quaternion.Euler(0f, 20f, 0f);
        // Radio body
        AttachMesh(radioObj, CreateBoxMesh(new Vector3(0.055f, 0.12f, 0.045f), Vector3.zero), MatLeatherBlack);
        // Antenna
        GameObject antObj = new GameObject("Antenna");
        antObj.transform.SetParent(radioObj.transform, false);
        antObj.transform.localPosition = new Vector3(-0.015f, 0.10f, 0f);
        AttachMesh(antObj, CreateCylinderMesh(0.005f, 0.007f, 0.10f, 6, Vector3.zero), MatMetalSilverEquipment);

        // Equipment Pouches (Right hip)
        GameObject pouchObj = new GameObject("Pouches");
        pouchObj.transform.SetParent(beltObj.transform, false);
        pouchObj.transform.localPosition = new Vector3(0.175f, 0f, 0.02f);
        pouchObj.transform.localRotation = Quaternion.Euler(0f, -20f, 0f);
        AttachMesh(pouchObj, CreateBoxMesh(new Vector3(0.06f, 0.09f, 0.05f), Vector3.zero), MatLeatherBlack);
    }

    private static void BuildTorsoGeometry(GameObject spineObj, Material uniformMat, Material vestMat, CharacterRole role)
    {
        // Main chest & back
        AttachMesh(spineObj, CreateBoxMesh(new Vector3(0.38f, 0.32f, 0.24f), new Vector3(0f, 0.16f, 0f)), uniformMat);

        // Uniform Collar
        GameObject collarObj = new GameObject("Collar");
        collarObj.transform.SetParent(spineObj.transform, false);
        collarObj.transform.localPosition = new Vector3(0f, 0.31f, 0f);
        AttachMesh(collarObj, CreateBoxMesh(new Vector3(0.20f, 0.05f, 0.18f), Vector3.zero), uniformMat);

        // Chest Pockets
        GameObject pocketL = new GameObject("Pocket_L");
        pocketL.transform.SetParent(spineObj.transform, false);
        pocketL.transform.localPosition = new Vector3(-0.10f, 0.20f, 0.122f);
        AttachMesh(pocketL, CreateBoxMesh(new Vector3(0.08f, 0.09f, 0.015f), Vector3.zero), uniformMat);

        GameObject pocketR = new GameObject("Pocket_R");
        pocketR.transform.SetParent(spineObj.transform, false);
        pocketR.transform.localPosition = new Vector3(0.10f, 0.20f, 0.122f);
        AttachMesh(pocketR, CreateBoxMesh(new Vector3(0.08f, 0.09f, 0.015f), Vector3.zero), uniformMat);

        // Security Badge (Left chest above pocket)
        GameObject badgeObj = new GameObject("SecurityBadge");
        badgeObj.transform.SetParent(spineObj.transform, false);
        badgeObj.transform.localPosition = new Vector3(-0.10f, 0.27f, 0.126f);
        AttachMesh(badgeObj, CreateBadgeMesh(), MatMetalGoldBadge);

        // Shoulder Patches / Epaulettes
        GameObject patchL = new GameObject("Patch_L");
        patchL.transform.SetParent(spineObj.transform, false);
        patchL.transform.localPosition = new Vector3(-0.195f, 0.28f, 0f);
        patchL.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        AttachMesh(patchL, CreateBoxMesh(new Vector3(0.05f, 0.01f, 0.07f), Vector3.zero), MatMetalGoldBadge);

        GameObject patchR = new GameObject("Patch_R");
        patchR.transform.SetParent(spineObj.transform, false);
        patchR.transform.localPosition = new Vector3(0.195f, 0.28f, 0f);
        patchR.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
        AttachMesh(patchR, CreateBoxMesh(new Vector3(0.05f, 0.01f, 0.07f), Vector3.zero), MatMetalGoldBadge);

        // Role-Specific Vest & Tactical Armor Overlays
        if (role == CharacterRole.Search_Sanjeevsaran && vestMat != null)
        {
            // Utility search vest with front compartments
            GameObject vest = new GameObject("UtilityVest");
            vest.transform.SetParent(spineObj.transform, false);
            vest.transform.localPosition = new Vector3(0f, 0.16f, 0f);
            AttachMesh(vest, CreateBoxMesh(new Vector3(0.40f, 0.31f, 0.26f), Vector3.zero), vestMat);

            // Shoulder radio mic on left strap
            GameObject mic = new GameObject("ShoulderMic");
            mic.transform.SetParent(vest.transform, false);
            mic.transform.localPosition = new Vector3(-0.13f, 0.12f, 0.135f);
            AttachMesh(mic, CreateBoxMesh(new Vector3(0.045f, 0.06f, 0.035f), Vector3.zero), MatLeatherBlack);
        }
        else if (role == CharacterRole.Defensive_Aqqil && vestMat != null)
        {
            // Protective ballistic armor vest with high neck collar
            GameObject armor = new GameObject("DefensiveArmor");
            armor.transform.SetParent(spineObj.transform, false);
            armor.transform.localPosition = new Vector3(0f, 0.17f, 0f);
            AttachMesh(armor, CreateBoxMesh(new Vector3(0.42f, 0.33f, 0.27f), Vector3.zero), vestMat);

            // Kevlar neck shield
            GameObject throatGuard = new GameObject("ThroatGuard");
            throatGuard.transform.SetParent(armor.transform, false);
            throatGuard.transform.localPosition = new Vector3(0f, 0.18f, 0.06f);
            AttachMesh(throatGuard, CreateBoxMesh(new Vector3(0.22f, 0.06f, 0.14f), Vector3.zero), vestMat);
        }
        else if (role == CharacterRole.Boss_Jadavan)
        {
            // Heavy tactical carapace with reinforced shoulder pauldrons & crimson accents
            GameObject bossArmor = new GameObject("TacticalCarapace");
            bossArmor.transform.SetParent(spineObj.transform, false);
            bossArmor.transform.localPosition = new Vector3(0f, 0.17f, 0f);
            AttachMesh(bossArmor, CreateBoxMesh(new Vector3(0.44f, 0.34f, 0.28f), Vector3.zero), MatUniformBossTactical);

            // Crimson chevron rank stripe across chest
            GameObject chevron = new GameObject("CrimsonChevron");
            chevron.transform.SetParent(bossArmor.transform, false);
            chevron.transform.localPosition = new Vector3(0f, 0.04f, 0.145f);
            AttachMesh(chevron, CreateBoxMesh(new Vector3(0.24f, 0.035f, 0.015f), Vector3.zero), MatCrimsonAccent);

            // Heavy reinforced shoulder pauldrons
            GameObject pauldronL = new GameObject("Pauldron_L");
            pauldronL.transform.SetParent(bossArmor.transform, false);
            pauldronL.transform.localPosition = new Vector3(-0.25f, 0.15f, 0f);
            AttachMesh(pauldronL, CreateBoxMesh(new Vector3(0.12f, 0.08f, 0.20f), Vector3.zero), MatCrimsonAccent);

            GameObject pauldronR = new GameObject("Pauldron_R");
            pauldronR.transform.SetParent(bossArmor.transform, false);
            pauldronR.transform.localPosition = new Vector3(0.25f, 0.15f, 0f);
            AttachMesh(pauldronR, CreateBoxMesh(new Vector3(0.12f, 0.08f, 0.20f), Vector3.zero), MatCrimsonAccent);
        }
    }

    private static void BuildHeadGeometry(GameObject headObj, CharacterRole role)
    {
        // Proportioned head base (jaw, cheeks, cranium)
        AttachMesh(headObj, CreateHeadBaseMesh(), MatSkin);

        // Facial Features
        // Nose (pyramid profile)
        GameObject noseObj = new GameObject("Nose");
        noseObj.transform.SetParent(headObj.transform, false);
        noseObj.transform.localPosition = new Vector3(0f, 0.04f, 0.115f);
        AttachMesh(noseObj, CreateWedgeMesh(new Vector3(0.035f, 0.05f, 0.035f)), MatSkin);

        // Eyebrows
        GameObject browL = new GameObject("Brow_L");
        browL.transform.SetParent(headObj.transform, false);
        browL.transform.localPosition = new Vector3(-0.045f, 0.08f, 0.105f);
        browL.transform.localRotation = Quaternion.Euler(0f, 0f, 5f);
        AttachMesh(browL, CreateBoxMesh(new Vector3(0.045f, 0.012f, 0.015f), Vector3.zero), MatHairDark);

        GameObject browR = new GameObject("Brow_R");
        browR.transform.SetParent(headObj.transform, false);
        browR.transform.localPosition = new Vector3(0.045f, 0.08f, 0.105f);
        browR.transform.localRotation = Quaternion.Euler(0f, 0f, -5f);
        AttachMesh(browR, CreateBoxMesh(new Vector3(0.045f, 0.012f, 0.015f), Vector3.zero), MatHairDark);

        // Eyes (Pupils)
        GameObject eyeL = new GameObject("Eye_L");
        eyeL.transform.SetParent(headObj.transform, false);
        eyeL.transform.localPosition = new Vector3(-0.045f, 0.055f, 0.104f);
        AttachMesh(eyeL, CreateBoxMesh(new Vector3(0.024f, 0.015f, 0.01f), Vector3.zero), MatHairDark);

        GameObject eyeR = new GameObject("Eye_R");
        eyeR.transform.SetParent(headObj.transform, false);
        eyeR.transform.localPosition = new Vector3(0.045f, 0.055f, 0.104f);
        AttachMesh(eyeR, CreateBoxMesh(new Vector3(0.024f, 0.015f, 0.01f), Vector3.zero), MatHairDark);

        // Left & Right Ears
        GameObject earL = new GameObject("Ear_L");
        earL.transform.SetParent(headObj.transform, false);
        earL.transform.localPosition = new Vector3(-0.105f, 0.04f, 0f);
        AttachMesh(earL, CreateBoxMesh(new Vector3(0.025f, 0.055f, 0.035f), Vector3.zero), MatSkin);

        GameObject earR = new GameObject("Ear_R");
        earR.transform.SetParent(headObj.transform, false);
        earR.transform.localPosition = new Vector3(0.105f, 0.04f, 0f);
        AttachMesh(earR, CreateBoxMesh(new Vector3(0.025f, 0.055f, 0.035f), Vector3.zero), MatSkin);

        // Headgear / Hair according to role
        if (role == CharacterRole.Patrol_Varun)
        {
            // Peaked security officer cap with brim and gold insignia crest
            GameObject capObj = new GameObject("PatrolCap");
            capObj.transform.SetParent(headObj.transform, false);
            capObj.transform.localPosition = new Vector3(0f, 0.11f, 0.01f);
            // Cap crown
            AttachMesh(capObj, CreateBoxMesh(new Vector3(0.22f, 0.08f, 0.23f), Vector3.zero), MatUniformNavy);
            // Cap visor / brim
            GameObject visor = new GameObject("CapVisor");
            visor.transform.SetParent(capObj.transform, false);
            visor.transform.localPosition = new Vector3(0f, -0.02f, 0.13f);
            visor.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
            AttachMesh(visor, CreateBoxMesh(new Vector3(0.18f, 0.015f, 0.08f), Vector3.zero), MatLeatherBlack);
            // Gold crest badge on cap front
            GameObject crest = new GameObject("CapCrest");
            crest.transform.SetParent(capObj.transform, false);
            crest.transform.localPosition = new Vector3(0f, 0.02f, 0.12f);
            AttachMesh(crest, CreateBadgeMesh(), MatMetalGoldBadge);
        }
        else if (role == CharacterRole.Search_Sanjeevsaran)
        {
            // Short hair + communication headset
            GameObject hair = new GameObject("ShortHair");
            hair.transform.SetParent(headObj.transform, false);
            hair.transform.localPosition = new Vector3(0f, 0.10f, -0.01f);
            AttachMesh(hair, CreateBoxMesh(new Vector3(0.21f, 0.07f, 0.22f), Vector3.zero), MatHairDark);

            // Tactical communication headset on left ear with mic boom
            GameObject headset = new GameObject("Headset");
            headset.transform.SetParent(headObj.transform, false);
            headset.transform.localPosition = new Vector3(-0.115f, 0.04f, 0.02f);
            AttachMesh(headset, CreateBoxMesh(new Vector3(0.025f, 0.045f, 0.045f), Vector3.zero), MatLeatherBlack);

            GameObject micBoom = new GameObject("MicBoom");
            micBoom.transform.SetParent(headset.transform, false);
            micBoom.transform.localPosition = new Vector3(0.01f, -0.03f, 0.06f);
            AttachMesh(micBoom, CreateCylinderMesh(0.004f, 0.004f, 0.08f, 6, Vector3.zero), MatMetalSilverEquipment);
        }
        else if (role == CharacterRole.Defensive_Aqqil)
        {
            // Protective security tactical cap / low-profile helmet
            GameObject helmet = new GameObject("DefensiveHelmet");
            helmet.transform.SetParent(headObj.transform, false);
            helmet.transform.localPosition = new Vector3(0f, 0.10f, 0f);
            AttachMesh(helmet, CreateBoxMesh(new Vector3(0.23f, 0.10f, 0.24f), Vector3.zero), MatUniformVestDefensive);

            // Chin strap
            GameObject strap = new GameObject("ChinStrap");
            strap.transform.SetParent(headObj.transform, false);
            strap.transform.localPosition = new Vector3(0f, -0.06f, 0.02f);
            AttachMesh(strap, CreateBoxMesh(new Vector3(0.18f, 0.015f, 0.12f), Vector3.zero), MatLeatherBlack);
        }
        else if (role == CharacterRole.Boss_Jadavan)
        {
            // Heavy tactical ballistic combat helmet with dark visor and crimson trim
            GameObject bossHelmet = new GameObject("TacticalCombatHelmet");
            bossHelmet.transform.SetParent(headObj.transform, false);
            bossHelmet.transform.localPosition = new Vector3(0f, 0.11f, 0.01f);
            AttachMesh(bossHelmet, CreateBoxMesh(new Vector3(0.25f, 0.12f, 0.26f), Vector3.zero), MatUniformBossTactical);

            // Dark ballistic eye visor
            GameObject visor = new GameObject("BallisticVisor");
            visor.transform.SetParent(bossHelmet.transform, false);
            visor.transform.localPosition = new Vector3(0f, -0.04f, 0.135f);
            AttachMesh(visor, CreateBoxMesh(new Vector3(0.20f, 0.045f, 0.025f), Vector3.zero), MatLeatherBlack);

            // Crimson tactical brow trim
            GameObject browTrim = new GameObject("CrimsonTrim");
            browTrim.transform.SetParent(bossHelmet.transform, false);
            browTrim.transform.localPosition = new Vector3(0f, 0.02f, 0.135f);
            AttachMesh(browTrim, CreateBoxMesh(new Vector3(0.22f, 0.025f, 0.02f), Vector3.zero), MatCrimsonAccent);
        }
    }

    private static void BuildLimbSegment(GameObject parent, float radiusTop, float radiusBottom, float length, Material mat)
    {
        Vector3 center = new Vector3(0f, -length * 0.5f, 0f);
        AttachMesh(parent, CreateCylinderMesh(radiusTop, radiusBottom, length, 10, center), mat);
    }

    private static void BuildForearmAndHand(GameObject parent, Material uniformMat, Material skinMat, bool isLeft)
    {
        // Forearm sleeve
        AttachMesh(parent, CreateCylinderMesh(0.065f, 0.055f, 0.22f, 10, new Vector3(0f, -0.11f, 0f)), uniformMat);

        // Hand (at Y = -0.25m)
        GameObject hand = new GameObject(isLeft ? "Hand_L" : "Hand_R");
        hand.transform.SetParent(parent.transform, false);
        hand.transform.localPosition = new Vector3(0f, -0.25f, 0f);
        // Palm
        AttachMesh(hand, CreateBoxMesh(new Vector3(0.075f, 0.08f, 0.035f), new Vector3(0f, -0.04f, 0f)), skinMat);
        // Stylized relaxed fingers
        GameObject fingers = new GameObject("Fingers");
        fingers.transform.SetParent(hand.transform, false);
        fingers.transform.localPosition = new Vector3(0f, -0.08f, 0.005f);
        fingers.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
        AttachMesh(fingers, CreateBoxMesh(new Vector3(0.07f, 0.045f, 0.03f), Vector3.zero), skinMat);
    }

    private static void BuildBootGeometry(GameObject footObj)
    {
        // Sole rests exactly at local Y = 0.00 relative to visual root ground!
        // Ankle collar
        AttachMesh(footObj, CreateCylinderMesh(0.068f, 0.072f, 0.10f, 8, new Vector3(0f, 0.05f, 0f)), MatLeatherBlack);

        // Main Foot / Toe Box
        GameObject footBody = new GameObject("BootBody");
        footBody.transform.SetParent(footObj.transform, false);
        footBody.transform.localPosition = new Vector3(0f, 0.04f, 0.06f);
        AttachMesh(footBody, CreateBoxMesh(new Vector3(0.11f, 0.075f, 0.22f), Vector3.zero), MatLeatherBlack);

        // Lugged Heavy Sole (bottom rests at Y = 0.00m)
        GameObject sole = new GameObject("BootSole");
        sole.transform.SetParent(footObj.transform, false);
        sole.transform.localPosition = new Vector3(0f, 0.012f, 0.06f);
        AttachMesh(sole, CreateBoxMesh(new Vector3(0.12f, 0.024f, 0.24f), Vector3.zero), MatLeatherBlack);
    }

    private static void BuildFirstPersonArms(GameObject fpRoot, Material uniformMat, Material skinMat)
    {
        // Left FP Arm
        GameObject armL = new GameObject("FP_Arm_L");
        armL.transform.SetParent(fpRoot.transform, false);
        armL.transform.localPosition = new Vector3(-0.24f, 0f, 0.15f);
        armL.transform.localRotation = Quaternion.Euler(30f, 15f, -10f);

        // Forearm
        AttachMesh(armL, CreateCylinderMesh(0.06f, 0.052f, 0.26f, 10, new Vector3(0f, 0f, -0.13f)), uniformMat);
        // Hand
        GameObject handL = new GameObject("FP_Hand_L");
        handL.transform.SetParent(armL.transform, false);
        handL.transform.localPosition = new Vector3(0f, 0f, 0.02f);
        AttachMesh(handL, CreateBoxMesh(new Vector3(0.07f, 0.065f, 0.09f), Vector3.zero), skinMat);

        // Right FP Arm
        GameObject armR = new GameObject("FP_Arm_R");
        armR.transform.SetParent(fpRoot.transform, false);
        armR.transform.localPosition = new Vector3(0.24f, 0f, 0.15f);
        armR.transform.localRotation = Quaternion.Euler(30f, -15f, 10f);

        // Forearm
        AttachMesh(armR, CreateCylinderMesh(0.06f, 0.052f, 0.26f, 10, new Vector3(0f, 0f, -0.13f)), uniformMat);
        // Hand
        GameObject handR = new GameObject("FP_Hand_R");
        handR.transform.SetParent(armR.transform, false);
        handR.transform.localPosition = new Vector3(0f, 0f, 0.02f);
        AttachMesh(handR, CreateBoxMesh(new Vector3(0.07f, 0.065f, 0.09f), Vector3.zero), skinMat);
    }

    /// <summary>
    /// Runtime safety: Ensures Character_Visuals exists, is active, and all MeshFilters have valid meshes.
    /// If any mesh is missing or null, rebuilds the humanoid visual.
    /// </summary>
    public static void EnsureRuntimeVisuals(Transform parentRoot, CharacterRole role)
    {
        if (parentRoot == null) return;
        Transform visuals = parentRoot.Find("Character_Visuals");
        if (visuals == null)
        {
            BuildHumanoidVisual(parentRoot, role);
            return;
        }

        var mfs = visuals.GetComponentsInChildren<MeshFilter>(true);
        if (mfs == null || mfs.Length == 0)
        {
            BuildHumanoidVisual(parentRoot, role);
            return;
        }

        foreach (var mf in mfs)
        {
            if (mf.sharedMesh == null)
            {
                BuildHumanoidVisual(parentRoot, role);
                return;
            }
        }
    }

    private static void AttachMesh(GameObject go, Mesh mesh, Material mat)
    {
        var mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        var mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
    }

    // ==========================================
    // PROCEDURAL MESH GENERATORS & PERSISTENCE
    // ==========================================

    private static string SanitizeFloat(float v)
    {
        return v.ToString("F3", System.Globalization.CultureInfo.InvariantCulture)
            .Replace('.', '_')
            .Replace('-', 'm');
    }

    private static Mesh GetOrSaveMeshAsset(string assetName, System.Func<Mesh> generate)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Meshes"))
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Meshes");
            if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Meshes/Humanoid"))
                UnityEditor.AssetDatabase.CreateFolder("Assets/Meshes", "Humanoid");

            string path = $"Assets/Meshes/Humanoid/{assetName}.asset";
            Mesh asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (asset != null) return asset;

            Mesh m = generate();
            m.name = assetName;
            UnityEditor.AssetDatabase.CreateAsset(m, path);
            return m;
        }
#endif
        Mesh runMesh = generate();
        runMesh.name = assetName;
        return runMesh;
    }

    public static Mesh CreateBoxMesh(Vector3 size, Vector3 center)
    {
        string key = $"Box_{SanitizeFloat(size.x)}_{SanitizeFloat(size.y)}_{SanitizeFloat(size.z)}_C_{SanitizeFloat(center.x)}_{SanitizeFloat(center.y)}_{SanitizeFloat(center.z)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawBoxMesh(size, center));
    }

    private static Mesh GenerateRawBoxMesh(Vector3 size, Vector3 center)
    {
        Mesh m = new Mesh { name = "Proc_Box" };
        float x = size.x * 0.5f;
        float y = size.y * 0.5f;
        float z = size.z * 0.5f;

        Vector3[] v = new Vector3[]
        {
            // Front
            new Vector3(-x, -y,  z) + center, new Vector3( x, -y,  z) + center,
            new Vector3( x,  y,  z) + center, new Vector3(-x,  y,  z) + center,
            // Back
            new Vector3( x, -y, -z) + center, new Vector3(-x, -y, -z) + center,
            new Vector3(-x,  y, -z) + center, new Vector3( x,  y, -z) + center,
            // Top
            new Vector3(-x,  y,  z) + center, new Vector3( x,  y,  z) + center,
            new Vector3( x,  y, -z) + center, new Vector3(-x,  y, -z) + center,
            // Bottom
            new Vector3(-x, -y, -z) + center, new Vector3( x, -y, -z) + center,
            new Vector3( x, -y,  z) + center, new Vector3(-x, -y,  z) + center,
            // Left
            new Vector3(-x, -y, -z) + center, new Vector3(-x, -y,  z) + center,
            new Vector3(-x,  y,  z) + center, new Vector3(-x,  y, -z) + center,
            // Right
            new Vector3( x, -y,  z) + center, new Vector3( x, -y, -z) + center,
            new Vector3( x,  y, -z) + center, new Vector3( x,  y,  z) + center,
        };

        Vector3[] n = new Vector3[]
        {
            Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
            Vector3.back, Vector3.back, Vector3.back, Vector3.back,
            Vector3.up, Vector3.up, Vector3.up, Vector3.up,
            Vector3.down, Vector3.down, Vector3.down, Vector3.down,
            Vector3.left, Vector3.left, Vector3.left, Vector3.left,
            Vector3.right, Vector3.right, Vector3.right, Vector3.right
        };

        Vector2[] uv = new Vector2[24];
        for (int i = 0; i < 6; i++)
        {
            uv[i * 4 + 0] = new Vector2(0, 0);
            uv[i * 4 + 1] = new Vector2(1, 0);
            uv[i * 4 + 2] = new Vector2(1, 1);
            uv[i * 4 + 3] = new Vector2(0, 1);
        }

        int[] tri = new int[]
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };

        m.vertices = v;
        m.normals = n;
        m.uv = uv;
        m.triangles = tri;
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateCylinderMesh(float radiusTop, float radiusBottom, float height, int segments, Vector3 center)
    {
        string key = $"Cyl_{SanitizeFloat(radiusTop)}_{SanitizeFloat(radiusBottom)}_{SanitizeFloat(height)}_{segments}_C_{SanitizeFloat(center.x)}_{SanitizeFloat(center.y)}_{SanitizeFloat(center.z)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawCylinderMesh(radiusTop, radiusBottom, height, segments, center));
    }

    private static Mesh GenerateRawCylinderMesh(float radiusTop, float radiusBottom, float height, int segments, Vector3 center)
    {
        Mesh m = new Mesh { name = "Proc_Cylinder" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float halfH = height * 0.5f;

        // Side vertices
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector3 topPos = new Vector3(cos * radiusTop, halfH, sin * radiusTop) + center;
            Vector3 btmPos = new Vector3(cos * radiusBottom, -halfH, sin * radiusBottom) + center;
            Vector3 normal = new Vector3(cos, 0f, sin).normalized;

            verts.Add(topPos);
            norms.Add(normal);
            uvs.Add(new Vector2((float)i / segments, 1f));

            verts.Add(btmPos);
            norms.Add(normal);
            uvs.Add(new Vector2((float)i / segments, 0f));
        }

        for (int i = 0; i < segments; i++)
        {
            int t0 = i * 2;
            int b0 = t0 + 1;
            int t1 = t0 + 2;
            int b1 = t0 + 3;

            tris.Add(t0); tris.Add(b0); tris.Add(t1);
            tris.Add(t1); tris.Add(b0); tris.Add(b1);
        }

        m.vertices = verts.ToArray();
        m.normals = norms.ToArray();
        m.uv = uvs.ToArray();
        m.triangles = tris.ToArray();
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateHeadBaseMesh()
    {
        Vector3 size = new Vector3(0.20f, 0.22f, 0.20f);
        return CreateBoxMesh(size, new Vector3(0f, 0.05f, 0f));
    }

    public static Mesh CreateWedgeMesh(Vector3 size)
    {
        string key = $"Wedge_{SanitizeFloat(size.x)}_{SanitizeFloat(size.y)}_{SanitizeFloat(size.z)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawWedgeMesh(size));
    }

    private static Mesh GenerateRawWedgeMesh(Vector3 size)
    {
        Mesh m = new Mesh { name = "Proc_Wedge" };
        float x = size.x * 0.5f;
        float y = size.y * 0.5f;
        float z = size.z;

        Vector3[] v = new Vector3[]
        {
            // Base
            new Vector3(-x, -y, 0), new Vector3( x, -y, 0),
            new Vector3( x,  y, 0), new Vector3(-x,  y, 0),
            // Tip
            new Vector3(0, 0, z)
        };

        int[] tri = new int[]
        {
            0, 1, 2, 0, 2, 3, // Base
            0, 4, 1,          // Bottom
            1, 4, 2,          // Right
            2, 4, 3,          // Top
            3, 4, 0           // Left
        };

        m.vertices = v;
        m.triangles = tri;
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateBadgeMesh()
    {
        return GetOrSaveMeshAsset("Badge_GoldShield", GenerateRawBadgeMesh);
    }

    private static Mesh GenerateRawBadgeMesh()
    {
        Mesh m = new Mesh { name = "Proc_Badge" };
        float w = 0.035f;
        float h = 0.045f;
        float d = 0.01f;

        Vector3[] v = new Vector3[]
        {
            new Vector3(-w,  h, d), new Vector3( w,  h, d),
            new Vector3( w, -h*0.3f, d), new Vector3( 0, -h, d),
            new Vector3(-w, -h*0.3f, d),
            // Back
            new Vector3(-w,  h, 0), new Vector3( w,  h, 0),
            new Vector3( w, -h*0.3f, 0), new Vector3( 0, -h, 0),
            new Vector3(-w, -h*0.3f, 0)
        };

        int[] tri = new int[]
        {
            // Front
            0, 1, 2, 0, 2, 4, 4, 2, 3,
            // Back
            5, 7, 6, 5, 9, 7, 9, 8, 7,
            // Sides
            0, 5, 1, 1, 5, 6,
            1, 6, 2, 2, 6, 7,
            2, 7, 3, 3, 7, 8,
            3, 8, 4, 4, 8, 9,
            4, 9, 0, 0, 9, 5
        };

        m.vertices = v;
        m.triangles = tri;
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }
}
