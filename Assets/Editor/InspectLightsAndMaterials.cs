using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class InspectLightsAndMaterials
{
    [MenuItem("Warehouse/Inspect Lights and Materials")]
    public static void Execute()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== LIGHTS, MATERIALS & RENDERER INSPECTION ===");

        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);

        // 1. Lights
        Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\n--- LIGHTS (Total: {allLights.Length}) ---");
        int realtimeShadowCasters = 0;
        foreach (var l in allLights)
        {
            bool castsShadows = (l.shadows != LightShadows.None);
            if (castsShadows) realtimeShadowCasters++;
            sb.AppendLine($"Light: '{l.name}' | Type: {l.type} | Shadows: {l.shadows} (res={l.shadowCustomResolution}) | Intensity: {l.intensity:F2} | Range: {l.range:F1} | Color: {l.color} | Mode: {l.lightmapBakeType} | Active: {l.gameObject.activeInHierarchy} | Pos: {l.transform.position:F2} | Parent: {(l.transform.parent != null ? l.transform.parent.name : "root")}");
        }
        sb.AppendLine($"Total Shadow Casting Lights: {realtimeShadowCasters}");

        // 2. Probes
        ReflectionProbe[] reflectionProbes = Object.FindObjectsByType<ReflectionProbe>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\n--- REFLECTION PROBES (Total: {reflectionProbes.Length}) ---");
        foreach (var rp in reflectionProbes)
        {
            sb.AppendLine($"ReflectionProbe: '{rp.name}' | Mode: {rp.mode} | BoxSize: {rp.size} | Pos: {rp.transform.position:F2} | BakedTexture: {(rp.bakedTexture != null ? rp.bakedTexture.name : "null")}");
        }

        LightProbeGroup[] lightProbeGroups = Object.FindObjectsByType<LightProbeGroup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\n--- LIGHT PROBE GROUPS (Total: {lightProbeGroups.Length}) ---");
        foreach (var lpg in lightProbeGroups)
        {
            sb.AppendLine($"LightProbeGroup: '{lpg.name}' | Probes Count: {lpg.probePositions.Length} | Pos: {lpg.transform.position:F2}");
        }
        sb.AppendLine($"Scene Baked Lightmaps: {LightmapSettings.lightmaps.Length} | Light Probes Data: {(LightmapSettings.lightProbes != null ? LightmapSettings.lightProbes.count.ToString() : "none")}");

        try
        {
            Volume[] volumes = Object.FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            sb.AppendLine($"\n--- VOLUMES (Total: {volumes.Length}) ---");
            foreach (var v in volumes)
            {
                sb.AppendLine($"Volume: '{v.name}' | IsGlobal: {v.isGlobal} | Priority: {v.priority} | Profile: {(v.sharedProfile != null ? v.sharedProfile.name : "null")}");
                if (v.sharedProfile != null)
                {
                    foreach (var comp in v.sharedProfile.components)
                    {
                        if (comp != null)
                        {
                            sb.AppendLine($"    Component: {comp.GetType().Name} | Active: {comp.active}");
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            sb.AppendLine($"Error inspecting volumes: {ex.Message}");
        }

        // 4. Static Flags & MeshRenderers
        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        sb.AppendLine($"\n--- MESH RENDERERS (Total: {renderers.Length}) ---");
        int staticCount = 0;
        int receiveShadowsCount = 0;
        int castShadowsCount = 0;
        foreach (var r in renderers)
        {
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(r.gameObject);
            bool isStatic = (flags & StaticEditorFlags.ContributeGI) != 0;
            if (isStatic) staticCount++;
            if (r.receiveShadows) receiveShadowsCount++;
            if (r.shadowCastingMode != ShadowCastingMode.Off) castShadowsCount++;
        }
        sb.AppendLine($"Total MeshRenderers: {renderers.Length} | Static (ContributeGI): {staticCount} | Cast Shadows: {castShadowsCount} | Receive Shadows: {receiveShadowsCount}");

        // 5. URP Pipeline Asset Settings
        var pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (pipelineAsset != null)
        {
            sb.AppendLine($"\n--- URP PIPELINE ASSET: {pipelineAsset.name} ---");
            sb.AppendLine($"Main Light Shadows Supported: {pipelineAsset.supportsMainLightShadows}");
            sb.AppendLine($"Additional Lights Cast Shadows: {pipelineAsset.supportsAdditionalLightShadows}");
            sb.AppendLine($"Shadow Distance: {pipelineAsset.shadowDistance}");
            sb.AppendLine($"Max Additional Lights Count: {pipelineAsset.maxAdditionalLightsCount}");
        }

        string outPath = Path.Combine(Application.dataPath, "../scratch/lights_materials_report.txt");
        File.WriteAllText(outPath, sb.ToString());
        Debug.Log($"[InspectLightsAndMaterials] Report written to {outPath}");
    }
}
