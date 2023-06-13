using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(JetControl))]
[RequireComponent(typeof(Health))]
public class JetWeapons : MonoBehaviour
{
    private JetControl m_control;
    private Health m_health;
    private WeaponControl[] m_weapons;
    
    // Use this for initialization
    void Start()
    {
        m_control = GetComponent<JetControl>();
        m_health = GetComponent<Health>();
        m_weapons = GetComponentsInChildren<WeaponControl>();
    }
    
    void Update()
    {
        bool fire = m_control.PrimaryFire && !m_health.Dead;
        foreach(WeaponControl weapon in m_weapons)
        {
            weapon.Fire = fire;
        }
    }
}
