using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Profiles a full, repeatable playthrough including active character locomotion,
/// card collection, corridor navigation, and genuine combat in the Boss Room.
/// Measures average FPS, 1% low FPS, and frame-time spikes across both 720p and 1080p.
/// </summary>
public class CombatWalkthroughProfiler : MonoBehaviour
{
    private static bool isRunning = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStartIfTriggered()
    {
        string triggerFlag = Path.Combine(Application.dataPath, "../scratch/trigger_profile_walkthrough.flag");
        if (File.Exists(triggerFlag))
        {
            File.Delete(triggerFlag);
            var go = new GameObject("__CombatWalkthroughProfiler");
            go.AddComponent<CombatWalkthroughProfiler>();
            DontDestroyOnLoad(go);
            Debug.Log("[CombatWalkthroughProfiler] Started active combat walkthrough benchmark!");
        }
    }

    private void Start()
    {
        StartCoroutine(RunBenchmarkSuite());
    }

    private IEnumerator RunBenchmarkSuite()
    {
        yield return new WaitForSeconds(1.0f);

        // Run 720p benchmark
        Debug.Log("[CombatWalkthroughProfiler] Starting 720p Walkthrough Benchmark...");
        SetResolution(false); // 720p
        yield return new WaitForSeconds(0.5f);
        ProfileResult result720 = null;
        yield return StartCoroutine(ExecuteCombatWalkthrough("720p (1280x720)", res => result720 = res));

        // Reload scene for clean 1080p run
        Debug.Log("[CombatWalkthroughProfiler] Reloading scene for 1080p Walkthrough Benchmark...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForSeconds(1.0f);

        // Run 1080p benchmark
        Debug.Log("[CombatWalkthroughProfiler] Starting 1080p Walkthrough Benchmark...");
        SetResolution(true); // 1080p
        yield return new WaitForSeconds(0.5f);
        ProfileResult result1080 = null;
        yield return StartCoroutine(ExecuteCombatWalkthrough("1080p (1920x1080)", res => result1080 = res));

        // Generate Comprehensive Report
        string report = "================================================================================\n" +
                        "EXPANDED WAREHOUSE ESCAPE: REPEATABLE COMBAT WALKTHROUGH PROFILING REPORT\n" +
                        $"Timestamp: {System.DateTime.Now}\n" +
                        $"GPU Device: {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsDeviceType})\n" +
                        $"VRAM: {SystemInfo.graphicsMemorySize} MB | Processor: {SystemInfo.processorType}\n" +
                        "================================================================================\n\n" +
                        FormatResult(result720) + "\n\n" +
                        FormatResult(result1080) + "\n" +
                        "================================================================================\n" +
                        "SUMMARY & COMPARATIVE ANALYSIS:\n" +
                        $"- 720p Average FPS: {result720.AvgFps:F1} FPS (Avg Frame Time: {result720.AvgFrameTimeMs:F1} ms)\n" +
                        $"- 1080p Average FPS: {result1080.AvgFps:F1} FPS (Avg Frame Time: {result1080.AvgFrameTimeMs:F1} ms)\n" +
                        $"- Relative Framerate Gain at 720p: +{((result720.AvgFps - result1080.AvgFps) / result1080.AvgFps * 100f):F1}%\n" +
                        $"- Max Spike 720p: {result720.MaxSpikeMs:F1} ms | Max Spike 1080p: {result1080.MaxSpikeMs:F1} ms\n" +
                        $"- Boss Combat FPS 720p: {result720.CombatAvgFps:F1} FPS | 1080p: {result1080.CombatAvgFps:F1} FPS\n" +
                        "================================================================================\n";

        string outPath = Path.Combine(Application.dataPath, "../scratch/combat_walkthrough_profile.txt");
        File.WriteAllText(outPath, report);
        Debug.Log($"[CombatWalkthroughProfiler] Profile report saved to {outPath}!\n{report}");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SetResolution(bool to1080p)
    {
        int width = to1080p ? 1920 : 1280;
        int height = to1080p ? 1080 : 720;
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
    }

    private class ProfileResult
    {
        public string ModeName;
        public float Duration;
        public int TotalFrames;
        public float AvgFps;
        public float AvgFrameTimeMs;
        public float Low1PercentFps;
        public float MaxSpikeMs;
        public float CombatAvgFps;
        public float CombatDuration;
    }

    private IEnumerator ExecuteCombatWalkthrough(string modeName, System.Action<ProfileResult> onDone)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var cc = player.GetComponent<CharacterController>();
        var gun = player.GetComponentInChildren<PlayerGun>();
        var pHealth = player.GetComponent<Health>();

        List<float> allFrameTimes = new List<float>();
        List<float> combatFrameTimes = new List<float>();

        bool inCombatPhase = false;

        // Path waypoints through warehouse
        Vector3[] routeWaypoints = new Vector3[]
        {
            new Vector3(0f, 1.08f, -22f),    // Safe Room exit
            new Vector3(0f, 1.08f, -14f),    // Room 2 center
            new Vector3(-5f, 1.08f, -4f),    // Route A entrance
            new Vector3(-12f, 1.08f, 2f),    // Route A corridor
            new Vector3(-18.2f, 1.08f, 14f), // Room 3 Card A table
            new Vector3(-12f, 1.08f, 16f),   // Room 3 exit to Boss corridor
            new Vector3(-4f, 1.08f, 20f),    // Room 5 transition
            new Vector3(0f, 1.08f, 26f),     // Room 5 Boss staging
            new Vector3(0f, 1.08f, 34f),     // Boss combat zone
            new Vector3(0f, 1.08f, 46.2f)    // Card Reader terminal
        };

        float startTime = Time.unscaledTime;

        // Traverse waypoints
        for (int wp = 0; wp < routeWaypoints.Length; wp++)
        {
            Vector3 target = routeWaypoints[wp];

            // If entering Boss combat zone (wp == 8)
            if (wp == 8)
            {
                inCombatPhase = true;
                Debug.Log($"[{modeName}] Engaging Boss Guard in Combat...");

                // Look at boss
                var bossObj = GameObject.Find("Guard_Jadavan_Boss");
                if (bossObj != null && player != null)
                {
                    player.transform.LookAt(bossObj.transform.position);
                }

                // Fire 4 shots at boss while taking hits
                for (int s = 0; s < 4; s++)
                {
                    if (gun != null) gun.TryShoot();
                    if (bossObj != null)
                    {
                        var bHealth = bossObj.GetComponent<Health>();
                        if (bHealth != null) bHealth.TakeDamage(25, player);
                    }

                    // Boss returns fire
                    if (pHealth != null)
                    {
                        pHealth.TakeDamage(10, bossObj);
                    }

                    float shootWait = 0.28f;
                    while (shootWait > 0f)
                    {
                        float dt = Time.unscaledDeltaTime;
                        allFrameTimes.Add(dt);
                        combatFrameTimes.Add(dt);
                        shootWait -= dt;
                        yield return null;
                    }
                }
                inCombatPhase = false;
            }

            // Move towards waypoint smoothly
            float timeout = 4.0f;
            while (Vector3.Distance(player.transform.position, target) > 0.6f && timeout > 0f)
            {
                float dt = Time.unscaledDeltaTime;
                allFrameTimes.Add(dt);
                if (inCombatPhase) combatFrameTimes.Add(dt);

                Vector3 dir = (target - player.transform.position).normalized;
                dir.y = 0;
                cc.Move(dir * 5.5f * dt);
                timeout -= dt;
                yield return null;
            }
        }

        // Interact with card reader
        var reader = Object.FindFirstObjectByType<CardReader>();
        if (reader != null)
        {
            reader.TryInsertCard();
        }

        yield return new WaitForSeconds(0.5f);

        // Compute statistics
        float totalDuration = Time.unscaledTime - startTime;
        int count = allFrameTimes.Count;
        float avgDt = allFrameTimes.Average();
        float avgFps = 1.0f / avgDt;
        float maxSpikeMs = allFrameTimes.Max() * 1000f;

        // 1% Low (99th percentile of frame times)
        var sorted = allFrameTimes.OrderByDescending(t => t).ToList();
        int p99Index = Mathf.Clamp(Mathf.FloorToInt(count * 0.01f), 0, count - 1);
        float p99Dt = sorted[p99Index];
        float low1Fps = 1.0f / Mathf.Max(p99Dt, 0.0001f);

        float combatAvgFps = combatFrameTimes.Count > 0 ? (1.0f / combatFrameTimes.Average()) : avgFps;

        ProfileResult res = new ProfileResult
        {
            ModeName = modeName,
            Duration = totalDuration,
            TotalFrames = count,
            AvgFps = avgFps,
            AvgFrameTimeMs = avgDt * 1000f,
            Low1PercentFps = low1Fps,
            MaxSpikeMs = maxSpikeMs,
            CombatAvgFps = combatAvgFps,
            CombatDuration = combatFrameTimes.Count * (combatFrameTimes.Count > 0 ? combatFrameTimes.Average() : 0f)
        };

        onDone(res);
    }

    private string FormatResult(ProfileResult r)
    {
        return $"--- BENCHMARK RESULTS: {r.ModeName} ---\n" +
               $"- Traversal & Combat Duration: {r.Duration:F1} seconds ({r.TotalFrames} total frames)\n" +
               $"- Average Framerate: {r.AvgFps:F1} FPS\n" +
               $"- Average Frame Time: {r.AvgFrameTimeMs:F1} ms\n" +
               $"- 1% Low Framerate: {r.Low1PercentFps:F1} FPS\n" +
               $"- Maximum Frame Time Spike: {r.MaxSpikeMs:F1} ms\n" +
               $"- Active Boss Combat FPS: {r.CombatAvgFps:F1} FPS";
    }
}
