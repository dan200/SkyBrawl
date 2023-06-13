using UnityEngine;
using System.Collections;
using URandom = UnityEngine.Random;

[RequireComponent(typeof(Weapon))]
public class WeaponMuzzleFlash : MonoBehaviour, IWeaponListener
{
    public MeshRenderer Mesh;
    public float Duration = 0.2f;

    private float m_timeRemaining = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Mesh.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_timeRemaining > 0.0f)
        {
            m_timeRemaining -= Time.deltaTime;
            if(m_timeRemaining <= 0.0f)
            {
                Mesh.enabled = false;
            }
        }
    }

    public void OnFire()
    {
        m_timeRemaining = Duration;
        Mesh.enabled = true;
        Mesh.transform.localEulerAngles = Vector3.forward * URandom.Range(0.0f, 360.0f);
    }

    public void OnDryFire()
    {
    }
}
