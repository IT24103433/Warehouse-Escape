using System;
using UnityEngine;

/// <summary>
/// Centralized health and damage handler for both the Player and Enemy guards.
/// Enforces single-death dispatch, damage immutability after game end,
/// safe-room immunity, and clean state restoration on restart.
/// </summary>
public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private string characterName = "Unit";

    [Header("State")]
    [SerializeField] private bool isDead = false;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float HealthNormalized => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    public bool IsDead => isDead;
    public bool IsPlayer => isPlayer;
    public string CharacterName { get => characterName; set => characterName = value; }

    public void SetIsPlayer(bool value) => isPlayer = value;

    public event Action<int, GameObject> OnDamaged;
    public event Action<GameObject> OnDied;

    void Awake()
    {
        if (CompareTag("Player") || gameObject.name == "Player" || gameObject.name.Contains("Player"))
        {
            isPlayer = true;
            characterName = "Operative";
        }
        currentHealth = maxHealth;
        isDead = false;
    }

    /// <summary>
    /// Applies damage to this character from an attacker source.
    /// Rejects damage if dead, game ended, or if target/attacker is inside the safe room (Z < -18.0).
    /// </summary>
    public bool TakeDamage(int damage, GameObject attacker)
    {
        if (damage <= 0 || isDead) return false;

        // Disallow combat after game has ended (Victory or Game Over)
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            return false;

        // Safe Room Protection: Room 1 (Z < -18.0) is 100% safe
        if (transform.position.z < -18.0f)
            return false;

        if (attacker != null && attacker.transform.position.z < -18.0f)
            return false;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnDamaged?.Invoke(damage, attacker);

        if (currentHealth <= 0 && !isDead)
        {
            Die(attacker);
        }

        return true;
    }

    private void Die(GameObject attacker)
    {
        isDead = true;
        OnDied?.Invoke(attacker);

        if (isPlayer)
        {
            Debug.Log($"[Health] Player eliminated in combat by {(attacker != null ? attacker.name : "security")}!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver("Eliminated in combat by warehouse security forces!");
            }
        }
        else
        {
            Debug.Log($"[Health] Guard '{gameObject.name}' ({characterName}) defeated!");
            DisableEnemyBehavior();
        }
    }

    /// <summary>
    /// Stops AI, navigation, attacks, and capture behavior upon enemy death.
    /// </summary>
    public void DisableEnemyBehavior()
    {
        // Stop NavMeshAgent
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Disable AI movement authority
        var movement = GetComponent<EnemyMovement>();
        if (movement != null) movement.enabled = false;

        var patrol = GetComponent<GuardPatrol>();
        if (patrol != null) patrol.enabled = false;

        // Disable specific AI state machines
        var pAI = GetComponent<PatrolGuardAI>();
        if (pAI != null) pAI.enabled = false;

        var sAI = GetComponent<SearchGuardAI>();
        if (sAI != null) sAI.enabled = false;

        var dAI = GetComponent<DefensiveGuardAI>();
        if (dAI != null) dAI.enabled = false;

        var fAI = GetComponent<FlankingGuardAI>();
        if (fAI != null) fAI.enabled = false;

        // Disable capture & weapon attacks
        var cap = GetComponent<GuardCapture>();
        if (cap != null) cap.enabled = false;

        var shoot = GetComponent<GuardShooting>();
        if (shoot != null) shoot.enabled = false;

        // Disable physics capsule collider so corpse doesn't block player
        var col = GetComponent<CapsuleCollider>();
        if (col != null) col.enabled = false;

        // Rotate character to flat defeat pose on floor
        var visuals = transform.Find("Character_Visuals");
        if (visuals != null)
        {
            visuals.localRotation = Quaternion.Euler(85f, 0f, 0f);
            visuals.localPosition = new Vector3(0f, -0.95f, 0.4f);
        }
    }

    /// <summary>
    /// Restores full health and revives unit on restart.
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (!isPlayer)
        {
            var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = true;

            var movement = GetComponent<EnemyMovement>();
            if (movement != null) movement.enabled = true;

            var patrol = GetComponent<GuardPatrol>();
            if (patrol != null) patrol.enabled = true;

            var pAI = GetComponent<PatrolGuardAI>();
            if (pAI != null) pAI.enabled = true;

            var sAI = GetComponent<SearchGuardAI>();
            if (sAI != null) sAI.enabled = true;

            var dAI = GetComponent<DefensiveGuardAI>();
            if (dAI != null) dAI.enabled = true;

            var fAI = GetComponent<FlankingGuardAI>();
            if (fAI != null) fAI.enabled = true;

            var cap = GetComponent<GuardCapture>();
            if (cap != null) cap.enabled = true;

            var shoot = GetComponent<GuardShooting>();
            if (shoot != null) shoot.enabled = true;

            var col = GetComponent<CapsuleCollider>();
            if (col != null) col.enabled = true;

            var visuals = transform.Find("Character_Visuals");
            if (visuals != null)
            {
                visuals.localRotation = Quaternion.identity;
                visuals.localPosition = new Vector3(0f, -1.08f, 0f);
            }
        }
    }

    public void SetMaxHealth(int amount)
    {
        maxHealth = amount;
        currentHealth = amount;
    }
}
