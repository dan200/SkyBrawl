using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellorAnimation : MonoBehaviour
{
    private JetEngine m_engine;

    public float MinSpeed = 30.0f;
    public float MaxSpeed = 720.0f;
    public bool Clockwise = true;

    private float m_angle;

    // Start is called before the first frame update
    void Start()
    {
        m_engine = GetComponentInParent<JetEngine>();
        m_angle = Random.Range(0.0f, 360.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_angle += (Clockwise ? -1.0f : 1.0f) * Mathf.Lerp(MinSpeed, MaxSpeed, m_engine.RotorProp) * dt;
        transform.localEulerAngles = Vector3.forward * m_angle;
    }
}
