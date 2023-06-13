using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Starfield : MonoBehaviour
{
    public int NumStars = 1000;
    public float Radius = 1000.0f;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>(NumStars);
        List<int> indices = new List<int>(NumStars);
        for(int i=0; i<NumStars; ++i)
        {
            Vector3 pos = Random.onUnitSphere * Radius;

            int firstVertex = verts.Count;
            verts.Add(pos);
            /*
            verts.Add(pos);
            verts.Add(pos);
            verts.Add(pos);
            */

            indices.Add(firstVertex);
            /*
            indices.Add(firstVertex + 1);
            indices.Add(firstVertex + 2);
            indices.Add(firstVertex + 3);
            */
        }
        mesh.SetVertices(verts);
        mesh.SetIndices(indices, MeshTopology.Points, 0);

        MeshFilter filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
    }
}
