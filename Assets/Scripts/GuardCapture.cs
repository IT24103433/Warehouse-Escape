using System;
using UnityEngine;

/// <summary>
/// Shared guard capture component attached to all guard AI characters.
/// Catches the player only within a configurable short range with unobstructed line of sight.
/// Warehouse walls, shelving, crates, and closed doors block line of sight and prevent capture.
/// Triggers a single Game Over screen through GameManager.
/// </summary>
public class GuardCapture : MonoBehaviour
{
    [Header("Capture Settings")]
    [Tooltip("Maximum distance from guard to player within which capture can occur.")]
    [SerializeField] private float captureRange = 2.0f;

    [Tooltip("Guard eye height offset for line-of-sight raycasts.")]
    [SerializeField] private float eyeHeight = 1.6f;

    [Tooltip("Target player body/eye height offset for line-of-sight raycasts.")]
    [SerializeField] private float playerTargetHeight = 1.0f;

    [Tooltip("Layers representing physical obstacles (walls, shelves, crates, doors) that block capture.")]
    [SerializeField] private LayerMask obstacleMask = ~0;

    [Tooltip("Display name of the guard for Game Over UI telemetry.")]
    [SerializeField] private string guardDisplayName = "Patrol Guard";

    private Transform playerTransform;

    public float CaptureRange { get => captureRange; set => captureRange = value; }
    public float EyeHeight { get => eyeHeight; set => eyeHeight = value; }
    public float PlayerTargetHeight { get => playerTargetHeight; set => playerTargetHeight = value; }
    public LayerMask ObstacleMask { get => obstacleMask; set => obstacleMask = value; }
    public string GuardDisplayName { get => guardDisplayName; set => guardDisplayName = value; }

    void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return;

        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null) return;
        }

        CheckCapture();
    }

    [Header("Combat Attack Settings")]
    [Tooltip("Damage dealt per close-quarters melee strike.")]
    [SerializeField] private int meleeDamage = 20;

    [Tooltip("Cooldown in seconds between melee attacks.")]
    [SerializeField] private float attackCooldown = 1.2f;

    private float lastAttackTime = -99f;

    /// <summary>
    /// Evaluates proximity and physical line of sight.
    /// Replaces instant proximity capture with a balanced damage attack system.
    /// Deals balanced melee damage to the player if in close range and line of sight is clear.
    /// </summary>
    public bool CheckCapture()
    {
        if (playerTransform == null) return false;

        // Do not attack if guard is dead or game is ended
        var health = GetComponent<Health>();
        if (health != null && health.IsDead) return false;
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return false;

        // Safe Room Protection: Room 1 (Z < -18.0) is safe
        if (transform.position.z < -18.0f || playerTransform.position.z < -18.0f)
            return false;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > captureRange) return false;

        // Attack cooldown check
        if (Time.time - lastAttackTime < attackCooldown) return false;

        // Proximity satisfied; now verify unobstructed physical line of sight
        if (HasUnobstructedLineOfSight())
        {
            var playerHealth = playerTransform.GetComponent<Health>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                lastAttackTime = Time.time;
                Debug.Log($"[GuardCapture] {guardDisplayName} struck player for {meleeDamage} melee damage!");
                return playerHealth.TakeDamage(meleeDamage, gameObject);
            }
        }

        return false;
    }

    /// <summary>
    /// Raycasts between guard eye height and player target height.
    /// Returns true only if the direct ray reaches the player without intersecting walls or obstacles.
    /// </summary>
    public bool HasUnobstructedLineOfSight()
    {
        if (playerTransform == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = playerTransform.position + Vector3.up * playerTargetHeight;
        Vector3 toTarget = targetPos - eyePos;
        float totalDist = toTarget.magnitude;

        if (totalDist <= 0.05f) return true;

        float startOffset = 0.35f;
        if (totalDist <= startOffset) return true;

        Vector3 rayOrigin = eyePos + toTarget.normalized * startOffset;
        float rayDist = totalDist - startOffset;

        Ray ray = new Ray(rayOrigin, toTarget.normalized);
        RaycastHit[] hits = Physics.RaycastAll(ray, rayDist, obstacleMask, QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (var hit in hits)
        {
            // Ignore guard's own colliders
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                continue;

            // Direct line of sight to player reached
            if (hit.transform == playerTransform || hit.transform.IsChildOf(playerTransform) || hit.transform.CompareTag("Player"))
                return true;

            // Hit intervening geometry (wall, rack, crate, door) -> LOS is blocked!
            return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, captureRange);

        if (playerTransform != null)
        {
            Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
            Vector3 targetPos = playerTransform.position + Vector3.up * playerTargetHeight;
            bool los = HasUnobstructedLineOfSight();
            Gizmos.color = los ? Color.red : Color.green;
            Gizmos.DrawLine(eyePos, targetPos);
        }
    }
}
