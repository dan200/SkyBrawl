using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetControl : MonoBehaviour
{
    public float Thrust
    {
        get;
        set;
    }

    public float Pitch
    {
        get;
        set;
    }

    public float Yaw
    {
        get;
        set;
    }

    public float Roll
    {
        get;
        set;
    }

    public bool PrimaryFire
    {
        get;
        set;
    }

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Thrust = 0.0f;
        Pitch = 0.0f;
        Yaw = 0.0f;
        Roll = 0.0f;
        PrimaryFire = false;
    }
}