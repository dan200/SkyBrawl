using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Projectile))]
public class ProjectileDamage : MonoBehaviour, IProjectileListener
{
    [Min(0.0f)]
    public float Damage = 1.0f;

    public DamagePerpetrator Perpetrator;

    // Use this for initialization
    void Start()
    {
    }

    public void OnImpact(Collider hitObject, Vector3 hitPos, Vector3 hitNormal, Vector3 hitVelocity)
    {
        Health health = hitObject.GetComponentInParent<Health>();
        if(health != null)
        {
            health.ApplyDamage(new Damage(DamageType.ProjectileImpact, Damage, Perpetrator));
        }
    }
}
