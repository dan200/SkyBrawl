using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JetEngine))]
[RequireComponent(typeof(JetPhysics))]
public class JetAudio : MonoBehaviour, IHealthListener
{
    private JetEngine m_engine;
    private JetPhysics m_physics;

    public AudioSource EngineEmitter;
    public float MinEnginePitch = 0.5f;
    public float MaxEnginePitch = 1.2f;
    public float MaxEngineVolume = 1.0f;

    public AudioSource WindEmitter;
    public float MaxWindVolume = 1.0f;

    public AudioSource ExplosionEmitter;
    public AudioClip[] ExplosionClips;
    public AudioClip SplashClip;

    // Start is called before the first frame update
    void Start()
    {
        m_engine = GetComponentInParent<JetEngine>();
        m_physics = GetComponentInParent<JetPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        EngineEmitter.pitch = Mathf.Lerp(MinEnginePitch, MaxEnginePitch, m_engine.RotorProp);

        float targetEngineVolume = m_engine.EngineWorking ? MaxEngineVolume : (MaxEngineVolume * m_engine.RotorProp);
        EngineEmitter.volume = MathUtils.AdvanceTo(EngineEmitter.volume, targetEngineVolume, 0.25f, dt);
        if(WindEmitter != null)
        {
            WindEmitter.volume = 
                MaxWindVolume *
                (1.0f - (EngineEmitter.volume / MaxEngineVolume)) *
                Mathf.Clamp01(m_physics.Speed / m_physics.MaxForwardSpeed);
        }
    }

    public void Explode()
    {
        var clip = ExplosionClips[Random.Range(0, ExplosionClips.Length)];
        ExplosionEmitter.PlayOneShot(clip);
    }

    public void Splash()
    {
        var clip = SplashClip;
        ExplosionEmitter.PlayOneShot(clip);
    }

    public void DetachEmitters()
    {
        DetachEmitter(ExplosionEmitter);
        EngineEmitter.Stop();
        if(WindEmitter != null)
        {
            WindEmitter.Stop();
        }
    }

    private void DetachEmitter(AudioSource emitter)
    {
        emitter.transform.SetParent(null);
        emitter.gameObject.AddComponent<DestroyAfterTime>().Timeout = 5.0f;
    }

    public void OnHealthChanged(Health health)
    {
    }

    public void OnDamageReceived(Health health, in Damage damage)
    {
        if(health.Dead)
        {
            Explode();
        }
    }
}
