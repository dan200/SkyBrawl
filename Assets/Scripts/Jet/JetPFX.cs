using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class JetPFX : MonoBehaviour, IHealthListener
{
    private Health m_health;

    public ParticleSystem Smoke;
    public ParticleSystem Explosion;

    float m_initialSmokeEmission;

    // Start is called before the first frame update
    void Start()
    {
        m_health = GetComponent<Health>();
        if(Smoke != null)
        {
            m_initialSmokeEmission = Smoke.emission.rateOverTimeMultiplier;
        }
        UpdateSmoke();
    }

    public void OnHealthChanged(Health health)
    {
        UpdateSmoke();
    }

    public void OnDamageReceived(Health health, in Damage damage)
    {
        if(health.Dead)
        {
            Explode();
        }
    }
    
    public void Explode()
    {
        if(Explosion != null)
        {
            Explosion.Play();
        }
    }

    public void DetachEffects()
    {
        if(Smoke != null)
        {
            DetachEffect(Smoke);
            Smoke = null;
        }

        if (Explosion != null)
        {
            DetachEffect(Explosion);
            Explosion = null;
        }
    }

    private void DetachEffect(ParticleSystem system)
    {
        var main = system.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        if (main.loop)
        {
            system.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        system.transform.SetParent(null);
        system.gameObject.AddComponent<DestroyAfterTime>().Timeout = 5.0f;
    }

    private void UpdateSmoke()
    {
        if(Smoke != null)
        {
            var emission = Smoke.emission;
            var scale = m_health.Dead ? 1.0f : 0.6f * (1.0f - m_health.HealthProp);
            emission.rateOverTimeMultiplier = m_initialSmokeEmission * scale;
        }
    }
}
