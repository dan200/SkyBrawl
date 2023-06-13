using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEyeFocus : MonoBehaviour
{
    private Rigidbody m_rigidBody;

    public float MinDistance = 0.5f;
    public float MaxDistance = 500.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var parent = transform.parent;

        RaycastHit hitInfo;
        if (Physics.Raycast(parent.position + parent.forward * MinDistance, parent.forward, out hitInfo))
        {
            float hitDitance = MinDistance + hitInfo.distance;
            transform.localPosition = Vector3.forward * Mathf.Clamp(hitDitance, MinDistance, MaxDistance);
        }
        else
        {
            transform.localPosition = Vector3.forward * MaxDistance;
        }
    }
}
