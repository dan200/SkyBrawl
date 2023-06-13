using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JetControl))]
[RequireComponent(typeof(Health))]
public class JetEngine : MonoBehaviour
{
    private Health m_health;
    private JetControl m_control;

    public float RotorSpinupTime = 5.0f;
    public bool StartActive = false;

    public float FuelLifetime = 60.0f;
    public bool InfiniteFuel = false;

    public float RotorProp
    {
        get;
        private set;
    }

    public float FuelProp
    {
        get;
        private set;
    }

    public bool HasFuel
    {
        get
        {
            return InfiniteFuel || FuelProp > 0.0f;
        }
    }

    public bool EngineTurnedOn = true;

    public bool EngineWorking
    {
        get
        {
            if(m_health == null)
            {
                Debug.Break();
            }
            return EngineTurnedOn && HasFuel && !m_health.Dead;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_health = GetComponent<Health>();
        m_control = GetComponent<JetControl>();
    }

    void Start()
    {
        FuelProp = 1.0f;
        RotorProp = StartActive ? 1.0f : 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        // Drain fuel
        float fuelDrainFrac = m_health.Dead ? 0.0f : RotorProp;
        float fuelPropDrain = fuelDrainFrac * (dt / FuelLifetime);
        FuelProp = Mathf.Max(FuelProp - fuelPropDrain, 0.0f);

        // Spin up rotor
        float targetRotorProp = EngineWorking ? m_control.Thrust : 0.0f;
        RotorProp = MathUtils.AdvanceTo(RotorProp, targetRotorProp, 1.0f / RotorSpinupTime, dt);
    }
}
