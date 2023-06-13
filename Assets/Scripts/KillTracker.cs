using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamagePerpetrator))]
public class KillTracker : MonoBehaviour, IDamagePerpetratorListener
{
    public int KillCount
    {
        get;
        private set;
    }

    // Use this for initialization
    void Start()
    {
    }

    void OnGUI()
    {
    }

    public void OnDamageDealt(Health victim, in Damage damage)
    {
    }

    public void OnKilled(Health victim, in Damage fatalDamage)
    {
        KillCount++;
    }
}
