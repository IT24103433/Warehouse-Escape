using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildWarehouseStandalone
{
    [MenuItem("Warehouse/Build Windows Release")]
    public static void ExecuteRelease()
    {
        string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string directory = Path.Combine(project, "Builds/WarehouseEscape_Windows");
        Directory.CreateDirectory(directory);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Warehouse.unity" },
            locationPathName = Path.Combine(directory, "WarehouseEscape.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        string result = $"Result: {report.summary.result}\nErrors: {report.summary.totalErrors}\nWarnings: {report.summary.totalWarnings}\nSize: {report.summary.totalSize}\n";
        Directory.CreateDirectory(Path.Combine(project, "scratch"));
        File.WriteAllText(Path.Combine(project, "scratch/windows-release-result.txt"), result);
        Debug.Log(result);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.InvalidOperationException("Windows release build failed.");
    }

    [MenuItem("Warehouse/Build Windows Standalone")]
    public static void Execute()
    {
        Debug.Log("[BuildWarehouseStandalone] Starting StandaloneWindows64 Development Build...");

        string buildDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds/WarehouseEscape_Dev"));
        if (!Directory.Exists(buildDir))
        {
            Directory.CreateDirectory(buildDir);
        }

        string exePath = Path.Combine(buildDir, "WarehouseEscape.exe");

        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
        var strayVerifier = GameObject.Find("[WinLosePlayModeVerifier]");
        bool dirty = false;
        if (strayVerifier != null)
        {
            Debug.Log("[BuildWarehouseStandalone] Removing obsolete [WinLosePlayModeVerifier] from scene...");
            Object.DestroyImmediate(strayVerifier);
            dirty = true;
        }
        var strayCaptureRunner = GameObject.Find("__CaptureRunner");
        if (strayCaptureRunner != null)
        {
            Object.DestroyImmediate(strayCaptureRunner);
            dirty = true;
        }
        if (dirty)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }

        string[] scenes = new string[] { scenePath };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = exePath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        string resultLog = $"BUILD_RESULT: {summary.result}\n" +
                           $"Total Time: {summary.totalTime}\n" +
                           $"Total Size: {summary.totalSize} bytes\n" +
                           $"Total Errors: {summary.totalErrors}\n" +
                           $"Total Warnings: {summary.totalWarnings}\n" +
                           $"Output: {exePath}\n";

        Debug.Log($"[BuildWarehouseStandalone] {resultLog}");
        string reportPath = Path.Combine(Application.dataPath, "../scratch/build_result.txt");
        File.WriteAllText(reportPath, resultLog);
    }
}
