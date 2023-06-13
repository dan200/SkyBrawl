using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialProperty
{
    Altitude,
    Speed,
    Fuel,
}

public class DialAnimation : MonoBehaviour
{
    private JetPhysics m_physics;
    private JetEngine m_engine;

    public DialProperty Property = DialProperty.Altitude;
    public float MinAngle = -140.0f;
    public float MaxAngle = 140.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_physics = GetComponentInParent<JetPhysics>();
        m_engine = GetComponentInParent<JetEngine>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.Lerp(MinAngle, MaxAngle, ReadProperty());
        transform.localEulerAngles = Vector3.forward * angle;
    }

    private float ReadProperty()
    {
        switch(Property)
        {
            case DialProperty.Altitude:
                {
                    float altitude = m_physics.Altitude;
                    float maxAltitude = m_physics.MaxAltitude;
                    return Mathf.Clamp(altitude / maxAltitude, 0.0f, 1.0f);
                }
            case DialProperty.Speed:
                {
                    float forwardSpeed = m_physics.ForwardSpeed;
                    float maxForwardSpeed = m_physics.MaxForwardSpeed;
                    return Mathf.Clamp(forwardSpeed / maxForwardSpeed, 0.0f, 1.0f);
                }
            case DialProperty.Fuel:
                {
                    return m_engine.FuelProp; 
                }
            default:
                {
                    Debug.LogError("Unhandled enum value");
                    return 0.0f;
                }
        }
    }
}
