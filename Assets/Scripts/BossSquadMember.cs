using System.Collections.Generic;
using UnityEngine;

/// <summary>Cooperative flanking, fresh sighting reports and alternating suppression.</summary>
[RequireComponent(typeof(FlankingGuardAI))]
public class BossSquadMember : MonoBehaviour
{
    private static readonly List<BossSquadMember> members = new List<BossSquadMember>();
    [SerializeField] private int squadSlot;
    private FlankingGuardAI ai;
    private GuardShooting shooting;
    private GuardPerceptionMemory memory;
    private Health health;
    private float nextReportAt;
    public int SquadSlot { get => squadSlot; set => squadSlot = value; }
    public int ReportsReceived { get; private set; }
    public float LastReportTime { get; private set; } = -100;
    public Vector3 LastSharedPosition { get; private set; }
    public bool Available => isActiveAndEnabled && ai != null && ai.isActiveAndEnabled && health != null && !health.IsDead;
    public BossSquadMember Partner
    {
        get
        {
            foreach (var member in members)
                if (member != this && member != null && member.Available) return member;
            return null;
        }
    }
    public FlankSide PreferredSide => Partner == null ? FlankSide.None : squadSlot % 2 == 0 ? FlankSide.Left : FlankSide.Right;
    public bool CanFireNow
    {
        get
        {
            var partner = Partner;
            if (partner == null || partner.shooting.IsReloading || !partner.ai.HasConfirmedVisualContact()) return true;
            return OwnsSuppressionTurn(Time.time, squadSlot);
        }
    }
    public static bool OwnsSuppressionTurn(float clock, int slot) => Mathf.FloorToInt(clock / 1.3f) % 2 == slot % 2;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetRegistry() => members.Clear();
    void Awake()
    {
        ai = GetComponent<FlankingGuardAI>();
        shooting = GetComponent<GuardShooting>();
        health = GetComponent<Health>();
        memory = GetComponent<GuardPerceptionMemory>();
    }
    void OnEnable()
    {
        if (!members.Contains(this)) members.Add(this);
        if (memory != null) memory.OnDisturbanceDetected += HeardDisturbance;
    }
    void OnDisable()
    {
        members.Remove(this);
        if (memory != null) memory.OnDisturbanceDetected -= HeardDisturbance;
    }
    private void HeardDisturbance(Vector3 point)
    { if (Available && !ai.HasConfirmedVisualContact()) ai.ReceiveSquadObservation(point); }

    void Update()
    {
        if (!Available || Time.time < nextReportAt) return;
        if (GameManager.Instance != null && (GameManager.Instance.IsPaused || GameManager.Instance.IsGameEnded)) return;
        ShareConfirmedObservation();
    }

    public bool ShareConfirmedObservation()
    {
        if (!Available || !ai.HasConfirmedVisualContact() || ai.PlayerTransform == null || Time.time < nextReportAt) return false;
        nextReportAt = Time.time + .5f;
        var partner = Partner;
        if (partner == null) return false;
        partner.LastSharedPosition = ai.PlayerTransform.position;
        partner.LastReportTime = Time.time;
        partner.ReportsReceived++;
        partner.ai.ReceiveSquadObservation(partner.LastSharedPosition);
        return true;
    }

    public float FlankPenalty(Vector3 candidate, FlankSide side)
    {
        var partner = Partner;
        if (partner == null) return 0;
        float penalty = side == PreferredSide ? 0 : 8;
        if (partner.ai.CurrentState == GuardState.Flank && Vector3.Distance(candidate, partner.ai.SelectedFlankPosition) < 3)
            penalty += 12;
        return penalty;
    }
}
