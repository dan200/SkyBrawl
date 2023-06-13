using UnityEngine;
using System.Collections;
using URandom = UnityEngine.Random;

[RequireComponent(typeof(Ammo))]
[RequireComponent(typeof(WeaponControl))]
public class Weapon : MonoBehaviour
{
    private ObjectPool m_projectilePool;
    private DamagePerpetrator m_damagePerpetrator;
    private Rigidbody m_rigidBody;
    private WeaponControl m_control;
    private Ammo m_ammo;
    private IWeaponListener[] m_weaponListeners;

    [Min(0.0f)]
    public float RateOfFire = 10.0f;
    public bool Automatic = false;

    public GameObject ProjectilePrefab;
    public Transform ProjectileTransform;
    public bool RandomiseProjectileRoll = false;

    public float ProjectileSpeed = 20.0f;
    public Vector3 ProjectileVelocityInheritance = Vector3.one;

    [Min(0.0f)]
    public float SpreadAngle = 0.0f;

    [Min(0)]
    public int AmmoPerShot = 1;

    [Min(0)]
    public int ProjectilesPerAmmo = 1;

    private float m_timeUntilFireAllowed = 0.0f;
    private bool m_firing = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_projectilePool = ObjectPool.GetPool(ProjectilePrefab);
        m_damagePerpetrator = GetComponentInParent<DamagePerpetrator>();
        m_rigidBody = GetComponentInParent<Rigidbody>();
        m_control = GetComponent<WeaponControl>();
        m_ammo = GetComponent<Ammo>();
        m_weaponListeners = GetComponents<IWeaponListener>();
    }

    private bool UpdateFiringMechanism(bool fireInput, float dt)
    {
        m_timeUntilFireAllowed = Mathf.Max(m_timeUntilFireAllowed - dt, 0.0f);

        if (fireInput)
        {
            bool wasFiring = m_firing;
            m_firing = true;

            if (m_timeUntilFireAllowed <= 0.0f && (Automatic || !wasFiring))
            {
                m_timeUntilFireAllowed = (1.0f / RateOfFire);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            m_firing = false;
            return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        bool fireInput = m_control.Fire;
        bool fire = UpdateFiringMechanism(fireInput, dt);

        if (fire)
        {
            int roundsToFire = m_ammo.Consume(AmmoPerShot);
            if (roundsToFire > 0)
            {
                // Get base aim
                var transform = ProjectileTransform;
                var position = transform.position;
                var baseForward = transform.forward;
                if (m_control.AimTarget != null)
                {
                    baseForward = (m_control.AimTarget.position - position).normalized;
                }
                Debug.DrawLine(position, position + 100.0f * baseForward);

                int numProjectiles = roundsToFire * ProjectilesPerAmmo;
                for (int i = 0; i < numProjectiles; ++i)
                {
                    var forward = baseForward;

                    // Apply spread
                    var up = ProjectileTransform.up;
                    if (SpreadAngle > 0.0f)
                    {
                        var right = Vector3.Cross(forward, up).normalized;
                        var twistAxis = Quaternion.AngleAxis(URandom.Range(0.0f, 360.0f), forward) * right;
                        forward = Quaternion.AngleAxis(URandom.Range(0.0f, SpreadAngle), twistAxis) * forward;
                    }

                    // Determine rotation
                    var rotation = Quaternion.LookRotation(forward, up);
                    if (RandomiseProjectileRoll)
                    {
                        rotation *= Quaternion.Euler(0.0f, 0.0f, URandom.value * 360.0f);
                    }

                    // Determine velocity
                    var velocity = GetVelocityToInherit(transform) + ProjectileSpeed * forward;

                    // Instantiate projectile
                    GameObject projectile = m_projectilePool.Instantiate(position, rotation, null);
                    projectile.GetComponent<Projectile>().Velocity = velocity;
                    projectile.GetComponent<ProjectileDamage>().Perpetrator = m_damagePerpetrator;
                }

                // Notify listeners
                foreach(IWeaponListener listener in m_weaponListeners)
                {
                    listener.OnFire();
                }
            }
            else
            {
                // Notify listeners
                foreach (IWeaponListener listener in m_weaponListeners)
                {
                    listener.OnDryFire();
                }
            }
        }
    }

    private Vector3 GetVelocityToInherit(Transform muzzleTransform)
    {
        Vector3 velocityToInherit = Vector3.zero;
        if (m_rigidBody != null && ProjectileVelocityInheritance != Vector3.zero)
        {
            Vector3 parentVelocityWS = m_rigidBody.GetPointVelocity(muzzleTransform.position);
            if (ProjectileVelocityInheritance == Vector3.one)
            {
                velocityToInherit += parentVelocityWS;
            }
            else
            {
                Vector3 parentVelocityLS = muzzleTransform.InverseTransformDirection(parentVelocityWS);
                Vector3 velocityToInheritLS = Vector3.Scale(parentVelocityLS, ProjectileVelocityInheritance);
                velocityToInherit += muzzleTransform.TransformDirection(velocityToInheritLS);
            }
        }
        return velocityToInherit;
    }
}
