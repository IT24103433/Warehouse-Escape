using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class FinishCinematicPresentation
{
    public static void Execute()
    {
        var scene=EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        ApplyInOpenScene();
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
    }
    static Material Material(string name,Color color,float metal,float smooth,bool textured)
    {
        string path="Assets/Materials/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat==null){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,path);}
        mat.SetColor("_BaseColor",color);mat.SetFloat("_Metallic",metal);mat.SetFloat("_Smoothness",smooth);
        if(textured)
        {
            mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Gunmetal.png"));
            mat.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Tex_Gunmetal_Normal.png"));
            mat.SetFloat("_BumpScale",.35f);mat.EnableKeyword("_NORMALMAP");
        }
        EditorUtility.SetDirty(mat);return mat;
    }
    public static void ApplyInOpenScene()
    {
        var steel=Material("Mat_Cinematic_Gunmetal",new Color(.30f,.34f,.38f),.7f,.48f,true);
        var polymer=Material("Mat_Cinematic_Polymer",new Color(.08f,.09f,.10f),.05f,.24f,false);
        var wood=Material("Mat_Cinematic_WoodGrip",new Color(.27f,.12f,.055f),0f,.25f,false);
        foreach(var file in System.IO.Directory.GetFiles("Assets/Resources/Weapons","*.prefab"))
        {
            string path=file.Replace('\\','/');
            var instance=PrefabUtility.LoadPrefabContents(path);
            foreach(var renderer in instance.GetComponentsInChildren<MeshRenderer>(true))
            {
                string n=renderer.name.ToLowerInvariant();
                renderer.sharedMaterial=n.Contains("stock")||n.Contains("grip")||n.Contains("handguard")||n.Contains("foreend")
                    ? (file.Contains("AK47")?wood:polymer):steel;
            }
            PrefabUtility.SaveAsPrefabAsset(instance,path);PrefabUtility.UnloadPrefabContents(instance);
        }
        foreach(var file in System.IO.Directory.GetFiles("Assets/Meshes/Weapons","*.fbx"))
        {
            var importer=AssetImporter.GetAtPath(file.Replace('\\','/')) as ModelImporter;
            if(importer!=null && (importer.importNormals!=ModelImporterNormals.Calculate || importer.normalSmoothingAngle!=60f))
            {importer.importNormals=ModelImporterNormals.Calculate;importer.normalSmoothingAngle=60f;importer.SaveAndReimport();}
        }
        foreach(var light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            if(light.name=="Face_Fill_Light"){light.intensity=.2f;EditorUtility.SetDirty(light);}
        var root=GameObject.Find("Cinematic_Visual_Details");
        if(root!=null)
        {
            Vector3[] centers={new Vector3(0,2.8f,-26),new Vector3(0,2.8f,-10),new Vector3(-16,2.8f,9),new Vector3(16,2.8f,9),new Vector3(0,2.8f,35)};
            for(int i=0;i<centers.Length;i++)
            {
                string name="Room_Readability_Fill_"+(i+1);var child=root.transform.Find(name);
                if(child==null){var go=new GameObject(name);go.transform.SetParent(root.transform,false);child=go.transform;}
                child.position=centers[i];var light=child.GetComponent<Light>();if(light==null)light=child.gameObject.AddComponent<Light>();
                light.type=LightType.Point;light.range=i==4?13:10;light.intensity=1.15f;
                light.color=new Color(.85f,.90f,1f);light.shadows=LightShadows.None;light.lightmapBakeType=LightmapBakeType.Realtime;
            }
        }
    }
}
