using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

/// <summary>Surface detail changes leave all gameplay colliders, transforms and AI untouched.</summary>
public static class DetailWarehouseSurfaces
{
    const string Folder="Assets/Meshes/SurfaceDetail";
    static Mesh cube,bolt;
    static readonly Dictionary<string,Mesh> bevels=new Dictionary<string,Mesh>();
    static Material Mat(string n)=>AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/"+n+".mat");
    public static void Execute() => Run(true);
    public static void RepairDetailTrim() => Run(false);
    static void Run(bool bakeLighting)
    {
        var scene=EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        Directory.CreateDirectory(Folder);AssetDatabase.Refresh();bevels.Clear();
        var primitive=GameObject.CreatePrimitive(PrimitiveType.Cube);cube=primitive.GetComponent<MeshFilter>().sharedMesh;UnityEngine.Object.DestroyImmediate(primitive);bolt=HexBolt();
        foreach(string path in Directory.GetFiles("Assets/Textures","*.png",SearchOption.AllDirectories))
        {
            if(path.Contains("/UI/")||path.Contains("\\UI\\"))continue;
            var importer=AssetImporter.GetAtPath(path.Replace('\\','/')) as TextureImporter;if(importer==null)continue;
            importer.filterMode=FilterMode.Trilinear;importer.anisoLevel=8;importer.mipmapEnabled=true;importer.maxTextureSize=2048;
            importer.textureCompression=TextureImporterCompression.Compressed;importer.compressionQuality=90;importer.SaveAndReimport();
        }
        QualitySettings.anisotropicFiltering=AnisotropicFiltering.Enable;QualitySettings.globalTextureMipmapLimit=0;
        foreach(string path in Directory.GetFiles("Assets/Materials","*.mat"))
        {
            var mat=AssetDatabase.LoadAssetAtPath<Material>(path.Replace('\\','/'));if(mat==null||mat.shader.name!="Universal Render Pipeline/Lit")continue;
            string n=mat.name;
            if(n.Contains("Uniform")||n.Contains("Vest")||n.Contains("Player_Uniform")) Texture(mat,"Tex_Tactical_Fabric",.25f,.08f);
            else if(n.Contains("Barrel")||n.Contains("Forklift")) Texture(mat,"Tex_Metal_Plate",.3f,.42f);
            else if(n.Contains("Cargo_Wrapped")) Texture(mat,"Tex_Cardboard",.25f,.3f);
            else if(n.Contains("Leather")) {mat.SetFloat("_Smoothness",.3f);}
            if(n.Contains("Human_Skin")||n.StartsWith("Mat_Skin_"))mat.SetFloat("_Smoothness",.22f);
            EditorUtility.SetDirty(mat);
        }
        var old=GameObject.Find("Warehouse_Surface_Details");if(old!=null)UnityEngine.Object.DestroyImmediate(old);
        var root=new GameObject("Warehouse_Surface_Details").transform;
        var batches=new List<CombineInstance>[5];for(int i=0;i<5;i++)batches[i]=new List<CombineInstance>();
        int rounded=0,crates=0,barrels=0;
        foreach(var filter in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))
        {
            if(filter.GetComponentInParent<Health>()!=null)continue;
            var renderer=filter.GetComponent<MeshRenderer>();if(renderer==null||renderer.sharedMaterial==null)continue;
            string material=renderer.sharedMaterial.name;
            bool structural=material.Contains("Wall")||material.Contains("Floor")||material.Contains("Ceiling")||material.Contains("Sign")||material.Contains("Stripe")||material.Contains("Reader")||material.Contains("Trap");
            var scale=filter.transform.lossyScale;var dimensions=new Vector3(Mathf.Abs(scale.x),Mathf.Abs(scale.y),Mathf.Abs(scale.z));
            string meshPath=AssetDatabase.GetAssetPath(filter.sharedMesh);
            if(!structural && filter.sharedMesh!=null && filter.sharedMesh.name=="Cube" && !meshPath.StartsWith("Assets/") && Mathf.Min(dimensions.x,dimensions.y,dimensions.z)>.055f)
            {
                filter.sharedMesh=Bevel(dimensions);EditorUtility.SetDirty(filter);rounded++;
            }
            var b=renderer.bounds;int room=b.center.z< -18?0:b.center.z< -2?1:b.center.z>20?4:b.center.x<0?2:3;
            // Decorations follow axis-aligned original crate blocks, never isolated planks or signs.
            bool aligned=Mathf.Abs(Mathf.DeltaAngle(0,filter.transform.eulerAngles.y)%90)<.1f;
            if(material.Contains("Crate_Wood") && aligned && b.size.x>.45f && b.size.y>.45f && b.size.z>.45f)
            {
                foreach(float x in new[]{b.min.x+b.size.x*.2f,b.max.x-b.size.x*.2f})
                {
                    foreach(float z in new[]{b.min.z-.006f,b.max.z+.006f})
                    {
                        Add(batches[room],cube,new Vector3(x,b.center.y,z),new Vector3(.028f,b.size.y+.012f,.014f),Quaternion.identity);
                        foreach(float y in new[]{b.min.y+.07f,b.max.y-.07f})
                            Add(batches[room],bolt,new Vector3(x,y,z),new Vector3(.035f,.018f,.035f),Quaternion.Euler(90,0,0));
                    }
                    Add(batches[room],cube,new Vector3(x,b.max.y+.006f,b.center.z),new Vector3(.028f,.014f,b.size.z+.012f),Quaternion.identity);
                }
                crates++;
            }
            else if(material.Contains("Barrel") && b.size.y>.5f && b.size.x>.3f && b.size.z>.3f)
            {
                float radius=Mathf.Min(b.size.x,b.size.z)*.5f;
                foreach(float y in new[]{b.min.y+b.size.y*.22f,b.min.y+b.size.y*.78f})
                {
                    Mesh ring=Ring(radius+.008f,.012f);Add(batches[room],ring,new Vector3(b.center.x,y,b.center.z),Vector3.one,Quaternion.identity);
                }
                barrels++;
            }
        }
        for(int i=0;i<5;i++)
        {
            if(batches[i].Count==0)continue;
            var mesh=new Mesh();mesh.name="Room"+(i+1)+"_MetalTrim";mesh.indexFormat=IndexFormat.UInt32;mesh.CombineMeshes(batches[i].ToArray(),true,true);mesh.RecalculateBounds();
            var saved=Save(mesh,Folder+"/Room"+(i+1)+"_MetalTrim.asset");
            var go=new GameObject("Room"+(i+1)+"_Batched_Fasteners_And_Bands",typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(root,false);
            go.GetComponent<MeshFilter>().sharedMesh=saved;var r=go.GetComponent<MeshRenderer>();r.sharedMaterial=Mat("Mat_Steel_Dark");r.shadowCastingMode=ShadowCastingMode.Off;r.lightProbeUsage=LightProbeUsage.BlendProbes;
            GameObjectUtility.SetStaticEditorFlags(go,StaticEditorFlags.BatchingStatic|StaticEditorFlags.ReflectionProbeStatic);
        }
        foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            if(light.name.StartsWith("Room_Readability_Fill_")){light.intensity=1.65f;EditorUtility.SetDirty(light);}
        rounded=0;
        foreach(var filter in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))
            if(filter.sharedMesh!=null && filter.sharedMesh.name.StartsWith("Beveled_"))rounded++;
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        Debug.Log($"[SurfaceDetail] Beveled {rounded} props; {crates} strapped crates; {barrels} banded barrels; five combined trim batches maximum.");
        if(bakeLighting && !Lightmapping.Bake())throw new InvalidOperationException("Surface detail lighting bake failed.");
        foreach(var probe in UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None))
        {
            probe.resolution=256;string path="Assets/Scenes/Warehouse/"+probe.name+".exr";
            if(!Lightmapping.BakeReflectionProbe(probe,path))throw new InvalidOperationException("Reflection bake failed.");
            AssetDatabase.ImportAsset(path);probe.bakedTexture=AssetDatabase.LoadAssetAtPath<Texture>(path);
        }
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        File.WriteAllText("scratch/surface-detail-result.txt",$"Beveled props: {rounded}\nStrapped crates: {crates}\nBanded barrels: {barrels}\nReflectors: 256px baked\nColliders and gameplay unchanged.");
    }
    static void Texture(Material m,string texture,float bump,float smooth)
    {
        m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/"+texture+".png"));
        m.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/"+texture+"_Normal.png"));
        m.SetFloat("_BumpScale",bump);m.SetFloat("_Smoothness",smooth);m.EnableKeyword("_NORMALMAP");
    }
    static Mesh Save(Mesh mesh,string path)
    {
        var old=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(old==null){AssetDatabase.CreateAsset(mesh,path);return mesh;}
        EditorUtility.CopySerialized(mesh,old);UnityEngine.Object.DestroyImmediate(mesh);EditorUtility.SetDirty(old);return old;
    }
    static Mesh Bevel(Vector3 dimensions)
    {
        string key=$"{Mathf.RoundToInt(dimensions.x*1000)}_{Mathf.RoundToInt(dimensions.y*1000)}_{Mathf.RoundToInt(dimensions.z*1000)}";
        if(bevels.TryGetValue(key,out var cached))return cached;
        string path=Folder+"/Beveled_"+key+".asset";cached=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(cached!=null){bevels[key]=cached;return cached;}
        Vector3 h=dimensions*.5f;float radius=Mathf.Min(.018f,Mathf.Min(dimensions.x,dimensions.y,dimensions.z)*.12f);
        Vector3 inner=h-Vector3.one*radius;
        var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var indices=new List<int>();
        Vector3[] ns={Vector3.right,Vector3.left,Vector3.up,Vector3.down,Vector3.forward,Vector3.back};
        Vector3[] us={Vector3.up,Vector3.forward,Vector3.forward,Vector3.right,Vector3.right,Vector3.up};
        Vector3[] vs={Vector3.forward,Vector3.up,Vector3.right,Vector3.forward,Vector3.up,Vector3.right};
        for(int f=0;f<6;f++)
        {
            float hn=Vector3.Scale(ns[f],h).magnitude,hu=Vector3.Scale(us[f],h).magnitude,hv=Vector3.Scale(vs[f],h).magnitude;
            float[] a={-hu,-hu+radius,hu-radius,hu},b={-hv,-hv+radius,hv-radius,hv};int start=vertices.Count;
            for(int y=0;y<4;y++)for(int x=0;x<4;x++)
            {
                Vector3 p=ns[f]*hn+us[f]*a[x]+vs[f]*b[y];
                Vector3 q=new Vector3(Mathf.Clamp(p.x,-inner.x,inner.x),Mathf.Clamp(p.y,-inner.y,inner.y),Mathf.Clamp(p.z,-inner.z,inner.z));
                Vector3 n=(p-q).normalized;Vector3 v=q+n*radius;
                vertices.Add(new Vector3(v.x/dimensions.x,v.y/dimensions.y,v.z/dimensions.z));normals.Add(Vector3.Scale(n,dimensions).normalized);uv.Add(new Vector2(a[x]/(2*hu)+.5f,b[y]/(2*hv)+.5f));
            }
            for(int y=0;y<3;y++)for(int x=0;x<3;x++){int k=start+y*4+x;indices.AddRange(new[]{k,k+1,k+5,k,k+5,k+4});}
        }
        var mesh=new Mesh();mesh.name="Beveled_"+key;mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(indices,0);mesh.RecalculateTangents();mesh.RecalculateBounds();Unwrapping.GenerateSecondaryUVSet(mesh);
        bevels[key]=Save(mesh,path);return bevels[key];
    }
    static void Add(List<CombineInstance> list,Mesh mesh,Vector3 position,Vector3 scale,Quaternion rotation)=>list.Add(new CombineInstance{mesh=mesh,transform=Matrix4x4.TRS(position,rotation,scale)});
    static Mesh Ring(float radius,float tube)
    {
        var v=new List<Vector3>();var t=new List<int>();
        for(int a=0;a<=24;a++)for(int b=0;b<=6;b++){float p=a*Mathf.PI*2/24,q=b*Mathf.PI*2/6;v.Add(new Vector3(Mathf.Cos(p)*(radius+Mathf.Cos(q)*tube),Mathf.Sin(q)*tube,Mathf.Sin(p)*(radius+Mathf.Cos(q)*tube)));}
        for(int a=0;a<24;a++)for(int b=0;b<6;b++){int k=a*7+b;t.AddRange(new[]{k,k+8,k+7,k,k+1,k+8});}
        var mesh=new Mesh();mesh.SetVertices(v);mesh.SetTriangles(t,0);mesh.RecalculateNormals();return mesh;
    }
    static Mesh HexBolt()
    {
        var v=new List<Vector3>();var t=new List<int>();
        for(int y=0;y<2;y++)for(int a=0;a<6;a++){float r=a*Mathf.PI/3;v.Add(new Vector3(Mathf.Cos(r)*.5f,y-.5f,Mathf.Sin(r)*.5f));}
        for(int a=0;a<6;a++){int b=(a+1)%6;t.AddRange(new[]{a,a+6,b+6,a,b+6,b});}
        for(int a=1;a<5;a++){t.AddRange(new[]{0,a,a+1,6,6+a+1,6+a});}
        var mesh=new Mesh();mesh.SetVertices(v);mesh.SetTriangles(t,0);mesh.RecalculateNormals();return mesh;
    }
}
