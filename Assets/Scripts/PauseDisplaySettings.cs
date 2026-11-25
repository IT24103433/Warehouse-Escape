using UnityEngine;
using UnityEngine.UI;

/// <summary>Discoverable resolution choices in the tactical pause menu.</summary>
public class PauseDisplaySettings : MonoBehaviour
{
    public Button Button720 { get; private set; }
    public Button Button1080 { get; private set; }
    private Text status;
    private bool? lastMode;
    void OnEnable()
    {
        if(status==null)
        {
            status=Label("Resolution_Status",.52f,16);
            Button720=Choice("Resolution_720p","1280 × 720",-82f,false);
            Button1080=Choice("Resolution_1080p","1920 × 1080",82f,true);
            Label("Resolution_Help",.08f,13).text="F1 also switches resolution • Fullscreen stays enabled";
        }
        lastMode=null;
    }
    void Update()
    {
        var hud=PlayerHUD.Instance;if(hud==null)return;
        if(lastMode==hud.Is1080p)return;
        lastMode=hud.Is1080p;
        status.text="DISPLAY RESOLUTION: "+(hud.Is1080p?"1080p":"720p");
        Button720.GetComponent<Image>().color=!hud.Is1080p?new Color(.18f,.52f,.66f):new Color(.13f,.18f,.24f);
        Button1080.GetComponent<Image>().color=hud.Is1080p?new Color(.18f,.52f,.66f):new Color(.13f,.18f,.24f);
    }
    Text Label(string name,float anchor,float size)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(transform,false);
        var rt=go.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=new Vector2(.5f,anchor);rt.sizeDelta=new Vector2(410,26);
        var text=go.GetComponent<Text>();text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=(int)size;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;text.raycastTarget=false;return text;
    }
    Button Choice(string name,string title,float x,bool hd)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(transform,false);
        var rt=go.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=new Vector2(.5f,.29f);rt.anchoredPosition=new Vector2(x,0);rt.sizeDelta=new Vector2(150,40);
        var button=go.GetComponent<Button>();button.onClick.AddListener(()=>{if(PlayerHUD.Instance!=null)PlayerHUD.Instance.SetResolution(hd);});
        var label=new GameObject("Label",typeof(RectTransform),typeof(Text));label.transform.SetParent(go.transform,false);
        var lr=label.GetComponent<RectTransform>();lr.anchorMin=Vector2.zero;lr.anchorMax=Vector2.one;lr.offsetMin=lr.offsetMax=Vector2.zero;
        var text=label.GetComponent<Text>();text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=18;text.fontStyle=FontStyle.Bold;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;text.text=title;text.raycastTarget=false;
        return button;
    }
}
