using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class TestRecompileTrigger
{
    [InitializeOnLoadMethod]
    static void OnLoaded()
    {
        string outPath = Path.Combine(Application.dataPath, "../scratch/editor_recompile_test.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
        File.WriteAllText(outPath, $"Recompiled at {System.DateTime.Now}\nScene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}\nIsPlaying: {EditorApplication.isPlaying}");
        // Trigger run timestamp: 14:31
        Debug.Log("[TestRecompileTrigger] Unity Editor successfully executed OnLoaded!");

        EditorApplication.update -= UpdateLoop;
        EditorApplication.update += UpdateLoop;

        CheckFlags();
    }

    static void UpdateLoop()
    {
        CheckFlags();
    }

    static void CheckFlags()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;

        string triggerFix = Path.Combine(Application.dataPath, "../scratch/trigger_fix_room4.flag");
        if (File.Exists(triggerFix))
        {
            File.Delete(triggerFix);
            Debug.Log("[TestRecompileTrigger] Calling FixRoom4AndSaveScene.Execute()...");
            FixRoom4AndSaveScene.Execute();
            return;
        }

        string triggerCapture = Path.Combine(Application.dataPath, "../scratch/trigger_capture_playmode.flag");
        if (File.Exists(triggerCapture))
        {
            // Do not delete triggerCapture here so RuntimeInitializeOnLoadMethod can see it,
            // or write run_capture_now.flag
            File.Delete(triggerCapture);
            File.WriteAllText(Path.Combine(Application.dataPath, "../scratch/run_capture_now.flag"), "1");
            Debug.Log("[TestRecompileTrigger] Setting up PlayModeCaptureRunner and entering Play Mode...");

            if (!EditorApplication.isPlaying)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Warehouse")
                    EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
                EditorApplication.isPlaying = true;
            }
            return;
        }

        string triggerProfile = Path.Combine(Application.dataPath, "../scratch/trigger_profile_walkthrough.flag");
        if (File.Exists(triggerProfile))
        {
            File.Delete(triggerProfile);
            File.WriteAllText(Path.Combine(Application.dataPath, "../scratch/run_profile_now.flag"), "1");
            Debug.Log("[TestRecompileTrigger] Setting up CombatWalkthroughProfiler and entering Play Mode...");

            if (!EditorApplication.isPlaying)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Warehouse")
                    EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity", OpenSceneMode.Single);
                EditorApplication.isPlaying = true;
            }
            return;
        }

        string triggerTests = Path.Combine(Application.dataPath, "../scratch/trigger_run_tests.flag");
        if (File.Exists(triggerTests))
        {
            File.Delete(triggerTests);
            Debug.Log("[TestRecompileTrigger] Calling RunPlayModeTestsInEditor.RunAllTests()...");
            RunPlayModeTestsInEditor.RunAllTests();
            return;
        }

        string triggerAssignWeapons = Path.Combine(Application.dataPath, "../scratch/trigger_assign_weapons.flag");
        if (File.Exists(triggerAssignWeapons))
        {
            File.Delete(triggerAssignWeapons);
            Debug.Log("[TestRecompileTrigger] Calling AssignWeaponPrefabsAndSaveScene.Execute()...");
            AssignWeaponPrefabsAndSaveScene.Execute();
            return;
        }

        string triggerBuild = Path.Combine(Application.dataPath, "../scratch/trigger_build.flag");
        if (File.Exists(triggerBuild))
        {
            File.Delete(triggerBuild);
            Debug.Log("[TestRecompileTrigger] Calling BuildWarehouseStandalone.Execute()...");
            BuildWarehouseStandalone.Execute();
            return;
        }

        string triggerInspect = Path.Combine(Application.dataPath, "../scratch/trigger_inspect.flag");
        if (File.Exists(triggerInspect))
        {
            File.Delete(triggerInspect);
            Debug.Log("[TestRecompileTrigger] Calling InspectLightsAndMaterials.Execute()...");
            InspectLightsAndMaterials.Execute();
            return;
        }

        string triggerInspectGuards = Path.Combine(Application.dataPath, "../scratch/trigger_inspect_guards.flag");
        if (File.Exists(triggerInspectGuards))
        {
            File.Delete(triggerInspectGuards);
            Debug.Log("[TestRecompileTrigger] Calling InspectGuardPositions.Execute()...");
            InspectGuardPositions.Execute();
            return;
        }

        string triggerIgi = Path.Combine(Application.dataPath, "../scratch/trigger_igi_pass.flag");
        if (File.Exists(triggerIgi))
        {
            File.Delete(triggerIgi);
            Debug.Log("[TestRecompileTrigger] Calling ApplyIGIVisualAndLightingPass.Execute()...");
            ApplyIGIVisualAndLightingPass.Execute();
            return;
        }
    }
}

// build trigger touch

// run tests touch

// inspect trigger touch

// inspect trigger touch

// igi trigger touch
// touch igi

// igi bake touch

// igi bake touch

// 13-Oct-26 8:58:32 PM

// test trigger probe
