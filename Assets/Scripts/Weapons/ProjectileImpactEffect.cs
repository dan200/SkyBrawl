using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Projectile))]
public class ProjectileImpactEffect : MonoBehaviour, IProjectileListener
{
    public GameObject ImpactPrefab;

    // Use this for initialization
    void Start()
    {
    }

    public void OnImpact(Collider hitObject, Vector3 hitPos, Vector3 hitNormal, Vector3 hitVelocity)
    {
        ObjectPool pool = ObjectPool.GetPool(ImpactPrefab);
        pool.Instantiate(hitPos, transform.rotation, null);
    }
}
