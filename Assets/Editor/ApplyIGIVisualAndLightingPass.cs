using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class ApplyIGIVisualAndLightingPass
{
    [MenuItem("Warehouse/Apply IGI Visual and Lighting Pass")]
    public static void Execute()
    {
        Debug.Log("[ApplyIGIVisualAndLightingPass] Starting IGI Military-Industrial visual pass...");

        // 1. Open Warehouse scene
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError("[ApplyIGIVisualAndLightingPass] Failed to open Warehouse scene!");
            return;
        }

        // 2. Update Materials to IGI Military-Industrial Palette
        UpdateMaterials();

        // 3. Optimize Lights & Eliminate 36 Shadow Maps contention
        ConfigureLighting();

        // 4. Add Ceiling Fixtures & Emergency Bulkheads
        CreateCeilingFixturesAndEmergencyLights();

        // 5. Setup Static Flags for Baking, Light Probes & Reflection Probes
        SetupProbesAndStaticFlags();

        // 6. Setup Post Processing Global Volume
        SetupPostProcessingVolume();

        // 7. Ensure Camera Post Processing is Enabled
        EnsureCameraPostProcessing();

        // 8. Bake Static Lighting, Light Probes, and Reflection Probes
        BakeLighting();

        // 9. Mark Dirty and Save Scene
        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        string reportPath = Path.Combine(Application.dataPath, "../scratch/igi_pass_result.txt");
        File.WriteAllText(reportPath, $"IGI_PASS_COMPLETED at {System.DateTime.Now}\nSaved: {saved}\n");
        Debug.Log($"[ApplyIGIVisualAndLightingPass] Completed and saved cleanly! Saved={saved}");

        // 10. Automatically run full inspection to produce fresh lights_materials_report.txt
        InspectLightsAndMaterials.Execute();
    }

    private static void UpdateMaterials()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        Texture2D texWall = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wall_Panels.png");
        Texture2D texWallNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wall_Panels_Normal.png");
        Texture2D texFloor = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Concrete_Floor.png");
        Texture2D texFloorNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Concrete_Floor_Normal.png");
        Texture2D texMetal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Metal_Plate.png");
        Texture2D texMetalNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Metal_Plate_Normal.png");
        Texture2D texWood = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wood_Crate.png");
        Texture2D texWoodNorm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Wood_Crate_Normal.png");

        // Walls: Muted reinforced industrial concrete
        UpdateMat("Mat_Wall", new Color(0.48f, 0.50f, 0.52f, 1f), 0.05f, 0.18f, texWall, texWallNorm, new Vector2(6, 4));

        // Floor: Weathered warehouse concrete slab
        UpdateMat("Mat_Floor", new Color(0.44f, 0.46f, 0.48f, 1f), 0.08f, 0.22f, texFloor, texFloorNorm, new Vector2(8, 12));

        // Ceiling: Dark industrial corrugated steel
        UpdateMat("Mat_Ceiling", new Color(0.14f, 0.15f, 0.17f, 1f), 0.45f, 0.22f, null, null, Vector2.one);

        // Steel Racks: Muted olive-amber safety steel (replaces bright toy orange)
        UpdateMat("Mat_Rack_Orange", new Color(0.50f, 0.35f, 0.16f, 1f), 0.70f, 0.35f, texMetal, texMetalNorm, new Vector2(2, 4));

        // Steel Racks: Dark battleship grey enameled steel (replaces bright toy blue)
        UpdateMat("Mat_Rack_Blue", new Color(0.24f, 0.27f, 0.30f, 1f), 0.65f, 0.35f, texMetal, texMetalNorm, new Vector2(2, 4));

        // Dark Structural Steel
        UpdateMat("Mat_Steel_Dark", new Color(0.20f, 0.22f, 0.24f, 1f), 0.85f, 0.42f, texMetal, texMetalNorm, new Vector2(2, 2));

        // Shutters & HVAC Ducts
        UpdateMat("Mat_Shutter_Metal", new Color(0.35f, 0.37f, 0.40f, 1f), 0.75f, 0.35f, texMetal, texMetalNorm, new Vector2(4, 4));
        UpdateMat("Mat_Duct_Metal", new Color(0.42f, 0.45f, 0.48f, 1f), 0.80f, 0.42f, texMetal, texMetalNorm, new Vector2(3, 3));

        // Crates & Pallets
        UpdateMat("Mat_Crate_Wood", new Color(0.45f, 0.40f, 0.33f, 1f), 0.0f, 0.15f, texWood, texWoodNorm, Vector2.one);
        UpdateMat("Mat_Crate_Dark", new Color(0.25f, 0.27f, 0.24f, 1f), 0.20f, 0.25f, texMetal, texMetalNorm, Vector2.one);
        UpdateMat("Mat_Pallet", new Color(0.46f, 0.42f, 0.35f, 1f), 0.0f, 0.14f, texWood, texWoodNorm, Vector2.one);

        // Barrels & Equipment
        UpdateMat("Mat_Barrel_Hazard", new Color(0.62f, 0.48f, 0.12f, 1f), 0.55f, 0.38f, null, null, Vector2.one);
        UpdateMat("Mat_Barrel_Blue", new Color(0.18f, 0.22f, 0.28f, 1f), 0.60f, 0.40f, null, null, Vector2.one);
        UpdateMat("Mat_Forklift_Yellow", new Color(0.65f, 0.50f, 0.12f, 1f), 0.45f, 0.38f, null, null, Vector2.one);

        // Weathered Stripes
        UpdateMat("Mat_Floor_Stripe_Yellow", new Color(0.70f, 0.58f, 0.14f, 1f), 0.05f, 0.18f, null, null, Vector2.one);
        UpdateMat("Mat_Floor_Stripe_White", new Color(0.60f, 0.62f, 0.64f, 1f), 0.05f, 0.18f, null, null, Vector2.one);

        // Tactical Uniforms (Muted Military Aesthetics)
        UpdateMat("Mat_Guard_Varun", new Color(0.18f, 0.21f, 0.24f, 1f), 0.05f, 0.20f, null, null, Vector2.one); // Urban patrol slate
        UpdateMat("Mat_Guard_Sanjeevsaran", new Color(0.22f, 0.26f, 0.20f, 1f), 0.05f, 0.20f, null, null, Vector2.one); // Olive drab search
        UpdateMat("Mat_Guard_Aqqil", new Color(0.15f, 0.16f, 0.18f, 1f), 0.15f, 0.25f, null, null, Vector2.one); // Night ops charcoal
        UpdateMat("Mat_Guard_Jadavan", new Color(0.10f, 0.11f, 0.12f, 1f), 0.30f, 0.35f, null, null, Vector2.one); // Elite black command
        UpdateMat("Mat_Guard_Boss", new Color(0.10f, 0.11f, 0.12f, 1f), 0.30f, 0.35f, null, null, Vector2.one);

        // Weapons: Dark blackened steel
        UpdateMat("Mat_Tactical_Pistol", new Color(0.14f, 0.15f, 0.16f, 1f), 0.85f, 0.45f, null, null, Vector2.one);
        UpdateMat("Mat_Security_Rifle", new Color(0.12f, 0.13f, 0.14f, 1f), 0.85f, 0.40f, null, null, Vector2.one);
    }

    private static void UpdateMat(string matName, Color color, float metallic, float smoothness, Texture2D albedo, Texture2D normal, Vector2 tiling)
    {
        string path = $"Assets/Materials/{matName}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null) return;

        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);

        if (albedo != null)
        {
            mat.SetTexture("_BaseMap", albedo);
            mat.SetTextureScale("_BaseMap", tiling);
        }
        if (normal != null)
        {
            mat.EnableKeyword("_NORMALMAP");
            mat.SetTexture("_BumpMap", normal);
            mat.SetTextureScale("_BumpMap", tiling);
            mat.SetFloat("_BumpScale", 1.0f);
        }

        EditorUtility.SetDirty(mat);
    }

    private static void ConfigureLighting()
    {
        Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // 1. Disable shadows on ALL existing lights first (wipes out the 36 shadow map warning)
        foreach (var l in allLights)
        {
            l.shadows = LightShadows.None;
            if (l.type == LightType.Point)
            {
                // Soft warm/cool ambient fill without shadow-map overhead
                l.intensity = Mathf.Min(l.intensity, 1.4f);
            }
        }

        // 2. Setup Warehouse_DirectionalFill as the single authoritative Real-Time Shadow Caster
        GameObject dirObj = GameObject.Find("Warehouse_DirectionalFill");
        if (dirObj == null)
        {
            dirObj = new GameObject("Warehouse_DirectionalFill");
            dirObj.transform.position = Vector3.zero;
        }
        dirObj.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        Light dirLight = dirObj.GetComponent<Light>();
        if (dirLight == null) dirLight = dirObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = new Color(0.72f, 0.76f, 0.84f, 1f); // Cool military skylight/ambient
        dirLight.intensity = 0.65f;
        dirLight.shadows = LightShadows.Soft; // 1 shadow map face with cascades
        dirLight.shadowBias = 0.05f;
        dirLight.shadowNormalBias = 0.4f;

        // 3. Setup Room 2 Central Surveillance Spotlight with real-time shadow
        GameObject spotR2 = GameObject.Find("Security_Spotlight_Room2");
        if (spotR2 == null)
        {
            spotR2 = new GameObject("Security_Spotlight_Room2");
        }
        spotR2.transform.position = new Vector3(0f, 4.4f, -9f);
        spotR2.transform.rotation = Quaternion.Euler(75f, 0f, 0f);
        Light lightR2 = spotR2.GetComponent<Light>();
        if (lightR2 == null) lightR2 = spotR2.AddComponent<Light>();
        lightR2.type = LightType.Spot;
        lightR2.color = new Color(0.95f, 0.98f, 1.0f, 1f);
        lightR2.intensity = 2.2f;
        lightR2.range = 10f;
        lightR2.spotAngle = 65f;
        lightR2.shadows = LightShadows.Soft; // 1 shadow map face
        lightR2.shadowBias = 0.05f;

        // 4. Setup Room 5 Evacuation Gate Spotlight with real-time shadow
        GameObject spotR5 = GameObject.Find("Security_Spotlight_Room5");
        if (spotR5 == null)
        {
            spotR5 = new GameObject("Security_Spotlight_Room5");
        }
        spotR5.transform.position = new Vector3(0f, 4.5f, 44f);
        spotR5.transform.rotation = Quaternion.Euler(70f, 0f, 0f);
        Light lightR5 = spotR5.GetComponent<Light>();
        if (lightR5 == null) lightR5 = spotR5.AddComponent<Light>();
        lightR5.type = LightType.Spot;
        lightR5.color = new Color(0.95f, 0.98f, 0.92f, 1f);
        lightR5.intensity = 2.4f;
        lightR5.range = 12f;
        lightR5.spotAngle = 60f;
        lightR5.shadows = LightShadows.Soft; // 1 shadow map face
        lightR5.shadowBias = 0.05f;

        // Total real-time shadow maps: 1 Directional + 2 Spots = 3 shadow maps total!
        Debug.Log("[ApplyIGIVisualAndLightingPass] Configured 3 selective shadow casters (1 Directional + 2 Spotlights). Point light shadows disabled.");
    }

    private static void CreateCeilingFixturesAndEmergencyLights()
    {
        Transform fixtureRoot = GetOrCreateRoot("IGI_Ceiling_Fixtures");
        Transform emergencyRoot = GetOrCreateRoot("IGI_Emergency_Bulkheads");

        Material matSteel = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Steel_Dark.mat");
        Material matTube = GetOrCreateEmissiveMat("Mat_Fluorescent_Tube", new Color(0.85f, 0.92f, 1.0f), 1.6f);
        Material matEmergency = GetOrCreateEmissiveMat("Mat_Emergency_Bulb", new Color(0.95f, 0.28f, 0.05f), 2.2f);

        // Ceiling fixtures in key room centers
        Vector3[] fixturePositions = new Vector3[]
        {
            new Vector3(0f, 4.45f, -26f),   // Room 1
            new Vector3(-4f, 4.45f, -12f),  // Room 2 West
            new Vector3(4f, 4.45f, -12f),   // Room 2 East
            new Vector3(0f, 4.45f, -6f),    // Room 2 Center
            new Vector3(-16f, 4.45f, 11f),  // Room 3
            new Vector3(16f, 4.45f, 11f),   // Room 4
            new Vector3(0f, 4.45f, 26f),    // Room 5 Entrance
            new Vector3(0f, 4.45f, 36f),    // Room 5 Mid
            new Vector3(-6f, 4.45f, 42f),   // Room 5 West Aisle
            new Vector3(6f, 4.45f, 42f)     // Room 5 East Aisle
        };

        foreach (var pos in fixturePositions)
        {
            string fName = $"Fixture_{pos.x:F0}_{pos.z:F0}";
            if (fixtureRoot.Find(fName) != null) continue;

            GameObject fixture = new GameObject(fName);
            fixture.transform.SetParent(fixtureRoot, false);
            fixture.transform.position = pos;

            // Housing channel
            GameObject housing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            housing.name = "Housing";
            housing.transform.SetParent(fixture.transform, false);
            housing.transform.localScale = new Vector3(0.35f, 0.08f, 2.2f);
            housing.GetComponent<Renderer>().sharedMaterial = matSteel;
            Object.DestroyImmediate(housing.GetComponent<Collider>());

            // Twin tubes
            GameObject tube1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tube1.name = "Tube1";
            tube1.transform.SetParent(fixture.transform, false);
            tube1.transform.localPosition = new Vector3(-0.08f, -0.05f, 0f);
            tube1.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            tube1.transform.localScale = new Vector3(0.04f, 1.0f, 0.04f);
            tube1.GetComponent<Renderer>().sharedMaterial = matTube;
            Object.DestroyImmediate(tube1.GetComponent<Collider>());

            GameObject tube2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tube2.name = "Tube2";
            tube2.transform.SetParent(fixture.transform, false);
            tube2.transform.localPosition = new Vector3(0.08f, -0.05f, 0f);
            tube2.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            tube2.transform.localScale = new Vector3(0.04f, 1.0f, 0.04f);
            tube2.GetComponent<Renderer>().sharedMaterial = matTube;
            Object.DestroyImmediate(tube2.GetComponent<Collider>());
        }

        // Emergency Bulkhead lights above key doorways and exits
        Vector3[] emergencyPositions = new Vector3[]
        {
            new Vector3(0f, 3.2f, -18f),   // Room 1 -> Room 2 threshold
            new Vector3(-5f, 3.2f, -2f),   // Room 2 -> Route A threshold
            new Vector3(5f, 3.2f, -2f),    // Room 2 -> Route B threshold
            new Vector3(-12f, 3.2f, 16f),  // Room 3 -> Boss Corridor threshold
            new Vector3(12f, 3.2f, 16f),   // Room 4 -> Boss Corridor threshold
            new Vector3(0f, 3.2f, 48.5f)   // Room 5 Evacuation Gate
        };

        foreach (var pos in emergencyPositions)
        {
            string eName = $"Bulkhead_{pos.x:F0}_{pos.z:F0}";
            if (emergencyRoot.Find(eName) != null) continue;

            GameObject bulkhead = new GameObject(eName);
            bulkhead.transform.SetParent(emergencyRoot, false);
            bulkhead.transform.position = pos;

            // Frame
            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.name = "Frame";
            frame.transform.SetParent(bulkhead.transform, false);
            frame.transform.localScale = new Vector3(0.35f, 0.22f, 0.12f);
            frame.GetComponent<Renderer>().sharedMaterial = matSteel;
            Object.DestroyImmediate(frame.GetComponent<Collider>());

            // Amber bulb
            GameObject bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb.name = "Bulb";
            bulb.transform.SetParent(bulkhead.transform, false);
            bulb.transform.localPosition = new Vector3(0f, -0.02f, 0.05f);
            bulb.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            bulb.GetComponent<Renderer>().sharedMaterial = matEmergency;
            Object.DestroyImmediate(bulb.GetComponent<Collider>());

            // Low-intensity point light (no shadows)
            Light bLight = bulkhead.AddComponent<Light>();
            bLight.type = LightType.Point;
            bLight.color = new Color(0.95f, 0.35f, 0.08f, 1f);
            bLight.intensity = 0.8f;
            bLight.range = 4.5f;
            bLight.shadows = LightShadows.None;
        }
    }

    private static Material GetOrCreateEmissiveMat(string name, Color color, float emissionIntensity)
    {
        string path = $"Assets/Materials/{name}.mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            m = new Material(litShader);
            m.name = name;
            AssetDatabase.CreateAsset(m, path);
        }
        m.SetColor("_BaseColor", color);
        m.EnableKeyword("_EMISSION");
        m.SetColor("_EmissionColor", color * emissionIntensity);
        EditorUtility.SetDirty(m);
        return m;
    }

    private static void SetupProbesAndStaticFlags()
    {
        // 1. Static flags for static geometry
        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int flaggedCount = 0;
        foreach (var r in renderers)
        {
            if (r.CompareTag("Player") || r.GetComponentInParent<CharacterController>() != null || r.GetComponentInParent<PatrolGuardAI>() != null || r.name.Contains("Guard") || r.name.Contains("Weapon"))
                continue;

            StaticEditorFlags flags = StaticEditorFlags.ContributeGI | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic;
            GameObjectUtility.SetStaticEditorFlags(r.gameObject, flags);
            flaggedCount++;
        }
        Debug.Log($"[ApplyIGIVisualAndLightingPass] Configured {flaggedCount} static geometry renderers for Light Baking & Probes.");

        // 2. Light Probe Group across all 5 warehouse sectors
        Transform probeRoot = GetOrCreateRoot("IGI_Probes");
        GameObject lpgGo = GameObject.Find("Warehouse_LightProbeGroup");
        if (lpgGo == null)
        {
            lpgGo = new GameObject("Warehouse_LightProbeGroup");
            lpgGo.transform.SetParent(probeRoot, false);
        }
        var lpg = lpgGo.GetComponent<LightProbeGroup>();
        if (lpg == null) lpg = lpgGo.AddComponent<LightProbeGroup>();

        List<Vector3> probePositions = new List<Vector3>();
        float[] yPlanes = new float[] { 0.8f, 2.0f };

        // Generate grid throughout rooms
        for (float z = -28f; z <= 48f; z += 4.0f)
        {
            float minX = -8f, maxX = 8f;
            if (z < -18f) { minX = -5f; maxX = 5f; } // Room 1
            else if (z < 0f) { minX = -8f; maxX = 8f; } // Room 2
            else if (z < 18f) { minX = -20f; maxX = 20f; } // Rooms 3 & 4
            else { minX = -12f; maxX = 12f; } // Room 5

            for (float x = minX; x <= maxX; x += 4.0f)
            {
                foreach (var y in yPlanes)
                {
                    probePositions.Add(new Vector3(x, y, z));
                }
            }
        }
        lpg.probePositions = probePositions.ToArray();
        Debug.Log($"[ApplyIGIVisualAndLightingPass] Configured LightProbeGroup with {probePositions.Count} probe positions.");

        // 3. Reflection Probes for the 5 rooms
        SetupReflectionProbe(probeRoot, "ReflectionProbe_Room1_Safe", new Vector3(0f, 2.5f, -26f), new Vector3(14f, 5.5f, 12f));
        SetupReflectionProbe(probeRoot, "ReflectionProbe_Room2_Patrol", new Vector3(0f, 2.5f, -9f), new Vector3(20f, 5.5f, 20f));
        SetupReflectionProbe(probeRoot, "ReflectionProbe_Room3_Search", new Vector3(-16f, 2.5f, 11f), new Vector3(16f, 5.5f, 14f));
        SetupReflectionProbe(probeRoot, "ReflectionProbe_Room4_Defensive", new Vector3(16f, 2.5f, 11f), new Vector3(16f, 5.5f, 14f));
        SetupReflectionProbe(probeRoot, "ReflectionProbe_Room5_Boss", new Vector3(0f, 2.5f, 36f), new Vector3(30f, 5.5f, 30f));
    }

    private static void SetupReflectionProbe(Transform parent, string name, Vector3 pos, Vector3 size)
    {
        GameObject rpGo = GameObject.Find(name);
        if (rpGo == null)
        {
            rpGo = new GameObject(name);
            rpGo.transform.SetParent(parent, false);
        }
        rpGo.transform.position = pos;
        var rp = rpGo.GetComponent<ReflectionProbe>();
        if (rp == null) rp = rpGo.AddComponent<ReflectionProbe>();
        rp.size = size;
        rp.resolution = 128; // Lightweight for iGPU
        rp.mode = ReflectionProbeMode.Baked;
        rp.intensity = 0.85f;
    }

    private static void SetupPostProcessingVolume()
    {
        GameObject volGo = GameObject.Find("Global_PostProcess_Volume");
        if (volGo == null)
        {
            volGo = new GameObject("Global_PostProcess_Volume");
        }
        var volume = volGo.GetComponent<Volume>();
        if (volume == null) volume = volGo.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 1.0f;

        string profilePath = "Assets/Settings/IGI_PostProcess_Profile.asset";
        VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, profilePath);

            AddAndRegisterComponent<Tonemapping>(profile, c => {
                c.mode.Override(TonemappingMode.Neutral);
            });
            AddAndRegisterComponent<ColorAdjustments>(profile, c => {
                c.postExposure.Override(0.1f);
                c.contrast.Override(12f);
                c.saturation.Override(-12f);
                c.colorFilter.Override(new Color(0.92f, 0.96f, 0.98f));
            });
            AddAndRegisterComponent<Vignette>(profile, c => {
                c.intensity.Override(0.20f);
                c.smoothness.Override(0.45f);
            });
            AddAndRegisterComponent<Bloom>(profile, c => {
                c.threshold.Override(1.15f);
                c.intensity.Override(0.22f);
            });
            AddAndRegisterComponent<FilmGrain>(profile, c => {
                c.type.Override(FilmGrainLookup.Thin2);
                c.intensity.Override(0.12f);
            });

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
        }
        volume.sharedProfile = profile;
        Debug.Log("[ApplyIGIVisualAndLightingPass] Configured Global Volume with IGI_PostProcess_Profile.");
    }

    private static void AddAndRegisterComponent<T>(VolumeProfile profile, System.Action<T> configure) where T : VolumeComponent
    {
        T comp = ScriptableObject.CreateInstance<T>();
        comp.name = typeof(T).Name;
        comp.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        configure(comp);
        AssetDatabase.AddObjectToAsset(comp, profile);
        profile.components.Add(comp);
    }

    private static void EnsureCameraPostProcessing()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                var ucd = cam.GetComponent<UniversalAdditionalCameraData>();
                if (ucd != null)
                {
                    ucd.renderPostProcessing = true;
                    Debug.Log("[ApplyIGIVisualAndLightingPass] Ensured Player Camera renderPostProcessing = true.");
                }
            }
        }
    }

    private static void BakeLighting()
    {
        Debug.Log("[ApplyIGIVisualAndLightingPass] Configuring Lightmapping settings via LightingSettings...");
        LightingSettings ls = Lightmapping.lightingSettings;
        if (ls == null)
        {
            ls = new LightingSettings();
            Lightmapping.lightingSettings = ls;
        }
        ls.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
        ls.lightmapResolution = 6;
        ls.lightmapMaxSize = 1024;
        ls.directSampleCount = 16;
        ls.indirectSampleCount = 32;
        ls.maxBounces = 1;

        Debug.Log("[ApplyIGIVisualAndLightingPass] Starting synchronous lighting bake (Lightmapping.Bake)...");
        bool baked = Lightmapping.Bake();
        Debug.Log($"[ApplyIGIVisualAndLightingPass] Lightmapping.Bake completed with result: {baked}. Lightmaps: {LightmapSettings.lightmaps.Length}, LightProbes: {(LightmapSettings.lightProbes != null ? LightmapSettings.lightProbes.count : 0)}");

        Debug.Log("[ApplyIGIVisualAndLightingPass] Baking Reflection Probes across warehouse sectors...");
        string sceneDir = "Assets/Scenes/Warehouse";
        if (!Directory.Exists(sceneDir)) Directory.CreateDirectory(sceneDir);

        var probes = Object.FindObjectsByType<ReflectionProbe>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int bakedProbesWithTex = 0;
        for (int i = 0; i < probes.Length; i++)
        {
            var rp = probes[i];
            string probeAssetPath = $"{sceneDir}/{rp.gameObject.name}.exr";
            Debug.Log($"[ApplyIGIVisualAndLightingPass] Baking Reflection Probe {rp.gameObject.name} to {probeAssetPath}...");
            bool probeBaked = Lightmapping.BakeReflectionProbe(rp, probeAssetPath);
            Debug.Log($"[ApplyIGIVisualAndLightingPass] Bake Reflection Probe {rp.gameObject.name}: Success={probeBaked}");

            AssetDatabase.ImportAsset(probeAssetPath, ImportAssetOptions.ForceUpdate);
            Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(probeAssetPath);
            if (tex != null)
            {
                rp.bakedTexture = tex;
                rp.customBakedTexture = tex;
                bakedProbesWithTex++;
                EditorUtility.SetDirty(rp);
                Debug.Log($"[ApplyIGIVisualAndLightingPass] Assigned bakedTexture to {rp.gameObject.name} from {probeAssetPath}");
            }
            else if (rp.bakedTexture != null)
            {
                bakedProbesWithTex++;
            }
        }

        Debug.Log($"[ApplyIGIVisualAndLightingPass] Lighting bake completed! Success={baked}, LightmapsCount={LightmapSettings.lightmaps.Length}, LightProbesCount={(LightmapSettings.lightProbes != null ? LightmapSettings.lightProbes.count : 0)}, BakedReflectionProbesWithTexture={bakedProbesWithTex}");

        string bakeReport = $"LIGHTING_BAKE_COMPLETED at {System.DateTime.Now}\n" +
                            $"Success: {baked}\n" +
                            $"LightmapsCount: {LightmapSettings.lightmaps.Length}\n" +
                            $"LightProbesCount: {(LightmapSettings.lightProbes != null ? LightmapSettings.lightProbes.count : 0)}\n" +
                            $"BakedReflectionProbesWithTexture: {bakedProbesWithTex}\n";
        File.WriteAllText(Path.Combine(Application.dataPath, "../scratch/lighting_bake_result.txt"), bakeReport);
    }

    private static Transform GetOrCreateRoot(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null) root = new GameObject(name);
        return root.transform;
    }
}
