using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class VisualQualityCaptureTests
{
    [UnityTest]
    public IEnumerator Capture_Five_Rooms_With_Real_Player_Camera()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.7f);
        foreach (var s in Object.FindObjectsByType<GuardShooting>(FindObjectsSortMode.None)) s.enabled=false;
        foreach (var s in Object.FindObjectsByType<GuardCapture>(FindObjectsSortMode.None)) s.enabled=false;
        foreach (var s in Object.FindObjectsByType<WarehouseTrap>(FindObjectsSortMode.None)) s.enabled=false;
        var details=GameObject.Find("Cinematic_Visual_Details");
        if(details!=null)
        {
            Assert.AreEqual(0,details.GetComponentsInChildren<Collider>(true).Length,"Visual art must not add collision blockers.");
            Assert.GreaterOrEqual(details.transform.childCount,16,"Blender panels and conduits cover the rooms.");
            Assert.Greater(LightmapSettings.lightmaps.Length,0,"Static lighting bake must be present.");
            int shadowLights=0;
            foreach(var light in Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
                if(light.enabled && light.shadows!=LightShadows.None)shadowLights++;
            Assert.LessOrEqual(shadowLights,3,"Keep the existing limited shadow budget.");
            foreach(var probe in Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None)) Assert.IsNotNull(probe.bakedTexture);
        }
        var player=GameObject.FindGameObjectWithTag("Player");
        var pm=player.GetComponent<PlayerMovement>(); pm.enabled=false;
        player.GetComponent<CharacterController>().enabled=false;
        var cam=player.GetComponentInChildren<Camera>();
        string root=Path.GetFullPath(Path.Combine(Application.dataPath,"../scratch/visual-upgrade"));
        string stage=File.Exists(root+"/stage.txt") ? File.ReadAllText(root+"/stage.txt").Trim() : "after";
        Directory.CreateDirectory(root+"/"+stage);
        Vector3[] points={new Vector3(-3,1.08f,-28),new Vector3(0,1.08f,-16),new Vector3(-16,1.08f,2),new Vector3(16,1.08f,2),new Vector3(-5.5f,1.08f,25)};
        Vector3[] targets={new Vector3(0,1.6f,-20),new Vector3(0,1.6f,-5),new Vector3(-16,1.6f,13),new Vector3(16,1.6f,12),new Vector3(0,1.6f,42)};
        var rt=new RenderTexture(1920,1080,24,RenderTextureFormat.ARGB32);
        cam.targetTexture=rt;
        try
        {
            for(int i=0;i<points.Length;i++)
            {
                player.transform.position=points[i]; pm.AimAt(targets[i]);
                Physics.SyncTransforms();
                yield return null; yield return null; yield return null;
                var old=RenderTexture.active; RenderTexture.active=rt;
                var tex=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);
                tex.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0); tex.Apply();
                File.WriteAllBytes(root+"/"+stage+"/room"+(i+1)+".png",tex.EncodeToPNG());
                int visible=0; var colors=tex.GetPixels32();
                for(int p=0;p<colors.Length;p+=64) if(colors[p].r+colors[p].g+colors[p].b>30) visible++;
                Assert.Greater(visible,200,"Room render must contain visible scene detail.");
                Object.Destroy(tex); RenderTexture.active=old;
            }
        }
        finally { cam.targetTexture=null; rt.Release(); Object.Destroy(rt); }
    }
}
