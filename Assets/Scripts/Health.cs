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

    [Header("Regeneration Settings (Player Retreat)")]
    [SerializeField] private float regenRate = 10f; // 10 HP/second
    [SerializeField] private float regenCooldown = 3.0f; // 3 seconds delay without damage AND no visual contact
    [SerializeField] private bool isRegenerating = false;
    [SerializeField] private float timeSinceLastDamage = 0f;
    private float regenAccumulator = 0f;
    private bool forceVisualContactForTesting = false;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float HealthNormalized => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    public bool IsDead => isDead;
    public bool IsPlayer => isPlayer;
    public string CharacterName { get => characterName; set => characterName = value; }
    public bool IsRegenerating => isRegenerating;
    public float TimeSinceLastDamage => timeSinceLastDamage;
    public float RegenRate => regenRate;
    public float RegenCooldown => regenCooldown;

    public void SetIsPlayer(bool value) => isPlayer = value;

    public void SetForceVisualContactForTesting(bool contact)
    {
        forceVisualContactForTesting = contact;
    }

    public event Action<int, GameObject> OnDamaged;
    public event Action<GameObject> OnDied;

    void Awake()
    {
        if (CompareTag("Player") || gameObject.name == "Player" || gameObject.name.Contains("Player"))
        {
            isPlayer = true;
            characterName = "Operative";
        }
        if (GetComponent<DamageBloodEffect>() == null) gameObject.AddComponent<DamageBloodEffect>();
        currentHealth = maxHealth;
        isDead = false;
        isRegenerating = false;
        timeSinceLastDamage = 0f;
        regenAccumulator = 0f;
    }

    void Update()
    {
        if (!isPlayer || isDead) return;

        // Disable regeneration during pause, defeat, and victory
        if (GameManager.Instance != null && (GameManager.Instance.IsPaused || GameManager.Instance.IsGameEnded))
        {
            if (isRegenerating)
            {
                isRegenerating = false;
                regenAccumulator = 0f;
            }
            return;
        }

        // Full health: clamp and stop regeneration
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            if (isRegenerating)
            {
                isRegenerating = false;
                regenAccumulator = 0f;
            }
            return;
        }

        // Evaluate visual contact with all living guards
        bool hasVisualContact = GuardPerception.AnyLivingGuardHasVisualContact() || forceVisualContactForTesting;

        // Moving backward alone must not grant healing while still under attack:
        // Any confirmed visual contact immediately stops regeneration and resets timer
        if (hasVisualContact)
        {
            if (isRegenerating)
            {
                isRegenerating = false;
                regenAccumulator = 0f;
            }
            timeSinceLastDamage = 0f;
            return;
        }

        // Disengaged from visual contact: increment elapsed gameplay time
        timeSinceLastDamage += Time.deltaTime;

        // Start regeneration after 3 seconds without taking damage AND when no guard has confirmed visual contact
        if (timeSinceLastDamage >= regenCooldown)
        {
            isRegenerating = true;
            regenAccumulator += regenRate * Time.deltaTime;

            if (regenAccumulator >= 1.0f)
            {
                int hpToAdd = Mathf.FloorToInt(regenAccumulator);
                regenAccumulator -= hpToAdd;
                currentHealth = Mathf.Min(maxHealth, currentHealth + hpToAdd);

                if (currentHealth >= maxHealth)
                {
                    currentHealth = maxHealth;
                    isRegenerating = false;
                    regenAccumulator = 0f;
                }
            }
        }
        else
        {
            if (isRegenerating)
            {
                isRegenerating = false;
                regenAccumulator = 0f;
            }
        }
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
        timeSinceLastDamage = 0f;
        isRegenerating = false;
        regenAccumulator = 0f;
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
        isRegenerating = false;
        regenAccumulator = 0f;
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
        isRegenerating = false;
        timeSinceLastDamage = 0f;
        regenAccumulator = 0f;
        forceVisualContactForTesting = false;

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
