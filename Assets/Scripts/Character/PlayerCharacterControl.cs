using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(CharacterControl))]
public class PlayerCharacterControl : MonoBehaviour
{
    private const float MOUSELOOK_BASE_DEGREES_PER_PIXEL = 0.022f;

    public CharacterControl m_character;

    public InputAction MoveInput;
    public AnimationCurve MoveInputResponseCurve;
    public InputAction WalkInput;
    public InputAction JumpInput;
    public InputAction TurnInput;
    public AnimationCurve TurnInputResponseCurve;
    public InputAction PrimaryFireInput;

    [SerializeField]
    private bool m_mouseLook;

    public bool MouseLook
    {
        get
        {
            return m_mouseLook;
        }
        set
        {
            m_mouseLook = value;
            if (enabled)
            {
                Cursor.lockState = m_mouseLook ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !m_mouseLook;
            }
        }
    }

    public float MouseLookSensitivity = 5.0f;
    public bool InvertMouseY = false;

    public float YawSensitivity = 1.0f;
    public float PitchSensitivity = 0.5f;
    public float BaseAimSpeed = 100.0f;
    public float BaseTurnSpeed = 320.0f;
    public float AimToTurnTransitionTime = 0.33f;

    private float m_timeSpentTurning;

    // Start is called before the first frame update
    void Start()
    {
        m_character = GetComponent<CharacterControl>();
        m_timeSpentTurning = 0.0f;
    }

    void OnEnable()
    {
        if (MouseLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        MoveInput.Enable();
        WalkInput.Enable();
        JumpInput.Enable();
        TurnInput.Enable();
        PrimaryFireInput.Enable();
    }

    void OnDisable()
    {
        if (MouseLook)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        MoveInput.Disable();
        WalkInput.Disable();
        JumpInput.Disable();
        TurnInput.Disable();
        PrimaryFireInput.Disable();

        m_character.Reset();
    }

    private Vector2 ApplyStickCurve(Vector2 input, AnimationCurve curve)
    {
        if (input != Vector2.zero)
        {
            var newMagnitude = Mathf.Clamp01(curve.Evaluate(Mathf.Clamp01(input.magnitude)));
            return input.normalized * newMagnitude;
        }
        else
        {
            return input;
        }
    }


    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

#if false && UNITY_EDITOR
        if(Keyboard.current.escapeKey.isPressed)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif

        // Movement
        var moveInput = ApplyStickCurve(MoveInput.ReadValue<Vector2>(), MoveInputResponseCurve);
        m_character.Movement = moveInput;
        m_character.Walk = WalkInput.ReadValue<float>() < 0.5f;
        m_character.Jump = JumpInput.triggered;

        // Aim
        var turnDelta = Vector2.zero;
        if (MouseLook)
        {
            var mouseTurnInput = Mouse.current.delta.ReadValue();
            turnDelta.x += mouseTurnInput.x * MouseLookSensitivity * MOUSELOOK_BASE_DEGREES_PER_PIXEL;
            turnDelta.y += mouseTurnInput.y * MouseLookSensitivity * MOUSELOOK_BASE_DEGREES_PER_PIXEL * (InvertMouseY ? -1.0f : 1.0f);
        }

        var turnInput = ApplyStickCurve(TurnInput.ReadValue<Vector2>(), TurnInputResponseCurve);
        if (turnInput.magnitude > 1.0f)
        {
            turnInput = turnInput.normalized;
        }
        if (turnInput.magnitude >= 0.99f)
        {
            m_timeSpentTurning += dt;
        }
        else
        {
            m_timeSpentTurning = 0.0f;
        }

        float turnRate = Mathf.Lerp(BaseAimSpeed, BaseTurnSpeed, Mathf.Clamp01(m_timeSpentTurning / AimToTurnTransitionTime));
        turnDelta.x += turnInput.x * YawSensitivity * turnRate * dt;
        turnDelta.y += turnInput.y * PitchSensitivity * turnRate * dt;
        m_character.TurnDelta = turnDelta;

        // Weapons
        m_character.PrimaryFire = PrimaryFireInput.ReadValue<float>() >= 0.5f;
    }
}
