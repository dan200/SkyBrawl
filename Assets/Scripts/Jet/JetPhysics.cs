using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(JetControl))]
[RequireComponent(typeof(JetEngine))]
[RequireComponent(typeof(JetCollision))]
public class JetPhysics : MonoBehaviour
{
    private Rigidbody m_rigidBody;
    private JetControl m_control;
    private JetEngine m_engine;
    private JetCollision m_collision;

    public float MaxForwardSpeed = 100.0f;
    public float ForwardAcceleration = 40.0f;
    public float BrakeDrag = 0.2f;
    public float LateralDrag = 0.1f;

    public float GravityScale = 1.0f;

    public float MaxYawSpeed = 45.0f;
    public float YawAcceleration = 180.0f;

    public float MaxPitchSpeed = 90.0f;
    public float PitchAcceleration = 180.0f;

    public float MaxRollSpeed = 180.0f;
    public float RollAcceleration = 180.0f;

    public float MaxAltitude = 1000.0f;

    public float Altitude
    {
        get
        {
            return transform.position.y;
        }
    }

    public float Speed
    {
        get
        {
            return m_rigidBody.velocity.magnitude;
        }
    }

    public float ForwardSpeed
    {
        get
        {
            return Vector3.Dot( m_rigidBody.velocity, transform.forward );
        }
    }

    public float Pitch
    {
        get
        {
            return transform.eulerAngles.x;
        }
    }

    public float Yaw
    {
        get
        {
            return transform.eulerAngles.y;
        }
    }

    public float Roll
    {
        get
        {
            return -transform.eulerAngles.z;
        }
    }

    // Use this for initialization
    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_control = GetComponent<JetControl>();
        m_engine = GetComponent<JetEngine>();
        m_collision = GetComponent<JetCollision>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        Vector3 velocity = m_rigidBody.velocity;
        float speed = velocity.magnitude;

        float forwardSpeed = Vector3.Dot(velocity, transform.forward);
        Vector3 forwardVelocity = forwardSpeed * transform.forward;        
        Vector3 lateralVelocity = velocity - forwardVelocity;
        float lateralSpeed = lateralVelocity.magnitude;
        
        float rotationControl = Mathf.Clamp(forwardSpeed / MaxForwardSpeed, 0.0f, 1.0f);

        // Update rotation rate
        Vector3 localAngularVelocity = transform.InverseTransformDirection(m_rigidBody.angularVelocity) * Mathf.Rad2Deg;

        float targetPitchRate = MaxPitchSpeed * m_control.Pitch;
        localAngularVelocity.x = MathUtils.AdvanceTo(localAngularVelocity.x, targetPitchRate, PitchAcceleration * rotationControl, dt);

        float targetYawRate = MaxYawSpeed * m_control.Yaw;
        localAngularVelocity.y = MathUtils.AdvanceTo(localAngularVelocity.y, targetYawRate, YawAcceleration * rotationControl, dt);

        float targetRollRate = MaxRollSpeed * -m_control.Roll;
        localAngularVelocity.z = MathUtils.AdvanceTo(localAngularVelocity.z, targetRollRate, RollAcceleration * rotationControl, dt);

        m_rigidBody.angularVelocity = transform.TransformDirection( localAngularVelocity * Mathf.Deg2Rad );

        // Apply thrust
        m_rigidBody.AddForce(m_engine.RotorProp * ForwardAcceleration * transform.forward, ForceMode.Acceleration);

        // Apply brake
        float thrust = m_engine.EngineWorking ? m_control.Thrust : 0.0f;
        float brakeFrac = m_collision.IsOnRunway ? (1.0f - thrust) : 0.0f;
        float brakeDrag = brakeFrac * BrakeDrag;
        m_rigidBody.AddForce(brakeDrag * speed * -velocity, ForceMode.Acceleration);

        // Apply drag
        float forwardDrag = ForwardAcceleration / (MaxForwardSpeed * MaxForwardSpeed);
        m_rigidBody.AddForce(forwardDrag * forwardSpeed * -forwardVelocity, ForceMode.Acceleration);
        m_rigidBody.AddForce(LateralDrag * lateralSpeed * -lateralVelocity, ForceMode.Acceleration);

        // Apply gravity
        var gravity = Physics.gravity * GravityScale;
        m_rigidBody.AddForce(gravity, ForceMode.Acceleration);

        /*
        Vector3 totalAccel = Vector3.zero;

        Vector3 gravityAccel = Physics.gravity * GravityScale;
        totalAccel += gravityAccel;

        Vector3 up = -Physics.gravity.normalized;
        Vector3 liftAccel = up * Mathf.Abs(Vector3.Dot(transform.up, up)) * gravityAccel.magnitude * (forwardSpeed / MaxSpeed);
        totalAccel += liftAccel;

        Vector3 targetVelocity = transform.forward * m_control.Thrust * MaxSpeed;
        Vector3 maxAccel = (targetVelocity - Velocity) / dt;
        Vector3 thrustAccel = (targetVelocity - Velocity).normalized * Acceleration;
        if (thrustAccel.magnitude < maxAccel.magnitude)
        {
            totalAccel += thrustAccel;
        }
        else
        {
            totalAccel += maxAccel;
        }

        Velocity += totalAccel * dt;
        transform.position += Velocity * dt;
        */
    }
}
