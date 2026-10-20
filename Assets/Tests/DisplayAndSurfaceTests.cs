using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class DisplayAndSurfaceTests
{
    [UnityTest]
    public IEnumerator Pause_Menu_Selects_Both_Resolutions_And_Resumes()
    {
        SceneManager.LoadScene("Warehouse");yield return new WaitForSeconds(.6f);
        var gm=GameManager.Instance;gm.TogglePause();yield return null;
        var menu=Object.FindFirstObjectByType<PauseDisplaySettings>();Assert.IsNotNull(menu);
        Assert.IsNotNull(menu.Button720);Assert.IsNotNull(menu.Button1080);
        menu.Button1080.onClick.Invoke();yield return null;yield return null;
        Assert.IsTrue(PlayerHUD.Instance.Is1080p);
        Assert.IsFalse(PlayerHUD.Instance.CrosshairRoot.activeSelf);
        Canvas.ForceUpdateCanvases();
        var left=menu.Button720.GetComponent<RectTransform>();var right=menu.Button1080.GetComponent<RectTransform>();
        Assert.Greater(right.anchoredPosition.x-left.anchoredPosition.x,left.sizeDelta.x,"Resolution buttons cannot overlap.");
        var hitResults=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        pointer.position=RectTransformUtility.WorldToScreenPoint(null,left.position);
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointer,hitResults);
        Assert.Greater(hitResults.Count,0);Assert.AreEqual(menu.Button720.gameObject,hitResults[0].gameObject,"Pause buttons must receive pointer hits above the pause overlay.");
        var canvas=menu.GetComponentInParent<Canvas>();var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldPlane=canvas.planeDistance;
        var camera=GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();var oldTarget=camera.targetTexture;
        var rt=new RenderTexture(1920,1080,24);camera.targetTexture=rt;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=.5f;Canvas.ForceUpdateCanvases();
        try
        {
            yield return null;yield return null;
            var oldActive=RenderTexture.active;RenderTexture.active=rt;
            var screenshot=new Texture2D(1920,1080,TextureFormat.RGB24,false);screenshot.ReadPixels(new Rect(0,0,1920,1080),0,0);screenshot.Apply();RenderTexture.active=oldActive;
            Directory.CreateDirectory("scratch/surface-detail");File.WriteAllBytes("scratch/surface-detail/pause-display-1080p.png",screenshot.EncodeToPNG());Object.Destroy(screenshot);
        }
        finally {camera.targetTexture=oldTarget;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.planeDistance=oldPlane;rt.Release();Object.Destroy(rt);Canvas.ForceUpdateCanvases();}
        menu.Button720.onClick.Invoke();yield return null;yield return null;
        Assert.IsFalse(PlayerHUD.Instance.Is1080p);
        gm.TogglePause();yield return null;
        Assert.IsFalse(gm.IsPaused);Assert.IsFalse(menu.gameObject.activeInHierarchy);
        Assert.IsTrue(PlayerHUD.Instance.CrosshairRoot.activeSelf);
    }
    [UnityTest]
    public IEnumerator Prop_Detail_Is_Batched_And_Texture_Quality_Is_Full()
    {
        SceneManager.LoadScene("Warehouse");yield return new WaitForSeconds(.5f);
        var root=GameObject.Find("Warehouse_Surface_Details");Assert.IsNotNull(root);
        Assert.AreEqual(0,root.GetComponentsInChildren<Collider>(true).Length,"Cosmetic trim cannot block paths.");
        Assert.LessOrEqual(root.GetComponentsInChildren<MeshRenderer>().Length,5);
        int beveled=0;
        foreach(var filter in Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))
            if(filter.sharedMesh!=null && filter.sharedMesh.name.StartsWith("Beveled_"))beveled++;
        Assert.GreaterOrEqual(beveled,300);
        Assert.AreEqual(0,QualitySettings.globalTextureMipmapLimit);
        Assert.AreEqual(AnisotropicFiltering.Enable,QualitySettings.anisotropicFiltering);
        foreach(var probe in Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None))
        {Assert.AreEqual(256,probe.resolution);Assert.IsNotNull(probe.bakedTexture);}
        var floor=GameObject.Find("Room_2_Patrol").transform.Find("Floor").GetComponent<MeshRenderer>();
        var texture=floor.sharedMaterial.GetTexture("_BaseMap");Assert.IsNotNull(texture);
        Assert.AreEqual(FilterMode.Trilinear,texture.filterMode);Assert.GreaterOrEqual(texture.anisoLevel,8);
    }
}
