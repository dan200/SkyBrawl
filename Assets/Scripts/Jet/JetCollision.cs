using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(JetPhysics))]
[RequireComponent(typeof(Health))]
public class JetCollision : MonoBehaviour
{
    private JetPhysics m_physics;
    private Rigidbody m_rigidBody;
    private Health m_health;

    public float CollisionDamageAtFullSpeed = 40.0f;

    public bool IsOnRunway
    {
        get
        {
            return m_runwayColliderCount > 0;
        }
    }

    private int m_runwayColliderCount = 0;
    private bool m_destroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        m_physics = GetComponent<JetPhysics>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_health = GetComponent<Health>();
    }

    private void OnTriggerEnter( Collider other )
    {
        OnImpact(other, m_rigidBody.velocity.magnitude);
    }

    void OnCollisionEnter( Collision collision )
    {
        Vector3 averageNormal = Vector3.zero;
        for(int i=0; i<collision.contactCount; ++i)
        {
            averageNormal += collision.GetContact(i).normal / collision.contactCount;
        }
        OnImpact(collision.collider, Vector3.Dot( collision.relativeVelocity, averageNormal ));

        if (IsRunway(collision.collider))
        {
            m_runwayColliderCount++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(IsRunway(collision.collider))
        {
            m_runwayColliderCount--;
        }
    }

    private bool IsRunway(Collider collider)
    {
        return collider.GetComponentInParent<Runway>() != null;
    }

    private bool IsOcean(Collider collider)
    {
        return collider.GetComponentInParent<Ocean>() != null;
    }

    private void OnImpact( Collider other, float impactSpeed )
    {
        bool shouldDestroy = false;
        bool hitWater = false;
        if (m_health.Dead && !IsRunway(other))
        {
            shouldDestroy = true;
        }
        if(IsOcean(other))
        {
            shouldDestroy = true;
            hitWater = true;
        }
 
        if(shouldDestroy)
        {
            Destroy(hitWater);
        }
        else
        {
            DamagePerpetrator perp = other.GetComponentInParent<DamagePerpetrator>();
            float speedFrac = Mathf.Clamp(impactSpeed / m_physics.MaxForwardSpeed, 0.0f, 1.0f);
            m_health.ApplyDamage(new Damage(DamageType.Collision, speedFrac * CollisionDamageAtFullSpeed, perp));
        }
    }

    private void Destroy( bool hitWater )
    {
        if(!m_destroyed)
        {
            // PFX
            var pfx = GetComponent<JetPFX>();
            if (pfx != null)
            {
                pfx.Explode();
                pfx.DetachEffects();
            }

            // Audio
            var audio = GetComponent<JetAudio>();
            if (audio != null)
            {
                if (hitWater)
                {
                    audio.Splash();
                }
                else
                {
                    audio.Explode();
                }
                audio.DetachEmitters();
            }

            // Detach the camera
            var camera = GetComponentInChildren<Camera>();
            if (camera != null)
            {
                camera.transform.SetParent(null);

                Vector3 deathCamPos = camera.transform.position - camera.transform.forward * 40.0f;
                deathCamPos.y = Mathf.Max(deathCamPos.y, 10.0f);
                camera.transform.position = deathCamPos;
                camera.transform.LookAt(transform.position, Vector3.up);
            }

            // Die
            Destroy(this.gameObject, 0.1f);
            m_destroyed = true;
        }
    }
}
