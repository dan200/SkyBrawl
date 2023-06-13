using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
    public static void ReturnToPool(GameObject obj)
    {
        var pooledObject = obj.GetComponent<PooledObject>();
        if(pooledObject != null && pooledObject.Pool != null)
        {
            pooledObject.Pool.ReturnToPool(obj);
        }
        else
        {
            Object.Destroy(obj);
        }
    }

    public ObjectPool Pool;
    
    private IResettable[] m_resettable;

    private void Awake()
    {
        m_resettable = GetComponents<IResettable>();
    }

    public void Reset(GameObject prefab)
    {
        foreach(IResettable resettable in m_resettable)
        {
            resettable.Reset(prefab.gameObject);
        }
    }
}
