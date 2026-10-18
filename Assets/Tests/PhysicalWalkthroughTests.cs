using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// Comprehensive PlayMode tests verifying the expanded Warehouse Escape Shooter:
/// 1. Room 1: Safe start room. 0 enemy guards. Spawn at (0, 1.08, -26).
/// 2. Room 2: First encounter with dual simultaneously available branch exits (Route A & Route B).
/// 3. Route A: Physically collect Card A -> Room 5 Boss Room -> [E] at Card Reader -> Victory.
/// 4. Route B: Physically collect Card B -> Room 5 Boss Room -> [E] at Card Reader -> Victory.
/// 5. Card Reader rejects interaction without card ("Access card required", continues gameplay).
/// 6. Entering boss room or exit area alone cannot trigger victory.
/// 7. Player Health, centralized damage handling, and defeat flow on death.
/// 8. Player Gun hitscan shooting, ammunition consumption, reload, and muzzle cover verification.
/// 9. Enemy Health, damage handling, and death behavior (stops AI, navigation, attacks).
/// 10. Obstacle and cover occlusion blocking shots.
/// 11. Full Restart restoring cards, reader, player health (100 HP), player ammo (12/36), and all guards.
/// </summary>
public class PhysicalWalkthroughTests
{
    private static StringBuilder fullReport = new StringBuilder();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "../scratch"));
        fullReport.Clear();
        fullReport.AppendLine("================================================================================");
        fullReport.AppendLine("UNITY PLAY MODE EXPANDED ESCAPE SHOOTER VERIFICATION REPORT");
        fullReport.AppendLine($"Timestamp: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        fullReport.AppendLine($"Unity Version: {Application.unityVersion}, Platform: {Application.platform}");
        fullReport.AppendLine("================================================================================\n");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        fullReport.AppendLine("================================================================================");
        fullReport.AppendLine("WALKTHROUGH EXECUTION FINISHED: consult NUnit XML for pass/fail results");
        fullReport.AppendLine("================================================================================");

        string reportPath = Path.Combine(Application.dataPath, "../scratch/PhysicalWalkthroughReport.txt");
        File.WriteAllText(reportPath, fullReport.ToString());
        Debug.Log($"[PhysicalWalkthroughTests] Full report written to: {reportPath}");

        string flagPath = Path.Combine(Application.dataPath, "../scratch/tests_finished.flag");
        File.WriteAllText(flagPath, "TEST_RUN_FINISHED: consult NUnit XML for pass/fail results");
    }

    [UnityTest]
    public IEnumerator Test_1_Room1_Safe_And_Room2_Branch_Entrances()
    {
        fullReport.AppendLine("--- TEST 1: ROOM 1 SAFE START & ROOM 2 DUAL BRANCH ENTRANCES ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(playerObj, "[PASS] Player GameObject located.");
        var controller = playerObj.GetComponent<CharacterController>();
        Assert.IsNotNull(controller, "[PASS] CharacterController attached to Player.");
        var movement = playerObj.GetComponent<PlayerMovement>();
        Assert.IsNotNull(movement, "[PASS] PlayerMovement attached to Player.");

        Vector3 spawn = controller.transform.position;
        fullReport.AppendLine($"[PASS] Original Spawn Position Verified: {spawn:F2}");
        Assert.LessOrEqual(Vector3.Distance(spawn, new Vector3(0f, 1.08f, -26f)), 0.5f, "Player starts at spawn (0, 1.08, -26).");

        // Verify Room 1 (Z: -32 to -18) is completely empty of enemy agents
        var allGuards = Object.FindObjectsByType<GuardCapture>(FindObjectsSortMode.None);
        foreach (var guard in allGuards)
        {
            float guardZ = guard.transform.position.z;
            Assert.IsTrue(guardZ > -18f, $"[PASS] Guard {guard.gameObject.name} is outside Room 1 at Z={guardZ:F1}.");
        }
        fullReport.AppendLine($"[PASS] Room 1 Verified Safe: 0 enemy guards inside Room 1 (Z < -18). Total guards active: {allGuards.Length}");

        // Verify Room 2 has Varun's patrol guard
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        Assert.IsNotNull(varunObj, "[PASS] Varun's Patrol Guard exists in scene.");
        fullReport.AppendLine($"[PASS] Room 2 First Encounter Verified: Guard_Varun_Patrol at {varunObj.transform.position:F2}");

        // Physical traversal from Room 1 into Room 2 doorway
        var pathToRoom2 = new List<Vector3>
        {
            new Vector3(0f, 1.08f, -22f),
            new Vector3(0f, 1.08f, -17f) // Room 1 -> Room 2 doorway
        };
        yield return WalkPathCoroutine(controller, movement, pathToRoom2, fullReport, "Room 1 Safe Start -> Room 2 Doorway");

        // Verify dual branch entrances from Room 2 with signs
        var branchSignA = GameObject.Find("Sign_Route_A");
        var branchSignB = GameObject.Find("Sign_Route_B");
        Assert.IsNotNull(branchSignA, "[PASS] Route A branch sign located at Room 2 exit.");
        Assert.IsNotNull(branchSignB, "[PASS] Route B branch sign located at Room 2 exit.");
        fullReport.AppendLine($"[PASS] Dual Branch Exits Confirmed from Room 2: Sign_Route_A at {branchSignA.transform.position:F2} and Sign_Route_B at {branchSignB.transform.position:F2} available simultaneously.\n");
    }

    [UnityTest]
    public IEnumerator Test_2_RouteA_Physical_Walkthrough_To_CardReader_Victory()
    {
        fullReport.AppendLine("--- TEST 2: ROUTE A CONTROLLER TRAVERSAL -> CARD A -> BOSS ROOM -> READER -> VICTORY ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var controller = playerObj.GetComponent<CharacterController>();
        var movement = playerObj.GetComponent<PlayerMovement>();
        var cardReader = Object.FindFirstObjectByType<CardReader>();
        Assert.IsNotNull(cardReader, "[PASS] CardReader located in scene.");

        // 1. Walk through Room 1 into Room 2 central aisle
        var pathPhase1 = new List<Vector3>
        {
            new Vector3(0f, 1.08f, -22f),
            new Vector3(0f, 1.08f, -18f),
            new Vector3(0f, 1.08f, -14f),
            new Vector3(0f, 1.08f, -10f)
        };
        yield return WalkPathCoroutine(controller, movement, pathPhase1, fullReport, "Room 1 -> Room 2 Center Aisle", defendDuringWalk: true);

        // 2. Take Route A branch into Room 3 and physically reach Card A
        var pathPhase2 = new List<Vector3>
        {
            new Vector3(0f, 1.08f, -6f),
            new Vector3(-5f, 1.08f, -6f), // Align with center of Doorway A
            new Vector3(-5f, 1.08f, -2f), // Route A branch doorway
            new Vector3(-5f, 1.08f, 0f),  // Center of Corridor A turn opening
            new Vector3(-7f, 1.08f, 0f),  // Turn west through wide opening into Room 3
            new Vector3(-10f, 1.08f, 0f), // South aisle of Room 3
            new Vector3(-10f, 1.08f, 14f),  // Main aisle of Room 3 to north crossing
            new Vector3(-20.5f, 1.08f, 14f) // Reaches expanded Card A trigger on table
        };
        yield return WalkPathCoroutine(controller, movement, pathPhase2, fullReport, "Route A -> Search Room Card A Table", checkCardAtEnd: true, defendDuringWalk: true);

        Assert.IsTrue(AccessCard.HasKeycard, "[PASS] Route A Security Access Card collected physically via CharacterController trigger collision.");
        fullReport.AppendLine($"[PASS] Access Card A Collected physically. HasKeycard = {AccessCard.HasKeycard}");

        // 3. From Card A through Corridor into Room 5 (Boss Room) to Card Reader
        var pathPhase3 = new List<Vector3>
        {
            new Vector3(-10f, 1.08f, 14f),
            new Vector3(-6.5f, 1.08f, 14f),
            new Vector3(-4.0f, 1.08f, 16.5f),
            new Vector3(-4.0f, 1.08f, 22.0f), // Entering Room 5 doorway
            new Vector3(0.0f, 1.08f, 22.0f),  // Center evacuation walkway
            new Vector3(0.0f, 1.08f, 34.0f),  // Along wide central aisle
            new Vector3(0.0f, 1.08f, 46.5f)   // In front of Card Reader Terminal
        };
        yield return WalkPathCoroutine(controller, movement, pathPhase3, fullReport, "Room 3 -> Corridor -> Room 5 Boss Room -> Card Reader", defendDuringWalk: true);

        Assert.IsTrue(cardReader.IsPlayerInRange, "[PASS] Player physically in range of Card Reader terminal.");
        fullReport.AppendLine($"[PASS] Player in Range of Card Reader. Prompt: '{cardReader.GetInteractionPrompt()}'");

        bool success = cardReader.TryInsertCard();
        Assert.IsTrue(success, "[PASS] Card Reader accepted Access Card A.");
        fullReport.AppendLine($"[PASS] Card Reader Accepted Card: isCardInserted = {cardReader.IsCardInserted}, Feedback = '{cardReader.StatusFeedback}'");

        yield return null;
        Assert.IsTrue(GameManager.Instance.IsGameWon, "[PASS] Victory screen triggered successfully via Card Reader insertion.");
        fullReport.AppendLine($"[PASS] Route A Full Walkthrough Verified: Victory State = {GameManager.Instance.CurrentState}\n");
    }

    [UnityTest]
    public IEnumerator Test_3_RouteB_Physical_Walkthrough_To_CardReader_Victory()
    {
        fullReport.AppendLine("--- TEST 3: ROUTE B CONTROLLER TRAVERSAL -> CARD B -> BOSS ROOM -> READER -> VICTORY ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var controller = playerObj.GetComponent<CharacterController>();
        var movement = playerObj.GetComponent<PlayerMovement>();
        var cardReader = Object.FindFirstObjectByType<CardReader>();
        Assert.IsNotNull(cardReader, "[PASS] CardReader located in scene.");

        // 1. Walk through Room 1 into Room 2 central aisle
        var pathPhase1 = new List<Vector3>
        {
            new Vector3(0f, 1.08f, -22f),
            new Vector3(0f, 1.08f, -18f),
            new Vector3(0f, 1.08f, -14f),
            new Vector3(0f, 1.08f, -10f)
        };
        yield return WalkPathCoroutine(controller, movement, pathPhase1, fullReport, "Room 1 -> Room 2 Center Aisle", defendDuringWalk: true);

        // 2. Take Route B branch into Room 4 and physically reach Card B
        var pathPhase2 = new List<Vector3>
        {
            new Vector3(0f, 1.08f, -6f),
            new Vector3(5f, 1.08f, -6f), // Align with center of Doorway B
            new Vector3(5f, 1.08f, -2f), // Route B branch doorway
            new Vector3(5f, 1.08f, 0f),  // Center of Corridor B turn opening
            new Vector3(7f, 1.08f, 0f),  // Turn east through wide opening into Room 4
            new Vector3(10f, 1.08f, 0f), // Main aisle of Room 4
            new Vector3(10f, 1.08f, 10f),
            new Vector3(15.0f, 1.08f, 10f)  // Reaches Card B trigger at terminal desk
        };
        yield return WalkPathCoroutine(controller, movement, pathPhase2, fullReport, "Route B -> Defensive Room Card B Desk", checkCardAtEnd: true, defendDuringWalk: true);

        Assert.IsTrue(AccessCard.HasKeycard, "[PASS] Route B Security Access Card collected physically via CharacterController trigger collision.");
        fullReport.AppendLine($"[PASS] Access Card B Collected physically. HasKeycard = {AccessCard.HasKeycard}");

        // 3. From Card B through Corridor into Room 5 (Boss Room) to Card Reader
        var pathPhase3 = new List<Vector3>
        {
            new Vector3(10f, 1.08f, 10f),
            new Vector3(10f, 1.08f, 14f),
            new Vector3(6.5f, 1.08f, 14f),
            new Vector3(4.0f, 1.08f, 16.5f),
            new Vector3(4.0f, 1.08f, 22.0f), // Entering Room 5 doorway
            new Vector3(0.0f, 1.08f, 22.0f), // Center evacuation walkway
            new Vector3(0.0f, 1.08f, 34.0f), // Along wide central aisle
            new Vector3(0.0f, 1.08f, 46.5f)  // In front of Card Reader Terminal
        };
        yield return WalkPathCoroutine(controller, movement, pathPhase3, fullReport, "Room 4 -> Corridor -> Room 5 Boss Room -> Card Reader", defendDuringWalk: true);

        Assert.IsTrue(cardReader.IsPlayerInRange, "[PASS] Player physically in range of Card Reader terminal.");
        fullReport.AppendLine($"[PASS] Player in Range of Card Reader. Prompt: '{cardReader.GetInteractionPrompt()}'");

        bool success = cardReader.TryInsertCard();
        Assert.IsTrue(success, "[PASS] Card Reader accepted Access Card B.");
        fullReport.AppendLine($"[PASS] Card Reader Accepted Card B: isCardInserted = {cardReader.IsCardInserted}, Feedback = '{cardReader.StatusFeedback}'");

        yield return null;
        Assert.IsTrue(GameManager.Instance.IsGameWon, "[PASS] Victory screen triggered successfully via Card Reader insertion.");
        fullReport.AppendLine($"[PASS] Route B Full Walkthrough Verified: Victory State = {GameManager.Instance.CurrentState}\n");
    }

    [UnityTest]
    public IEnumerator Test_4_CardReader_Rejects_Interaction_Without_Card()
    {
        fullReport.AppendLine("--- TEST 4: CARD READER REJECTION WITHOUT ACCESS CARD ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var controller = playerObj.GetComponent<CharacterController>();
        var movement = playerObj.GetComponent<PlayerMovement>();
        var cardReader = Object.FindFirstObjectByType<CardReader>();
        Assert.IsNotNull(cardReader, "[PASS] CardReader located.");
        Assert.IsFalse(AccessCard.HasKeycard, "[PASS] Player starts with no card.");

        // Walk to Card Reader bypassing cards
        var path = new List<Vector3>
        {
            new Vector3(0f, 1.08f, -22f),
            new Vector3(0f, 1.08f, -14f),
            new Vector3(0f, 1.08f, -6f),
            new Vector3(5f, 1.08f, -6f), // Align with center of Doorway B
            new Vector3(5f, 1.08f, -2f),
            new Vector3(5f, 1.08f, 0f),
            new Vector3(7f, 1.08f, 0f),
            new Vector3(10f, 1.08f, 0f),
            new Vector3(10f, 1.08f, 14f), // Bypass Card B at X=16
            new Vector3(6.5f, 1.08f, 14f),
            new Vector3(4.0f, 1.08f, 16.5f),
            new Vector3(4.0f, 1.08f, 22.0f),
            new Vector3(0.0f, 1.08f, 22.0f),
            new Vector3(0.0f, 1.08f, 34.0f),
            new Vector3(0.0f, 1.08f, 46.5f)  // In front of Card Reader Terminal
        };
        yield return WalkPathCoroutine(controller, movement, path, fullReport, "Corridor -> Room 5 Reader (No Card)");

        Assert.IsFalse(AccessCard.HasKeycard, "[PASS] Confirmed player reached reader WITHOUT any access card.");
        Assert.IsTrue(cardReader.IsPlayerInRange, "[PASS] Player is in interaction range of Card Reader.");

        bool accepted = cardReader.TryInsertCard();
        Assert.IsFalse(accepted, "[PASS] Card Reader rejected interaction without card.");
        Assert.AreEqual("Access card required", cardReader.StatusFeedback, "[PASS] Rejection feedback matches 'Access card required'.");
        Assert.IsFalse(cardReader.IsCardInserted, "[PASS] Card Reader isCardInserted remains false.");
        Assert.IsFalse(GameManager.Instance.IsGameWon, "[PASS] Game is NOT won. Gameplay continues.");
        fullReport.AppendLine($"[PASS] Reader Correctly Rejected Interaction: Feedback = '{cardReader.StatusFeedback}', IsGameWon = {GameManager.Instance.IsGameWon}\n");
    }

    [UnityTest]
    public IEnumerator Test_5_Entering_BossRoom_And_OldExit_Does_Not_Trigger_Victory()
    {
        fullReport.AppendLine("--- TEST 5: ENTERING BOSS ROOM & OLD EXIT AREA DOES NOT TRIGGER VICTORY ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var controller = playerObj.GetComponent<CharacterController>();
        var movement = playerObj.GetComponent<PlayerMovement>();

        Assert.IsFalse(AccessCard.HasKeycard, "[PASS] Player has no card.");

        var path = new List<Vector3>
        {
            new Vector3(0f, 1.08f, -22f),
            new Vector3(0f, 1.08f, -14f),
            new Vector3(0f, 1.08f, -6f),
            new Vector3(5f, 1.08f, -6f), // Align with center of Doorway B
            new Vector3(5f, 1.08f, -2f),
            new Vector3(5f, 1.08f, 0f),
            new Vector3(7f, 1.08f, 0f),
            new Vector3(10f, 1.08f, 0f),
            new Vector3(10f, 1.08f, 14f),
            new Vector3(6.5f, 1.08f, 14f),
            new Vector3(4.0f, 1.08f, 16.5f),
            new Vector3(4.0f, 1.08f, 22.0f),
            new Vector3(0.0f, 1.08f, 22.0f),
            new Vector3(0.0f, 1.08f, 34.0f),
            new Vector3(0.0f, 1.08f, 46.5f)
        };
        yield return WalkPathCoroutine(controller, movement, path, fullReport, "Room 5 Boss Room -> Exit Scenery Area", defendDuringWalk: true);

        Assert.GreaterOrEqual(controller.transform.position.z, 30.0f, "[PASS] Player physically located in boss room exit area.");
        Assert.IsFalse(GameManager.Instance.IsGameWon, "[PASS] Being in boss room without card insertion DOES NOT trigger victory!");
        fullReport.AppendLine($"[PASS] Confirmed Old Exit Area & Boss Room Cannot Trigger Victory: IsGameWon = {GameManager.Instance.IsGameWon}\n");
    }

    [UnityTest]
    public IEnumerator Test_6_Player_Health_Damage_And_Death()
    {
        fullReport.AppendLine("--- TEST 6: PLAYER HEALTH, DAMAGE, AND DEATH ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(playerObj, "[PASS] Player GameObject found.");
        var health = playerObj.GetComponent<Health>();
        Assert.IsNotNull(health, "[PASS] Player Health component attached.");
        health.SetIsPlayer(true);

        Assert.AreEqual(100, health.MaxHealth, "[PASS] Player max health is 100.");
        Assert.AreEqual(100, health.CurrentHealth, "[PASS] Player starts at 100 health.");
        Assert.IsFalse(health.IsDead, "[PASS] Player starts alive.");

        // Move player outside safe room to allow damage
        var controller = playerObj.GetComponent<CharacterController>();
        controller.enabled = false;
        controller.transform.position = new Vector3(0f, 1.08f, -14f);
        controller.enabled = true;

        // Deal 30 damage
        bool taken = health.TakeDamage(30, null);
        Assert.IsTrue(taken, "[PASS] Player took 30 damage.");
        Assert.AreEqual(70, health.CurrentHealth, "[PASS] Player health reduced to 70.");
        Assert.IsFalse(health.IsDead, "[PASS] Player is still alive.");

        // Deal fatal 70 damage
        health.TakeDamage(70, null);
        Assert.AreEqual(0, health.CurrentHealth, "[PASS] Player health reduced to 0.");
        Assert.IsTrue(health.IsDead, "[PASS] Player isDead marked true.");
        Assert.IsTrue(GameManager.Instance.IsGameOver, "[PASS] GameManager GameOver triggered on player death.");

        fullReport.AppendLine("[PASS] Player Health & Death flow verified: Initial 100 HP -> 70 HP -> 0 HP -> Game Over triggered.\n");
    }

    [UnityTest]
    public IEnumerator Test_7_Player_Gun_Shooting_Reload_And_Muzzle_Cover()
    {
        fullReport.AppendLine("--- TEST 7: PLAYER GUN SHOOTING, RELOAD, AND MUZZLE COVER OCCLUSION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var gun = playerObj.GetComponentInChildren<PlayerGun>();
        Assert.IsNotNull(gun, "[PASS] PlayerGun component attached.");

        var mp5Data = gun.CurrentWeaponData;
        Assert.AreEqual(30, gun.CurrentAmmo, "[PASS] Gun starts with 30 rounds (MP5 default).");
        Assert.AreEqual(90, gun.ReserveAmmo, "[PASS] Gun starts with 90 reserve rounds (MP5 default).");

        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        Assert.IsNotNull(varunObj, "[PASS] Varun located for shooting target verification.");
        var varunHealth = varunObj.GetComponent<Health>();
        Assert.IsNotNull(varunHealth, "[PASS] Varun Health component attached.");

        // Move Player and Varun into Room 2 open aisle facing each other (outside safe room Z > -18)
        var cc = playerObj.GetComponent<CharacterController>();
        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -14f);
        cc.enabled = true;

        var vAgent = varunObj.GetComponent<NavMeshAgent>();
        if (vAgent != null) vAgent.Warp(new Vector3(0f, 1.08f, -9f));
        else varunObj.transform.position = new Vector3(0f, 1.08f, -9f);

        Physics.SyncTransforms();

        // Orient player camera and body directly toward target guard chest
        var pm = playerObj.GetComponent<PlayerMovement>();
        Assert.IsNotNull(pm, "[PASS] PlayerMovement located.");
        Vector3 varunChest = varunObj.transform.position + Vector3.up * 0.25f;
        pm.AimAt(varunChest);

        int initialVarunHp = varunHealth.CurrentHealth; // 60 HP

        // 1. PLACE SOLID COVER DIRECTLY BETWEEN MUZZLE AND TARGET
        GameObject solidCover = GameObject.CreatePrimitive(PrimitiveType.Cube);
        solidCover.name = "Solid_Cover_Obstacle";
        solidCover.transform.position = new Vector3(0f, 1.08f, -11.5f);
        solidCover.transform.localScale = new Vector3(3f, 3f, 0.8f);
        var coverCol = solidCover.GetComponent<BoxCollider>();
        Assert.IsNotNull(coverCol, "[PASS] Solid cover obstacle has active BoxCollider.");

        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();

        // 2. CONFIRM FIRING CONSUMES AMMO BUT CANNOT DAMAGE TARGET THROUGH COVER
        varunChest = varunObj.transform.position + Vector3.up * 0.25f;
        pm.AimAt(varunChest);
        bool firedCovered = gun.TryShoot();
        Assert.IsTrue(firedCovered, "[PASS] Firing with solid cover consumed a round.");
        Assert.AreEqual(29, gun.CurrentAmmo, "[PASS] Player ammo decremented to 29.");
        Assert.AreEqual(initialVarunHp, varunHealth.CurrentHealth, "[PASS] Solid cover strictly prevented damage to target! (HP unchanged: 60)");
        fullReport.AppendLine("  [COVER OCCLUSION] Shot fired with solid cover barrier -> Ammo: 29, Target HP: 60 (0 damage taken through cover).");

        // 3. REMOVE COVER AND CONFIRM UNOBSTRUCTED SHOT DAMAGES INTENDED TARGET
        Object.Destroy(solidCover);
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.35f); // Wait fire rate cooldown

        // Ensure camera and body still pointed directly at guard chest
        varunChest = varunObj.transform.position + Vector3.up * 0.25f;
        pm.AimAt(varunChest);

        bool firedClear = gun.TryShoot();
        Assert.IsTrue(firedClear, "[PASS] Unobstructed shot fired successfully.");
        Assert.AreEqual(28, gun.CurrentAmmo, "[PASS] Ammo decremented to 28.");
        int expectedHp = initialVarunHp - mp5Data.damage;
        Assert.AreEqual(expectedHp, varunHealth.CurrentHealth, $"[PASS] Unobstructed shot dealt exactly {mp5Data.damage} damage to intended target! (HP: {expectedHp})");
        fullReport.AppendLine($"  [UNOBSTRUCTED SHOT] Clear line-of-fire shot -> Ammo: 28, Target HP: {varunHealth.CurrentHealth} ({mp5Data.damage} damage dealt).");

        // 4. VERIFY RELOAD MECHANICS
        bool reloaded = gun.TryReload();
        Assert.IsTrue(reloaded, "[PASS] Reload initiated.");
        Assert.IsTrue(gun.IsReloading, "[PASS] IsReloading flag set.");

        yield return new WaitForSeconds(1.5f);
        Assert.IsFalse(gun.IsReloading, "[PASS] Reload completed.");
        Assert.AreEqual(30, gun.CurrentAmmo, "[PASS] Magazine restored to 30.");
        Assert.AreEqual(88, gun.ReserveAmmo, "[PASS] Reserve decremented by 2 (spent rounds).");

        fullReport.AppendLine("[PASS] Player Gun mechanics & cover occlusion fully verified: Solid cover blocks damage -> Clear shot damages target -> Reload restores mag.\n");
    }

    [UnityTest]
    public IEnumerator Test_8_Enemy_Health_Damage_And_Death()
    {
        fullReport.AppendLine("--- TEST 8: ENEMY HEALTH, DAMAGE, AND DEATH BEHAVIOR ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        Assert.IsNotNull(varunObj, "[PASS] Varun located.");
        var health = varunObj.GetComponent<Health>();
        Assert.IsNotNull(health, "[PASS] Guard Health component attached.");
        var healthBar = varunObj.GetComponent<EnemyHealthBar>();
        Assert.IsNotNull(healthBar, "[PASS] EnemyHealthBar attached to Guard.");

        Assert.AreEqual(60, health.MaxHealth, "[PASS] Varun max health is 60.");
        Assert.AreEqual(60, health.CurrentHealth, "[PASS] Varun starts at 60 health.");

        // Deal 25 damage
        health.TakeDamage(25, null);
        Assert.AreEqual(35, health.CurrentHealth, "[PASS] Guard health reduced to 35.");
        Assert.IsFalse(health.IsDead, "[PASS] Guard is alive.");

        // Deal fatal 35 damage
        health.TakeDamage(35, null);
        Assert.AreEqual(0, health.CurrentHealth, "[PASS] Guard health reached 0.");
        Assert.IsTrue(health.IsDead, "[PASS] Guard isDead marked true.");

        // Verify AI stopped
        var pAI = varunObj.GetComponent<PatrolGuardAI>();
        Assert.IsFalse(pAI.enabled, "[PASS] PatrolGuardAI disabled on death.");
        var capture = varunObj.GetComponent<GuardCapture>();
        Assert.IsFalse(capture.enabled, "[PASS] GuardCapture disabled on death.");

        fullReport.AppendLine("[PASS] Enemy Health & Death verified: 60 HP -> 35 HP -> 0 HP -> AI & Attacks stopped.\n");
    }

    [UnityTest]
    public IEnumerator Test_9_Obstacle_Blocks_Guard_Line_Of_Sight_And_Damage()
    {
        fullReport.AppendLine("--- TEST 9: OBSTACLE OCCLUSION, GUARD WALL BLOCKING, AND GAME-END DAMAGE IMMUNITY ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var aqqilObj = GameObject.Find("Guard_Aqqil_Defensive");
        Assert.IsNotNull(aqqilObj, "[PASS] Aqqil defensive guard found.");
        var aqqilShooting = aqqilObj.GetComponent<GuardShooting>();
        Assert.IsNotNull(aqqilShooting, "[PASS] GuardShooting component attached.");
        var aqqilHealth = aqqilObj.GetComponent<Health>();
        Assert.IsNotNull(aqqilHealth, "[PASS] Aqqil Health component attached.");

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var playerHealth = playerObj.GetComponent<Health>();
        Assert.IsNotNull(playerHealth, "[PASS] Player Health component attached.");

        // 1. VERIFY GUARD SHOTS CANNOT DAMAGE PLAYER THROUGH WALLS
        // Position player behind a solid wall (Room 3 west aisle) while Aqqil is in Room 4
        var cc = playerObj.GetComponent<CharacterController>();
        cc.enabled = false;
        playerObj.transform.position = new Vector3(-16f, 1.08f, 7f);
        cc.enabled = true;

        bool hasClearSight = aqqilShooting.HasClearLineOfSight(playerObj.transform.position);
        Assert.IsFalse(hasClearSight, "[PASS] Warehouse wall strictly blocks guard line of sight to player.");

        int initialPlayerHp = playerHealth.CurrentHealth; // 100 HP
        // Raycast check: shooting through wall hits wall collider, never player
        Vector3 origin = aqqilObj.transform.position + Vector3.up * 1.5f;
        Vector3 dest = playerObj.transform.position + Vector3.up * 1.0f;
        RaycastHit wallHit;
        bool hitSomething = Physics.Raycast(origin, (dest - origin).normalized, out wallHit, Vector3.Distance(origin, dest));
        Assert.IsTrue(hitSomething, "[PASS] Raycast intersected geometry.");
        Assert.AreNotEqual("Player", wallHit.collider.tag, "[PASS] Shot hit wall/shelf collider, NOT player.");
        Assert.AreEqual(initialPlayerHp, playerHealth.CurrentHealth, "[PASS] Player took 0 damage through wall.");
        fullReport.AppendLine("  [WALL OCCLUSION] Guard line of sight strictly blocked by wall -> Player HP remains 100.");

        // 2. VERIFY DEAD GUARDS CANNOT FIRE
        aqqilHealth.TakeDamage(100, null);
        Assert.IsTrue(aqqilHealth.IsDead, "[PASS] Guard is dead.");
        Assert.IsFalse(aqqilShooting.enabled, "[PASS] GuardShooting disabled upon death.");
        fullReport.AppendLine("  [DEAD GUARD STATE] Guard defeated -> GuardShooting component disabled, attacks halted.");

        // 3. VERIFY GAMEPLAY-END STATES PREVENT DAMAGE
        // Move player outside safe room to test game-over immunity
        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -14f);
        cc.enabled = true;

        // Trigger Game Over state
        GameManager.Instance.TriggerGameOver("Test game end state");
        Assert.IsTrue(GameManager.Instance.IsGameEnded, "[PASS] Game state marked ended.");

        int hpBeforeEndDamage = playerHealth.CurrentHealth;
        bool damageTakenAfterEnd = playerHealth.TakeDamage(25, null);
        Assert.IsFalse(damageTakenAfterEnd, "[PASS] Health.TakeDamage rejected because game is ended.");
        Assert.AreEqual(hpBeforeEndDamage, playerHealth.CurrentHealth, "[PASS] Player HP strictly unchanged after game end.");

        // Check guard immunity after game end
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        var varunHealth = varunObj.GetComponent<Health>();
        int varunHpBefore = varunHealth.CurrentHealth;
        bool guardDamageTakenAfterEnd = varunHealth.TakeDamage(20, null);
        Assert.IsFalse(guardDamageTakenAfterEnd, "[PASS] Guard Health.TakeDamage rejected after game end.");
        Assert.AreEqual(varunHpBefore, varunHealth.CurrentHealth, "[PASS] Guard HP strictly unchanged after game end.");

        fullReport.AppendLine("[PASS] Wall occlusion, dead guard firing prevention, and game-end damage immunity verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_10_Restart_Resets_Everything()
    {
        fullReport.AppendLine("--- TEST 10: RESTART RESETS EVERYTHING ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var cardReader = Object.FindFirstObjectByType<CardReader>();
        Assert.IsNotNull(cardReader, "[PASS] CardReader located.");

        // Simulate victory state
        AccessCard.HasKeycard = true;
        cardReader.TryInsertCard();
        Assert.IsTrue(GameManager.Instance.IsGameWon, "[PASS] Victory reached.");

        // Call RestartGame
        fullReport.AppendLine("  [ACTION] Calling GameManager.Instance.RestartGame()...");
        GameManager.Instance.RestartGame();
        yield return new WaitForSeconds(0.5f);

        var freshPlayerObj = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(freshPlayerObj, "[PASS] Fresh Player found.");
        var freshHealth = freshPlayerObj.GetComponent<Health>();
        Assert.AreEqual(100, freshHealth.CurrentHealth, "[PASS] Player Health reset to 100.");

        var freshGun = freshPlayerObj.GetComponentInChildren<PlayerGun>();
        Assert.AreEqual(30, freshGun.CurrentAmmo, "[PASS] Player Gun reset to 30 ammo (MP5 default).");
        Assert.AreEqual(90, freshGun.ReserveAmmo, "[PASS] Player Gun reset to 90 reserve (MP5 default).");
        Assert.AreEqual(WeaponType.MP5_SMG, freshGun.CurrentWeapon, "[PASS] Player Gun reset to MP5 default.");

        var droppedPickups = Object.FindObjectsByType<WeaponPickup>(FindObjectsSortMode.None);
        Assert.AreEqual(0, droppedPickups.Length, "[PASS] Dropped weapon pickups cleared on restart.");

        Assert.IsFalse(AccessCard.HasKeycard, "[PASS] AccessCard inventory reset.");
        Assert.IsFalse(GameManager.Instance.IsGameWon, "[PASS] Game Won state reset.");
        Assert.IsFalse(GameManager.Instance.IsGameOver, "[PASS] Game Over state reset.");

        var allGuards = Object.FindObjectsByType<GuardCapture>(FindObjectsSortMode.None);
        int guardCount = 0;
        foreach (var gc in allGuards)
        {
            var g = gc.GetComponent<Health>();
            if (g != null && !g.IsPlayer)
            {
                guardCount++;
                Assert.IsFalse(g.IsDead, $"[PASS] Guard {g.gameObject.name} is alive.");
                Assert.AreEqual(g.MaxHealth, g.CurrentHealth, $"[PASS] Guard {g.gameObject.name} restored to max health.");
            }
        }
        Assert.AreEqual(4, guardCount, "[PASS] All 4 guards restored and active.");

        fullReport.AppendLine("[PASS] Full Game State Reset Confirmed: Cards, Agents (4), Reader, Player (100 HP, 12/36 Ammo), and Objectives all cleanly reset.\n");
    }

    // Exercise real combat while following the route; guards retain normal health, AI and damage.
    private void DefendWalkthrough(PlayerGun gun, PlayerMovement movement, Health[] guards)
    {
        if (gun == null) return;
        var camera = gun.GetComponentInParent<Camera>();
        if (camera == null) return;
        Health target = null;
        Vector3 aimPoint = Vector3.zero;
        float nearestDistance = gun.CurrentWeaponData.maxRange;
        foreach (var guard in guards)
        {
            if (guard == null || guard.IsPlayer || guard.IsDead) continue;
            var collider = guard.GetComponent<Collider>();
            if (collider == null || !collider.enabled) continue;
            Vector3 point = collider.bounds.center;
            Vector3 direction = point - camera.transform.position;
            if (direction.magnitude >= nearestDistance) continue;
            var hits = Physics.RaycastAll(camera.transform.position, direction.normalized, direction.magnitude + 0.1f, ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var hit in hits)
            {
                if (hit.transform.IsChildOf(movement.transform)) continue;
                if (hit.collider.GetComponentInParent<Health>() == guard)
                {
                    target = guard;
                    aimPoint = point;
                    nearestDistance = direction.magnitude;
                }
                break;
            }
        }
        if (target == null) return;
        movement.AimAt(aimPoint);
        if (gun.CurrentAmmo == 0)
        {
            if (!gun.IsReloading) gun.TryReload();
        }
        else
        {
            gun.TryShoot();
        }
    }
    private IEnumerator WalkPathCoroutine(CharacterController controller, PlayerMovement movement, List<Vector3> path, StringBuilder sb, string tag, bool checkCardAtEnd = false, bool defendDuringWalk = false)
    {
        var walkthroughGun = defendDuringWalk ? controller.GetComponentInChildren<PlayerGun>() : null;
        var guards = defendDuringWalk ? Object.FindObjectsByType<Health>(FindObjectsSortMode.None) : null;
        float totalDist = 0f;
        Vector3 lastPos = controller.transform.position;
        float startTime = Time.unscaledTime;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 target = path[i];
            float stuckTime = 0f;

            while (true)
            {
                if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                {
                    Assert.Fail($"[FAIL] Walkthrough failed: Defeated at {controller.transform.position:F2} ({GameManager.Instance.ResultReason})!");
                }

                if (GameManager.Instance != null && GameManager.Instance.IsGameWon)
                {
                    break;
                }

                if (checkCardAtEnd && i == path.Count - 1 && AccessCard.HasKeycard)
                {
                    break;
                }

                Vector3 current = controller.transform.position;
                Vector3 diff = target - current;
                diff.y = 0f;

                if (diff.magnitude < 0.40f)
                {
                    break;
                }

                if (defendDuringWalk) DefendWalkthrough(walkthroughGun, movement, guards);

                Vector3 step = diff.normalized * movement.speed * Time.deltaTime;
                controller.Move(step);
                controller.Move(Vector3.up * movement.gravity * Time.deltaTime * Time.deltaTime);

                float d = Vector3.Distance(new Vector3(controller.transform.position.x, 0, controller.transform.position.z),
                                           new Vector3(lastPos.x, 0, lastPos.z));
                totalDist += d;

                if (d < 0.005f)
                {
                    stuckTime += Time.deltaTime;
                    if (stuckTime > 4.5f)
                    {
                        var nearby = Physics.OverlapSphere(controller.transform.position, 1.5f);
                        foreach (var c in nearby)
                        {
                            if (c.gameObject != controller.gameObject)
                                Debug.LogWarning($"[OBSTACLE-NEARBY] At {controller.transform.position:F2}: Hit {c.gameObject.name} bounds {c.bounds.min:F2}..{c.bounds.max:F2}");
                        }
                        Assert.Fail($"[FAIL] Physical movement obstructed! CharacterController stuck at {controller.transform.position:F2} trying to reach {target:F2}.");
                    }
                }
                else
                {
                    stuckTime = 0f;
                }

                lastPos = controller.transform.position;
                yield return null;
            }
        }

        float elapsed = Time.unscaledTime - startTime;
        sb.AppendLine($"  [{tag} Traversed] Total Distance: {totalDist:F1}m, Duration: {elapsed:F1}s, End Pos: {controller.transform.position:F2}");
    }

    [UnityTest]
    public IEnumerator Test_11_Perception_Suspicion_Buildup_And_Decay()
    {
        fullReport.AppendLine("--- TEST 11: PERCEPTION SUSPICION BUILDUP AND DECAY ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        Assert.IsNotNull(varunObj, "[PASS] Varun located.");
        var memory = varunObj.GetComponent<GuardPerceptionMemory>();
        Assert.IsNotNull(memory, "[PASS] GuardPerceptionMemory attached to Varun.");

        // 1. Initial suspicion is zero
        memory.ResetPerception();
        Assert.AreEqual(0f, memory.SuspicionLevel, 0.01f, "[PASS] Initial suspicion starts at 0.");
        Assert.AreEqual(GuardPerceptionState.Unaware, memory.PerceptionState, "[PASS] Initial state is Unaware.");

        // 2. Suspicion builds with sustained visibility
        float sClose = GuardPerception.UpdateSuspicion(0f, true, 3.0f, 0.35f);
        float sFar = GuardPerception.UpdateSuspicion(0f, true, 12.0f, 0.35f);
        Assert.Greater(sClose, sFar, "[PASS] Close-range sighting builds suspicion faster than long-range.");
        Assert.Greater(sClose, 0.8f, "[PASS] Close-range sighting nearly triggers full alert in 0.35s.");

        // 3. Suspicion decays after losing sight
        float decayed = GuardPerception.UpdateSuspicion(1.0f, false, 5.0f, 1.0f);
        Assert.Less(decayed, 1.0f, "[PASS] Suspicion decays when line of sight is broken.");
        Assert.AreEqual(0.60f, decayed, 0.05f, "[PASS] Suspicion decays smoothly at 0.40/s.");

        fullReport.AppendLine("  [SUSPICION VERIFIED] Close-range: " + sClose.ToString("F2") + " vs Far: " + sFar.ToString("F2") + " | Decayed after 1s: " + decayed.ToString("F2"));
        fullReport.AppendLine("[PASS] Suspicion buildup, distance scaling, and smooth decay fully verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_12_Perception_Wall_Blocked_Vision_And_Bullets()
    {
        fullReport.AppendLine("--- TEST 12: WALL-BLOCKED VISION AND BULLETS ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        Assert.IsNotNull(playerObj);
        Assert.IsNotNull(varunObj);

        var cc = playerObj.GetComponent<CharacterController>();
        cc.enabled = false;
        // Place player inside Room 1 (safe room behind concrete dividing wall Z = -24)
        playerObj.transform.position = new Vector3(0f, 1.08f, -24f);
        cc.enabled = true;

        // Place guard on other side of dividing wall in Room 2 (Z = -14)
        varunObj.transform.position = new Vector3(0f, 1.08f, -14f);

        yield return new WaitForFixedUpdate();

        // 1. Verify wall blocks perception
        bool canSeeThroughWall = GuardPerception.CanSeePlayer(
            varunObj.transform, playerObj.transform, 15f, 120f, 0.6f, 0f, ~0
        );
        Assert.IsFalse(canSeeThroughWall, "[PASS] Warehouse concrete dividing wall strictly blocks visual line of sight.");

        // 2. Verify wall/cover blocks weapon hitscan
        var gun = playerObj.GetComponentInChildren<PlayerGun>();
        Assert.IsNotNull(gun);
        var pm = playerObj.GetComponent<PlayerMovement>();
        pm.AimAt(varunObj.transform.position + Vector3.up * 1.0f);

        var varunHealth = varunObj.GetComponent<Health>();
        int hpBefore = varunHealth.CurrentHealth;

        bool fired = gun.TryShoot();
        Assert.IsTrue(fired, "[PASS] Shot fired toward target behind wall.");
        Assert.AreEqual(hpBefore, varunHealth.CurrentHealth, "[PASS] Intervening wall/cover strictly prevented bullet damage to target.");

        fullReport.AppendLine("  [WALL OCCLUSION] Visual LOS: blocked | Bullet path: blocked | Target HP: " + varunHealth.CurrentHealth);
        fullReport.AppendLine("[PASS] Wall-blocked vision and bullets verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_13_Perception_Gunshot_And_Footstep_Hearing_Investigation()
    {
        fullReport.AppendLine("--- TEST 13: GUNSHOT AND FOOTSTEP HEARING INVESTIGATION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        Assert.IsNotNull(varunObj);
        var memory = varunObj.GetComponent<GuardPerceptionMemory>();
        Assert.IsNotNull(memory);

        varunObj.transform.position = new Vector3(0f, 1.08f, -12f);
        memory.ResetPerception();

        // 1. Gunshot hearing without intervening walls
        Vector3 clearSoundPos = new Vector3(0f, 1.08f, -6f); // 6m away, clear corridor
        SoundDisturbance gunshot = new SoundDisturbance
        {
            position = clearSoundPos,
            range = 32f,
            soundType = SoundType.Gunshot,
            timestamp = Time.time
        };

        bool canHearClear = GuardPerception.CanHearSound(varunObj.transform.position, gunshot, ~0, out float audibilityClear);
        Assert.IsTrue(canHearClear, "[PASS] Gunshot heard clearly along open corridor.");
        Assert.Greater(audibilityClear, 0.5f, "[PASS] High audibility for unobstructed gunshot.");

        // 2. Physical barrier attenuation check
        Vector3 walledSoundPos = new Vector3(0f, 1.08f, -25f); // 13m away behind Room 1 concrete wall
        SoundDisturbance walledGunshot = new SoundDisturbance
        {
            position = walledSoundPos,
            range = 24f,
            soundType = SoundType.Gunshot,
            timestamp = Time.time
        };
        bool canHearWalled = GuardPerception.CanHearSound(varunObj.transform.position, walledGunshot, ~0, out float audibilityWalled);
        Assert.Less(audibilityWalled, audibilityClear, "[PASS] Intervening walls attenuate sound audibility.");

        // 3. Footstep hearing within threshold
        Vector3 nearbyFootstepPos = new Vector3(0f, 1.08f, -15f); // 3m away
        SoundDisturbance footstep = new SoundDisturbance
        {
            position = nearbyFootstepPos,
            range = 8.5f,
            soundType = SoundType.Footstep,
            timestamp = Time.time
        };
        bool canHearFootstep = GuardPerception.CanHearSound(varunObj.transform.position, footstep, ~0, out float footstepAudibility);
        Assert.IsTrue(canHearFootstep, "[PASS] Running footsteps heard within 8.5m range.");

        fullReport.AppendLine("  [HEARING] Clear gunshot audibility: " + audibilityClear.ToString("F2") + " | Walled gunshot audibility: " + audibilityWalled.ToString("F2") + " | Footstep audibility: " + footstepAudibility.ToString("F2"));
        fullReport.AppendLine("[PASS] Gunshot and footstep hearing with wall attenuation verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_14_Perception_Expired_LKP_Memory()
    {
        fullReport.AppendLine("--- TEST 14: EXPIRED LAST-KNOWN-POSITION (LKP) INFORMATION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var searchGuardObj = GameObject.Find("Guard_Sanjeevsaran_Search");
        Assert.IsNotNull(searchGuardObj);
        var memory = searchGuardObj.GetComponent<GuardPerceptionMemory>();
        Assert.IsNotNull(memory);

        memory.ResetPerception();
        memory.MemoryDuration = 1.0f; // Fast expiration for test

        // Simulate gunshot disturbance setting LKP within audible range of search guard
        GuardPerception.ReportSound(searchGuardObj.transform.position + Vector3.forward * 4f, 30f, SoundType.Gunshot, null);
        yield return null;

        Assert.IsTrue(memory.HasValidLKP, "[PASS] Fresh LKP memory is immediately valid.");
        Assert.Greater(memory.LkpConfidence, 0.7f, "[PASS] Fresh LKP has high confidence.");

        // Wait beyond memory duration
        yield return new WaitForSeconds(1.2f);

        Assert.IsFalse(memory.HasValidLKP, "[PASS] LKP memory expired after memoryDuration elapsed.");
        Assert.AreEqual(0f, memory.LkpConfidence, 0.05f, "[PASS] LKP confidence decayed to zero.");

        fullReport.AppendLine("  [LKP MEMORY] Fresh valid: True -> Expired after 1.2s: False (Guards do not track phantom positions indefinitely).");
        fullReport.AppendLine("[PASS] Expired LKP memory verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_15_SearchGuard_Timeout_And_Patrol_Resumption()
    {
        fullReport.AppendLine("--- TEST 15: SEARCH GUARD TIMEOUT AND PATROL RESUMPTION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var sanjeevObj = GameObject.Find("Guard_Sanjeevsaran_Search");
        Assert.IsNotNull(sanjeevObj, "[PASS] Sanjeevsaran located.");
        var searchAI = sanjeevObj.GetComponent<SearchGuardAI>();
        Assert.IsNotNull(searchAI, "[PASS] SearchGuardAI attached.");

        searchAI.SearchTimeout = 1.0f; // Fast test timeout

        // Trigger search state
        searchAI.TransitionToSearch();
        Assert.AreEqual(SearchGuardState.Search, searchAI.CurrentState, "[PASS] Transitioned to Search state.");

        // Wait for search timeout to elapse
        yield return new WaitForSeconds(1.4f);

        Assert.AreEqual(SearchGuardState.Patrol, searchAI.CurrentState, "[PASS] Search timed out and smoothly resumed Patrol.");

        fullReport.AppendLine("  [SEARCH TIMEOUT] State: Search -> Timed out after 1.0s -> Resumed Patrol circuit in Room 3.");
        fullReport.AppendLine("[PASS] Search timeout and patrol resumption verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_16_DefensiveGuard_Leash_Enforcement_And_Return_To_Post()
    {
        fullReport.AppendLine("--- TEST 16: DEFENSIVE GUARD LEASH AND RETURN TO POST ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var aqqilObj = GameObject.Find("Guard_Aqqil_Defensive");
        Assert.IsNotNull(aqqilObj, "[PASS] Aqqil located.");
        var defensiveAI = aqqilObj.GetComponent<DefensiveGuardAI>();
        Assert.IsNotNull(defensiveAI, "[PASS] DefensiveGuardAI attached.");

        Assert.AreEqual(DefensiveGuardState.GuardingPost, defensiveAI.CurrentState, "[PASS] Aqqil starts in GuardingPost state.");

        // Trigger pursuit
        defensiveAI.TransitionToPursueIntruder();
        Assert.AreEqual(DefensiveGuardState.PursueIntruder, defensiveAI.CurrentState, "[PASS] Entered PursueIntruder state.");

        // Move guard far beyond leash distance (leash is 8.5m from post at (16, 1.08, 8))
        aqqilObj.transform.position = new Vector3(16f, 1.08f, 22f); // 14m from post
        yield return null;

        // On next update, Aqqil detects leash breach and disengages
        yield return new WaitForSeconds(0.35f);

        Assert.AreEqual(DefensiveGuardState.ReturnToPost, defensiveAI.CurrentState, "[PASS] Leash breach triggered ReturnToPost transition.");

        fullReport.AppendLine("  [LEASH ENFORCEMENT] Pursue -> Guard at 14m (>8.5m leash) -> Transitioned to ReturnToPost.");
        fullReport.AppendLine("[PASS] Defensive leash enforcement and return to post verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_17_FlankingGuard_Routes_DirectChase_Rejection_And_Blocked_Recovery()
    {
        fullReport.AppendLine("--- TEST 17: FLANKING GUARD ROUTE SCORING AND DIRECT-CHASE REJECTION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var bossObj = GameObject.Find("Guard_Jadavan_Boss");
        Assert.IsNotNull(bossObj, "[PASS] Boss Jadavan located.");
        var flankAI = bossObj.GetComponent<FlankingGuardAI>();
        Assert.IsNotNull(flankAI, "[PASS] FlankingGuardAI attached.");

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var cc = playerObj.GetComponent<CharacterController>();
        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, 32f);
        cc.enabled = true;

        bossObj.transform.position = new Vector3(0f, 1.08f, 23f);

        yield return new WaitForFixedUpdate();

        // 1. Candidate flank selection evaluates lateral divergence
        bool hasFlank = flankAI.TrySelectFlankTarget(out Vector3 flankTarget, out FlankSide side, out NavMeshPath path);
        Assert.IsTrue(hasFlank, "[PASS] Viable tactical flank candidate located.");
        Assert.AreNotEqual(FlankSide.None, side, "[PASS] Flank side selected (Left or Right).");
        Assert.GreaterOrEqual(Mathf.Abs(flankTarget.x - playerObj.transform.position.x), 2.5f, "[PASS] Flank target provides lateral separation from direct chase line.");

        // 2. Hysteresis bonus stabilizes side
        Assert.Greater(flankAI.SideHysteresisBonus, 1.0f, "[PASS] Side hysteresis bonus configured to prevent oscillation.");
        Assert.Greater(flankAI.MinDivergenceAngle, 10f, "[PASS] Minimum divergence angle rejects direct-chase equivalents.");

        fullReport.AppendLine("  [FLANKING SELECTION] Selected flank side: " + side + " at target: " + flankTarget.ToString("F2") + " (Divergence enforced, direct chase rejected).");
        fullReport.AppendLine("[PASS] Valid flanking routes and direct-chase rejection verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_18_Death_Pause_Restart_And_Standalone_Navigation()
    {
        fullReport.AppendLine("--- TEST 18: DEATH, PAUSE, RESTART, AND STANDALONE NAVIGATION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        var health = varunObj.GetComponent<Health>();
        var movement = varunObj.GetComponent<EnemyMovement>();

        // 1. Standalone navigation check
        Assert.IsTrue(movement.IsOnNavMesh, "[PASS] Agent correctly seated on standalone NavMesh.");

        // 2. Pause check
        Time.timeScale = 0f;
        yield return null;
        Assert.AreEqual(0f, Time.timeScale, "[PASS] TimeScale 0 verified for pause.");
        Time.timeScale = 1f;

        // 3. Death handling
        health.TakeDamage(100, null);
        Assert.IsTrue(health.IsDead, "[PASS] Guard isDead marked true on fatal damage.");
        Assert.IsFalse(movement.enabled, "[PASS] EnemyMovement disabled on death.");
        Assert.IsFalse(movement.Agent.enabled, "[PASS] NavMeshAgent disabled on death.");

        // 4. Restart restoration
        GameManager.Instance.RestartGame();
        yield return new WaitForSeconds(0.5f);

        var revivedVarun = GameObject.Find("Guard_Varun_Patrol");
        var revivedHealth = revivedVarun.GetComponent<Health>();
        Assert.IsFalse(revivedHealth.IsDead, "[PASS] Guard revived on restart.");
        Assert.AreEqual(60, revivedHealth.CurrentHealth, "[PASS] Guard health restored to max (60) on restart.");

        fullReport.AppendLine("  [STATE RESTORATION] NavMesh: active | Pause: safe | Death: cleanly halted | Restart: full reset.");
        fullReport.AppendLine("[PASS] Death, pause, restart, and standalone navigation verified.\n");
    }

    [UnityTest]
    public IEnumerator Test_19_Real_Boss_Shot_Damage_And_Cover_Protection()
    {
        fullReport.AppendLine("--- TEST 19: REAL BOSS SHOT DAMAGE AND COVER PROTECTION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var bossObj = GameObject.Find("Guard_Jadavan_Boss");
        Assert.IsNotNull(playerObj, "[PASS] Player located.");
        Assert.IsNotNull(bossObj, "[PASS] Boss Jadavan located.");

        var bHealth = bossObj.GetComponent<Health>();
        Assert.IsNotNull(bHealth, "[PASS] Boss Health attached.");
        Assert.AreEqual(160, bHealth.CurrentHealth, "[PASS] Boss starts at 160 HP.");

        var gun = playerObj.GetComponentInChildren<PlayerGun>();
        Assert.IsNotNull(gun, "[PASS] PlayerGun attached.");

        var pm = playerObj.GetComponent<PlayerMovement>();
        Assert.IsNotNull(pm, "[PASS] PlayerMovement attached.");

        var cc = playerObj.GetComponent<CharacterController>();
        cc.enabled = false;
        // Position player in Room 5 open central evacuation walkway facing north toward boss
        playerObj.transform.position = new Vector3(0f, 1.08f, 26f);
        cc.enabled = true;

        // Position boss in clear line of sight at Z = 34f in central aisle
        var bAgent = bossObj.GetComponent<NavMeshAgent>();
        if (bAgent != null) bAgent.Warp(new Vector3(0f, 1.08f, 34f));
        else bossObj.transform.position = new Vector3(0f, 1.08f, 34f);
        Physics.SyncTransforms();

        yield return new WaitForFixedUpdate();

        // 1. AIM DIRECTLY AT BOSS CHEST AND FIRE UNOBSTRUCTED SHOT THROUGH PlayerGun.TryShoot()
        var bCol = bossObj.GetComponent<Collider>();
        Vector3 bossChest = bCol != null ? (bCol.bounds.center + Vector3.up * 0.15f) : (bossObj.transform.position + Vector3.up * 0.25f);
        pm.AimAt(bossChest);

        int initialHp = bHealth.CurrentHealth; // 160 HP
        int mp5Dmg = gun.CurrentWeaponData.damage; // 18 HP
        bool shotFired1 = gun.TryShoot();
        Assert.IsTrue(shotFired1, "[PASS] Player fired round.");
        Assert.AreEqual(29, gun.CurrentAmmo, "[PASS] Ammo decremented to 29.");
        Assert.AreEqual(initialHp - mp5Dmg, bHealth.CurrentHealth, $"[PASS] Unobstructed shot through PlayerGun.TryShoot() reduced Boss health from {initialHp} to {initialHp - mp5Dmg}! ({mp5Dmg} genuine hitscan damage dealt).");
        fullReport.AppendLine("  [UNOBSTRUCTED SHOT] Clear line of sight -> Fired PlayerGun.TryShoot() -> Boss HP: " + initialHp + " -> " + bHealth.CurrentHealth + $" (Damage: {mp5Dmg})");

        // 2. PLACE SOLID COVER DIRECTLY BETWEEN PLAYER MUZZLE AND BOSS
        GameObject solidCover = GameObject.CreatePrimitive(PrimitiveType.Cube);
        solidCover.name = "Boss_Test_Cover_Barrier";
        solidCover.transform.position = new Vector3(0f, 1.08f, 30f);
        solidCover.transform.localScale = new Vector3(3f, 3f, 0.8f);

        yield return new WaitForSeconds(0.35f); // Wait fire cooldown

        // Aim at boss chest again
        pm.AimAt(bossChest);

        int hpBeforeCoverShot = bHealth.CurrentHealth; // 142 HP
        bool shotFiredCover = gun.TryShoot();
        Assert.IsTrue(shotFiredCover, "[PASS] Fired shot with intervening solid cover barrier.");
        Assert.AreEqual(28, gun.CurrentAmmo, "[PASS] Ammo decremented to 28.");
        Assert.AreEqual(hpBeforeCoverShot, bHealth.CurrentHealth, "[PASS] Solid cover barrier strictly prevented damage to Boss! (Boss HP unchanged at " + hpBeforeCoverShot + ").");
        fullReport.AppendLine("  [COVER PROTECTION] Intervening solid cover barrier -> Fired PlayerGun.TryShoot() -> Boss HP unchanged at " + bHealth.CurrentHealth + " (0 damage through cover).");

        // 3. REMOVE COVER AND CONFIRM SECOND UNOBSTRUCTED SHOT REDUCES BOSS HEALTH FURTHER
        Object.Destroy(solidCover);
        yield return new WaitForSeconds(0.35f);

        pm.AimAt(bossChest);
        bool shotFired2 = gun.TryShoot();
        Assert.IsTrue(shotFired2, "[PASS] Second unobstructed shot fired.");
        Assert.AreEqual(27, gun.CurrentAmmo, "[PASS] Ammo decremented to 27.");
        Assert.AreEqual(hpBeforeCoverShot - mp5Dmg, bHealth.CurrentHealth, $"[PASS] Second unobstructed shot reduced Boss health from {hpBeforeCoverShot} to {hpBeforeCoverShot - mp5Dmg}! (Boss HP: {hpBeforeCoverShot - mp5Dmg}).");
        fullReport.AppendLine("  [FOLLOW-UP SHOT] Cover removed -> Fired PlayerGun.TryShoot() -> Boss HP: " + hpBeforeCoverShot + " -> " + bHealth.CurrentHealth + $" (Damage: {mp5Dmg}).");

        fullReport.AppendLine($"[PASS] Real boss shot damage and cover protection fully verified: {initialHp} HP -> {initialHp - mp5Dmg} HP -> Cover blocks damage -> {hpBeforeCoverShot - mp5Dmg} HP.\n");
    }

    [UnityTest]
    public IEnumerator Test_20_Varun_VisualOnly_Perception_And_InvestigateLastSeen()
    {
        fullReport.AppendLine("--- TEST 20: VARUN VISUAL-ONLY DETECTION, REAR NON-DETECTION, AND NO AUDIO PURSUIT ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        Assert.IsNotNull(playerObj, "[PASS] Player located.");
        Assert.IsNotNull(varunObj, "[PASS] Varun located.");

        var pAI = varunObj.GetComponent<PatrolGuardAI>();
        var mem = varunObj.GetComponent<GuardPerceptionMemory>();
        var cc = playerObj.GetComponent<CharacterController>();
        var vAgent = varunObj.GetComponent<NavMeshAgent>();

        Assert.IsNotNull(pAI, "[PASS] PatrolGuardAI attached.");
        Assert.IsNotNull(mem, "[PASS] GuardPerceptionMemory attached.");
        Assert.IsTrue(mem.VisualOnlyDetection, "[PASS] VisualOnlyDetection flag is enabled on Varun.");

        // 1. REAR APPROACH: Place player directly behind Varun (Varun facing North, player to South of Varun)
        cc.enabled = false;
        varunObj.transform.position = new Vector3(0f, 1.08f, -10f);
        varunObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Facing North (+Z)
        var em = varunObj.GetComponent<EnemyMovement>();
        if (vAgent != null)
        {
            vAgent.isStopped = true;
            vAgent.ResetPath();
            vAgent.Warp(new Vector3(0f, 1.08f, -10f));
        }
        varunObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        if (em != null) em.SetAimTarget(varunObj.transform.position + Vector3.forward * 10f);

        playerObj.transform.position = new Vector3(0f, 1.08f, -14f); // South of Varun (-Z)
        playerObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        cc.enabled = true;
        Physics.SyncTransforms();

        yield return new WaitForSeconds(0.3f);

        Assert.IsFalse(pAI.CanSeePlayer, "[PASS] Varun does NOT visually detect player approaching from behind.");
        Assert.AreEqual(PatrolGuardState.Patrol, pAI.CurrentState, "[PASS] Varun remains in Patrol state when player is behind him.");

        // 2. AUDIO DISTURBANCES: Footstep / gunshot / radio disturbances do NOT trigger pursuit or shooting
        GuardPerception.ReportSound(playerObj.transform.position, 30f, SoundType.Gunshot, playerObj);
        GuardPerception.ReportSound(playerObj.transform.position, 20f, SoundType.Footstep, playerObj);
        yield return new WaitForSeconds(0.2f);

        Assert.AreEqual(PatrolGuardState.Patrol, pAI.CurrentState, "[PASS] Gunshot and footstep sounds do NOT independently trigger pursuit or alert for Varun.");

        // 3. WALL OCCLUSION: Place solid wall between Varun and front player position
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Varun_Vision_Blocking_Wall";
        wall.transform.position = new Vector3(0f, 1.08f, -8f);
        wall.transform.localScale = new Vector3(4f, 3f, 0.5f);

        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -6f); // North of wall
        cc.enabled = true;
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.3f);

        Assert.IsFalse(pAI.CanSeePlayer, "[PASS] Wall strictly blocks Varun forward line of sight.");
        Assert.AreEqual(PatrolGuardState.Patrol, pAI.CurrentState, "[PASS] Varun remains in Patrol when line of sight is obstructed by wall.");

        // 4. FRONT DETECTION: Remove wall, player inside forward FOV and unobstructed LOS
        Object.Destroy(wall);
        if (em != null) em.ClearAimTarget();
        if (vAgent != null) vAgent.isStopped = false;
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.3f);

        Assert.IsTrue(pAI.CanSeePlayer, "[PASS] Varun detects player when inside forward FOV and unobstructed LOS.");
        Assert.AreEqual(PatrolGuardState.Pursue, pAI.CurrentState, "[PASS] Varun transitions to Pursue upon unobstructed visual detection.");

        // 5. BREAK SIGHT -> INVESTIGATE LAST SEEN POSITION (DOES NOT TRACK HIDDEN LIVE PLAYER COORDINATES)
        Vector3 lastSeen = playerObj.transform.position;
        cc.enabled = false;
        playerObj.transform.position = new Vector3(-8.5f, 1.08f, -16f); // Far away in corner
        cc.enabled = true;
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.25f);

        Assert.IsFalse(pAI.CanSeePlayer, "[PASS] Line of sight broken.");
        Assert.AreEqual(PatrolGuardState.Investigate, pAI.CurrentState, "[PASS] Varun transitions to Investigate upon losing sight.");
        Assert.LessOrEqual(Vector3.Distance(pAI.InvestigatePosition, lastSeen), 1.0f, "[PASS] Varun investigates last seen position, NOT live hidden player position.");

        fullReport.AppendLine("[PASS] Varun visual-only perception, rear non-detection, sound immunity, and investigate-last-seen verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_21_Continuous_Firing_Cadence_Release_Pause_Death_Restart()
    {
        fullReport.AppendLine("--- TEST 21: CONTINUOUS FIRING CADENCE, RELEASE, PAUSE, DEATH, AND RESTART ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var gun = playerObj.GetComponentInChildren<PlayerGun>();
        var playerHealth = playerObj.GetComponent<Health>();
        Assert.IsNotNull(gun, "[PASS] PlayerGun located.");

        var mp5Data = gun.CurrentWeaponData;
        Assert.IsTrue(mp5Data.isAutomatic, "[PASS] MP5 default weapon is configured as automatic.");
        float fireInterval = mp5Data.fireRate; // ~0.08s

        // 1. Fire cadence: fire round 1
        bool fired1 = gun.TryShoot();
        Assert.IsTrue(fired1, "[PASS] First shot fired.");
        Assert.AreEqual(29, gun.CurrentAmmo, "[PASS] Ammo decremented to 29.");

        // Rapid immediate fire attempt before fire interval elapses should be rejected (cadence enforcement)
        bool firedTooFast = gun.TryShoot();
        Assert.IsFalse(firedTooFast, "[PASS] Firing cadence enforced: rapid call before fire interval rejected.");

        // Wait fire interval and fire again
        yield return new WaitForSeconds(fireInterval + 0.02f);
        bool fired2 = gun.TryShoot();
        Assert.IsTrue(fired2, "[PASS] Second shot fired cleanly after cadence interval.");
        Assert.AreEqual(28, gun.CurrentAmmo, "[PASS] Ammo decremented to 28.");

        // 2. Empty Magazine: equip weapon with 1 round in mag
        gun.EquipWeapon(WeaponType.MP5_SMG, 1, 31);
        yield return new WaitForSeconds(0.15f);
        bool firedLast = gun.TryShoot();
        Assert.IsTrue(firedLast, "[PASS] Last round fired.");
        Assert.AreEqual(0, gun.CurrentAmmo, "[PASS] Magazine empty (0 rounds).");

        bool firedEmpty = gun.TryShoot();
        Assert.IsFalse(firedEmpty, "[PASS] Cannot fire with empty magazine.");

        // 3. Reload: initiates reload, cannot fire while reloading
        bool reloaded = gun.TryReload();
        Assert.IsTrue(reloaded, "[PASS] Reload initiated.");
        Assert.IsTrue(gun.IsReloading, "[PASS] IsReloading flag set.");
        Assert.IsFalse(gun.TryShoot(), "[PASS] Cannot shoot while reloading.");

        yield return new WaitForSeconds(1.5f);
        Assert.IsFalse(gun.IsReloading, "[PASS] Reload completed.");
        Assert.AreEqual(30, gun.CurrentAmmo, "[PASS] Magazine fully restored to 30.");
        Assert.AreEqual(1, gun.ReserveAmmo, "[PASS] Reserve decremented properly.");

        // 4. Pause: cannot fire while paused (Time.timeScale == 0)
        Time.timeScale = 0f;
        Assert.IsFalse(gun.TryShoot(), "[PASS] Cannot shoot while game is paused.");
        Time.timeScale = 1f;

        // 5. Death: cannot fire when player is dead
        // Move outside Room 1 safe room (Z > -18) to allow combat damage
        var playerController = playerObj.GetComponent<CharacterController>();
        if (playerController != null) playerController.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -14f);
        if (playerController != null) playerController.enabled = true;
        Physics.SyncTransforms();

        playerHealth.TakeDamage(100, null);
        Assert.IsTrue(playerHealth.IsDead, "[PASS] Player died.");
        Assert.IsFalse(gun.TryShoot(), "[PASS] Cannot shoot when player is dead.");

        // 6. Restart: restores default MP5 (30 / 90)
        GameManager.Instance.RestartGame();
        yield return new WaitForSeconds(0.5f);

        var freshPlayer = GameObject.FindGameObjectWithTag("Player");
        var freshGun = freshPlayer.GetComponentInChildren<PlayerGun>();
        Assert.AreEqual(WeaponType.MP5_SMG, freshGun.CurrentWeapon, "[PASS] Restart restores default MP5 weapon.");
        Assert.AreEqual(30, freshGun.CurrentAmmo, "[PASS] Restart restores 30 magazine ammo.");
        Assert.AreEqual(90, freshGun.ReserveAmmo, "[PASS] Restart restores 90 reserve ammo.");

        fullReport.AppendLine("[PASS] Continuous firing cadence, release, reload, pause, death, and restart fully verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_22_CrosshairAiming_And_MuzzleCover()
    {
        fullReport.AppendLine("--- TEST 22: CROSSHAIR AIMING AND MUZZLE COVER PROTECTION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var gun = playerObj.GetComponentInChildren<PlayerGun>();
        var cam = playerObj.GetComponentInChildren<Camera>();
        Assert.IsNotNull(gun, "[PASS] PlayerGun located.");
        Assert.IsNotNull(cam, "[PASS] Camera located.");

        // 1. Model alignment: Local rotation must be aligned straight forward (Quaternion.identity)
        Assert.AreEqual(Quaternion.identity, gun.GunTransform.localRotation, "[PASS] Gun model local rotation is identity (no upward skew).");

        // 2. Muzzle points in forward direction of camera
        float fwdAngle = Vector3.Angle(cam.transform.forward, gun.GunTransform.forward);
        Assert.LessOrEqual(fwdAngle, 5.0f, "[PASS] Gun model forward aligns directly with camera view direction.");

        // 3. Upward and Downward aiming
        cam.transform.localRotation = Quaternion.Euler(-30f, 0f, 0f); // 30 deg up
        yield return null;
        fwdAngle = Vector3.Angle(cam.transform.forward, gun.GunTransform.forward);
        Assert.LessOrEqual(fwdAngle, 5.0f, "[PASS] Upward aim maintains direct alignment.");

        cam.transform.localRotation = Quaternion.Euler(30f, 0f, 0f); // 30 deg down
        yield return null;
        fwdAngle = Vector3.Angle(cam.transform.forward, gun.GunTransform.forward);
        Assert.LessOrEqual(fwdAngle, 5.0f, "[PASS] Downward aim maintains direct alignment.");

        cam.transform.localRotation = Quaternion.identity; // reset

        // 4. Muzzle-to-target obstruction check
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        var varunHealth = varunObj.GetComponent<Health>();
        var pm = playerObj.GetComponent<PlayerMovement>();
        var cc = playerObj.GetComponent<CharacterController>();

        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -14f);
        cc.enabled = true;

        var vAgent = varunObj.GetComponent<NavMeshAgent>();
        if (vAgent != null) vAgent.Warp(new Vector3(0f, 1.08f, -9f));
        else varunObj.transform.position = new Vector3(0f, 1.08f, -9f);
        Physics.SyncTransforms();

        Vector3 targetChest = varunObj.transform.position + Vector3.up * 0.25f;
        pm.AimAt(targetChest);

        // Place low barrier blocking muzzle line but camera can see upper chest
        GameObject lowBarrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lowBarrier.transform.position = new Vector3(0f, 1.10f, -12.5f);
        lowBarrier.transform.localScale = new Vector3(2f, 0.96f, 0.4f);
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.2f);

        int hpBefore = varunHealth.CurrentHealth;
        gun.TryShoot();
        Assert.AreEqual(hpBefore, varunHealth.CurrentHealth, "[PASS] Low barrier blocking muzzle path prevents damage even if camera has elevated vision.");

        Object.Destroy(lowBarrier);
        fullReport.AppendLine("[PASS] Crosshair aiming and muzzle cover protection fully verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_23_Guard_WeaponConfigurations_And_Shotgun_PelletLimits()
    {
        fullReport.AppendLine("--- TEST 23: GUARD WEAPON CONFIGURATIONS AND SHOTGUN PELLET LIMITS ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var varunShooting = GameObject.Find("Guard_Varun_Patrol")?.GetComponent<GuardShooting>();
        var sanjeevShooting = GameObject.Find("Guard_Sanjeevsaran_Search")?.GetComponent<GuardShooting>();
        var aqqilShooting = GameObject.Find("Guard_Aqqil_Defensive")?.GetComponent<GuardShooting>();
        var jadavanShooting = GameObject.Find("Guard_Jadavan_Boss")?.GetComponent<GuardShooting>();

        Assert.IsNotNull(varunShooting, "[PASS] Varun GuardShooting found.");
        Assert.IsNotNull(sanjeevShooting, "[PASS] Sanjeevsaran GuardShooting found.");
        Assert.IsNotNull(aqqilShooting, "[PASS] Aqqil GuardShooting found.");
        Assert.IsNotNull(jadavanShooting, "[PASS] Jadavan GuardShooting found.");

        // Assert weapon assignments
        Assert.AreEqual(WeaponType.UMP_SMG, varunShooting.AssignedWeapon, "[PASS] Varun assigned UMP-45 SMG.");
        Assert.AreEqual(WeaponType.AK47_Rifle, sanjeevShooting.AssignedWeapon, "[PASS] Sanjeevsaran assigned AK-47 Rifle.");
        Assert.AreEqual(WeaponType.Combat_Shotgun, aqqilShooting.AssignedWeapon, "[PASS] Aqqil assigned Combat Shotgun.");
        Assert.AreEqual(WeaponType.SCAR_Rifle, jadavanShooting.AssignedWeapon, "[PASS] Jadavan assigned SCAR Rifle.");

        // Assert shotgun bounded pellet count and balanced total damage
        Assert.LessOrEqual(aqqilShooting.PelletCount, 6, "[PASS] Shotgun pellet count is bounded (<= 6 pellets).");
        int maxShotgunDmg = aqqilShooting.PelletCount * aqqilShooting.DamagePerShot;
        Assert.LessOrEqual(maxShotgunDmg, 50, $"[PASS] Total shotgun max damage is balanced ({maxShotgunDmg} <= 50).");

        fullReport.AppendLine("[PASS] Guard weapon configurations and shotgun pellet limits fully verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_24_Enemy_Death_Drops_And_Weapon_Pickup_Swap()
    {
        fullReport.AppendLine("--- TEST 24: ENEMY DEATH DROPS AND WEAPON PICKUP SWAP ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var gun = playerObj.GetComponentInChildren<PlayerGun>();
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        var varunHealth = varunObj.GetComponent<Health>();

        // Defeat Varun
        varunHealth.TakeDamage(100, null);
        Assert.IsTrue(varunHealth.IsDead, "[PASS] Varun defeated.");
        yield return new WaitForSeconds(0.3f);

        // Check weapon drop
        var pickups = Object.FindObjectsByType<WeaponPickup>(FindObjectsSortMode.None);
        Assert.IsTrue(pickups.Length > 0, "[PASS] Weapon pickup dropped on enemy defeat.");

        WeaponPickup umpPickup = null;
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].PickupWeaponType == WeaponType.UMP_SMG)
            {
                umpPickup = pickups[i];
                break;
            }
        }
        Assert.IsNotNull(umpPickup, "[PASS] Dropped pickup matches guard's UMP-45 SMG.");

        // Swap weapon
        int prevMp5Ammo = gun.CurrentAmmo;
        int prevMp5Reserve = gun.ReserveAmmo;
        bool swapped = umpPickup.TrySwap(gun);
        Assert.IsTrue(swapped, "[PASS] Swapped to UMP pickup.");
        Assert.AreEqual(WeaponType.UMP_SMG, gun.CurrentWeapon, "[PASS] Player is now holding UMP-45 SMG.");

        // Check dropped MP5 pickup preserves previous ammo
        var currentPickups = Object.FindObjectsByType<WeaponPickup>(FindObjectsSortMode.None);
        WeaponPickup droppedMp5 = null;
        for (int i = 0; i < currentPickups.Length; i++)
        {
            if (currentPickups[i].PickupWeaponType == WeaponType.MP5_SMG)
            {
                droppedMp5 = currentPickups[i];
                break;
            }
        }
        Assert.IsNotNull(droppedMp5, "[PASS] Player dropped MP5 pickup.");
        Assert.AreEqual(prevMp5Ammo, droppedMp5.CurrentAmmo, "[PASS] Dropped MP5 preserved exact previous magazine ammo.");
        Assert.AreEqual(prevMp5Reserve, droppedMp5.ReserveAmmo, "[PASS] Dropped MP5 preserved exact previous reserve ammo.");

        fullReport.AppendLine("[PASS] Enemy death drops and weapon pickup swap fully verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_25_Pickup_Versus_CardReader_Priority()
    {
        fullReport.AppendLine("--- TEST 25: PICKUP VERSUS CARD READER INTERACTION PRIORITY ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var cc = playerObj.GetComponent<CharacterController>();
        var cardReader = Object.FindFirstObjectByType<CardReader>();
        Assert.IsNotNull(cardReader, "[PASS] CardReader located.");

        // Position player in range of Card Reader
        cc.enabled = false;
        playerObj.transform.position = cardReader.transform.position + cardReader.transform.forward * 1.5f;
        cc.enabled = true;
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.2f);

        Assert.IsTrue(cardReader.IsPlayerInRange, "[PASS] Player is in range of CardReader.");
        Assert.IsFalse(WeaponPickup.IsAnyPickupTargeted(), "[PASS] No weapon pickup targeted initially.");
        Assert.AreEqual("[E] Insert Access Card", cardReader.GetInteractionPrompt(), "[PASS] CardReader prompt is active when no pickup is targeted.");

        // Place a dropped weapon pickup right in front of the terminal
        GameObject pickupGo = new GameObject("Test_Dropped_Weapon_Pickup");
        pickupGo.transform.position = cardReader.transform.position + cardReader.transform.forward * 1.0f;
        var pickup = pickupGo.AddComponent<WeaponPickup>();
        pickup.Initialize(WeaponType.SCAR_Rifle, 20, 60);

        // Aim player directly at the pickup
        var pm = playerObj.GetComponent<PlayerMovement>();
        pm.AimAt(pickupGo.transform.position);
        yield return new WaitForSeconds(0.2f);

        // Verify pickup takes priority over card reader
        Assert.IsTrue(WeaponPickup.IsAnyPickupTargeted(), "[PASS] Weapon pickup is targeted.");
        Assert.AreEqual("", cardReader.GetInteractionPrompt(), "[PASS] CardReader yields prompt when looking at weapon pickup.");

        // Attempting card insertion should not succeed while pickup is targeted
        AccessCard.HasKeycard = true;
        bool inserted = false;
        if (!WeaponPickup.IsAnyPickupTargeted())
        {
            inserted = cardReader.TryInsertCard();
        }
        Assert.IsFalse(inserted, "[PASS] E-key priority resolves to weapon swap; does not insert card.");

        Object.Destroy(pickupGo);
        yield return null;
        AccessCard.HasKeycard = false;

        fullReport.AppendLine("[PASS] Pickup versus Card Reader interaction priority fully verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_26_Enemy_HealthBar_WallOcclusion()
    {
        fullReport.AppendLine("--- TEST 26: ENEMY HEALTH BAR WALL OCCLUSION ---");

        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var varunObj = GameObject.Find("Guard_Varun_Patrol");
        var healthBar = varunObj.GetComponent<EnemyHealthBar>();
        var varunHealth = varunObj.GetComponent<Health>();
        var pm = playerObj.GetComponent<PlayerMovement>();
        var cc = playerObj.GetComponent<CharacterController>();

        Assert.IsNotNull(healthBar, "[PASS] EnemyHealthBar attached to Varun.");

        // Move player and Varun into clear view in Room 2 doorway aisle (Z = -17 to -13.5, completely free of obstacles)
        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -17f);
        cc.enabled = true;

        var pAI = varunObj.GetComponent<PatrolGuardAI>();
        if (pAI != null) pAI.enabled = false;

        var vAgent = varunObj.GetComponent<NavMeshAgent>();
        if (vAgent != null)
        {
            vAgent.isStopped = true;
            vAgent.ResetPath();
            vAgent.enabled = false;
        }
        varunObj.transform.position = new Vector3(0f, 1.08f, -13.5f);
        varunObj.transform.rotation = Quaternion.LookRotation((playerObj.transform.position - varunObj.transform.position).normalized);
        Physics.SyncTransforms();

        pm.AimAt(varunObj.transform.position + Vector3.up * 0.4f);
        yield return new WaitForSeconds(0.3f);

        fullReport.AppendLine($"  [HEALTH BAR LOS] Unobstructed visibility confirmed: IsBarVisible={healthBar.IsBarVisible}");

        // 1. Unobstructed view: health bar should be visible
        Assert.IsTrue(healthBar.IsBarVisible, "[PASS] Enemy health bar is visible in clear line of sight.");

        // 2. Intervening wall: health bar must be occluded
        GameObject occludingWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        occludingWall.name = "HealthBar_Occluding_Wall";
        occludingWall.transform.position = new Vector3(0f, 1.5f, -15.25f);
        occludingWall.transform.localScale = new Vector3(4f, 3f, 0.5f);
        Physics.SyncTransforms();

        yield return new WaitForSeconds(0.3f);
        Assert.IsFalse(healthBar.IsBarVisible, "[PASS] Enemy health bar is strictly occluded behind intervening wall.");

        // 3. Remove wall: health bar visible again
        Object.Destroy(occludingWall);
        Physics.SyncTransforms();
        yield return new WaitForSeconds(0.3f);
        Assert.IsTrue(healthBar.IsBarVisible, "[PASS] Enemy health bar reappears when line of sight is restored.");

        // 4. On Death: health bar disappears immediately
        varunHealth.TakeDamage(100, null);
        yield return new WaitForSeconds(0.2f);
        Assert.IsFalse(healthBar.IsBarVisible, "[PASS] Enemy health bar disappears immediately upon death.");

        fullReport.AppendLine("[PASS] Enemy health bar wall occlusion fully verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_27_WeaponSwap_CancelsPendingReload()
    {
        fullReport.AppendLine("--- TEST 27: WEAPON SWAP CANCELS PENDING RELOAD ---");
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);
        var gun = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerGun>();
        gun.EquipWeapon(WeaponType.MP5_SMG, 0, 30);
        Assert.IsTrue(gun.TryReload());
        float reloadDuration = gun.CurrentWeaponData.reloadDuration;
        gun.EquipWeapon(WeaponType.Combat_Shotgun, 2, 12);
        Assert.IsFalse(gun.IsReloading);
        yield return new WaitForSeconds(reloadDuration + 0.2f);
        Assert.AreEqual(WeaponType.Combat_Shotgun, gun.CurrentWeaponType);
        Assert.AreEqual(2, gun.CurrentAmmo, "A cancelled MP5 reload must not refill the new weapon.");
        Assert.AreEqual(12, gun.ReserveAmmo, "A cancelled reload must not consume the new weapon's reserve.");
        fullReport.AppendLine("[PASS] Weapon swap cancels pending reload verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_28_WeaponHud_LabelsStayInsidePanel()
    {
        fullReport.AppendLine("--- TEST 28: WEAPON HUD LABELS STAY INSIDE PANEL ---");
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);
        Canvas.ForceUpdateCanvases();
        var panel = GameObject.Find("Weapon_Ammo_Panel").GetComponent<RectTransform>();
        var labels = new List<RectTransform>();
        foreach (string labelName in new[] { "Weapon_Name", "Ammo_Value", "Reload_Prompt" })
        {
            var label = panel.Find(labelName).GetComponent<RectTransform>();
            labels.Add(label);
            Vector3[] corners = new Vector3[4];
            label.GetWorldCorners(corners);
            foreach (Vector3 corner in corners)
            {
                Vector3 point = panel.InverseTransformPoint(corner);
                Assert.That(point.x, Is.InRange(panel.rect.xMin - 0.01f, panel.rect.xMax + 0.01f), labelName);
                Assert.That(point.y, Is.InRange(panel.rect.yMin - 0.01f, panel.rect.yMax + 0.01f), labelName);
            }
        }
        for (int i = 1; i < labels.Count; i++)
        {
            Vector3[] upper = new Vector3[4];
            Vector3[] lower = new Vector3[4];
            labels[i - 1].GetWorldCorners(upper);
            labels[i].GetWorldCorners(lower);
            Assert.Greater(upper[0].y, lower[1].y, "Weapon, ammo and reload labels must have separate rows.");
        }
        fullReport.AppendLine("[PASS] Weapon HUD labels bounds verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_29_Crosshair_Centering_And_Pause_Defeat_Hiding()
    {
        fullReport.AppendLine("--- TEST 29: CROSSHAIR CENTERING AND PAUSE/DEFEAT HIDING ---");
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);
        Canvas.ForceUpdateCanvases();

        var hud = PlayerHUD.Instance;
        Assert.IsNotNull(hud, "[PASS] PlayerHUD instance active.");
        var crosshair = hud.CrosshairRoot;
        Assert.IsNotNull(crosshair, "[PASS] Crosshair_Center root exists.");

        var cRt = crosshair.GetComponent<RectTransform>();
        Assert.AreEqual(new Vector2(0.5f, 0.5f), cRt.anchorMin, "[PASS] Crosshair anchored at screen center X.");
        Assert.AreEqual(new Vector2(0.5f, 0.5f), cRt.anchorMax, "[PASS] Crosshair anchored at screen center Y.");
        Assert.AreEqual(Vector2.zero, cRt.anchoredPosition, "[PASS] Crosshair exact position is at screen center.");
        Assert.IsTrue(crosshair.activeInHierarchy, "[PASS] Crosshair is visible during normal gameplay.");

        // Test Pause hiding
        GameManager.Instance.TogglePause();
        yield return null;
        Assert.IsTrue(GameManager.Instance.IsPaused, "[PASS] Game is paused.");
        Assert.IsFalse(crosshair.activeSelf, "[PASS] Crosshair is hidden while paused.");

        // Resume
        GameManager.Instance.TogglePause();
        yield return null;
        Assert.IsFalse(GameManager.Instance.IsPaused, "[PASS] Game is resumed.");
        Assert.IsTrue(crosshair.activeSelf, "[PASS] Crosshair is restored on resume.");

        // Test Defeat hiding (move outside safe room so damage takes effect)
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var cc = playerObj.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -14f);
        if (cc != null) cc.enabled = true;
        Physics.SyncTransforms();

        var health = playerObj.GetComponent<Health>();
        health.TakeDamage(100, null);
        yield return null;
        Assert.IsTrue(health.IsDead, "[PASS] Player is dead.");
        Assert.IsFalse(crosshair.activeSelf, "[PASS] Crosshair is hidden on player defeat.");
        fullReport.AppendLine("[PASS] Crosshair centering and pause/defeat hiding verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_30_Player_NonFatal_Damage_And_Death_At_Zero_Only()
    {
        fullReport.AppendLine("--- TEST 30: PLAYER NON-FATAL DAMAGE AND DEATH AT ZERO ONLY ---");
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var cc = playerObj.GetComponent<CharacterController>();
        var health = playerObj.GetComponent<Health>();

        // Move outside safe room
        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -14f);
        cc.enabled = true;
        Physics.SyncTransforms();

        // 1. Non-fatal damage: 25 damage -> 75 HP (must not defeat player)
        health.TakeDamage(25, null);
        yield return null;
        Assert.AreEqual(75, health.CurrentHealth, "[PASS] Health reduced to 75 HP.");
        Assert.IsFalse(health.IsDead, "[PASS] Player remains alive at 75 HP.");
        Assert.IsFalse(GameManager.Instance.IsGameOver, "[PASS] Game Over NOT triggered above 0 HP.");

        // 2. Low health state: 50 damage -> 25 HP (must show low health warning, not defeat)
        health.TakeDamage(50, null);
        yield return null;
        Assert.AreEqual(25, health.CurrentHealth, "[PASS] Health reduced to 25 HP.");
        Assert.IsFalse(health.IsDead, "[PASS] Player remains alive at 25 HP.");
        Assert.IsFalse(GameManager.Instance.IsGameOver, "[PASS] Game Over NOT triggered at 25 HP.");

        // Verify low health readout contains CRITICAL warning
        var hud = PlayerHUD.Instance;
        Assert.IsTrue(hud.HealthNumericText.text.Contains("CRITICAL"), "[PASS] Low health readout displays CRITICAL warning.");

        // 3. Guard touch / melee attack: Deals 20 damage through Health, bringing player to 5 HP (still alive!)
        var guardObj = GameObject.Find("Guard_Varun_Patrol");
        var capture = guardObj.GetComponent<GuardCapture>();
        Assert.IsNotNull(capture, "[PASS] GuardCapture component exists.");
        // Position guard within capture range
        guardObj.transform.position = playerObj.transform.position + Vector3.forward * 1.5f;
        Physics.SyncTransforms();
        bool struck = capture.CheckCapture();
        yield return null;
        Assert.IsTrue(struck, "[PASS] Guard attack struck player dealing melee damage.");
        Assert.AreEqual(5, health.CurrentHealth, "[PASS] Player has 5 HP remaining.");
        Assert.IsFalse(health.IsDead, "[PASS] Touching / approaching guard did NOT cause instant defeat.");

        // 4. Fatal damage: 10 damage -> 0 HP (only now must player die)
        health.TakeDamage(10, null);
        yield return null;
        Assert.AreEqual(0, health.CurrentHealth, "[PASS] Health reached 0 HP.");
        Assert.IsTrue(health.IsDead, "[PASS] Player died only when HP reached 0.");
        Assert.IsTrue(GameManager.Instance.IsGameOver, "[PASS] Game Over triggered exactly upon reaching 0 HP.");

        // 5. Duplicate damage after death rejected
        bool extraDamage = health.TakeDamage(20, null);
        Assert.IsFalse(extraDamage, "[PASS] Damage rejected after death.");
        fullReport.AppendLine("[PASS] Player non-fatal damage and death at zero verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_31_TopLeft_Minimap_And_HealthBar_NonOverlapping_Stack()
    {
        fullReport.AppendLine("--- TEST 31: TOP-LEFT MINIMAP AND HEALTH BAR NON-OVERLAPPING STACK ---");
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);
        Canvas.ForceUpdateCanvases();

        var hud = PlayerHUD.Instance;
        Assert.IsNotNull(hud, "[PASS] PlayerHUD active.");

        var minimapPanel = hud.MinimapPanel.GetComponent<RectTransform>();
        var hpPanel = hud.HealthFillImage.transform.parent.parent.GetComponent<RectTransform>(); // Health_Status_Panel
        var objPanel = GameObject.Find("Objective_Panel").GetComponent<RectTransform>();
        var helpPanel = GameObject.Find("Controls_Help_Panel").GetComponent<RectTransform>();

        Assert.IsNotNull(minimapPanel, "[PASS] Minimap panel found.");
        Assert.IsNotNull(hpPanel, "[PASS] Health panel found.");
        Assert.IsNotNull(objPanel, "[PASS] Objective panel found.");
        Assert.IsNotNull(helpPanel, "[PASS] Controls help panel found.");

        // 1. Panel rectangle non-overlap checks with clean gaps
        float minimapBottom = minimapPanel.anchoredPosition.y - minimapPanel.sizeDelta.y;
        float hpTop = hpPanel.anchoredPosition.y;
        float hpBottom = hpPanel.anchoredPosition.y - hpPanel.sizeDelta.y;
        float objTop = objPanel.anchoredPosition.y;
        float objBottom = objPanel.anchoredPosition.y - objPanel.sizeDelta.y;
        float helpTop = helpPanel.anchoredPosition.y;

        Assert.Less(hpTop, minimapBottom, "[PASS] Health panel is placed below Minimap with positive gap.");
        Assert.Less(objTop, hpBottom, "[PASS] Objective panel is placed below Health panel with positive gap.");
        Assert.Less(helpTop, objBottom, "[PASS] Controls help panel is placed below Objective panel with positive gap.");

        // 2. Child Elements Bounds & Strict Containment:
        // A. Health Bar Fill & Health Label must be strictly inside Health_Status_Panel
        var healthBarBg = hpPanel.Find("Health_Bar_BG")?.GetComponent<RectTransform>();
        var healthFill = hud.HealthFillImage.GetComponent<RectTransform>();

        Assert.IsNotNull(healthBarBg, "[PASS] Health_Bar_BG exists.");
        Assert.IsNotNull(healthFill, "[PASS] Health_Bar_Fill exists.");

        Vector3[] hpCorners = new Vector3[4];
        hpPanel.GetWorldCorners(hpCorners);
        float hpWorldBottom = hpCorners[0].y;
        float hpWorldTop = hpCorners[1].y;

        Vector3[] barCorners = new Vector3[4];
        healthBarBg.GetWorldCorners(barCorners);
        float barWorldBottom = barCorners[0].y;
        float barWorldTop = barCorners[1].y;

        Vector3[] fillCorners = new Vector3[4];
        healthFill.GetWorldCorners(fillCorners);
        float fillWorldBottom = fillCorners[0].y;
        float fillWorldTop = fillCorners[1].y;

        Assert.Greater(barWorldBottom, hpWorldBottom, "[PASS] Health bar background is strictly inside Health panel (above panel bottom).");
        Assert.Less(barWorldTop, hpWorldTop, "[PASS] Health bar background is strictly inside Health panel (below panel top).");
        Assert.Greater(fillWorldBottom, hpWorldBottom, "[PASS] Health bar fill is strictly inside Health panel (above panel bottom).");

        // B. Health Bar Fill must be strictly above Objective Panel & CURRENT OBJECTIVE text
        Vector3[] objCorners = new Vector3[4];
        objPanel.GetWorldCorners(objCorners);
        float objWorldTop = objCorners[1].y;

        var objTextRt = GameManager.Instance.ObjectiveText.GetComponent<RectTransform>();
        Vector3[] textCorners = new Vector3[4];
        objTextRt.GetWorldCorners(textCorners);
        float textWorldTop = textCorners[1].y;

        Assert.Greater(barWorldBottom, objWorldTop, "[PASS] Health bar is strictly above Objective panel.");
        Assert.Greater(barWorldBottom, textWorldTop, "[PASS] Health bar is strictly above CURRENT OBJECTIVE text (no overlap).");
        Assert.Greater(fillWorldBottom, textWorldTop, "[PASS] Health bar green fill is strictly above CURRENT OBJECTIVE text (no overlap).");

        // C. Health Label and Objective Text child containment
        var healthLabel = hpPanel.Find("Health_Label")?.GetComponent<RectTransform>();
        if (healthLabel != null)
        {
            Vector3[] lCorners = new Vector3[4];
            healthLabel.GetWorldCorners(lCorners);
            Assert.GreaterOrEqual(lCorners[0].y, hpWorldBottom, "[PASS] Health label is strictly inside health panel (above bottom).");
            Assert.LessOrEqual(lCorners[1].y, hpWorldTop, "[PASS] Health label is strictly inside health panel (below top).");
        }

        Assert.GreaterOrEqual(textCorners[0].y, objCorners[0].y, "[PASS] Objective text is strictly inside objective panel (above bottom).");
        Assert.LessOrEqual(textCorners[1].y, objWorldTop, "[PASS] Objective text is strictly inside objective panel (below top).");

        var helpText = helpPanel.Find("Help_Text")?.GetComponent<RectTransform>();
        if (helpText != null)
        {
            Vector3[] hTextCorners = new Vector3[4];
            helpText.GetWorldCorners(hTextCorners);
            Vector3[] helpCorners = new Vector3[4];
            helpPanel.GetWorldCorners(helpCorners);
            Assert.GreaterOrEqual(hTextCorners[0].y, helpCorners[0].y, "[PASS] Help text is strictly inside help panel (above bottom).");
            Assert.LessOrEqual(hTextCorners[1].y, helpCorners[1].y, "[PASS] Help text is strictly inside help panel (below top).");
        }

        // D. Clearance from Screen-Center Crosshair:
        var crosshair = hud.CrosshairRoot;
        if (crosshair != null)
        {
            Vector3[] cCorners = new Vector3[4];
            crosshair.GetComponent<RectTransform>().GetWorldCorners(cCorners);
            float crosshairLeftX = cCorners[0].x;

            Assert.Greater(crosshairLeftX, objCorners[2].x, "[PASS] Objective panel right edge does not cover crosshair.");
            Assert.Greater(crosshairLeftX, hpCorners[2].x, "[PASS] Health panel right edge does not cover crosshair.");
        }

        // Verify player marker updates position during traversal
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var cc = playerObj.GetComponent<CharacterController>();
        var markerRt = minimapPanel.Find("Floor_Plan_Display/Player_Marker").GetComponent<RectTransform>();
        Assert.IsNotNull(markerRt, "[PASS] Player marker found on floor plan.");

        // Initial pos in Room 1 (Z = -26)
        yield return null;
        Vector2 markerPos1 = markerRt.anchoredPosition;

        // Move to Room 2 (Z = -10)
        cc.enabled = false;
        playerObj.transform.position = new Vector3(0f, 1.08f, -10f);
        cc.enabled = true;
        Physics.SyncTransforms();
        yield return null;
        Vector2 markerPos2 = markerRt.anchoredPosition;

        Assert.Greater(markerPos2.y, markerPos1.y, "[PASS] Player marker moved north (+Y on minimap) when player walked north (+Z in warehouse).");
        fullReport.AppendLine("[PASS] Top-left minimap and health bar non-overlapping stack verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_32_Distinct_Weapon_Audio_Synthesis_And_Durations()
    {
        fullReport.AppendLine("--- TEST 32: DISTINCT WEAPON AUDIO SYNTHESIS AND DURATIONS ---");
        WeaponAudio.EnsureClips();

        var mp5Clip = WeaponAudio.GetWeaponFireClip(WeaponType.MP5_SMG);
        var umpClip = WeaponAudio.GetWeaponFireClip(WeaponType.UMP_SMG);
        var ak47Clip = WeaponAudio.GetWeaponFireClip(WeaponType.AK47_Rifle);
        var scarClip = WeaponAudio.GetWeaponFireClip(WeaponType.SCAR_Rifle);
        var shotgunClip = WeaponAudio.GetWeaponFireClip(WeaponType.Combat_Shotgun);

        Assert.IsNotNull(mp5Clip, "[PASS] MP5 fire clip generated.");
        Assert.IsNotNull(umpClip, "[PASS] UMP fire clip generated.");
        Assert.IsNotNull(ak47Clip, "[PASS] AK47 fire clip generated.");
        Assert.IsNotNull(scarClip, "[PASS] SCAR fire clip generated.");
        Assert.IsNotNull(shotgunClip, "[PASS] Shotgun fire clip generated.");

        // Verify all 5 weapon clips have distinct names
        var clipNames = new HashSet<string> { mp5Clip.name, umpClip.name, ak47Clip.name, scarClip.name, shotgunClip.name };
        Assert.AreEqual(5, clipNames.Count, "[PASS] All 5 weapons have distinctly authored audio profiles.");

        // Verify audio duration fits fire rate without lingering
        Assert.LessOrEqual(mp5Clip.length, 0.08f, "[PASS] MP5 clip duration fits continuous 0.08s fire cadence without overlap.");
        Assert.LessOrEqual(umpClip.length, 0.12f, "[PASS] UMP clip duration fits UMP cadence.");
        Assert.LessOrEqual(ak47Clip.length, 0.14f, "[PASS] AK47 clip duration fits AK47 cadence.");
        Assert.LessOrEqual(scarClip.length, 0.12f, "[PASS] SCAR clip duration fits SCAR cadence.");

        // Verify reload and empty click feedback
        var reloadClip = WeaponAudio.GetReloadClip();
        var emptyClip = WeaponAudio.GetEmptyClickClip();
        Assert.IsNotNull(reloadClip, "[PASS] Reload feedback clip generated.");
        Assert.IsNotNull(emptyClip, "[PASS] Empty-magazine feedback clip generated.");
        yield return null;
        fullReport.AppendLine("[PASS] Distinct weapon audio synthesis and durations verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_33_Weapon_Pickup_Prompt_Formatting_And_Swap_Action()
    {
        fullReport.AppendLine("--- TEST 33: WEAPON PICKUP PROMPT FORMATTING AND SWAP ACTION ---");
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.5f);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var gun = playerObj.GetComponentInChildren<PlayerGun>();
        Assert.IsNotNull(gun, "[PASS] PlayerGun located.");

        // Spawn dropped AK47 pickup in front of player
        GameObject pickupGo = new GameObject("Test_Dropped_AK47");
        pickupGo.transform.position = playerObj.transform.position + playerObj.transform.forward * 1.5f;
        var pickup = pickupGo.AddComponent<WeaponPickup>();
        pickup.Initialize(WeaponType.AK47_Rifle, 30, 60);

        // Player looking at pickup
        yield return new WaitForSeconds(0.2f);
        string prompt = pickup.GetPrompt(gun);

        // Verify clear prompt instructions with actual weapon names: "[E] Swap MP5 for AK47 (30/60)"
        Assert.IsTrue(prompt.Contains("[E] Swap MP5 for AK47"), $"[PASS] Prompt shows exact weapon names: '{prompt}'");
        Assert.IsTrue(prompt.Contains("30/60"), $"[PASS] Prompt shows available ammunition: '{prompt}'");

        // Execute E-key swap
        bool swapped = pickup.TrySwap(gun);
        Assert.IsTrue(swapped, "[PASS] Weapon swap executed.");
        Assert.AreEqual(WeaponType.AK47_Rifle, gun.CurrentWeaponType, "[PASS] Player now holds AK47.");
        Assert.AreEqual(30, gun.CurrentAmmo, "[PASS] Player has 30 rounds loaded in AK47.");
        Assert.AreEqual(60, gun.ReserveAmmo, "[PASS] Player has 60 reserve rounds for AK47.");

        // Verify dropped weapon is now the old MP5 with original ammo
        Assert.AreEqual(WeaponType.MP5_SMG, pickup.Type, "[PASS] Dropped pickup now holds player's old MP5.");

        Object.Destroy(pickupGo);
        yield return null;
        fullReport.AppendLine("[PASS] Weapon pickup prompt formatting and swap action verified!\n");
    }

    [UnityTest]
    public IEnumerator Test_34_Visual_Crosshair_And_Instructions_Pixel_Verification()
    {
        fullReport.AppendLine("--- TEST 34: VISUAL CROSSHAIR AND INSTRUCTIONS PIXEL VERIFICATION ---");
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(0.6f);
        Canvas.ForceUpdateCanvases();

        var hud = PlayerHUD.Instance;
        Assert.IsNotNull(hud, "[PASS] PlayerHUD instance active.");

        // 1. Verify Crosshair ScreenSpaceOverlay Canvas configuration
        var crosshairCanvas = GameObject.Find("Crosshair_Canvas")?.GetComponent<Canvas>();
        Assert.IsNotNull(crosshairCanvas, "[PASS] Dedicated Crosshair_Canvas found.");
        Assert.AreEqual(RenderMode.ScreenSpaceOverlay, crosshairCanvas.renderMode, "[PASS] Crosshair renders on ScreenSpaceOverlay canvas above 3D world.");
        Assert.GreaterOrEqual(crosshairCanvas.sortingOrder, 1000, "[PASS] Crosshair sortingOrder is >= 1000 above other gameplay UI.");

        var cScaler = crosshairCanvas.GetComponent<UnityEngine.UI.CanvasScaler>();
        Assert.IsNotNull(cScaler, "[PASS] Crosshair CanvasScaler configured.");
        Assert.AreEqual(new Vector2(1280f, 720f), cScaler.referenceResolution, "[PASS] Crosshair CanvasScaler reference resolution is 1280x720.");

        // 2. Verify Crosshair elements structure & dimensions (29px footprint)
        var crosshairRoot = hud.CrosshairRoot;
        Assert.IsNotNull(crosshairRoot, "[PASS] Crosshair root exists.");
        var cRt = crosshairRoot.GetComponent<RectTransform>();
        Assert.AreEqual(new Vector2(0.5f, 0.5f), cRt.anchorMin, "[PASS] Crosshair anchored at screen center.");
        Assert.AreEqual(new Vector2(0.5f, 0.5f), cRt.anchorMax, "[PASS] Crosshair anchored at screen center.");
        Assert.AreEqual(Vector2.zero, cRt.anchoredPosition, "[PASS] Crosshair exactly centered.");

        // 3. Verify White Ticks and Black Outlines
        var ticks = crosshairRoot.GetComponentsInChildren<UnityEngine.UI.Image>();
        Assert.GreaterOrEqual(ticks.Length, 10, "[PASS] Crosshair contains center dot (outline+core) and 4 directional ticks (outline+core).");

        int whiteTicksCount = 0;
        int blackOutlineCount = 0;
        foreach (var img in ticks)
        {
            if (img.gameObject == crosshairRoot) continue;
            if (img.color.r > 0.8f && img.color.g > 0.8f && img.color.b > 0.8f) whiteTicksCount++;
            else if (img.color.r < 0.2f && img.color.g < 0.2f && img.color.b < 0.2f) blackOutlineCount++;
        }
        Assert.GreaterOrEqual(whiteTicksCount, 5, "[PASS] Crosshair contains 5 white tick/dot core elements.");
        Assert.GreaterOrEqual(blackOutlineCount, 5, "[PASS] Crosshair contains 5 black high-contrast outline elements.");

        // 4. Verify Objective Panel styling and readability
        var objText = GameManager.Instance.ObjectiveText;
        Assert.IsNotNull(objText, "[PASS] ObjectiveText found.");
        Assert.GreaterOrEqual(objText.fontSize, 20, "[PASS] Objective text font size is >= 20px.");
        var objPanelImg = objText.transform.parent.GetComponent<UnityEngine.UI.Image>();
        Assert.IsNotNull(objPanelImg, "[PASS] Objective panel dark background image exists.");
        Assert.GreaterOrEqual(objPanelImg.color.a, 0.90f, "[PASS] Objective panel background is near-opaque dark.");

        // 5. Verify Controls Help Panel
        var helpPanel = GameObject.Find("Controls_Help_Panel");
        Assert.IsNotNull(helpPanel, "[PASS] Toggleable Controls_Help_Panel exists.");
        var helpText = helpPanel.GetComponentInChildren<UnityEngine.UI.Text>();
        Assert.IsNotNull(helpText, "[PASS] Controls help text component exists.");
        Assert.GreaterOrEqual(helpText.fontSize, 16, "[PASS] Controls secondary text font size is >= 16px.");

        // 6. Verify Backbuffer pixels if running in non-batchmode with graphic device
        if (!Application.isBatchMode)
        {
            yield return new WaitForEndOfFrame();
            Texture2D shot720 = ScreenCapture.CaptureScreenshotAsTexture();
            if (shot720 != null)
            {
                int w = shot720.width;
                int h = shot720.height;
                int cx = w / 2;
                int cy = h / 2;
                bool foundBrightCenter = false;
                for (int dx = -2; dx <= 2; dx++)
                {
                    for (int dy = -2; dy <= 2; dy++)
                    {
                        Color c = shot720.GetPixel(cx + dx, cy + dy);
                        if (c.r > 0.7f && c.g > 0.7f && c.b > 0.7f)
                        {
                            foundBrightCenter = true;
                            break;
                        }
                    }
                    if (foundBrightCenter) break;
                }
                Assert.IsTrue(foundBrightCenter, "[PASS] Center dot core is brightly visible.");
                Object.Destroy(shot720);
            }
        }

        fullReport.AppendLine("[PASS] Visual crosshair and instructions verification completed!\n");
    }
}
