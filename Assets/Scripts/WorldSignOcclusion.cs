using UnityEngine;

/// <summary>
/// Attached to world-space signs and TextMesh labels to ensure:
/// 1. Proper depth testing (ZTest LEqual) against concrete walls, doors, shelves, and opaque objects.
/// 2. Distance-based visibility culling so distant room labels never bleed through doorways or clutter views.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class WorldSignOcclusion : MonoBehaviour
{
    [Tooltip("Maximum distance from main camera at which this sign is rendered.")]
    [SerializeField] private float maxVisibleDistance = 26.0f;

    private MeshRenderer meshRenderer;
    private Camera mainCamera;
    private float checkTimer = 0f;
    private float maxDistSqr;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        maxDistSqr = maxVisibleDistance * maxVisibleDistance;

        // Ensure depth-tested material is applied
        if (meshRenderer != null)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            Material depthMat = Resources.Load<Material>("Mat_Sign_Text_DepthTested");
            if (depthMat == null)
            {
#if UNITY_EDITOR
                depthMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Sign_Text_DepthTested.mat");
#endif
            }

            if (depthMat != null && (meshRenderer.sharedMaterial == null || meshRenderer.sharedMaterial.name.Contains("Font")))
            {
                meshRenderer.sharedMaterial = depthMat;
            }
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        UpdateVisibility();
    }

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = 0.12f;
            UpdateVisibility();
        }
    }

    private void UpdateVisibility()
    {
        if (meshRenderer == null || mainCamera == null) return;

        float sqrDist = (transform.position - mainCamera.transform.position).sqrMagnitude;
        bool shouldBeVisible = sqrDist <= maxDistSqr;

        if (meshRenderer.enabled != shouldBeVisible)
        {
            meshRenderer.enabled = shouldBeVisible;
        }
    }
}
