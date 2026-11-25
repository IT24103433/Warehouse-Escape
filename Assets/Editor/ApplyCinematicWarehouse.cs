using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>Reproducible visual-only upgrade. Adds no gameplay colliders or navigation obstacles.</summary>
public static class ApplyCinematicWarehouse
{
    const string Art="Assets/Textures/Cinematic";
    static Material Load(string name) => AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/"+name+".mat");
    static T Override<T>(VolumeProfile profile) where T:VolumeComponent
    {
        if(profile.TryGet<T>(out var value)) return value;
        value=profile.Add<T>(true); AssetDatabase.AddObjectToAsset(value,profile); return value;
    }
    [MenuItem("Warehouse/Apply Cinematic Visual Upgrade")]
    public static void Execute()
    {
        var scene=EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        Directory.CreateDirectory(Art);
        Mask("Concrete",.04f,.18f,.36f);
        Mask("Steel",.55f,.25f,.52f);
        Mask("Wood",0f,.08f,.24f);
        Surface("Mat_Floor","Concrete",new Color(.62f,.64f,.65f),.65f);
        Surface("Mat_Wall","Concrete",new Color(.62f,.65f,.67f),.55f);
        foreach(string n in new[]{"Mat_Steel_Dark","Mat_Shutter_Metal","Mat_Duct_Metal","Mat_Rack_Blue","Mat_Rack_Orange","Mat_Shelf_Level","Mat_Lamp_Metal"})
            Surface(n,"Steel",null,.7f);
        foreach(string n in new[]{"Mat_Crate_Wood","Mat_Pallet"}) Surface(n,"Wood",null,.8f);
        var ceiling=Load("Mat_Ceiling"); ceiling.SetColor("_BaseColor",new Color(.12f,.15f,.18f));EditorUtility.SetDirty(ceiling);
        var tube=Load("Mat_Fluorescent_Tube");
        tube.EnableKeyword("_EMISSION");tube.SetColor("_EmissionColor",new Color(.65f,.85f,1f)*3f);EditorUtility.SetDirty(tube);
        var emergency=Load("Mat_Emergency_Bulb");
        emergency.EnableKeyword("_EMISSION");emergency.SetColor("_EmissionColor",new Color(1f,.19f,.025f)*3f);EditorUtility.SetDirty(emergency);
        foreach(var light in Object.FindObjectsByType<Light>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {
            if(light.GetComponentInParent<Health>()!=null) continue;
            if(light.name=="HighBay_Fixture")
            {
                // Keep fixture geometry upright; place a separate lamp below the opaque housing.
                light.enabled=false;light.transform.rotation=Quaternion.identity;
                var child=light.transform.Find("Cinematic_Pool");
                if(child==null) {var lamp=new GameObject("Cinematic_Pool");lamp.transform.SetParent(light.transform,false);child=lamp.transform;}
                child.localPosition=Vector3.down*.45f;child.localRotation=Quaternion.Euler(90,0,0);
                var pool=child.GetComponent<Light>();if(pool==null)pool=child.gameObject.AddComponent<Light>();
                pool.type=LightType.Spot;pool.enabled=true;pool.spotAngle=115;pool.innerSpotAngle=82;pool.range=14;
                bool warm=Mathf.RoundToInt(light.transform.position.z/6f)%2==0;
                pool.color=warm ? new Color(1f,.82f,.58f):new Color(.65f,.83f,1f);
                pool.intensity=warm ? 12f:11f;pool.shadows=LightShadows.None;
                pool.lightmapBakeType=LightmapBakeType.Mixed;EditorUtility.SetDirty(pool);
            }
            else if(light.name.StartsWith("Bulkhead_")) {light.color=new Color(1f,.20f,.035f);light.intensity=1.3f;light.range=5f;}
            else if(light.name.StartsWith("Card")) {light.intensity=3.5f;light.color=new Color(.52f,.88f,1f);}
            else if(light.name=="Warehouse_DirectionalFill") {light.intensity=.48f;light.color=new Color(.65f,.76f,1f);}
            else if(light.name=="Security_Spotlight_Room2") {light.intensity=5f;light.range=14;light.spotAngle=78;light.color=new Color(.6f,.8f,1f);}
            else if(light.name=="Security_Spotlight_Room5") {light.intensity=6f;light.range=15;light.spotAngle=78;light.color=new Color(1f,.67f,.32f);}
            EditorUtility.SetDirty(light);
        }
        RenderSettings.ambientMode=AmbientMode.Trilight;
        RenderSettings.ambientSkyColor=new Color(.36f,.40f,.46f);
        RenderSettings.ambientEquatorColor=new Color(.23f,.27f,.32f);
        RenderSettings.ambientGroundColor=new Color(.14f,.15f,.17f);
        RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;
        RenderSettings.fogDensity=.008f;RenderSettings.fogColor=new Color(.09f,.13f,.17f);
        var volume=GameObject.Find("Global_PostProcess_Volume").GetComponent<Volume>();
        var profile=volume.sharedProfile;
        Override<Tonemapping>(profile).mode.Override(TonemappingMode.ACES);
        var color=Override<ColorAdjustments>(profile);color.postExposure.Override(.65f);color.contrast.Override(10f);color.saturation.Override(-6f);color.colorFilter.Override(Color.white);
        var bloom=Override<Bloom>(profile);bloom.intensity.Override(.28f);bloom.threshold.Override(1.15f);bloom.scatter.Override(.55f);
        var vignette=Override<Vignette>(profile);vignette.intensity.Override(.16f);vignette.smoothness.Override(.5f);
        Override<FilmGrain>(profile).intensity.Override(.025f);
        var split=Override<SplitToning>(profile);split.shadows.Override(new Color(.48f,.49f,.51f));split.highlights.Override(new Color(.55f,.51f,.46f));split.balance.Override(5f);
        EditorUtility.SetDirty(profile);
        var player=GameObject.FindGameObjectWithTag("Player");var camera=player.GetComponentInChildren<Camera>();
        camera.allowHDR=true;var data=camera.GetUniversalAdditionalCameraData();data.renderPostProcessing=true;
        data.antialiasing=AntialiasingMode.FastApproximateAntialiasing;
        var old=GameObject.Find("Cinematic_Visual_Details");if(old!=null) Object.DestroyImmediate(old);
        var root=new GameObject("Cinematic_Visual_Details").transform;
        AddWallDetails(root);
        FinishCinematicPresentation.ApplyInOpenScene();
        var pipeline=GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
        if(pipeline!=null) {pipeline.shadowDistance=45;pipeline.shadowCascadeCount=2;pipeline.supportsHDR=true;EditorUtility.SetDirty(pipeline);}
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        // Bake changed static light response and room reflections; runtime stays inexpensive.
        var settings=Lightmapping.lightingSettings;
        settings.lightmapper=LightingSettings.Lightmapper.ProgressiveCPU;
        settings.lightmapResolution=8;settings.lightmapMaxSize=1024;
        settings.directSampleCount=32;settings.indirectSampleCount=48;settings.maxBounces=2;
        Debug.Log("[Cinematic] Baking static light response...");
        bool baked=Lightmapping.Bake();
        if(!baked) throw new System.InvalidOperationException("Cinematic light bake failed.");
        foreach(var probe in Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None))
        {
            probe.resolution=128;probe.boxProjection=true;probe.intensity=.85f;
            string path="Assets/Scenes/Warehouse/"+probe.name+".exr";
            if(!Lightmapping.BakeReflectionProbe(probe,path)) throw new System.InvalidOperationException("Reflection bake failed: "+probe.name);
            AssetDatabase.ImportAsset(path);probe.bakedTexture=AssetDatabase.LoadAssetAtPath<Texture>(path);
        }
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        Directory.CreateDirectory("scratch/visual-upgrade");
        File.WriteAllText("scratch/visual-upgrade/result.txt","Cinematic pass saved; static lighting and five reflection probes baked. No gameplay colliders changed.");
        Debug.Log("[Cinematic] Visual pass complete and scene saved normally.");
    }
    static void Mask(string name,float metal,float rough,float polished)
    {
        const int size=512;
        var tex=new Texture2D(size,size,TextureFormat.RGBA32,true,true);var pixels=new Color[size*size];
        for(int y=0;y<size;y++) for(int x=0;x<size;x++)
        {
            // Periodic noise removes tile seams. Alpha is smoothness; green is ambient occlusion.
            float u=x/(float)size,v=y/(float)size;
            float wave=Mathf.Sin(u*Mathf.PI*2)*.5f+.5f, cross=Mathf.Cos(v*Mathf.PI*2)*.5f+.5f;
            float wear=Mathf.PerlinNoise(wave*5+13,cross*5+7);
            float scratches=Mathf.Pow(Mathf.Max(0,Mathf.Sin((u*61+v*3)*Mathf.PI*2)),24)*.22f;
            pixels[y*size+x]=new Color(metal,.85f+wear*.15f,0,Mathf.Lerp(rough,polished,wear)-scratches);
        }
        tex.SetPixels(pixels);tex.Apply();string path=Art+"/"+name+"_Mask.png";
        File.WriteAllBytes(path,tex.EncodeToPNG());Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(path);
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.sRGBTexture=false;importer.mipmapEnabled=true;importer.anisoLevel=4;importer.textureCompression=TextureImporterCompression.Compressed;importer.SaveAndReimport();
    }
    static void Surface(string name,string mask,Color? tint,float normal)
    {
        var mat=Load(name);if(mat==null)return;
        if(tint.HasValue)mat.SetColor("_BaseColor",tint.Value);
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"/"+mask+"_Mask.png");
        mat.SetTexture("_MetallicGlossMap",texture);mat.SetTexture("_OcclusionMap",texture);
        mat.SetFloat("_Smoothness",1);mat.SetFloat("_OcclusionStrength",.55f);mat.SetFloat("_BumpScale",normal);
        mat.EnableKeyword("_METALLICSPECGLOSSMAP");mat.EnableKeyword("_OCCLUSIONMAP");
        EditorUtility.SetDirty(mat);
    }
    static void AddWallDetails(Transform root)
    {
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Meshes/Props/IndustrialServicePanel.fbx");
        if(prefab==null)throw new System.InvalidOperationException("Blender service panel missing.");
        foreach(string room in new[]{"Room_1_Start","Room_2_Patrol","Room_3_Search","Room_4_Defensive","Room_5_Boss"})
        {
            var go=GameObject.Find(room);if(go==null)continue;
            var floor=go.transform.Find("Floor");if(floor==null)continue;
            var bounds=floor.GetComponent<Renderer>().bounds;
            for(int side=-1;side<=1;side+=2)
            {
                var panel=(GameObject)PrefabUtility.InstantiatePrefab(prefab);
                panel.name=room+"_ServicePanel_"+side;panel.transform.SetParent(root,false);
                panel.transform.position=new Vector3(side<0?bounds.min.x+.24f:bounds.max.x-.24f,2f,bounds.center.z+2f);
                panel.transform.rotation=Quaternion.Euler(0,side<0?90:-90,0);
                foreach(var col in panel.GetComponentsInChildren<Collider>())Object.DestroyImmediate(col);
                foreach(var renderer in panel.GetComponentsInChildren<MeshRenderer>())
                {
                    string n=renderer.name;
                    renderer.sharedMaterial=n.Contains("Indicator")?Load("Mat_Reader_Ready"):n.Contains("Caution")||n.Contains("Bolt")||n.Contains("Hinge")?Load("Mat_Rack_Orange"):n.Contains("Recess")||n.Contains("Vent")||n.Contains("Handle")?Load("Mat_Steel_Dark"):Load("Mat_Duct_Metal");
                    GameObjectUtility.SetStaticEditorFlags(renderer.gameObject,StaticEditorFlags.ContributeGI|StaticEditorFlags.ReflectionProbeStatic);
                }
                // Wall-bound conduit does not block walkways or alter NavMesh.
                var pipe=GameObject.CreatePrimitive(PrimitiveType.Cylinder);pipe.name=room+"_Service_Conduit_"+side;pipe.transform.SetParent(root,false);
                pipe.transform.position=panel.transform.position+Vector3.up*1.2f;
                pipe.transform.localScale=new Vector3(.045f,1.1f,.045f);
                pipe.GetComponent<Renderer>().sharedMaterial=Load("Mat_Duct_Metal");Object.DestroyImmediate(pipe.GetComponent<Collider>());
            }
        }
    }
}
