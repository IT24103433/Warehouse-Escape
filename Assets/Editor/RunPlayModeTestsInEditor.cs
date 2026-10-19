using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayModeTestCallbacks : ScriptableObject, ICallbacks
{
    public void RunStarted(ITestAdaptor testsToRun)
    {
        Debug.Log($"[TestRunnerApi] Run started: {testsToRun.Name}");
    }

    public void RunFinished(ITestResultAdaptor result)
    {
        Debug.Log($"[TestRunnerApi] Run finished. ResultState: {result.ResultState}, PassCount: {result.PassCount}, FailCount: {result.FailCount}");
        string summary = $"TEST_RUN_FINISHED\nResultState: {result.ResultState}\nPassed: {result.PassCount}\nFailed: {result.FailCount}\nDuration: {result.Duration}\n";
        string resultDirectory = Path.Combine(Application.dataPath, "../scratch");
        Directory.CreateDirectory(resultDirectory);
        TestRunnerApi.SaveResultToFile(result, Path.Combine(resultDirectory, "editor-tests.xml"));
        string persistentXml = Path.Combine(Application.persistentDataPath, "TestResults.xml");
        if (File.Exists(persistentXml))
        {
            try {
                File.Copy(persistentXml, Path.Combine(resultDirectory, "verification-results.xml"), true);
                File.Copy(persistentXml, Path.Combine(resultDirectory, "codex-verification-results.xml"), true);
            } catch {}
        }
        File.WriteAllText(Path.Combine(resultDirectory, "tests_finished.flag"), summary);
    }

    public void TestStarted(ITestAdaptor test)
    {
        if (!test.HasChildren)
            Debug.Log($"[TestRunnerApi] Test started: {test.Name}");
    }

    public void TestFinished(ITestResultAdaptor result)
    {
        if (!result.Test.HasChildren)
            Debug.Log($"[TestRunnerApi] Test finished: {result.Test.Name} -> {result.TestStatus}");
    }
}

public static class RunPlayModeTestsInEditor
{
    private static TestRunnerApi s_Api;
    private static PlayModeTestCallbacks s_Callbacks;

    [MenuItem("Warehouse/Run Physical Walkthrough Tests")]
    public static void RunAllTests()
    {
        AssetDatabase.Refresh();
        if (EditorApplication.isCompiling)
        {
            Debug.Log("[RunPlayModeTestsInEditor] Editor is compiling. Waiting...");
            EditorApplication.update -= WaitForReady;
            EditorApplication.update += WaitForReady;
            return;
        }

        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("[RunPlayModeTestsInEditor] Editor is in playmode or transitioning. Stopping playmode before running tests...");
            if (EditorApplication.isPlaying) EditorApplication.isPlaying = false;
            EditorApplication.update -= WaitForReady;
            EditorApplication.update += WaitForReady;
            return;
        }

        // Preserve editor work before tests load/reload scenes.
        EditorSceneManager.SaveOpenScenes();

        // Clean any leftover temporary test scenes
        string[] tempScenes = Directory.GetFiles(Application.dataPath, "InitTestScene*.*");
        foreach (var s in tempScenes)
        {
            try { File.Delete(s); } catch { }
        }

        if (SceneManager.GetActiveScene().name != "Warehouse")
        {
            Debug.Log("[RunPlayModeTestsInEditor] Opening Warehouse.unity before running tests...");
            EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
        }

        Debug.Log("[RunPlayModeTestsInEditor] Triggering PhysicalWalkthroughTests via TestRunnerApi...");
        if (s_Api == null)
            s_Api = ScriptableObject.CreateInstance<TestRunnerApi>();
        if (s_Callbacks == null)
            s_Callbacks = ScriptableObject.CreateInstance<PlayModeTestCallbacks>();

        s_Api.UnregisterCallbacks(s_Callbacks);
        s_Api.RegisterCallbacks(s_Callbacks);

        var filter = new Filter()
        {
            testMode = TestMode.PlayMode,
            groupNames = File.Exists("scratch/test-filter.txt")
                ? File.ReadAllText("scratch/test-filter.txt").Trim().Split(';')
                : new string[] { "PhysicalWalkthroughTests", "InformedGuardDecisionTests" }
        };

        s_Api.Execute(new ExecutionSettings(filter));
    }

    private static void WaitForReady()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
        EditorApplication.update -= WaitForReady;
        RunAllTests();
    }

    [InitializeOnLoadMethod]
    static void InitStateListener()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            string persistentXml = Path.Combine(Application.persistentDataPath, "TestResults.xml");
            string resultDirectory = Path.Combine(Application.dataPath, "../scratch");
            if (File.Exists(persistentXml))
            {
                try
                {
                    Directory.CreateDirectory(resultDirectory);
                    File.Copy(persistentXml, Path.Combine(resultDirectory, "verification-results.xml"), true);
                    File.Copy(persistentXml, Path.Combine(resultDirectory, "codex-verification-results.xml"), true);
                }
                catch { }
            }
        }
    }
}
