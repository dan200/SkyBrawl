using UnityEngine;
using System.Collections;
using URandom = UnityEngine.Random;

[RequireComponent(typeof(Weapon))]
public class WeaponKickback : MonoBehaviour, IWeaponListener
{
    public float Kickback = -0.15f;

    private Vector3 m_basePosition;
    private float m_currentKickback;

    // Start is called before the first frame update
    void Start()
    {
        m_basePosition = transform.localPosition;
        m_currentKickback = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_currentKickback *= 0.95f;
        transform.localPosition = m_basePosition + Vector3.forward * m_currentKickback;
    }

    public void OnFire()
    {
        m_currentKickback = Kickback;
    }

    public void OnDryFire()
    {
    }
}
