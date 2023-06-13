using UnityEngine;
using System.Collections;
using System;

public class Ammo : MonoBehaviour
{
    [Min(0)]
    public int ClipSize = 16;
    public bool Infinite = false;

    [SerializeField]
    [Min(0)]
    private int m_ammoInClip = 16;
    public int AmmoInClip
    {
        get
        {
            return m_ammoInClip;
        }
        set
        {
            Debug.Assert(value >= 0 && value <= ClipSize);
            m_ammoInClip = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_ammoInClip <= ClipSize);
    }

    public int Consume(int ammo)
    {
        Debug.Assert(ammo >= 0);
        if (Infinite)
        {
            return ammo;
        }
        else
        {
            int ammoConsumed = Math.Min(ammo, AmmoInClip);
            AmmoInClip -= ammoConsumed;
            return ammoConsumed;
        }
    }

    public void Reload(int ammo)
    {
        Debug.Assert(ammo >= 0);
        AmmoInClip = Math.Min(AmmoInClip + ammo, ClipSize);
    }
}
