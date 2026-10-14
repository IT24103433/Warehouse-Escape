using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

/// <summary>
/// Ensures valid NavMesh data is active in the Warehouse scene at runtime.
/// Coordinates with WarehouseNavMeshBootstrapper to guarantee valid NavMesh is active,
/// falling back to runtime bake if no baked asset exists.
/// </summary>
[DefaultExecutionOrder(-100)]
public class NavMeshRuntimeBaker : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navSurface;

    void Awake()
    {
        if (navSurface == null)
            navSurface = GetComponent<NavMeshSurface>();

        // 1. Ensure bootstrapper has activated preloaded NavMeshData
        WarehouseNavMeshBootstrapper.EnsureNavMeshActive("NavMeshRuntimeBaker.Awake");

        // 2. If navSurface is present but has no data or engine has 0 vertices, assign or rebuild
        if (navSurface != null)
        {
            if (navSurface.navMeshData == null)
            {
                var resData = Resources.Load<NavMeshData>("NavMeshData_Warehouse");
                if (resData != null)
                {
                    navSurface.navMeshData = resData;
                }
            }

            var tri = NavMesh.CalculateTriangulation();
            if (tri.vertices == null || tri.vertices.Length == 0)
            {
                navSurface.BuildNavMesh();
                Debug.Log("[NavMeshRuntimeBaker] Runtime NavMesh built successfully for Warehouse scene.", this);
            }
            else
            {
                Debug.Log($"[NavMeshRuntimeBaker] NavMesh confirmed active for Warehouse scene ({tri.vertices.Length} vertices).", this);
            }
        }
    }
}
