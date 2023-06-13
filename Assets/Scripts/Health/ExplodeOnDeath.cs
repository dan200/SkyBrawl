using UnityEngine;
using System.Collections;

public class ExplodeOnDeath : MonoBehaviour, IHealthListener
{
    public GameObject ExplosionPrefab;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnHealthChanged(Health health)
    {
    }

    public void OnDamageReceived(Health health, in Damage damage)
    {
        if(health.Dead)
        {
            Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
