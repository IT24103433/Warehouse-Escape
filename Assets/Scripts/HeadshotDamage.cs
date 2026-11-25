using UnityEngine;

/// <summary>Projects the animated head's height/width onto the guard's existing hit capsule.</summary>
public static class HeadshotDamage
{
    public const int Multiplier = 2;
    public static int ForHit(Health target, Vector3 point, int baseDamage, out bool headshot)
    {
        headshot=false;
        if(target==null || target.IsPlayer || target.IsDead || baseDamage<=0)return baseDamage;
        var animator=target.GetComponentInChildren<HumanoidAnimator>();
        var renderer=animator!=null && animator.head!=null ? animator.head.GetComponent<MeshRenderer>() : null;
        if(renderer==null)return baseDamage;
        Bounds bounds=renderer.bounds;
        // Ignore depth: the capsule's surface sits in front of the visible face.
        // Restrict width so a shoulder hit at head height cannot earn a critical hit.
        float lateral=Mathf.Abs(Vector3.Dot(point-bounds.center,target.transform.right));
        float radius=Mathf.Max(bounds.extents.x,bounds.extents.z)+.025f;
        headshot=point.y>=bounds.min.y && point.y<=bounds.max.y && lateral<=radius;
        return headshot ? baseDamage*Multiplier : baseDamage;
    }
}
