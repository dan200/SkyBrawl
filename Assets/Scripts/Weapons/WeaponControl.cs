using UnityEngine;
using System.Collections;

public class WeaponControl : MonoBehaviour
{
    public bool Fire
    {
        get;
        set;
    } = true;

    public Transform AimTarget;

    // Use this for initialization
    void Start()
    {
    }

    public void Reset()
    {
        Fire = false;
    }
}
