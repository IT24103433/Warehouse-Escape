using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TrapAndSquadTests
{
    private void StopGuardAttacks()
    {
        foreach (var shooter in Object.FindObjectsByType<GuardShooting>(FindObjectsSortMode.None)) shooter.enabled = false;
        foreach (var melee in Object.FindObjectsByType<GuardCapture>(FindObjectsSortMode.None)) melee.enabled = false;
    }
    private static void MovePlayer(Vector3 point)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var cc = player.GetComponent<CharacterController>();
        cc.enabled = false; player.transform.position = point; cc.enabled = true;
        Physics.SyncTransforms();
    }

    [UnityTest]
    public IEnumerator Every_Room_Has_A_Bypassable_Trap_And_Final_Room_Has_Two_Same_Type_Guards()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        var traps = Object.FindObjectsByType<WarehouseTrap>(FindObjectsSortMode.None);
        var rooms = new System.Collections.Generic.HashSet<int>();
        foreach (var trap in traps)
        {
            rooms.Add(trap.RoomNumber);
            Assert.IsTrue(trap.GetComponent<BoxCollider>().isTrigger);
            foreach (var collider in trap.GetComponentsInChildren<Collider>()) Assert.IsTrue(collider.isTrigger,
                "Trap geometry must not become a physical wall.");
        }
        Assert.AreEqual(5, rooms.Count);
        Assert.AreEqual(6, traps.Length);
        var bosses = Object.FindObjectsByType<BossSquadMember>(FindObjectsSortMode.None);
        Assert.AreEqual(2, bosses.Length);
        Assert.AreNotEqual(bosses[0].SquadSlot, bosses[1].SquadSlot);
        Assert.AreNotEqual(bosses[0].PreferredSide, bosses[1].PreferredSide);
        foreach (var boss in bosses)
        {
            Assert.IsNotNull(boss.GetComponent<FlankingGuardAI>());
            Assert.AreEqual(WeaponType.SCAR_Rifle, boss.GetComponent<GuardShooting>().AssignedWeapon);
            Assert.AreEqual(360, boss.GetComponent<Health>().MaxHealth);
            Assert.IsTrue(boss.GetComponent<NavMeshAgent>().isOnNavMesh);
        }
        Assert.Greater(Vector3.Distance(bosses[0].transform.position, bosses[1].transform.position), 3f);
    }

    [UnityTest]
    public IEnumerator Training_Trap_Preserves_Safe_Room_Health_And_Does_Not_Alert_Guards()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        StopGuardAttacks();
        var trap = GameObject.Find("Room1_Training").GetComponent<WarehouseTrap>();
        Assert.AreEqual(WarehouseTrapKind.Training, trap.Kind);
        MovePlayer(trap.transform.position + Vector3.up * 1.08f);
        int alarms = 0;
        System.Action<SoundDisturbance> listener = sound => { if (sound.source == trap.gameObject) alarms++; };
        GuardPerception.OnSoundReported += listener;
        try
        {
            yield return new WaitForSeconds(2.6f);
            Assert.AreEqual(100, GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().CurrentHealth);
            Assert.AreEqual(0, alarms);
            Assert.AreNotEqual(WarehouseTrapState.Ready, trap.State);
        }
        finally { GuardPerception.OnSoundReported -= listener; }
    }

    [UnityTest]
    public IEnumerator Shock_Trap_Warns_Before_Damage_And_Pauses_With_Game()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        StopGuardAttacks();
        var trap = GameObject.Find("Room2_Shock").GetComponent<WarehouseTrap>();
        trap.transform.position = new Vector3(0, 0, -16);
        MovePlayer(new Vector3(0, 1.08f, -16));
        var health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        yield return new WaitForSeconds(.2f);
        Assert.AreEqual(WarehouseTrapState.Warning, trap.State);
        Assert.AreEqual(100, health.CurrentHealth);
        try
        {
            GameManager.Instance.TogglePause();
            yield return new WaitForSecondsRealtime(.3f);
            Assert.AreEqual(WarehouseTrapState.Warning, trap.State);
            Assert.AreEqual(100, health.CurrentHealth);
        }
        finally { if (GameManager.Instance.IsPaused) GameManager.Instance.TogglePause(); }
        yield return new WaitForSeconds(1.3f);
        Assert.Less(health.CurrentHealth, 100);
        Assert.GreaterOrEqual(health.CurrentHealth, 80);
    }

    [UnityTest]
    public IEnumerator Stepping_Off_A_Warning_Pad_Avoids_Damage()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        StopGuardAttacks();
        var trap = GameObject.Find("Room2_Shock").GetComponent<WarehouseTrap>();
        trap.transform.position = new Vector3(0, 0, -16);
        MovePlayer(new Vector3(0, 1.08f, -16));
        yield return new WaitForSeconds(.15f);
        Assert.AreEqual(WarehouseTrapState.Warning, trap.State);
        MovePlayer(new Vector3(3, 1.08f, -16));
        yield return new WaitForSeconds(2.3f);
        Assert.AreEqual(100, GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().CurrentHealth);
        Assert.AreEqual(WarehouseTrapState.Cooldown, trap.State);
    }

    [Test]
    public void Bosses_Alternate_Suppression_Turns()
    {
        for (float time = 0; time < 8; time += .1f)
            Assert.AreNotEqual(BossSquadMember.OwnsSuppressionTurn(time, 0), BossSquadMember.OwnsSuppressionTurn(time, 1));
        Assert.IsTrue(BossSquadMember.OwnsSuppressionTurn(0, 0));
        Assert.IsTrue(BossSquadMember.OwnsSuppressionTurn(1.4f, 1));
    }

    [UnityTest]
    public IEnumerator Bosses_Share_Only_Confirmed_Sightings_And_Survivor_Releases_Reservations()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        StopGuardAttacks();
        var leader = GameObject.Find("Guard_Jadavan_Boss");
        var wing = GameObject.Find("Guard_Jadavan_Boss_Wingman");
        var leaderAI = leader.GetComponent<FlankingGuardAI>();
        var wingAI = wing.GetComponent<FlankingGuardAI>();
        wingAI.SightDistance = .2f; // Isolate a teammate which cannot see the distant player.
        MovePlayer(new Vector3(-6, 1.08f, 24));
        Assert.IsTrue(leader.GetComponent<NavMeshAgent>().Warp(new Vector3(-6, 1.08f, 27)));
        leader.transform.rotation = Quaternion.LookRotation(Vector3.back);
        leaderAI.FieldOfView = 160;
        leaderAI.SightDistance = 30;
        Physics.SyncTransforms();
        yield return new WaitForSeconds(.8f);
        var leaderMember = leader.GetComponent<BossSquadMember>();
        var wingMember = wing.GetComponent<BossSquadMember>();
        Assert.IsTrue(leaderAI.HasConfirmedVisualContact(), "Shared reports must originate from a real sighting.");
        Assert.Greater(wingMember.ReportsReceived, 0);
        Assert.IsFalse(wing.GetComponent<GuardShooting>().HasVisualTarget, "A teammate report cannot grant firing vision.");
        Assert.AreEqual(wingMember.LastSharedPosition, wingAI.LastSeenPosition);
        leaderAI.SightDistance = .1f;
        yield return null;
        int reports = wingMember.ReportsReceived;
        yield return new WaitForSeconds(.7f);
        Assert.AreEqual(reports, wingMember.ReportsReceived, "Unseen live player coordinates must not be radio-tracked.");
        wingAI.ReceiveSquadObservation(new Vector3(-6, 1.08f, 24));
        float startGoal = wing.GetComponent<NavMeshAgent>().destination.z;
        for (int i = 1; i <= 3; i++) wingAI.ReceiveSquadObservation(new Vector3(-6, 1.08f, 24 + i));
        Assert.Greater(wing.GetComponent<NavMeshAgent>().destination.z, startGoal + 1.5f,
            "Small fresh sighting updates must accumulate into an updated navigation goal.");
        leader.GetComponent<Health>().TakeDamage(9999, null);
        Assert.IsNull(wingMember.Partner);
        Assert.AreEqual(FlankSide.None, wingMember.PreferredSide);
        Assert.IsTrue(wingMember.CanFireNow);
    }
}
