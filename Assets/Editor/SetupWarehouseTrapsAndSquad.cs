using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class SetupWarehouseTrapsAndSquad
{
    [MenuItem("Warehouse/Setup Traps And Boss Squad")]
    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        var traps = GameObject.Find("Warehouse_Traps");
        if (traps == null) traps = new GameObject("Warehouse_Traps");
        AddTrap(traps.transform, "Room1_Training", new Vector3(-2.7f, 0, -26), WarehouseTrapKind.Training, 1, 0);
        AddTrap(traps.transform, "Room2_Shock", new Vector3(-4, 0, -12), WarehouseTrapKind.Shock, 2, 10);
        AddTrap(traps.transform, "Room3_Alarm", new Vector3(-17, 0, 7), WarehouseTrapKind.Alarm, 3, 0);
        AddTrap(traps.transform, "Room4_Shock", new Vector3(17, 0, 5), WarehouseTrapKind.Shock, 4, 15);
        AddTrap(traps.transform, "Room5_WestShock", new Vector3(-6, 0, 29), WarehouseTrapKind.Shock, 5, 20);
        AddTrap(traps.transform, "Room5_EastShock", new Vector3(6, 0, 33), WarehouseTrapKind.Shock, 5, 20);

        var leader = GameObject.Find("Guard_Jadavan_Boss");
        if (leader == null) throw new System.InvalidOperationException("Boss guard not found");
        var leaderSquad = leader.GetComponent<BossSquadMember>() ?? leader.AddComponent<BossSquadMember>();
        leaderSquad.SquadSlot = 0;
        var wing = GameObject.Find("Guard_Jadavan_Boss_Wingman");
        if (wing == null) wing = Object.Instantiate(leader, leader.transform.parent);
        wing.name = "Guard_Jadavan_Boss_Wingman";
        var spawn = new Vector3(6, 0, 26);
        if (!NavMesh.SamplePosition(spawn, out var hit, 3, NavMesh.AllAreas))
            throw new System.InvalidOperationException("No safe wingman spawn on NavMesh");
        wing.transform.position = new Vector3(hit.position.x, leader.transform.position.y, hit.position.z);
        wing.GetComponent<BossSquadMember>().SquadSlot = 1;
        wing.GetComponent<Health>().CharacterName = "Jadavan - Flanking Wingman";
        wing.GetComponent<NavMeshAgent>().avoidancePriority = 45;
        leader.GetComponent<NavMeshAgent>().avoidancePriority = 50;
        var patrolRoot = GameObject.Find("Waypoints_Boss_Wingman") ?? new GameObject("Waypoints_Boss_Wingman");
        var points = new Transform[5];
        Vector3[] route = { new Vector3(6, 1.08f, 26), new Vector3(6, 1.08f, 30),
            new Vector3(5.5f, 1.08f, 34), new Vector3(3, 1.08f, 37), new Vector3(5, 1.08f, 29) };
        for (int i = 0; i < points.Length; i++)
        {
            var point = patrolRoot.transform.Find("Wingman_" + i);
            if (point == null) { point = new GameObject("Wingman_" + i).transform; point.SetParent(patrolRoot.transform); }
            point.position = route[i]; points[i] = point;
        }
        wing.GetComponent<GuardPatrol>().Waypoints = points;
        EditorUtility.SetDirty(wing.GetComponent<GuardPatrol>());
        EditorUtility.SetDirty(leaderSquad);
        EditorUtility.SetDirty(wing.GetComponent<BossSquadMember>());
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[TrapSquadSetup] Saved six traps, five rooms, two coordinated flanking bosses.");
    }

    private static void AddTrap(Transform parent, string name, Vector3 raw, WarehouseTrapKind kind, int room, int damage)
    {
        if (parent.Find(name) != null) return; // Re-running setup must not duplicate traps.
        if (!NavMesh.SamplePosition(raw, out var nav, 2.5f, NavMesh.AllAreas))
            throw new System.InvalidOperationException("No walkable trap position: " + name);
        var root = new GameObject(name);
        root.transform.SetParent(parent);
        root.transform.position = nav.position;
        var collider = root.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(1.6f, 1.8f, 1.6f);
        collider.center = new Vector3(0, .9f, 0);
        var trap = root.AddComponent<WarehouseTrap>();
        trap.Configure(kind, room, damage);
        var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plate.name = "Trap_Plate";
        plate.transform.SetParent(root.transform, false);
        plate.transform.localPosition = new Vector3(0, .035f, 0);
        plate.transform.localScale = new Vector3(1.6f, .035f, 1.6f);
        Object.DestroyImmediate(plate.GetComponent<Collider>());
        var renderer = plate.GetComponent<Renderer>();
        var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Trap_Warning.mat");
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetColor("_BaseColor", new Color(.1f, .7f, 1));
            material.SetColor("_EmissionColor", new Color(.06f, .4f, .6f));
            material.EnableKeyword("_EMISSION");
            AssetDatabase.CreateAsset(material, "Assets/Materials/Mat_Trap_Warning.mat");
        }
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        var text = new GameObject("Trap_Status").AddComponent<TextMesh>();
        text.transform.SetParent(root.transform, false);
        text.transform.localPosition = new Vector3(0, .65f, 0);
        text.characterSize = .045f; text.fontSize = 48;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.GetComponent<Renderer>().sharedMaterial = text.font.material;
        text.anchor = TextAnchor.MiddleCenter;
        text.text = kind == WarehouseTrapKind.Training ? "TRAINING PAD - NO DAMAGE" : kind.ToString().ToUpperInvariant() + " PAD";
    }
}
