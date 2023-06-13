using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattlefieldVolume))]
public class ShipSpawner : MonoBehaviour
{
    private BattlefieldVolume m_volume;

    public GameObject[] BoatPrefabs;
    public int NumBoats = 20;
    public Vector3 MinBoatSeperation = new Vector2(40.0f, 300.0f);

    public GameObject PlanePrefab;
    public int MinPlanes = 10;
    public int MaxPlanes = 30;
    public float NewPlaneInterval = 10.0f;

    private List<GameObject> m_boats = new List<GameObject>();
    private List<GameObject> m_planes = new List<GameObject>();
    private float m_newPlaneTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_volume = GetComponent<BattlefieldVolume>();

        // Spawn initial vessels
        m_boats.Add(GameObject.Find("Carrier"));
        while (m_boats.Count < NumBoats)
        {
            SpawnBoat();
        }
        while(m_planes.Count < MinPlanes)
        {
            SpawnPlane(false);
        }
        m_newPlaneTimer = NewPlaneInterval;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        // Cull dead planes
        for(int i=m_planes.Count-1; i>=0; --i)
        {
            if(m_planes[i] == null)
            {
                m_planes.RemoveAt(i);
            }
        }

        // Spawn new planes
        while (m_planes.Count < MinPlanes)
        {
            SpawnPlane(true);
            m_newPlaneTimer = NewPlaneInterval;
        }

        m_newPlaneTimer -= dt;
        if(m_newPlaneTimer <= 0.0f && m_planes.Count < MaxPlanes)
        {
            SpawnPlane(true);
            m_newPlaneTimer = NewPlaneInterval;
        }
    }

    private void SpawnBoat()
    {
        var pos = m_volume.PickRandomPointInVolume();
        for(int i=0; i<m_boats.Count; ++i)
        {
            var boat = m_boats[i];
            var seperation = boat.transform.position - pos;
            if(Mathf.Abs(seperation.x) < MinBoatSeperation.x && Mathf.Abs(seperation.z) < MinBoatSeperation.y)
            {
                // Start over
                pos = m_volume.PickRandomPointInVolume();
                i = -1;
            }
        }
        pos.y = -1.0f;

        var fwd = m_volume.transform.forward;
        var rot = Quaternion.LookRotation(fwd, Vector3.up);

        var prefab = BoatPrefabs[Random.Range(0, BoatPrefabs.Length)];
        m_boats.Add(Instantiate(prefab, pos, rot));
    }

    private void SpawnPlane(bool onEdge)
    {
        var pos = onEdge ? m_volume.PickRandomPointOnEdge() : m_volume.PickRandomPointInVolume();
        var target = m_volume.PickRandomPointInVolume();
        var rot = Quaternion.LookRotation(target - pos, Vector3.up);

        GameObject plane = Instantiate(PlanePrefab, pos, rot);
        var aiJetControl = plane. GetComponent<AIJetControl>();
        aiJetControl.Battlefield = m_volume;
        aiJetControl.RoamTarget = target;

        m_planes.Add(plane);
    }
}
