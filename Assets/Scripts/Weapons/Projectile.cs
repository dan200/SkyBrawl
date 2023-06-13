using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour, IResettable
{
    private IProjectileListener[] m_projectileListeners;

    [Min(0.0f)]
    public float GravityScale = 1.0f;

    public ProjectileImpactBehaviour ImpactBehaviour = ProjectileImpactBehaviour.Stop;

    [Range(0.0f, 1.0f)]
    public float Bounciness = 0.5f;
    
    public bool Stopped
    {
        get;
        private set;
    }

    public Vector3 Velocity;

    public float Speed
    {
        get { return Velocity.magnitude; }
    }

    private RaycastHit[] m_hitBuffer = new RaycastHit[8];

    // Use this for initialization
    void Awake()
    {
        m_projectileListeners = GetComponents<IProjectileListener>();
    }

    void Start()
    {
        Stopped = false;
    }

    public void Reset(GameObject prefab)
    {
        var baseComponent = prefab.GetComponent<Projectile>();
        Velocity = baseComponent.Velocity;
        Stopped = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Stopped)
        {
            return;
        }

        // Calculate movement
        float dt = Time.fixedDeltaTime;
        Vector3 gravity = GravityScale * Physics.gravity;

        Vector3 oldVelocity = Velocity;
        Vector3 newVelocity = Velocity + gravity * dt;

        Vector3 oldPos = transform.position;
        Vector3 newPos = transform.position + Velocity * dt + 0.5f * gravity * dt * dt;

        bool destroy = false;
        if(newPos != oldPos)
        {
            // Check for impacts
            Vector3 travel = newPos - oldPos;
            int numResults = Physics.RaycastNonAlloc(oldPos, travel.normalized, m_hitBuffer, travel.magnitude);
            for(int i=0; i<numResults; ++i)
            {
                ref RaycastHit hit = ref m_hitBuffer[i];
                Vector3 hitVelocity = Vector3.Lerp(oldVelocity, newVelocity, hit.distance / travel.magnitude);

                // Notify listeners
                foreach (IProjectileListener listener in m_projectileListeners)
                {
                    listener.OnImpact(hit.collider, hit.point, hit.normal, hitVelocity);
                }

                // Update position/velocity
                newPos = hit.point;
                switch(ImpactBehaviour)
                {
                    case ProjectileImpactBehaviour.Bounce:
                    {
                        newVelocity = ReflectVector(hitVelocity, hit.normal) * Bounciness;
                        if (newVelocity.magnitude < 0.1f && Vector3.Dot(hit.normal, gravity) < 0.0f)
                        {
                            Stopped = true;
                        }
                        break;
                    }
                    case ProjectileImpactBehaviour.Stop:
                    {
                        newVelocity = Vector3.zero;
                        Stopped = true;
                        break;
                    }
                    case ProjectileImpactBehaviour.Destroy:
                    {
                        destroy = true;
                        break;
                    }
                }
            }
        }

        Velocity = newVelocity;
        transform.position = newPos;
        if(destroy)
        {
            PooledObject.ReturnToPool(this.gameObject);
        }
    }

    private Vector3 ReflectVector(Vector3 vector, Vector3 normal)
    {
        return vector - 2.0f * normal * Vector3.Dot(normal, vector);
    }
}
