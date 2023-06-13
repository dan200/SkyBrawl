using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
    public Vector2 Movement
    {
        get;
        set;
    }

    public bool Walk
    {
        get;
        set;
    }

    public bool Jump
    {
        get;
        set;
    }

    public Vector2 TurnDelta
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
        Movement = Vector2.zero;
        Walk = false;
        Jump = false;
        TurnDelta = Vector2.zero;
        PrimaryFire = false;
    }
}
