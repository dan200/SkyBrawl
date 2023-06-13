using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool GetPool(GameObject prefab)
    {
        var poolName = prefab.name + "_Pool";
        var pool = GameObject.Find(poolName);
        if (pool == null)
        {
            pool = new GameObject(poolName);
            pool.AddComponent<ObjectPool>().Prefab = prefab;
        }
        return pool.GetComponent<ObjectPool>();
    }

    public GameObject Prefab;

    private List<GameObject> m_pooledObjects = new List<GameObject>();

    public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject obj;
        if(m_pooledObjects.Count > 0)
        {
            int i = m_pooledObjects.Count - 1;
            obj = m_pooledObjects[i];
            m_pooledObjects.RemoveAt(i);

            obj.transform.SetParent(parent, false);
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.GetComponent<PooledObject>().Reset(Prefab);
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = Instantiate(Prefab, position, rotation, parent);
            obj.AddComponent<PooledObject>().Pool = this;
        }
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        Debug.Assert(obj != null);
        Debug.Assert(!m_pooledObjects.Contains(obj));
        obj.transform.SetParent(this.transform, false);
        obj.gameObject.SetActive(false);
        m_pooledObjects.Add(obj);
    }
}
