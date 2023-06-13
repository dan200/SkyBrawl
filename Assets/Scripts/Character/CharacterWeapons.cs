using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterControl))]
public class CharacterWeapons : MonoBehaviour
{
    private CharacterControl m_control;

    public WeaponControl Weapon;
    public Transform AimTarget;

    // Start is called before the first frame update
    void Start()
    {
        m_control = GetComponent<CharacterControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Weapon != null)
        {
            Weapon.Fire = m_control.PrimaryFire;
            Weapon.AimTarget = AimTarget;
        }
    }
}
