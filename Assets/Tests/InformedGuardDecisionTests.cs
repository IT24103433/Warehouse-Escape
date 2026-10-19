using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class InformedGuardDecisionTests
{
    [Test]
    public void Patrol_Changes_Priority_With_Neglect_Visibility_And_Travel()
    {
        Assert.Greater(PatrolDecisionPolicy.Score(30, .5f, 10), PatrolDecisionPolicy.Score(0, .5f, 10));
        Assert.Greater(PatrolDecisionPolicy.Score(15, 1, 10), PatrolDecisionPolicy.Score(15, 0, 10));
        Assert.Greater(PatrolDecisionPolicy.Score(15, .5f, 2), PatrolDecisionPolicy.Score(15, .5f, 25));
        // Neglect can outweigh the extra travel; this is not nearest-neighbour patrol.
        Assert.Greater(PatrolDecisionPolicy.Score(30, .5f, 20), PatrolDecisionPolicy.Score(0, .5f, 1));
    }

    [Test]
    public void Search_Uses_Observed_Heading_Travel_And_Negative_Evidence()
    {
        float ahead = SearchDecisionPolicy.Prior(Vector3.forward * 3, Vector3.forward);
        float behind = SearchDecisionPolicy.Prior(Vector3.back * 3, Vector3.forward);
        Assert.Greater(ahead, behind);
        Assert.Greater(SearchDecisionPolicy.Score(.7f, 3), SearchDecisionPolicy.Score(.2f, 3));
        Assert.Greater(SearchDecisionPolicy.Score(.5f, 1), SearchDecisionPolicy.Score(.5f, 15));
        float reduced = SearchDecisionPolicy.NegativeObservation(.5f);
        Assert.Greater(reduced, 0, "An empty inspection is not certainty that the player cannot be there.");
        Assert.Less(SearchDecisionPolicy.Score(reduced, 1), SearchDecisionPolicy.Score(.4f, 1));
    }

    [Test]
    public void Defense_Trades_Off_Cover_Range_And_Objective_Protection()
    {
        float protectedFiringPost = DefenseDecisionPolicy.Score(.8f, 1, 1, .5f, .5f, 5, true);
        float exposedPoorRange = DefenseDecisionPolicy.Score(.8f, 0, 0, .5f, .5f, 5, true);
        Assert.Greater(protectedFiringPost, exposedPoorRange);
        Assert.Greater(DefenseDecisionPolicy.Score(1, 0, 0, .5f, .5f, 5, false),
            DefenseDecisionPolicy.Score(0, 0, 0, .5f, .5f, 5, false));
        Assert.Greater(DefenseDecisionPolicy.Score(.8f, 1, 1, .5f, .5f, 1, true),
            DefenseDecisionPolicy.Score(.8f, 1, 1, .5f, .5f, 19, true));
    }

    [UnityTest]
    public IEnumerator Patrol_Rejects_Out_Of_Room_Candidate_And_Reports_Decision()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        var patrol = GameObject.Find("Guard_Varun_Patrol").GetComponent<PatrolGuardAI>();
        var original = patrol.Waypoints;
        var outside = new GameObject("Test_Outside_Room");
        outside.transform.position = new Vector3(0, 1, -26);
        var points = new Transform[original.Length + 1];
        original.CopyTo(points, 0); points[original.Length] = outside.transform;
        patrol.Waypoints = points;
        patrol.TransitionToPatrol();
        Assert.Greater(patrol.DecisionCount, 0);
        Assert.IsTrue(patrol.IsInsideRoom(patrol.SelectedDecisionTarget));
        bool rejected = false;
        foreach (var c in patrol.DecisionCandidates)
            if (c.position.z < -18 && !c.reachable) rejected = true;
        Assert.IsTrue(rejected, "Room 1 must never become a candidate patrol target.");
        Object.Destroy(outside);
    }

    [UnityTest]
    public IEnumerator Search_Reorders_Hypotheses_And_Reduces_Belief_After_Inspection()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        var guard = GameObject.Find("Guard_Sanjeevsaran_Search");
        var search = guard.GetComponent<SearchGuardAI>();
        search.SearchTimeout = 15;
        GuardPerception.ReportSound(guard.transform.position, 35, SoundType.Gunshot,
            GameObject.FindGameObjectWithTag("Player"));
        yield return null;
        Assert.AreEqual(SearchGuardState.Search, search.CurrentState);
        int decisionsBeforeBurst = search.DecisionCount;
        for (int i = 0; i < 20; i++)
            GuardPerception.ReportSound(guard.transform.position, 35, SoundType.Gunshot,
                GameObject.FindGameObjectWithTag("Player"));
        Assert.AreEqual(decisionsBeforeBurst, search.DecisionCount,
            "Rapid automatic fire must not cause a complete search replan for every bullet.");
        Assert.Greater(search.DecisionCandidates.Count, 1);
        Assert.IsTrue(search.IsInsideRoom(search.SelectedDecisionTarget));
        float highest = float.NegativeInfinity, chosen = float.NegativeInfinity;
        foreach (var c in search.DecisionCandidates)
            if (c.reachable)
            {
                highest = Mathf.Max(highest, c.score);
                if (Vector3.Distance(c.position, search.SelectedDecisionTarget) < .1f) chosen = c.score;
            }
        Assert.AreEqual(highest, chosen, .0001f, "Select expected information per travel effort, not list order.");
        var agent = guard.GetComponent<NavMeshAgent>();
        Assert.IsTrue(agent.Warp(search.SelectedDecisionTarget));
        Physics.SyncTransforms();
        yield return new WaitForSeconds(1.4f);
        Assert.Less(search.RemainingBeliefMass, 1, "An empty inspected area must lose probability mass.");
    }

    [UnityTest]
    public IEnumerator Defense_Selects_Reachable_Post_And_Respects_Leash()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.5f);
        var defense = GameObject.Find("Guard_Aqqil_Defensive").GetComponent<DefensiveGuardAI>();
        GuardPerception.ReportSound(defense.PostPosition, 35, SoundType.Gunshot,
            GameObject.FindGameObjectWithTag("Player"));
        yield return null;
        Assert.Greater(defense.DecisionCount, 0);
        Assert.Greater(defense.DecisionCandidates.Count, 1);
        Assert.IsTrue(defense.IsInsideRoom(defense.SelectedDecisionTarget));
        Assert.LessOrEqual(Vector3.Distance(defense.SelectedDecisionTarget, defense.PostPosition), defense.LeashDistance);
        var path = new NavMeshPath();
        Assert.IsTrue(defense.GetComponent<EnemyMovement>().CalculatePath(defense.SelectedDecisionTarget, path));
        Assert.AreEqual(NavMeshPathStatus.PathComplete, path.status);
        Assert.IsFalse(defense.GetComponent<GuardShooting>().HasVisualTarget,
            "An informed defensive position does not grant permission to fire at a hidden player.");
    }
}
