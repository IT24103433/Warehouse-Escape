using UnityEngine;
using Unity.AI.Navigation;

/// <summary>
/// Ensures the NavMeshSurface is built at runtime if no baked NavMeshData asset exists yet.
/// Allows instant Play Mode testing without requiring a manual Editor bake step.
/// </summary>
[DefaultExecutionOrder(-100)]
public class NavMeshRuntimeBaker : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navSurface;

    void Awake()
    {
        if (navSurface == null)
            navSurface = GetComponent<NavMeshSurface>();

        if (navSurface != null && navSurface.navMeshData == null)
        {
            navSurface.BuildNavMesh();
            Debug.Log("[NavMeshRuntimeBaker] Runtime NavMesh built successfully for Warehouse scene.", this);
        }
    }
}
