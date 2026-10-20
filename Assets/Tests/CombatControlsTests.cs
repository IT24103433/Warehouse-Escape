using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class CombatControlsTests
{
    [UnityTest]
    public IEnumerator Health_Crouch_Blood_And_Hearing_Work_In_Saved_Scene()
    {
        SceneManager.LoadScene("Warehouse");
        yield return new WaitForSeconds(.6f);
        foreach (var shooter in Object.FindObjectsByType<GuardShooting>(FindObjectsSortMode.None)) shooter.enabled = false;
        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player.GetComponent<PlayerMovement>();
        var cc = player.GetComponent<CharacterController>();
        float height = cc.height;
        Assert.IsTrue(move.SetCrouching(true));
        Assert.Less(cc.height, height);
        Assert.IsFalse(move.TryJump(), "Crouched player cannot jump through low ceilings.");
        var ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.transform.position = player.transform.TransformPoint(cc.center) + Vector3.up * (height * .65f);
        ceiling.transform.localScale = new Vector3(2f, .1f, 2f);
        Physics.SyncTransforms();
        Assert.IsFalse(move.SetCrouching(false), "Standing blocked by overhead obstacle.");
        Object.DestroyImmediate(ceiling);
        Physics.SyncTransforms();
        Assert.IsTrue(move.SetCrouching(false));
        Assert.AreEqual(height, cc.height, .001f);
        yield return new WaitForSeconds(.3f);
        float floorY = player.transform.position.y;
        Assert.IsTrue(move.TryJump(), "Grounded player can jump.");
        yield return new WaitForSeconds(.2f);
        Assert.Greater(player.transform.position.y, floorY + .1f);
        Assert.IsFalse(move.TryJump(), "Cannot jump again in mid-air.");
        var material = Resources.Load<Material>("Effects/BloodParticles");
        Assert.IsNotNull(material);
        Assert.IsTrue(material.shader.isSupported);
        var guard = GameObject.Find("Guard_Varun_Patrol");
        var hp = guard.GetComponent<Health>();
        Assert.AreEqual(120, hp.MaxHealth);
        Assert.IsTrue(hp.TakeDamage(10, null));
        Assert.Greater(hp.GetComponent<DamageBloodEffect>().Intensity, 0f);
        Assert.AreEqual(12, WeaponData.CreateShotgun().damage);
        var memory = guard.GetComponent<GuardPerceptionMemory>();
        Vector3 origin = guard.transform.position + guard.transform.right * 1f + Vector3.up;
        GuardPerception.ReportSound(origin, 34f, SoundType.Gunshot, player);
        Assert.IsTrue(memory.HasValidSound);
        Assert.Less(Vector3.Distance(memory.LastSoundPosition, origin), .01f);
        foreach (var boss in Object.FindObjectsByType<BossSquadMember>(FindObjectsSortMode.None))
            Assert.AreEqual(360, boss.GetComponent<Health>().MaxHealth);
        yield return null;
    }
}
