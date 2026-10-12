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
/// High-fidelity procedural humanoid character builder and mesh generator.
/// Constructs proportioned human anatomy:
/// - Contoured head with cranial dome, cheekbones, tapered jaw, chin, sculpted nose, ears.
/// - Trapezius neck transition smoothly flaring into shoulders.
/// - Anatomically tapered limb lofts (biceps/deltoids, forearms, quads, calves).
/// - Sculpted knee caps and elbow olecranon caps.
/// - Detailed combat boots with instep, toe box, and lugged sole resting flat at floor Y = 0.00m.
/// - Sculpted hands with palm arches, opposable angled thumb, and naturally curled fingers.
/// - Role-specific fitted security equipment and tactical carapace for Boss Jadavan (1.15 scale).
/// - First-person body framing: natural lower-body proportions, clear thighs/knees/boots,
///   zero torso slab or duplicate arm obstruction, and pitch-compensated first-person arms.
/// Serializes every procedural mesh asset permanently to Assets/Meshes/Humanoid/.
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

        // Ground alignment & role scale offsets
        // Ground is at world Y = 0.00. Parent root is at Y = 1.08.
        if (role == CharacterRole.Boss_Jadavan)
        {
            // Boss root scale is 1.15.
            // Local Y offset: -1.08 / 1.15 = -0.93913 aligns visual ground (0,0,0) with floor Y = 0.00!
            float invScale = 1.0f / 1.15f;
            visualRoot.transform.localPosition = new Vector3(0f, -1.08f * invScale, 0f);
            visualRoot.transform.localRotation = Quaternion.identity;
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

        // 1. Pelvis / Hips Node
        // For Player: offset slightly rearward (-0.12m) to reflect eye-forward human posture,
        // preventing the torso from obscuring thighs and boots when looking down.
        GameObject pelvisObj = new GameObject("Pelvis");
        pelvisObj.transform.SetParent(visualRoot.transform, false);
        if (role == CharacterRole.Player)
        {
            pelvisObj.transform.localPosition = new Vector3(0f, 0.94f, -0.12f);
        }
        else
        {
            pelvisObj.transform.localPosition = new Vector3(0f, 0.94f, 0f);
        }

        // Pelvis geometry (hips + duty belt + pouches + radio)
        BuildPelvisGeometry(pelvisObj, primaryUniformMat, role);

        // 2. Spine / Torso Node
        GameObject spineObj = new GameObject("Spine");
        spineObj.transform.SetParent(pelvisObj.transform, false);
        if (role == CharacterRole.Player)
        {
            spineObj.transform.localPosition = new Vector3(0f, 0.14f, -0.02f);
        }
        else
        {
            spineObj.transform.localPosition = new Vector3(0f, 0.18f, 0f);
        }

        // Torso geometry (chest + collar + pockets + badge + vest/armor)
        BuildTorsoGeometry(spineObj, primaryUniformMat, vestMat, role);

        // 3. Neck & Head (Guards only; omitted for player to ensure clear first-person camera view)
        GameObject headObj = null;
        if (role != CharacterRole.Player)
        {
            GameObject neckObj = new GameObject("Neck");
            neckObj.transform.SetParent(spineObj.transform, false);
            neckObj.transform.localPosition = new Vector3(0f, 0.32f, 0f);
            AttachMesh(neckObj, CreateTrapeziusNeckMesh(0.065f, 0.12f, 0.09f, 0.11f), MatSkin);

            headObj = new GameObject("Head");
            headObj.transform.SetParent(neckObj.transform, false);
            headObj.transform.localPosition = new Vector3(0f, 0.10f, 0f);
            BuildHeadGeometry(headObj, role);
        }

        // 4. Arms (For guards only; for player, arms are exclusively rendered via Player_FP_Arms on the camera)
        GameObject upperArmL = null;
        GameObject forearmL = null;
        GameObject upperArmR = null;
        GameObject forearmR = null;

        if (role != CharacterRole.Player)
        {
            // Left Arm
            GameObject shoulderL = new GameObject("Shoulder_L");
            shoulderL.transform.SetParent(spineObj.transform, false);
            shoulderL.transform.localPosition = new Vector3(-0.24f, 0.28f, 0f);

            upperArmL = new GameObject("UpperArm_L");
            upperArmL.transform.SetParent(shoulderL.transform, false);
            BuildLimbSegment(upperArmL, 0.080f, 0.078f, 0.075f, 0.073f, 0.058f, 0.056f, 0.28f, primaryUniformMat);

            GameObject elbowL = new GameObject("ElbowCap_L");
            elbowL.transform.SetParent(upperArmL.transform, false);
            elbowL.transform.localPosition = new Vector3(0f, -0.28f, 0f);
            AttachMesh(elbowL, CreateSculptedElbowMesh(0.060f, 0.065f, 0.028f), primaryUniformMat);

            forearmL = new GameObject("Forearm_L");
            forearmL.transform.SetParent(upperArmL.transform, false);
            forearmL.transform.localPosition = new Vector3(0f, -0.28f, 0f);
            BuildForearmAndHand(forearmL, primaryUniformMat, MatSkin, isLeft: true);

            // Right Arm
            GameObject shoulderR = new GameObject("Shoulder_R");
            shoulderR.transform.SetParent(spineObj.transform, false);
            shoulderR.transform.localPosition = new Vector3(0.24f, 0.28f, 0f);

            upperArmR = new GameObject("UpperArm_R");
            upperArmR.transform.SetParent(shoulderR.transform, false);
            BuildLimbSegment(upperArmR, 0.080f, 0.078f, 0.075f, 0.073f, 0.058f, 0.056f, 0.28f, primaryUniformMat);

            GameObject elbowR = new GameObject("ElbowCap_R");
            elbowR.transform.SetParent(upperArmR.transform, false);
            elbowR.transform.localPosition = new Vector3(0f, -0.28f, 0f);
            AttachMesh(elbowR, CreateSculptedElbowMesh(0.060f, 0.065f, 0.028f), primaryUniformMat);

            forearmR = new GameObject("Forearm_R");
            forearmR.transform.SetParent(upperArmR.transform, false);
            forearmR.transform.localPosition = new Vector3(0f, -0.28f, 0f);
            BuildForearmAndHand(forearmR, primaryUniformMat, MatSkin, isLeft: false);
        }

        // 5. Legs (Grounded, anatomically contoured with knee caps and lugged boots)
        // Left Leg
        GameObject thighL = new GameObject("Thigh_L");
        thighL.transform.SetParent(pelvisObj.transform, false);
        thighL.transform.localPosition = new Vector3(-0.11f, -0.04f, role == CharacterRole.Player ? 0.02f : 0f);
        if (role == CharacterRole.Player)
        {
            thighL.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
        }
        BuildLimbSegment(thighL, 0.096f, 0.092f, 0.104f, 0.100f, 0.076f, 0.076f, 0.42f, MatUniformTrousers);

        GameObject kneeL = new GameObject("KneeCap_L");
        kneeL.transform.SetParent(thighL.transform, false);
        kneeL.transform.localPosition = new Vector3(0f, -0.42f, 0.015f);
        AttachMesh(kneeL, CreateSculptedKneeMesh(0.082f, 0.090f, 0.038f), MatUniformTrousers);

        GameObject shinL = new GameObject("Shin_L");
        shinL.transform.SetParent(thighL.transform, false);
        shinL.transform.localPosition = new Vector3(0f, -0.42f, 0f);
        if (role == CharacterRole.Player)
        {
            shinL.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
        }
        BuildLimbSegment(shinL, 0.078f, 0.078f, 0.088f, 0.094f, 0.066f, 0.066f, 0.38f, MatUniformTrousers);

        GameObject footL = new GameObject("Foot_L");
        footL.transform.SetParent(shinL.transform, false);
        footL.transform.localPosition = new Vector3(0f, -0.38f, 0f);
        BuildBootGeometry(footL);

        // Right Leg
        GameObject thighR = new GameObject("Thigh_R");
        thighR.transform.SetParent(pelvisObj.transform, false);
        thighR.transform.localPosition = new Vector3(0.11f, -0.04f, role == CharacterRole.Player ? 0.02f : 0f);
        if (role == CharacterRole.Player)
        {
            thighR.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
        }
        BuildLimbSegment(thighR, 0.096f, 0.092f, 0.104f, 0.100f, 0.076f, 0.076f, 0.42f, MatUniformTrousers);

        GameObject kneeR = new GameObject("KneeCap_R");
        kneeR.transform.SetParent(thighR.transform, false);
        kneeR.transform.localPosition = new Vector3(0f, -0.42f, 0.015f);
        AttachMesh(kneeR, CreateSculptedKneeMesh(0.082f, 0.090f, 0.038f), MatUniformTrousers);

        GameObject shinR = new GameObject("Shin_R");
        shinR.transform.SetParent(thighR.transform, false);
        shinR.transform.localPosition = new Vector3(0f, -0.42f, 0f);
        if (role == CharacterRole.Player)
        {
            shinR.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
        }
        BuildLimbSegment(shinR, 0.078f, 0.078f, 0.088f, 0.094f, 0.066f, 0.066f, 0.38f, MatUniformTrousers);

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
                cam.nearClipPlane = 0.05f;

                Transform oldFpArms = cam.transform.Find("Player_FP_Arms");
                if (oldFpArms != null)
                    Object.DestroyImmediate(oldFpArms.gameObject);

                fpArmsRoot = new GameObject("Player_FP_Arms");
                fpArmsRoot.transform.SetParent(cam.transform, false);
                fpArmsRoot.transform.localPosition = new Vector3(0f, -0.30f, 0.40f);
                fpArmsRoot.transform.localRotation = Quaternion.Euler(16f, 0f, 0f);

                BuildFirstPersonArms(fpArmsRoot, MatPlayerUniform, MatSkin);
            }
        }

        // 7. Attach and wire HumanoidAnimator
        var animator = visualRoot.AddComponent<HumanoidAnimator>();
        animator.pelvis = pelvisObj.transform;
        animator.spine = spineObj.transform;
        animator.head = headObj != null ? headObj.transform : null;
        animator.upperArmL = upperArmL != null ? upperArmL.transform : null;
        animator.forearmL = forearmL != null ? forearmL.transform : null;
        animator.upperArmR = upperArmR != null ? upperArmR.transform : null;
        animator.forearmR = forearmR != null ? forearmR.transform : null;
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

        // Enforce collider-free visuals
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
        if (role == CharacterRole.Player)
        {
            // Slimmer pelvic profile positioned slightly back so hips don't slab over legs when pitched down
            AttachMesh(pelvisObj, CreateBoxMesh(new Vector3(0.28f, 0.14f, 0.18f), Vector3.zero), MatUniformTrousers);

            GameObject beltObj = new GameObject("DutyBelt");
            beltObj.transform.SetParent(pelvisObj.transform, false);
            beltObj.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            AttachMesh(beltObj, CreateBoxMesh(new Vector3(0.30f, 0.055f, 0.20f), Vector3.zero), MatLeatherBlack);

            GameObject buckleObj = new GameObject("Buckle");
            buckleObj.transform.SetParent(beltObj.transform, false);
            buckleObj.transform.localPosition = new Vector3(0f, 0f, 0.105f);
            AttachMesh(buckleObj, CreateBoxMesh(new Vector3(0.06f, 0.045f, 0.015f), Vector3.zero), MatMetalSilverEquipment);
            return;
        }

        // Guard pelvis
        AttachMesh(pelvisObj, CreateBoxMesh(new Vector3(0.32f, 0.16f, 0.22f), Vector3.zero), MatUniformTrousers);

        // Duty Belt (Leather)
        GameObject belt = new GameObject("DutyBelt");
        belt.transform.SetParent(pelvisObj.transform, false);
        belt.transform.localPosition = new Vector3(0f, 0.06f, 0f);
        AttachMesh(belt, CreateBoxMesh(new Vector3(0.34f, 0.07f, 0.24f), Vector3.zero), MatLeatherBlack);

        // Belt Buckle (Gold/Silver metal)
        GameObject buckle = new GameObject("Buckle");
        buckle.transform.SetParent(belt.transform, false);
        buckle.transform.localPosition = new Vector3(0f, 0f, 0.125f);
        AttachMesh(buckle, CreateBoxMesh(new Vector3(0.07f, 0.055f, 0.015f), Vector3.zero), MatMetalGoldBadge);

        // Radio Holster & Radio (Left hip)
        GameObject radio = new GameObject("SecurityRadio");
        radio.transform.SetParent(belt.transform, false);
        radio.transform.localPosition = new Vector3(-0.175f, 0.02f, 0.02f);
        radio.transform.localRotation = Quaternion.Euler(0f, 20f, 0f);
        AttachMesh(radio, CreateBoxMesh(new Vector3(0.055f, 0.12f, 0.045f), Vector3.zero), MatLeatherBlack);

        GameObject ant = new GameObject("Antenna");
        ant.transform.SetParent(radio.transform, false);
        ant.transform.localPosition = new Vector3(-0.015f, 0.10f, 0f);
        AttachMesh(ant, CreateCylinderMesh(0.005f, 0.007f, 0.10f, 6, Vector3.zero), MatMetalSilverEquipment);

        // Equipment Pouches (Right hip)
        GameObject pouch = new GameObject("Pouches");
        pouch.transform.SetParent(belt.transform, false);
        pouch.transform.localPosition = new Vector3(0.175f, 0f, 0.02f);
        pouch.transform.localRotation = Quaternion.Euler(0f, -20f, 0f);
        AttachMesh(pouch, CreateBoxMesh(new Vector3(0.06f, 0.09f, 0.05f), Vector3.zero), MatLeatherBlack);
    }

    private static void BuildTorsoGeometry(GameObject spineObj, Material uniformMat, Material vestMat, CharacterRole role)
    {
        if (role == CharacterRole.Player)
        {
            // For Player: Slimmer, backward-sloped lower torso that avoids camera near-clip intersection
            // and opens up complete visibility of the waist, thighs, and boots when looking down
            spineObj.transform.localRotation = Quaternion.Euler(-8f, 0f, 0f);
            AttachMesh(spineObj, CreateBoxMesh(new Vector3(0.26f, 0.20f, 0.15f), new Vector3(0f, 0.10f, -0.03f)), uniformMat);
            return;
        }

        // Guard Torso
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

        // Shoulder Epaulettes
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
            GameObject vest = new GameObject("UtilityVest");
            vest.transform.SetParent(spineObj.transform, false);
            vest.transform.localPosition = new Vector3(0f, 0.16f, 0f);
            AttachMesh(vest, CreateBoxMesh(new Vector3(0.40f, 0.31f, 0.26f), Vector3.zero), vestMat);

            GameObject mic = new GameObject("ShoulderMic");
            mic.transform.SetParent(vest.transform, false);
            mic.transform.localPosition = new Vector3(-0.13f, 0.12f, 0.135f);
            AttachMesh(mic, CreateBoxMesh(new Vector3(0.045f, 0.06f, 0.035f), Vector3.zero), MatLeatherBlack);
        }
        else if (role == CharacterRole.Defensive_Aqqil && vestMat != null)
        {
            GameObject armor = new GameObject("DefensiveArmor");
            armor.transform.SetParent(spineObj.transform, false);
            armor.transform.localPosition = new Vector3(0f, 0.17f, 0f);
            AttachMesh(armor, CreateBoxMesh(new Vector3(0.42f, 0.33f, 0.27f), Vector3.zero), vestMat);

            GameObject throatGuard = new GameObject("ThroatGuard");
            throatGuard.transform.SetParent(armor.transform, false);
            throatGuard.transform.localPosition = new Vector3(0f, 0.18f, 0.06f);
            AttachMesh(throatGuard, CreateBoxMesh(new Vector3(0.22f, 0.06f, 0.14f), Vector3.zero), vestMat);
        }
        else if (role == CharacterRole.Boss_Jadavan)
        {
            GameObject bossArmor = new GameObject("TacticalCarapace");
            bossArmor.transform.SetParent(spineObj.transform, false);
            bossArmor.transform.localPosition = new Vector3(0f, 0.17f, 0f);
            AttachMesh(bossArmor, CreateBoxMesh(new Vector3(0.44f, 0.34f, 0.28f), Vector3.zero), MatUniformBossTactical);

            GameObject chevron = new GameObject("CrimsonChevron");
            chevron.transform.SetParent(bossArmor.transform, false);
            chevron.transform.localPosition = new Vector3(0f, 0.04f, 0.145f);
            AttachMesh(chevron, CreateBoxMesh(new Vector3(0.24f, 0.035f, 0.015f), Vector3.zero), MatCrimsonAccent);

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
        // Proportioned head base (jaw, cheekbones, chin, cranium)
        AttachMesh(headObj, CreateContouredHeadMesh(), MatSkin);

        // Facial Features
        // Nose (pyramid profile)
        GameObject noseObj = new GameObject("Nose");
        noseObj.transform.SetParent(headObj.transform, false);
        noseObj.transform.localPosition = new Vector3(0f, 0.04f, 0.105f);
        AttachMesh(noseObj, CreateWedgeMesh(new Vector3(0.035f, 0.05f, 0.035f)), MatSkin);

        // Eyebrows
        GameObject browL = new GameObject("Brow_L");
        browL.transform.SetParent(headObj.transform, false);
        browL.transform.localPosition = new Vector3(-0.045f, 0.08f, 0.095f);
        browL.transform.localRotation = Quaternion.Euler(0f, 0f, 5f);
        AttachMesh(browL, CreateBoxMesh(new Vector3(0.045f, 0.012f, 0.015f), Vector3.zero), MatHairDark);

        GameObject browR = new GameObject("Brow_R");
        browR.transform.SetParent(headObj.transform, false);
        browR.transform.localPosition = new Vector3(0.045f, 0.08f, 0.095f);
        browR.transform.localRotation = Quaternion.Euler(0f, 0f, -5f);
        AttachMesh(browR, CreateBoxMesh(new Vector3(0.045f, 0.012f, 0.015f), Vector3.zero), MatHairDark);

        // Eyes
        GameObject eyeL = new GameObject("Eye_L");
        eyeL.transform.SetParent(headObj.transform, false);
        eyeL.transform.localPosition = new Vector3(-0.045f, 0.055f, 0.094f);
        AttachMesh(eyeL, CreateBoxMesh(new Vector3(0.024f, 0.015f, 0.01f), Vector3.zero), MatHairDark);

        GameObject eyeR = new GameObject("Eye_R");
        eyeR.transform.SetParent(headObj.transform, false);
        eyeR.transform.localPosition = new Vector3(0.045f, 0.055f, 0.094f);
        AttachMesh(eyeR, CreateBoxMesh(new Vector3(0.024f, 0.015f, 0.01f), Vector3.zero), MatHairDark);

        // Ears
        GameObject earL = new GameObject("Ear_L");
        earL.transform.SetParent(headObj.transform, false);
        earL.transform.localPosition = new Vector3(-0.095f, 0.04f, 0f);
        AttachMesh(earL, CreateBoxMesh(new Vector3(0.025f, 0.055f, 0.035f), Vector3.zero), MatSkin);

        GameObject earR = new GameObject("Ear_R");
        earR.transform.SetParent(headObj.transform, false);
        earR.transform.localPosition = new Vector3(0.095f, 0.04f, 0f);
        AttachMesh(earR, CreateBoxMesh(new Vector3(0.025f, 0.055f, 0.035f), Vector3.zero), MatSkin);

        // Headgear / Hair according to role
        if (role == CharacterRole.Patrol_Varun)
        {
            GameObject capObj = new GameObject("PatrolCap");
            capObj.transform.SetParent(headObj.transform, false);
            capObj.transform.localPosition = new Vector3(0f, 0.11f, 0.01f);
            AttachMesh(capObj, CreateBoxMesh(new Vector3(0.22f, 0.08f, 0.23f), Vector3.zero), MatUniformNavy);

            GameObject visor = new GameObject("CapVisor");
            visor.transform.SetParent(capObj.transform, false);
            visor.transform.localPosition = new Vector3(0f, -0.02f, 0.13f);
            visor.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
            AttachMesh(visor, CreateBoxMesh(new Vector3(0.18f, 0.015f, 0.08f), Vector3.zero), MatLeatherBlack);

            GameObject crest = new GameObject("CapCrest");
            crest.transform.SetParent(capObj.transform, false);
            crest.transform.localPosition = new Vector3(0f, 0.02f, 0.12f);
            AttachMesh(crest, CreateBadgeMesh(), MatMetalGoldBadge);
        }
        else if (role == CharacterRole.Search_Sanjeevsaran)
        {
            GameObject hair = new GameObject("ShortHair");
            hair.transform.SetParent(headObj.transform, false);
            hair.transform.localPosition = new Vector3(0f, 0.10f, -0.01f);
            AttachMesh(hair, CreateBoxMesh(new Vector3(0.21f, 0.07f, 0.22f), Vector3.zero), MatHairDark);

            GameObject headset = new GameObject("Headset");
            headset.transform.SetParent(headObj.transform, false);
            headset.transform.localPosition = new Vector3(-0.105f, 0.04f, 0.02f);
            AttachMesh(headset, CreateBoxMesh(new Vector3(0.025f, 0.045f, 0.045f), Vector3.zero), MatLeatherBlack);

            GameObject micBoom = new GameObject("MicBoom");
            micBoom.transform.SetParent(headset.transform, false);
            micBoom.transform.localPosition = new Vector3(0.01f, -0.03f, 0.06f);
            AttachMesh(micBoom, CreateCylinderMesh(0.004f, 0.004f, 0.08f, 6, Vector3.zero), MatMetalSilverEquipment);
        }
        else if (role == CharacterRole.Defensive_Aqqil)
        {
            GameObject helmet = new GameObject("DefensiveHelmet");
            helmet.transform.SetParent(headObj.transform, false);
            helmet.transform.localPosition = new Vector3(0f, 0.10f, 0f);
            AttachMesh(helmet, CreateBoxMesh(new Vector3(0.23f, 0.10f, 0.24f), Vector3.zero), MatUniformVestDefensive);

            GameObject strap = new GameObject("ChinStrap");
            strap.transform.SetParent(headObj.transform, false);
            strap.transform.localPosition = new Vector3(0f, -0.06f, 0.02f);
            AttachMesh(strap, CreateBoxMesh(new Vector3(0.18f, 0.015f, 0.12f), Vector3.zero), MatLeatherBlack);
        }
        else if (role == CharacterRole.Boss_Jadavan)
        {
            GameObject bossHelmet = new GameObject("TacticalCombatHelmet");
            bossHelmet.transform.SetParent(headObj.transform, false);
            bossHelmet.transform.localPosition = new Vector3(0f, 0.11f, 0.01f);
            AttachMesh(bossHelmet, CreateBoxMesh(new Vector3(0.25f, 0.12f, 0.26f), Vector3.zero), MatUniformBossTactical);

            GameObject visor = new GameObject("BallisticVisor");
            visor.transform.SetParent(bossHelmet.transform, false);
            visor.transform.localPosition = new Vector3(0f, -0.04f, 0.135f);
            AttachMesh(visor, CreateBoxMesh(new Vector3(0.20f, 0.045f, 0.025f), Vector3.zero), MatLeatherBlack);

            GameObject browTrim = new GameObject("CrimsonTrim");
            browTrim.transform.SetParent(bossHelmet.transform, false);
            browTrim.transform.localPosition = new Vector3(0f, 0.02f, 0.135f);
            AttachMesh(browTrim, CreateBoxMesh(new Vector3(0.22f, 0.025f, 0.02f), Vector3.zero), MatCrimsonAccent);
        }
    }

    private static void BuildLimbSegment(
        GameObject parent,
        float rTopX, float rTopZ,
        float rMidX, float rMidZ,
        float rBotX, float rBotZ,
        float length, Material mat)
    {
        AttachMesh(parent, CreateTaperedLimbMesh(rTopX, rTopZ, rMidX, rMidZ, rBotX, rBotZ, length, 10, Vector3.zero), mat);
    }

    private static void BuildForearmAndHand(GameObject parent, Material uniformMat, Material skinMat, bool isLeft)
    {
        // Forearm sleeve with anatomical taper
        AttachMesh(parent, CreateTaperedLimbMesh(0.066f, 0.060f, 0.058f, 0.054f, 0.048f, 0.044f, 0.22f, 8, Vector3.zero), uniformMat);

        // Hand
        GameObject hand = new GameObject(isLeft ? "Hand_L" : "Hand_R");
        hand.transform.SetParent(parent.transform, false);
        hand.transform.localPosition = new Vector3(0f, -0.22f, 0f);
        AttachMesh(hand, CreateSculptedHandMesh(isLeft), skinMat);
    }

    private static void BuildBootGeometry(GameObject footObj)
    {
        // Sole rests precisely at local Y = 0.00m relative to ground
        AttachMesh(footObj, CreateSculptedBootMesh(0.11f, 0.23f, 0.12f), MatLeatherBlack);
    }

    private static void BuildFirstPersonArms(GameObject fpRoot, Material uniformMat, Material skinMat)
    {
        // Left FP Arm
        GameObject armL = new GameObject("FP_Arm_L");
        armL.transform.SetParent(fpRoot.transform, false);
        armL.transform.localPosition = new Vector3(-0.25f, -0.05f, 0.12f);
        armL.transform.localRotation = Quaternion.Euler(22f, 16f, -10f);

        AttachMesh(armL, CreateTaperedLimbMesh(0.050f, 0.048f, 0.046f, 0.044f, 0.040f, 0.038f, 0.24f, 8, Vector3.zero), uniformMat);

        GameObject handL = new GameObject("FP_Hand_L");
        handL.transform.SetParent(armL.transform, false);
        handL.transform.localPosition = new Vector3(0f, -0.24f, 0f);
        AttachMesh(handL, CreateSculptedHandMesh(isLeft: true), skinMat);

        // Right FP Arm
        GameObject armR = new GameObject("FP_Arm_R");
        armR.transform.SetParent(fpRoot.transform, false);
        armR.transform.localPosition = new Vector3(0.25f, -0.05f, 0.12f);
        armR.transform.localRotation = Quaternion.Euler(22f, -16f, 10f);

        AttachMesh(armR, CreateTaperedLimbMesh(0.050f, 0.048f, 0.046f, 0.044f, 0.040f, 0.038f, 0.24f, 8, Vector3.zero), uniformMat);

        GameObject handR = new GameObject("FP_Hand_R");
        handR.transform.SetParent(armR.transform, false);
        handR.transform.localPosition = new Vector3(0f, -0.24f, 0f);
        AttachMesh(handR, CreateSculptedHandMesh(isLeft: false), skinMat);
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

    public static Mesh CreateTaperedLimbMesh(
        float rTopX, float rTopZ,
        float rMidX, float rMidZ,
        float rBotX, float rBotZ,
        float length, int segments, Vector3 center)
    {
        string key = $"Taper_{SanitizeFloat(rTopX)}_{SanitizeFloat(rTopZ)}_{SanitizeFloat(rMidX)}_{SanitizeFloat(rMidZ)}_{SanitizeFloat(rBotX)}_{SanitizeFloat(rBotZ)}_{SanitizeFloat(length)}_{segments}_C_{SanitizeFloat(center.x)}_{SanitizeFloat(center.y)}_{SanitizeFloat(center.z)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawTaperedLimbMesh(rTopX, rTopZ, rMidX, rMidZ, rBotX, rBotZ, length, segments, center));
    }

    private static Mesh GenerateRawTaperedLimbMesh(
        float rTopX, float rTopZ,
        float rMidX, float rMidZ,
        float rBotX, float rBotZ,
        float length, int segments, Vector3 center)
    {
        Mesh m = new Mesh { name = "Proc_TaperedLimb" };
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float yTop = 0f;
        float yMid = -length * 0.45f;
        float yBot = -length;

        // Rings: 0 = Top, 1 = Mid, 2 = Bot
        for (int r = 0; r <= 2; r++)
        {
            float y = (r == 0) ? yTop : (r == 1) ? yMid : yBot;
            float rx = (r == 0) ? rTopX : (r == 1) ? rMidX : rBotX;
            float rz = (r == 0) ? rTopZ : (r == 1) ? rMidZ : rBotZ;
            float vCoord = (r == 0) ? 1f : (r == 1) ? 0.5f : 0f;

            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector3 pos = new Vector3(cos * rx, y, sin * rz) + center;
                Vector3 n = new Vector3(cos / rx, 0f, sin / rz).normalized;

                verts.Add(pos);
                norms.Add(n);
                uvs.Add(new Vector2((float)i / segments, vCoord));
            }
        }

        int ringVerts = segments + 1;
        // Quads between Ring 0 and Ring 1
        for (int i = 0; i < segments; i++)
        {
            int t0 = i;
            int t1 = i + 1;
            int b0 = ringVerts + i;
            int b1 = ringVerts + i + 1;

            tris.Add(t0); tris.Add(b0); tris.Add(t1);
            tris.Add(t1); tris.Add(b0); tris.Add(b1);
        }

        // Quads between Ring 1 and Ring 2
        for (int i = 0; i < segments; i++)
        {
            int t0 = ringVerts + i;
            int t1 = ringVerts + i + 1;
            int b0 = ringVerts * 2 + i;
            int b1 = ringVerts * 2 + i + 1;

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

    public static Mesh CreateSculptedKneeMesh(float width, float height, float depth)
    {
        string key = $"Knee_{SanitizeFloat(width)}_{SanitizeFloat(height)}_{SanitizeFloat(depth)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawSculptedKneeMesh(width, height, depth));
    }

    private static Mesh GenerateRawSculptedKneeMesh(float width, float height, float depth)
    {
        Mesh m = new Mesh { name = "Proc_KneeCap" };
        float w2 = width * 0.5f;
        float h2 = height * 0.5f;

        Vector3[] v = new Vector3[]
        {
            new Vector3(0f, 0f, depth),
            new Vector3(0f, h2, depth * 0.3f),
            new Vector3(w2, h2 * 0.6f, depth * 0.2f),
            new Vector3(w2 * 0.8f, -h2 * 0.6f, depth * 0.2f),
            new Vector3(0f, -h2, depth * 0.1f),
            new Vector3(-w2 * 0.8f, -h2 * 0.6f, depth * 0.2f),
            new Vector3(-w2, h2 * 0.6f, depth * 0.2f),
            new Vector3(0f, 0f, 0f)
        };

        int[] tri = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1,
            7, 2, 1,
            7, 3, 2,
            7, 4, 3,
            7, 5, 4,
            7, 6, 5,
            7, 1, 6
        };

        m.vertices = v;
        m.triangles = tri;
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateSculptedElbowMesh(float width, float height, float depth)
    {
        string key = $"Elbow_{SanitizeFloat(width)}_{SanitizeFloat(height)}_{SanitizeFloat(depth)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawSculptedElbowMesh(width, height, depth));
    }

    private static Mesh GenerateRawSculptedElbowMesh(float width, float height, float depth)
    {
        Mesh m = new Mesh { name = "Proc_ElbowCap" };
        float w2 = width * 0.5f;
        float h2 = height * 0.5f;

        Vector3[] v = new Vector3[]
        {
            new Vector3(0f, 0f, -depth),
            new Vector3(0f, h2, 0f),
            new Vector3(w2, 0f, 0f),
            new Vector3(0f, -h2, 0f),
            new Vector3(-w2, 0f, 0f),
            new Vector3(0f, 0f, 0.01f)
        };

        int[] tri = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,
            5, 2, 1,
            5, 3, 2,
            5, 4, 3,
            5, 1, 4
        };

        m.vertices = v;
        m.triangles = tri;
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateSculptedBootMesh(float width, float length, float height)
    {
        string key = $"Boot_{SanitizeFloat(width)}_{SanitizeFloat(length)}_{SanitizeFloat(height)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawSculptedBootMesh(width, length, height));
    }

    private static Mesh GenerateRawSculptedBootMesh(float width, float length, float height)
    {
        Mesh m = new Mesh { name = "Proc_SculptedBoot" };
        float w2 = width * 0.5f;
        float lHeel = -length * 0.35f;
        float lToe = length * 0.65f;
        float soleH = 0.025f;

        Vector3[] v = new Vector3[]
        {
            // Sole bottom (Y = 0)
            new Vector3(-w2, 0f, lHeel), new Vector3(w2, 0f, lHeel),
            new Vector3(w2 * 1.05f, 0f, length * 0.2f), new Vector3(-w2 * 1.05f, 0f, length * 0.2f),
            new Vector3(w2 * 0.85f, 0f, lToe), new Vector3(-w2 * 0.85f, 0f, lToe),

            // Sole top / welt (Y = soleH)
            new Vector3(-w2, soleH, lHeel), new Vector3(w2, soleH, lHeel),
            new Vector3(w2 * 1.05f, soleH, length * 0.2f), new Vector3(-w2 * 1.05f, soleH, length * 0.2f),
            new Vector3(w2 * 0.85f, soleH, lToe), new Vector3(-w2 * 0.85f, soleH, lToe),

            // Instep / Toe box upper
            new Vector3(-w2 * 0.9f, height * 0.45f, 0f), new Vector3(w2 * 0.9f, height * 0.45f, 0f),
            new Vector3(-w2 * 0.8f, height * 0.35f, length * 0.45f), new Vector3(w2 * 0.8f, height * 0.35f, length * 0.45f),
            new Vector3(-w2 * 0.65f, height * 0.22f, lToe - 0.02f), new Vector3(w2 * 0.65f, height * 0.22f, lToe - 0.02f),

            // Ankle collar (Y = height)
            new Vector3(-w2 * 0.8f, height, lHeel * 0.5f), new Vector3(w2 * 0.8f, height, lHeel * 0.5f),
            new Vector3(w2 * 0.8f, height, length * 0.15f), new Vector3(-w2 * 0.8f, height, length * 0.15f),

            // Heel counter
            new Vector3(-w2 * 0.85f, height * 0.5f, lHeel), new Vector3(w2 * 0.85f, height * 0.5f, lHeel)
        };

        List<int> tris = new List<int>();

        // Sole bottom
        tris.AddRange(new int[] { 0, 1, 2, 0, 2, 3, 3, 2, 4, 3, 4, 5 });

        // Sole sides
        tris.AddRange(new int[] { 0, 6, 7, 0, 7, 1 });
        tris.AddRange(new int[] { 1, 7, 8, 1, 8, 2, 2, 8, 10, 2, 10, 4 });
        tris.AddRange(new int[] { 4, 10, 11, 4, 11, 5 });
        tris.AddRange(new int[] { 5, 11, 9, 5, 9, 3, 3, 9, 6, 3, 6, 0 });

        // Vamp & toe box
        tris.AddRange(new int[] { 11, 10, 17, 11, 17, 16 });
        tris.AddRange(new int[] { 16, 17, 15, 16, 15, 14, 14, 15, 13, 14, 13, 12 });
        tris.AddRange(new int[] { 10, 8, 15, 8, 13, 15, 10, 15, 17 });
        tris.AddRange(new int[] { 9, 11, 16, 9, 16, 14, 9, 14, 12 });

        // Ankle shaft & tongue
        tris.AddRange(new int[] { 12, 13, 20, 12, 20, 21 });
        tris.AddRange(new int[] { 13, 8, 23, 13, 23, 20, 20, 23, 19, 8, 7, 23 });
        tris.AddRange(new int[] { 9, 12, 21, 9, 21, 22, 21, 18, 22, 6, 9, 22 });
        tris.AddRange(new int[] { 7, 6, 22, 7, 22, 23, 23, 22, 18, 23, 18, 19 });

        // Collar rim
        tris.AddRange(new int[] { 21, 20, 19, 21, 19, 18 });

        m.vertices = v;
        m.triangles = tris.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateSculptedHandMesh(bool isLeft)
    {
        string key = $"Hand_{(isLeft ? "L" : "R")}";
        return GetOrSaveMeshAsset(key, () => GenerateRawSculptedHandMesh(isLeft));
    }

    private static Mesh GenerateRawSculptedHandMesh(bool isLeft)
    {
        Mesh m = new Mesh { name = isLeft ? "Proc_Hand_L" : "Proc_Hand_R" };
        float sign = isLeft ? -1f : 1f;
        float pw = 0.040f;
        float ph = 0.050f;
        float pd = 0.020f;

        Vector3[] v = new Vector3[]
        {
            // Wrist base (Y = 0)
            new Vector3(-pw, 0f, -pd), new Vector3(pw, 0f, -pd),
            new Vector3(pw, 0f, pd), new Vector3(-pw, 0f, pd),

            // Knuckles (Y = -ph)
            new Vector3(-pw * 0.9f, -ph, -pd * 0.9f), new Vector3(pw * 0.9f, -ph, -pd * 0.9f),
            new Vector3(pw * 0.9f, -ph, pd * 0.9f), new Vector3(-pw * 0.9f, -ph, pd * 0.9f),

            // Curled finger tips
            new Vector3(-pw * 0.8f, -ph * 1.55f, pd * 0.4f), new Vector3(pw * 0.8f, -ph * 1.55f, pd * 0.4f),
            new Vector3(pw * 0.8f, -ph * 1.40f, pd * 1.1f), new Vector3(-pw * 0.8f, -ph * 1.40f, pd * 1.1f),

            // Thumb tip
            new Vector3(sign * pw * 1.55f, -ph * 0.70f, pd * 0.7f),
            new Vector3(sign * pw * 1.40f, -ph * 0.85f, pd * 0.3f)
        };

        List<int> tris = new List<int>();

        tris.AddRange(new int[] { 0, 1, 5, 0, 5, 4 });
        tris.AddRange(new int[] { 2, 3, 7, 2, 7, 6 });
        if (isLeft)
            tris.AddRange(new int[] { 1, 2, 6, 1, 6, 5 });
        else
            tris.AddRange(new int[] { 3, 0, 4, 3, 4, 7 });

        tris.AddRange(new int[] { 4, 5, 9, 4, 9, 8 });
        tris.AddRange(new int[] { 6, 7, 11, 6, 11, 10 });
        tris.AddRange(new int[] { 8, 9, 10, 8, 10, 11 });

        if (isLeft)
        {
            tris.AddRange(new int[] { 0, 3, 12, 3, 7, 12, 7, 4, 13, 4, 0, 13, 12, 13, 4 });
        }
        else
        {
            tris.AddRange(new int[] { 1, 2, 12, 2, 6, 12, 6, 5, 13, 5, 1, 13, 12, 13, 5 });
        }

        m.vertices = v;
        m.triangles = tris.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateContouredHeadMesh()
    {
        return GetOrSaveMeshAsset("Head_ContouredSculpt", GenerateRawContouredHeadMesh);
    }

    private static Mesh GenerateRawContouredHeadMesh()
    {
        Mesh m = new Mesh { name = "Proc_ContouredHead" };
        float wCranium = 0.095f;
        float hCrown = 0.16f;
        float dBack = -0.11f;
        float dFront = 0.09f;
        float wJaw = 0.065f;
        float wChin = 0.038f;
        float yChin = -0.07f;

        Vector3[] v = new Vector3[]
        {
            // Crown Top
            new Vector3(0f, hCrown, -0.01f), // 0
            // Cranium Upper Ring
            new Vector3(-wCranium * 0.85f, hCrown * 0.85f, 0.04f), // 1 Forehead L
            new Vector3(wCranium * 0.85f, hCrown * 0.85f, 0.04f),  // 2 Forehead R
            new Vector3(wCranium, hCrown * 0.80f, -0.04f),         // 3 Temple R
            new Vector3(wCranium * 0.85f, hCrown * 0.80f, dBack),  // 4 Occiput R
            new Vector3(-wCranium * 0.85f, hCrown * 0.80f, dBack), // 5 Occiput L
            new Vector3(-wCranium, hCrown * 0.80f, -0.04f),        // 6 Temple L

            // Brow & Cheek Ring
            new Vector3(-wCranium * 0.85f, 0.06f, dFront),         // 7 Brow L
            new Vector3(wCranium * 0.85f, 0.06f, dFront),          // 8 Brow R
            new Vector3(wCranium * 1.02f, 0.03f, 0f),              // 9 Cheek R
            new Vector3(wCranium * 0.90f, 0.03f, dBack * 0.95f),   // 10 Back R
            new Vector3(-wCranium * 0.90f, 0.03f, dBack * 0.95f),  // 11 Back L
            new Vector3(-wCranium * 1.02f, 0.03f, 0f),             // 12 Cheek L

            // Jaw & Maxilla Ring
            new Vector3(-wJaw, -0.01f, dFront * 0.8f),             // 13 Mouth L
            new Vector3(wJaw, -0.01f, dFront * 0.8f),              // 14 Mouth R
            new Vector3(wJaw * 1.05f, -0.02f, -0.03f),             // 15 Jaw Angle R
            new Vector3(wJaw * 0.90f, -0.02f, dBack * 0.8f),       // 16 Occiput Low R
            new Vector3(-wJaw * 0.90f, -0.02f, dBack * 0.8f),      // 17 Occiput Low L
            new Vector3(-wJaw * 1.05f, -0.02f, -0.03f),            // 18 Jaw Angle L

            // Chin & Jaw Base
            new Vector3(-wChin, yChin, dFront * 0.7f),             // 19 Chin L
            new Vector3(wChin, yChin, dFront * 0.7f),              // 20 Chin R
            new Vector3(0f, yChin - 0.01f, 0f)                     // 21 Under-chin base
        };

        List<int> tris = new List<int>();

        // Crown fan
        tris.AddRange(new int[] {
            0, 1, 2,  0, 2, 3,  0, 3, 4,
            0, 4, 5,  0, 5, 6,  0, 6, 1
        });

        // Upper cranium
        tris.AddRange(new int[] {
            1, 7, 8, 1, 8, 2,
            2, 8, 9, 2, 9, 3,
            3, 9, 10, 3, 10, 4,
            4, 10, 11, 4, 11, 5,
            5, 11, 12, 5, 12, 6,
            6, 12, 7, 6, 7, 1
        });

        // Mid face to jaw
        tris.AddRange(new int[] {
            7, 13, 14, 7, 14, 8,
            8, 14, 15, 8, 15, 9,
            9, 15, 16, 9, 16, 10,
            10, 16, 17, 10, 17, 11,
            11, 17, 18, 11, 18, 12,
            12, 18, 13, 12, 13, 7
        });

        // Chin & under-jaw
        tris.AddRange(new int[] {
            13, 19, 20, 13, 20, 14,
            14, 20, 15, 13, 18, 19,
            19, 21, 20,
            19, 18, 21, 20, 21, 15,
            18, 17, 21, 15, 21, 16,
            17, 16, 21
        });

        m.vertices = v;
        m.triangles = tris.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    public static Mesh CreateTrapeziusNeckMesh(float rTop, float rBotX, float rBotZ, float height)
    {
        string key = $"NeckTrap_{SanitizeFloat(rTop)}_{SanitizeFloat(rBotX)}_{SanitizeFloat(rBotZ)}_{SanitizeFloat(height)}";
        return GetOrSaveMeshAsset(key, () => GenerateRawTrapeziusNeckMesh(rTop, rBotX, rBotZ, height));
    }

    private static Mesh GenerateRawTrapeziusNeckMesh(float rTop, float rBotX, float rBotZ, float height)
    {
        Mesh m = new Mesh { name = "Proc_TrapeziusNeck" };
        int segs = 10;
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for (int i = 0; i <= segs; i++)
        {
            float a = (float)i / segs * Mathf.PI * 2f;
            float cos = Mathf.Cos(a);
            float sin = Mathf.Sin(a);

            verts.Add(new Vector3(cos * rTop, height, sin * rTop));
            norms.Add(new Vector3(cos, 0.2f, sin).normalized);
            uvs.Add(new Vector2((float)i / segs, 1f));

            verts.Add(new Vector3(cos * rBotX, 0f, sin * rBotZ));
            norms.Add(new Vector3(cos, 0.4f, sin).normalized);
            uvs.Add(new Vector2((float)i / segs, 0f));
        }

        for (int i = 0; i < segs; i++)
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
            new Vector3(-x, -y, 0), new Vector3( x, -y, 0),
            new Vector3( x,  y, 0), new Vector3(-x,  y, 0),
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
            0, 1, 2, 0, 2, 4, 4, 2, 3,
            5, 7, 6, 5, 9, 7, 9, 8, 7,
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
