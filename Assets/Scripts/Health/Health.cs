using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private IDamageFilter[] m_damageFilters;
    private IHealthListener[] m_healthListeners;

    [SerializeField]
    [Min(0.0f)]
    private float m_maxHealth = 100.0f;
    public float MaxHealth
    {
        get
        {
            return m_maxHealth;
        }
    }

    [SerializeField]
    [Min(0.0f)]
    private float m_health = 100.0f;
    public float CurrentHealth
    {
        get
        {
            return m_health;
        }
        set
        {
            Debug.Assert(value >= 0.0f && value <= MaxHealth);
            var oldHealth = m_health;
            m_health = value;
            if (m_health != oldHealth)
            {
                FireOnHealthChanged();
            }
        }
    }

    public bool Invulnerable = false;
    public bool Invincible = false;

    public float HealthProp
    {
        get
        {
            return CurrentHealth / MaxHealth;
        }
        set
        {
            Debug.Assert(value >= 0.0f && value <= 1.0f);
            CurrentHealth = value * MaxHealth;
        }
    }

    public bool Dead
    {
        get;
        private set;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_health <= m_maxHealth);
        m_damageFilters = GetComponents<IDamageFilter>();
        m_healthListeners = GetComponents<IHealthListener>();
    }

    public void ApplyDamage(in Damage damage)
    {
        Debug.Assert(damage.Amount >= 0.0f);
        if (Dead)
        {
            return;
        }

        // Modify the damage
        Damage filteredDamage = damage;
        if (!FilterDamage(ref filteredDamage))
        {
            return;
        }

        // Reduce the health
        float oldHealth = m_health;
        if (!Invulnerable)
        {
            m_health = Mathf.Max(m_health - filteredDamage.Amount, 0.0f);
            if(m_health <= 0.0f && !Invincible)
            {
                Dead = true;
            }
        }

        // Inform listeners
        if(m_health != oldHealth)
        {
            FireOnHealthChanged();
        }
        FireOnDamageReceived(filteredDamage);
        if (damage.Perpetrator != null)
        {
            damage.Perpetrator.FireOnDamageDealt(this, damage);
        }
    }

    private bool FilterDamage(ref Damage io_damage)
    {
        foreach (IDamageFilter filter in m_damageFilters)
        {
            if (!filter.FilterDamage(ref io_damage))
            {
                return false;
            }
        }
        return true;
    }

    private void FireOnHealthChanged()
    {
        foreach (IHealthListener listener in m_healthListeners)
        {
            listener.OnHealthChanged(this);
        }
    }

    private void FireOnDamageReceived(in Damage damage)
    {
        foreach (IHealthListener listener in m_healthListeners)
        {
            listener.OnDamageReceived(this, damage);
        }
    }
}
