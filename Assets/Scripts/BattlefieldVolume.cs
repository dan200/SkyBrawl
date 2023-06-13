using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldVolume : MonoBehaviour
{
    public Vector3 Size = new Vector3(500.0f, 20.0f, 500.0f);

    public Vector3 HalfSize
    {
        get
        {
            return 0.5f * Size;
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Size);
    }

    public Vector3 PickRandomPointInVolume()
    {
        var halfSize = HalfSize;
        var pos = new Vector3(
            Random.Range(-halfSize.x, halfSize.x),
            Random.Range(-halfSize.y, halfSize.y),
            Random.Range(-halfSize.z, halfSize.z)
        );
        return transform.TransformPoint(pos);
    }

    public Vector3 PickRandomPointOnEdge()
    {
        var halfSize = HalfSize;
        var pos = new Vector3(
            Random.Range(-halfSize.x, halfSize.x),
            Random.Range(-halfSize.y, halfSize.y),
            Random.Range(-halfSize.z, halfSize.z)
        );
        if( Random.value > (HalfSize.x / (HalfSize.x + HalfSize.z)) )
        {
            pos.x = Mathf.Sign(pos.x) * halfSize.x;
        }
        else
        {
            pos.z = Mathf.Sign(pos.z) * halfSize.z;
        }
        return transform.TransformPoint(pos);
    }

    public float GetDistanceToVolume(Vector3 pos)
    {
        var halfSize = HalfSize;
        var localPos = transform.InverseTransformPoint(pos);
        Vector3 closestPosInVolume = Vector3.Scale( halfSize, new Vector3(
            Mathf.Clamp(localPos.x / halfSize.x, -1.0f, 1.0f),
            Mathf.Clamp(localPos.y / halfSize.y, -1.0f, 1.0f),
            Mathf.Clamp(localPos.z / halfSize.z, -1.0f, 1.0f)
        ) );
        return (closestPosInVolume - pos).magnitude;
    }
}
