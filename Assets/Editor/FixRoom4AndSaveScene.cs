using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixRoom4AndSaveScene
{
    [MenuItem("Warehouse/Fix Room 4 And Save Scene")]
    public static void Execute()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.Log("[FixRoom4AndSaveScene] Exiting Play Mode to perform scene edit...");
            EditorApplication.isPlaying = false;
            EditorApplication.delayCall += () => Execute();
            return;
        }

        Debug.Log("[FixRoom4AndSaveScene] Starting Room 4 fix and scene update...");

        string scenePath = "Assets/Scenes/Warehouse.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[FixRoom4AndSaveScene] Failed to open scene {scenePath}");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== ROOM 4 FIX & SCENE VERIFICATION REPORT ===");

        // 1. Find and destroy [Placeholder]_Guard_CharacterMesh and Mannequin_Body in Room 4
        var room4 = GameObject.Find("Room_4_Defensive");
        if (room4 != null)
        {
            var placeholders = room4.transform.Find("[Aqqil_Placeholders]");
            if (placeholders != null)
            {
                var placeholderGuard = placeholders.Find("[Placeholder]_Guard_CharacterMesh");
                if (placeholderGuard != null)
                {
                    sb.AppendLine($"[ACTION] Found placeholder guard mesh '{placeholderGuard.name}' under [Aqqil_Placeholders] at pos {placeholderGuard.position}. Destroying permanently.");
                    Object.DestroyImmediate(placeholderGuard.gameObject);
                }
            }
        }

        // Global sweep for any other placeholder guard objects
        var globalPlaceholder = GameObject.Find("[Placeholder]_Guard_CharacterMesh");
        if (globalPlaceholder != null)
        {
            sb.AppendLine($"[ACTION] Found global [Placeholder]_Guard_CharacterMesh at pos {globalPlaceholder.transform.position}. Destroying.");
            Object.DestroyImmediate(globalPlaceholder);
        }

        var mannequin = GameObject.Find("Mannequin_Body");
        if (mannequin != null)
        {
            sb.AppendLine($"[ACTION] Found Mannequin_Body at pos {mannequin.transform.position}. Destroying.");
            Object.DestroyImmediate(mannequin);
        }

        // 2. Fix Guard_Aqqil_Defensive
        var aqqilObj = GameObject.Find("Guard_Aqqil_Defensive");
        if (aqqilObj == null)
        {
            sb.AppendLine("[ERROR] Guard_Aqqil_Defensive not found in scene!");
        }
        else
        {
            sb.AppendLine($"\n[VERIFY] Guard_Aqqil_Defensive found at {aqqilObj.transform.position}, rot={aqqilObj.transform.eulerAngles}");

            // Remove root MeshRenderer / MeshFilter
            var mr = aqqilObj.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                sb.AppendLine("   [ACTION] Destroying root MeshRenderer on Guard_Aqqil_Defensive.");
                Object.DestroyImmediate(mr);
            }
            var mf = aqqilObj.GetComponent<MeshFilter>();
            if (mf != null)
            {
                sb.AppendLine("   [ACTION] Destroying root MeshFilter on Guard_Aqqil_Defensive.");
                Object.DestroyImmediate(mf);
            }

            // Verify essential components
            var col = aqqilObj.GetComponent<CapsuleCollider>();
            sb.AppendLine($"   [COMPONENT] CapsuleCollider: present={col != null}, isTrigger={col?.isTrigger}, center={col?.center}, height={col?.height}");
            var agent = aqqilObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
            sb.AppendLine($"   [COMPONENT] NavMeshAgent: present={agent != null}, speed={agent?.speed}");
            var ai = aqqilObj.GetComponent<DefensiveGuardAI>();
            sb.AppendLine($"   [COMPONENT] DefensiveGuardAI: present={ai != null}, postPos={ai?.PostPosition}, defenseRadius={ai?.DefenseRadius}, leashDistance={ai?.LeashDistance}");
            var cap = aqqilObj.GetComponent<GuardCapture>();
            sb.AppendLine($"   [COMPONENT] GuardCapture: present={cap != null}, range={cap?.CaptureRange}, name='{cap?.GuardDisplayName}'");
            var mov = aqqilObj.GetComponent<EnemyMovement>();
            sb.AppendLine($"   [COMPONENT] EnemyMovement: present={mov != null}");

            // Build persistent humanoid visual
            HumanoidMeshBuilder.BuildHumanoidVisual(aqqilObj.transform, CharacterRole.Defensive_Aqqil);
            sb.AppendLine("   [VISUAL] Built Character_Visuals with persistent mesh assets attached directly to Guard_Aqqil_Defensive.");
        }

        // 3. Upgrade all other characters with persistent mesh assets
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        if (varunObj != null)
        {
            var mr = varunObj.GetComponent<MeshRenderer>();
            if (mr != null) Object.DestroyImmediate(mr);
            var mf = varunObj.GetComponent<MeshFilter>();
            if (mf != null) Object.DestroyImmediate(mf);
            HumanoidMeshBuilder.BuildHumanoidVisual(varunObj.transform, CharacterRole.Patrol_Varun);
            sb.AppendLine("[VISUAL] Built Character_Visuals for Guard_Varun_Patrol.");
        }

        var sanjeevObj = GameObject.Find("Guard_Sanjeevsaran_Search");
        if (sanjeevObj != null)
        {
            var mr = sanjeevObj.GetComponent<MeshRenderer>();
            if (mr != null) Object.DestroyImmediate(mr);
            var mf = sanjeevObj.GetComponent<MeshFilter>();
            if (mf != null) Object.DestroyImmediate(mf);
            HumanoidMeshBuilder.BuildHumanoidVisual(sanjeevObj.transform, CharacterRole.Search_Sanjeevsaran);
            sb.AppendLine("[VISUAL] Built Character_Visuals for Guard_Sanjeevsaran_Search.");
        }

        var jadavanObj = GameObject.Find("Guard_Jadavan_Boss");
        if (jadavanObj != null)
        {
            var mr = jadavanObj.GetComponent<MeshRenderer>();
            if (mr != null) Object.DestroyImmediate(mr);
            var mf = jadavanObj.GetComponent<MeshFilter>();
            if (mf != null) Object.DestroyImmediate(mf);
            HumanoidMeshBuilder.BuildHumanoidVisual(jadavanObj.transform, CharacterRole.Boss_Jadavan);
            sb.AppendLine("[VISUAL] Built Character_Visuals for Guard_Jadavan_Boss.");
        }

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            HumanoidMeshBuilder.BuildHumanoidVisual(playerObj.transform, CharacterRole.Player);
            sb.AppendLine("[VISUAL] Built Character_Visuals for Player.");
        }

        // 4. Verify all GameObjects in Room 4
        sb.AppendLine("\n--- FINAL ROOM 4 GUARD OBJECT AUDIT ---");
        var allGos = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        int guardObjectCountInRoom4 = 0;
        foreach (var go in allGos)
        {
            string lName = go.name.ToLower();
            bool isGuardOrMesh = lName.Contains("guard") || lName.Contains("aqqil") || lName.Contains("mannequin") || lName.Contains("placeholder");
            if (isGuardOrMesh)
            {
                Vector3 p = go.transform.position;
                if (p.x > 3f && p.z > -4f && p.z < 18f)
                {
                    guardObjectCountInRoom4++;
                    sb.AppendLine($"   Found object in Room 4: '{go.name}' at pos {p}, parent='{go.transform.parent?.name}', active={go.activeSelf}");
                }
            }
        }
        // Clean up any transient capture runners or cameras before saving shipping scene
        var runners = Object.FindObjectsByType<PlayModeCaptureRunner>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var r in runners)
        {
            sb.AppendLine($"   [CLEANUP] Destroying transient PlayModeCaptureRunner '{r.gameObject.name}'.");
            Object.DestroyImmediate(r.gameObject);
        }
        var strayCaptureCam = GameObject.Find("PlayMode_CaptureCamera");
        if (strayCaptureCam != null)
        {
            sb.AppendLine("   [CLEANUP] Destroying stray PlayMode_CaptureCamera.");
            Object.DestroyImmediate(strayCaptureCam);
        }

        // 5. Save Scene
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        sb.AppendLine("\n[SUCCESS] Warehouse.unity cleanly saved with persistent mesh assets and Room 4 fixes!");

        string outPath = Path.Combine(Application.dataPath, "../scratch/room4_fixed_verification.txt");
        File.WriteAllText(outPath, sb.ToString());
        File.WriteAllText(Path.Combine(Application.dataPath, "../scratch/fix_room4_done.flag"), "DONE");
        Debug.Log($"[FixRoom4AndSaveScene] Report saved to: {outPath}");
    }
}
