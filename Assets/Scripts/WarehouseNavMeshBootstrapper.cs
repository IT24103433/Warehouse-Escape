using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// Preloads and registers the baked Warehouse NavMeshData before scene objects or NavMeshAgents deserialize.
/// Ensures that in standalone builds, native NavMeshAgents find a valid, fully-formed NavMesh immediately
/// upon scene load, eliminating 'Failed to create agent because there is no valid NavMesh' errors.
/// Also hooks sceneLoaded to re-ensure valid NavMeshData across scene reloads (e.g. benchmark resolutions, game restart).
/// </summary>
[DefaultExecutionOrder(-1000)]
public static class WarehouseNavMeshBootstrapper
{
    private static NavMeshDataInstance s_NavMeshInstance;
    private static NavMeshData s_LoadedData;
    private static bool s_HookedSceneLoaded = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeNavMeshBeforeSceneLoad()
    {
        EnsureNavMeshActive("BeforeSceneLoad");

        if (!s_HookedSceneLoaded)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            s_HookedSceneLoaded = true;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void EnsureAfterSceneLoad()
    {
        EnsureNavMeshActive("AfterSceneLoad");
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureNavMeshActive($"OnSceneLoaded ({scene.name})");
    }

    /// <summary>
    /// Loads the persisted NavMeshData from Resources and activates it in the native NavMesh system.
    /// </summary>
    public static bool EnsureNavMeshActive(string context)
    {
        // Check if an instance is already valid and registered in the engine
        if (s_NavMeshInstance.valid && NavMesh.CalculateTriangulation().vertices.Length > 0)
        {
            return true;
        }

        if (s_LoadedData == null)
        {
            s_LoadedData = Resources.Load<NavMeshData>("NavMeshData_Warehouse");
        }

        if (s_LoadedData != null)
        {
            if (s_NavMeshInstance.valid)
            {
                NavMesh.RemoveNavMeshData(s_NavMeshInstance);
            }

            s_NavMeshInstance = NavMesh.AddNavMeshData(s_LoadedData, Vector3.zero, Quaternion.identity);
            Debug.Log($"[WarehouseNavMeshBootstrapper] NavMeshData loaded from Resources & activated successfully in {context}. (Instance valid: {s_NavMeshInstance.valid}, Vertices: {NavMesh.CalculateTriangulation().vertices.Length})");
            return s_NavMeshInstance.valid;
        }
        else
        {
            Debug.LogWarning($"[WarehouseNavMeshBootstrapper] Could not load 'NavMeshData_Warehouse' from Resources in {context}.");
            return false;
        }
    }
}
