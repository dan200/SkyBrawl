using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : MonoBehaviour, IResettable
{
    [Min(0.0f)]
    public float Range = 3.0f;

    [Min(0.0f)]
    public float Lifetime = 0.5f;

    public float Damage = 100.0f;

    public AnimationCurve DamageFalloff;

    private List<Collider> m_hitObjects = new List<Collider>();
    private float m_age = 0.0f;

    private Collider[] m_hitBuffer = new Collider[8];

    // Use this for initialization
    void Awake()
    {
    }

    void Start()
    {
        m_age = 0.0f;
        transform.localScale = Vector3.zero;
    }

    public void Reset(GameObject prefab)
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_age += Time.deltaTime;

        float lifeFraction = Mathf.Clamp( m_age / Lifetime, 0.0f, 1.0f );
        float radius = lifeFraction * Range;
        transform.localScale = Vector3.one * lifeFraction * Range;

        int numResults = Physics.OverlapSphereNonAlloc(transform.position, radius, m_hitBuffer);
        for(int i=0; i<numResults; ++i)
        {
            ref Collider hit = ref m_hitBuffer[i];
            if(!m_hitObjects.Contains(hit))
            {
                Health health = hit.GetComponentInParent<Health>();
                if(health != null)
                {
                    float damage = DamageFalloff.Evaluate(lifeFraction) * Damage;
                    health.ApplyDamage(new Damage(DamageType.Explosion, damage, null)); // TODO
                }
                m_hitObjects.Add(hit);
            }
        }

        if(lifeFraction >= 1.0f)
        {
            PooledObject.ReturnToPool(this.gameObject);
        }
    }
}
