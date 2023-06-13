using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Projectile))]
public class ProjectileImpulse : MonoBehaviour, IProjectileListener
{
    [Min(0.0f)]
    public float Mass = 1.0f;

    // Use this for initialization
    void Start()
    {
    }

    public void OnImpact(Collider hitObject, Vector3 hitPos, Vector3 hitNormal, Vector3 hitVelocity)
    {
        Rigidbody rigidBody = hitObject.GetComponentInParent<Rigidbody>();
        if(rigidBody != null)
        {
            rigidBody.AddForceAtPosition(hitVelocity * Mass, hitPos, ForceMode.Impulse);
        }
    }
}
