using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildWarehouseStandalone
{
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

        string[] scenes = new string[] { "Assets/Scenes/Warehouse.unity" };

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
