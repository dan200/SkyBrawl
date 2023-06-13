using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterControl))]
public class CharacterMovement : MonoBehaviour
{
    private CharacterControl m_control;
    private CharacterController m_character;

    public Transform Eye;

    public float Acceleration = 20.0f;
    public float Decceleration = 50.0f;
    public float AirAcceleration = 30.0f;
    public float RunSpeed = 5.2f;
    public float WalkSpeed = 3.2f;

    public float JumpSpeed = 4.7f;
    public float GravityScale = 1.25f;

    public float MinPitch = -89.0f;
    public float MaxPitch = 89.0f;

    public float StrideLength = 2.0f;
    public float EyeBob = 0.1f;

    public float Recoil = 0.0f;

    public float Pitch
    {
        get;
        private set;
    }

    public bool Grounded
    {
        get;
        private set;
    }

    public Vector3 Velocity
    {
        get;
        private set;
    }

    private Vector3 m_initialEyePos = Vector3.zero;
    private float m_steps = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_control = GetComponent<CharacterControl>();
        m_character = GetComponent<CharacterController>();

        m_initialEyePos = Eye.localPosition;
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        Vector2 moveInput = m_control.Movement;
        bool walkInput = m_control.Walk;
        bool jumpInput = m_control.Jump;

        float speed = walkInput ? WalkSpeed : RunSpeed;
        Vector3 localVelocity = transform.InverseTransformDirection(Velocity);
        Vector3 targetLocalVelocity = Vector3.zero;
        targetLocalVelocity.z += moveInput.y * speed;
        targetLocalVelocity.x += moveInput.x * speed;

        Vector3 localAcceleration = Vector3.zero;
        if (Grounded)
        {
            localAcceleration.x =
                ((Mathf.Abs(targetLocalVelocity.x) >= Mathf.Abs(localVelocity.x)) && (targetLocalVelocity.x * localVelocity.x) >= 0.0f) ?
                Acceleration : Decceleration;

            localAcceleration.z =
                ((Mathf.Abs(targetLocalVelocity.z) >= Mathf.Abs(localVelocity.z)) && (targetLocalVelocity.z * localVelocity.z) >= 0.0f) ?
                Acceleration : Decceleration;
        }
        else
        {
            localAcceleration.z = AirAcceleration * Mathf.Abs(moveInput.y);
            localAcceleration.x = AirAcceleration * Mathf.Abs(moveInput.x);
        }

        localVelocity.x = Mathf.MoveTowards(localVelocity.x, targetLocalVelocity.x, localAcceleration.x * dt);
        localVelocity.z = Mathf.MoveTowards(localVelocity.z, targetLocalVelocity.z, localAcceleration.z * dt);
        Velocity = transform.TransformDirection(localVelocity);

        if (!Grounded)
        {
            var gravity = Physics.gravity * GravityScale;
            Velocity += gravity * dt;
        }

        if (Grounded)
        {
            if (jumpInput)
            {
                Vector3 up = -Physics.gravity.normalized;
                Grounded = false;
                Velocity += up * JumpSpeed;
            }
        }

        Vector3 motion = Velocity * dt;
        if (Grounded)
        {
            motion.y -= 0.1f;
        }

        Vector3 oldPosition = transform.position;
        CollisionFlags flags = m_character.Move(motion);
        Velocity = (transform.position - oldPosition) / dt;
        if ((flags & CollisionFlags.Below) != 0)
        {
            if (!Grounded)
            {
                // Landed
                Grounded = true;
            }
        }
        else
        {
            if (Grounded)
            {
                // Left ground
                Grounded = false;
                m_character.Move(new Vector3(0.0f, -motion.y, 0.0f)); // Undo the ground stickiness
            }
        }
        Velocity = (transform.position - oldPosition) / dt;

        var flatVelocity = new Vector3(Velocity.x, 0.0f, Velocity.z);
        m_steps += (flatVelocity.magnitude / StrideLength) * dt;

        var yawChange = m_control.TurnDelta.x;
        var pitchChange = m_control.TurnDelta.y;
        transform.Rotate(0.0f, yawChange, 0.0f, Space.Self);

        Pitch = Mathf.Clamp(Pitch + pitchChange, MinPitch, MaxPitch);
        Eye.localEulerAngles = new Vector3(-Pitch - Recoil, 0.0f, 0.0f);
        Eye.localPosition = m_initialEyePos + Vector3.up * EyeBob * Mathf.Sin(m_steps * Mathf.PI);

        Recoil *= 0.99f;
    }
}
