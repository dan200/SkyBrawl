using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JetControl))]
[RequireComponent(typeof(JetPhysics))]
[RequireComponent(typeof(Health))]
public class AIJetControl : MonoBehaviour, IHealthListener
{
    private JetControl m_control;
    private JetPhysics m_physics;
    private Health m_health;

    public BattlefieldVolume Battlefield;
    public Vector3? RoamTarget;
    public Health AttackTarget;

    public float RoamingThrust = 0.8f;

    public float AttackingThrust = 1.0f;
    public float SwerveDistance = 30.0f;
    public float AttackDistance = 150.0f;
    public float GiveUpDistance = 500.0f;
    public float GiveUpTime = 60.0f;

    private enum State
    {
        Roaming,
        Attack,
        Dead,
    }

    private bool m_randomFiring;
    private float m_randomFireTimer;
    private float m_timeSinceCombatWithTarget;

    // Start is called before the first frame update
    void Awake()
    {
        m_control = GetComponent<JetControl>();
        m_physics = GetComponent<JetPhysics>();
        m_health = GetComponent<Health>();
    }

    void OnEnable()
    {
        m_randomFiring = false;
        m_randomFireTimer = Random.Range(5.0f, 10.0f);
        if(!m_health.Dead)
        {
            m_physics.GravityScale = 0.0f;
        }
    }

    void OnDisable()
    {
        m_control.Reset();
        m_physics.GravityScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_health.Dead)
        {
            UpdateDead();
        }
        else if(AttackTarget != null)
        {
            UpdateAttacking();
        }
        else
        {
            UpdateRoaming();
        }
    }

    void OnDrawGizmos()
    {
        if (m_health != null && !m_health.Dead)
        {
            if (AttackTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, AttackTarget.transform.position - transform.position);
            }
            else if (RoamTarget.HasValue)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, RoamTarget.Value);
            }
        }
    }

    void UpdateDead()
    {
        StopFiring();
    }

    void UpdateAttacking()
    {
        if (AttackTarget == null || AttackTarget.Dead || AttackTarget.GetComponent<JetCollision>().IsOnRunway)
        {
            AttackTarget = null;
        }

        if (AttackTarget != null)
        {
            var myPos = transform.position;
            var targetPos = AttackTarget.transform.position;
            var targetVel = AttackTarget.GetComponent<Rigidbody>().velocity;
            var targetRange = (targetPos - myPos).magnitude;

            m_timeSinceCombatWithTarget += Time.deltaTime;
            if(targetRange >= GiveUpDistance || m_timeSinceCombatWithTarget >= GiveUpTime)
            {
                FlyStraight(AttackingThrust);
                StopFiring();
                AttackTarget = null;
            }
            else if (targetRange < AttackDistance)
            {
                var weaponSpeed = 100.0f; // TODO UNHARDCODE
                var timeToTarget = (weaponSpeed > 0.0f) ? ((targetPos - myPos).magnitude / weaponSpeed) : 0.0f;
                var predictedTargetPos = targetPos + targetVel * timeToTarget;
                FlyTowardsPoint(AttackingThrust, predictedTargetPos);
                FireAtPoint(predictedTargetPos);
            }
            else
            {
                var mySpeed = m_physics.MaxForwardSpeed * AttackingThrust;
                var timeToTarget = (mySpeed > 0.0f) ? ((targetPos - myPos).magnitude / mySpeed) : 0.0f;
                var predictedTargetPos = targetPos + targetVel * timeToTarget;
                FlyTowardsPoint(AttackingThrust, predictedTargetPos);
                StopFiring();
            }
        }
        else
        {
            FlyStraight(AttackingThrust);
            StopFiring();
        }
    }

    void UpdateRoaming()
    {
        if(!RoamTarget.HasValue && Battlefield != null)
        {
            RoamTarget = Battlefield.PickRandomPointInVolume();
        }

        if(RoamTarget.HasValue)
        {
            FlyTowardsPoint(RoamingThrust, RoamTarget.Value);
            FireRandomly();

            Vector2 flatToTarget = RoamTarget.Value - transform.position;
            flatToTarget.y = 0.0f;
            if(flatToTarget.magnitude < 10.0f)
            {
                RoamTarget = null;
            }
        }
        else
        {
            FlyStraight(RoamingThrust);
            FireRandomly();
        }
    }

    private void FlyStraight(float maxThrust)
    {
        Fly(maxThrust, 0.0f, m_control.Yaw, 0.0f);
    }

    private void FlyTowardsPoint(float maxThrust, Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;
        float toTargetXZ = new Vector2(toTarget.x, toTarget.z).magnitude;
        float targetPitch = -Mathf.Atan2(toTarget.y, toTargetXZ) * Mathf.Rad2Deg;
        float targetYaw = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
        Fly(maxThrust, targetPitch, targetYaw, 0.0f);
    }

    private void Fly(float maxThrust, float targetPitch, float targetYaw, float targetRoll)
    {
        float currentPitch = m_physics.Pitch;
        float pitchDiff = Mathf.DeltaAngle(currentPitch, targetPitch);
        m_control.Pitch = Mathf.Clamp(pitchDiff / 30.0f, -1.0f, 1.0f);

        float currentYaw = m_physics.Yaw;
        float yawDiff = Mathf.DeltaAngle(currentYaw, targetYaw);
        m_control.Yaw = Mathf.Clamp(yawDiff / 10.0f, -1.0f, 1.0f);
        m_control.Thrust = maxThrust * Mathf.Lerp(0.5f, 1.0f, Mathf.Clamp01(Mathf.Abs(yawDiff) / 30.0f));

        float currentRoll = m_physics.Roll;
        float rollDiff = Mathf.DeltaAngle(currentRoll, targetRoll);
        m_control.Roll = Mathf.Clamp(rollDiff / 30.0f, -1.0f, 1.0f);
    }

    private void FireRandomly()
    {
        float dt = Time.deltaTime;
        if (m_randomFiring)
        {
            m_control.PrimaryFire = true;
            m_randomFireTimer -= dt;
            if (m_randomFireTimer <= 0.0f)
            {
                m_randomFiring = false;
                m_randomFireTimer = Random.Range(5.0f, 10.0f);
            }
        }
        else
        {
            m_control.PrimaryFire = false;
            m_randomFireTimer -= dt;
            if (m_randomFireTimer <= 0.0f)
            {
                m_randomFiring = true;
                m_randomFireTimer = Random.Range(1.0f, 2.0f);
            }
        }
    }

    private void FireAtPoint(Vector3 targetPos)
    {
        var toTarget = targetPos - transform.position;
        var facing = Vector3.Dot(transform.forward, toTarget.normalized);
        m_control.PrimaryFire = (facing > 0.8f);
    }

    private void StopFiring()
    {
        m_control.PrimaryFire = false;
    }

    public void OnHealthChanged(Health health)
    {
    }

    public void OnDamageReceived(Health health, in Damage damage)
    {
        if (isActiveAndEnabled)
        {
            if(health.Dead)
            {
                // We were killed
                m_control.Roll = Random.Range(-0.5f, 0.5f);
                m_control.Pitch = -0.25f;
                m_physics.GravityScale = 1.0f;
            }
            else if (damage.Type == DamageType.ProjectileImpact)
            {
                // We were shot at, decide whether to retatilate
                var attacker = (damage.Perpetrator != null) ? damage.Perpetrator.GetComponent<Health>() : null;
                if (attacker != null && !attacker.Dead)
                {
                    bool retaliate = false;
                    var aiJetControl = attacker.transform.GetComponent<AIJetControl>();
                    var playerJetControl = attacker.transform.GetComponent<PlayerJetControl>();
                    if (aiJetControl != null && aiJetControl.enabled)
                    {
                        retaliate = (aiJetControl.AttackTarget != m_health) && Random.value > 0.25f;
                    }
                    else if (playerJetControl != null && playerJetControl.enabled)
                    {
                        retaliate = true;
                    }
                    if (retaliate)
                    {
                        AttackTarget = damage.Perpetrator.GetComponent<Health>();
                        m_timeSinceCombatWithTarget = 0.0f;
                    }
                }
            }
        }
    }
}
