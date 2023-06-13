using UnityEngine;
using System.Collections;

public class DamagePerpetrator : MonoBehaviour
{
    private IDamagePerpetratorListener[] m_listeners;

    // Use this for initialization
    void Start()
    {
        m_listeners = GetComponents<IDamagePerpetratorListener>();
    }

    public void FireOnDamageDealt(Health victim, in Damage damage)
    {
        foreach(IDamagePerpetratorListener listener in m_listeners)
        {
            listener.OnDamageDealt(victim, damage);
        }
    }
}
