#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>Opt-in development capture; never runs during ordinary gameplay.</summary>
public class CombatPresentationCapture : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (!System.Array.Exists(System.Environment.GetCommandLineArgs(), a => a == "-capture-combat-polish")) return;
        new GameObject("CombatPresentationCapture").AddComponent<CombatPresentationCapture>();
    }

    private IEnumerator Start()
    {
        Application.runInBackground = true;
        Screen.SetResolution(1280, 720, false);
        yield return new WaitForSecondsRealtime(2);
        var cam = Camera.main;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent<PlayerMovement>(out var movement)) movement.enabled = false;
        foreach (var ai in Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            if (ai is IGuardPerceptionTracker || ai is GuardShooting || ai is GuardPatrol || ai is EnemyMovement)
                ai.enabled = false;
        foreach (var agent in Object.FindObjectsByType<UnityEngine.AI.NavMeshAgent>(FindObjectsSortMode.None))
            if (agent.isOnNavMesh) agent.isStopped = true;
        var output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../scratch/combat-polish"));
        Directory.CreateDirectory(output);
        yield return Capture(output, "01-player-mp5");
        var gun = player != null ? player.GetComponentInChildren<PlayerGun>() : null;
        if (gun != null)
        {
            gun.enabled = false;
            if (gun.GunTransform != null) gun.GunTransform.gameObject.SetActive(false);
        }
        foreach (var hud in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None)) hud.enabled = false;
        yield return FrameTarget(cam, "Guard_Varun_Patrol", 2.4f, 1.25f);
        yield return Capture(output, "02-human-guard");
        yield return FrameTarget(cam, "AccessCard_Pickup_Search", .7f, .10f);
        yield return Capture(output, "03-card-a");
        yield return FrameTarget(cam, "AccessCard_Pickup_Defensive", .7f, .10f);
        yield return Capture(output, "04-card-b");
        File.WriteAllText(Path.Combine(output, "complete.txt"), "Four staged offscreen URP captures, 1280x720. Overlay HUD excluded. Guards frozen for inspection; not a gameplay walkthrough.");
        Application.Quit();
    }

    private IEnumerator FrameTarget(Camera cam, string name, float distance, float height)
    {
        var target = GameObject.Find(name);
        if (target == null) throw new System.InvalidOperationException("Missing visual target " + name);
        bool isCard = target.TryGetComponent<AccessCard>(out var card);
        if (isCard) card.enabled = false;
        var focus = target.transform.position + Vector3.up * height;
        var animator = target.GetComponentInChildren<HumanoidAnimator>();
        if (animator != null && animator.head != null) focus = animator.head.position + Vector3.down * .2f;
        if (isCard) focus = target.transform.position;
        cam.transform.position = isCard ? focus + Vector3.up * .65f + Vector3.forward * .15f
            : focus + target.transform.forward * distance + Vector3.up * .15f;
        cam.transform.LookAt(focus);
        cam.nearClipPlane = .03f;
        yield return new WaitForSecondsRealtime(.4f);
    }

    private IEnumerator Capture(string output, string name)
    {
        yield return new WaitForEndOfFrame();
        var camera = Camera.main;
        var target = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
        target.Create();
        var previous = RenderTexture.active;
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        try
        {
            RenderPipeline.SubmitRenderRequest(camera,
                new UniversalRenderPipeline.SingleCameraRequest { destination = target });
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply();
            File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active = previous;
            target.Release();
            Destroy(target);
            Destroy(image);
        }
        yield return new WaitForSecondsRealtime(.4f);
    }
}
#endif
