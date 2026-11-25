using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class HeadshotAndShotgunTests
{
    [UnityTest]
    public IEnumerator Real_Gun_Headshots_Double_Damage_Pellets_And_Cover_Still_Blocks()
    {
        SceneManager.LoadScene("Warehouse");yield return new WaitForSeconds(.6f);
        foreach(var h in Object.FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if(h.IsPlayer)continue;
            foreach(var behavior in h.GetComponents<MonoBehaviour>())if(!(behavior is Health))behavior.enabled=false;
            var nav=h.GetComponent<NavMeshAgent>();if(nav!=null && nav.isOnNavMesh){nav.isStopped=true;nav.ResetPath();}
        }
        var player=GameObject.FindGameObjectWithTag("Player");var pm=player.GetComponent<PlayerMovement>();
        var cc=player.GetComponent<CharacterController>();cc.enabled=false;player.transform.position=new Vector3(-5.5f,1.08f,30);cc.enabled=true;
        var boss=GameObject.Find("Guard_Jadavan_Boss");boss.GetComponent<NavMeshAgent>().Warp(new Vector3(-5.5f,1.08f,34));boss.transform.rotation=Quaternion.identity;Physics.SyncTransforms();
        var health=boss.GetComponent<Health>();var animator=boss.GetComponentInChildren<HumanoidAnimator>();
        Vector3 head=animator.head.GetComponent<MeshRenderer>().bounds.center;
        Assert.AreEqual(18,HeadshotDamage.ForHit(health,boss.transform.position,18,out bool body));Assert.IsFalse(body);
        Assert.AreEqual(36,HeadshotDamage.ForHit(health,head,18,out bool critical));Assert.IsTrue(critical);
        Assert.AreEqual(18,HeadshotDamage.ForHit(health,head+boss.transform.right*.5f,18,out bool wide));Assert.IsFalse(wide,"Near misses at head height cannot count as headshots.");
        var gun=player.GetComponentInChildren<PlayerGun>();gun.CurrentWeaponData.spread=0;
        pm.AimAt(boss.transform.position+Vector3.up*.1f);int hp=health.CurrentHealth;
        Assert.IsTrue(gun.TryShoot());Assert.AreEqual(hp-18,health.CurrentHealth,"Actual body hit retains base damage.");
        yield return new WaitForSeconds(.2f);
        pm.AimAt(animator.head.GetComponent<MeshRenderer>().bounds.center);hp=health.CurrentHealth;
        Assert.IsTrue(gun.TryShoot());Assert.AreEqual(hp-36,health.CurrentHealth,"Actual camera/muzzle ray headshot doubles damage.");
        yield return null;
        Assert.Greater(GameObject.Find("Crosshair_HitMarker").GetComponent<UnityEngine.UI.Image>().color.g,.7f,"Critical hit marker is gold.");
        var cover=GameObject.CreatePrimitive(PrimitiveType.Cube);cover.transform.position=new Vector3(-5.5f,1.6f,32);cover.transform.localScale=new Vector3(2,3,.3f);Physics.SyncTransforms();
        yield return new WaitForSeconds(.2f);
        pm.AimAt(animator.head.GetComponent<MeshRenderer>().bounds.center);hp=health.CurrentHealth;
        Assert.IsTrue(gun.TryShoot());Assert.AreEqual(hp,health.CurrentHealth,"Headshots cannot bypass cover.");
        Object.DestroyImmediate(cover);Physics.SyncTransforms();
        gun.EquipWeapon(WeaponType.Combat_Shotgun,8,int.MaxValue);gun.CurrentWeaponData.pelletSpread=0;
        pm.AimAt(animator.head.GetComponent<MeshRenderer>().bounds.center);hp=health.CurrentHealth;
        Assert.IsTrue(gun.TryShoot());Assert.AreEqual(hp-12*6*2,health.CurrentHealth,"Only intersecting shotgun pellets receive their own headshot multiplier.");
        Assert.AreEqual(1f,gun.ShootAudioSource.pitch);
        var recording=WeaponAudio.GetWeaponFireClip(WeaponType.Combat_Shotgun);
        Assert.AreEqual(2.2f,recording.length,.01f);Assert.AreEqual(44100,recording.frequency);
        // Exercise the actual guard shotgun playback path in this isolated fixture.
        var shooter=GameObject.Find("Guard_Aqqil_Defensive").GetComponent<GuardShooting>();
        var source=(AudioSource)typeof(GuardShooting).GetField("audioSource",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(shooter);
        source.pitch=.95f;
        typeof(GuardShooting).GetMethod("ExecuteShotgunShot",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(shooter,null);
        Assert.AreEqual(1f,source.pitch,"Guard shotguns preserve the recording's original pitch too.");
    }
}
