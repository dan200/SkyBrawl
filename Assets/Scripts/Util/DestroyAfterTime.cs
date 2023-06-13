using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour, IResettable
{
    public float Timeout = 5.0f;

    private float m_timeLeft;

    // Start is called before the first frame update
    void Awake()
    {
    }

    void Start()
    {
        m_timeLeft = Timeout;
    }

    public void Reset(GameObject prefab)
    {
        var baseComponent = GetComponent<DestroyAfterTime>();
        Timeout = baseComponent.Timeout;
        m_timeLeft = Timeout;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_timeLeft -= dt;
        if(m_timeLeft <= 0.0f)
        {
            PooledObject.ReturnToPool(this.gameObject);
        }
    }
}
