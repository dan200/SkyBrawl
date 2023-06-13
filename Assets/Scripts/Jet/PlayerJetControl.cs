using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(JetControl))]
public class PlayerJetControl : MonoBehaviour
{
    private JetControl m_jet;

    public InputAction ThrustInput;
    public InputAction PitchInput;
    public InputAction YawInput;
    public InputAction RollInput;
    public InputAction PrimaryFireInput;


    // Start is called before the first frame update
    void Start()
    {
        m_jet = GetComponent<JetControl>();
    }

    void OnEnable()
    {
        ThrustInput.Enable();
        PitchInput.Enable();
        YawInput.Enable();
        RollInput.Enable();
        PrimaryFireInput.Enable();
    }

    void OnDisable()
    {
        ThrustInput.Disable();
        PitchInput.Disable();
        YawInput.Disable();
        RollInput.Disable();
        PrimaryFireInput.Disable();

        m_jet.Reset();
    }

     // Update is called once per frame
    void Update()
    {
        // Flight
        m_jet.Thrust = ThrustInput.ReadValue<float>();
        m_jet.Pitch = PitchInput.ReadValue<float>();
        m_jet.Yaw = YawInput.ReadValue<float>();
        m_jet.Roll = RollInput.ReadValue<float>();

        // Weapons
        m_jet.PrimaryFire = PrimaryFireInput.ReadValue<float>() >= 0.5f;
    }
}
