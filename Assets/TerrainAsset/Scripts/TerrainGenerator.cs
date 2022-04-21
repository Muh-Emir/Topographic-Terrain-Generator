using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    public List<Vector3> myVerts;
    public int[] myTris;
    public Vector3[] verticles;
    public int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    public int xNewSize;
    public int zNewSize;

    public float detailScale;

    public int minY, maxY;

    public float noiseDetail;
    public float noiseSize;


    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void Update()
    {

    }

    void CreateShape()
    {
        CalculateVerts();
        //CalculateTris();
        //CalculateMyTris();
    }

    void CalculateVerts()
    {
        verticles = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = (int)(Mathf.PerlinNoise(x * noiseDetail, z * noiseDetail) * noiseSize);

                if (y < minY)
                {
                    y = verticles[i - 1].y;
                }
                if (y > maxY)
                {
                    y = verticles[i - 1].y;
                }

                if (i - 1 >= 0 && i - 1 >= xSize && x > 0)
                {
                    if (verticles[i - 1].y != y && x != xSize - 1)
                    {
                        myVerts.Add(new Vector3(x - 1 * detailScale, y, z * detailScale));
                    }
                    if (verticles[i / zSize].y != y && z != zSize - 1)
                    {
                        myVerts.Add(new Vector3(x * detailScale, y, z - 1 * detailScale));
                    }
                }

                myVerts.Add(new Vector3(x * detailScale, y, z * detailScale));
                verticles[i] = new Vector3(x * detailScale, y, z * detailScale);
                i++;
            }
            if (z == 0)
                xNewSize = myVerts.Count - 1;
        }
        zNewSize = myVerts.Count / xNewSize - 1;
    }

    void CalculateTris()
    {
        triangles = new int[verticles.Length * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void CalculateMyTris()
    {
        myTris = new int[myVerts.Count * 6];
        int tris = 0;

        for (int vert = 0, z = 0; z < zNewSize; z++,vert++)
        {
            for (int x = 0; x < xNewSize; x++)
            {
                myTris[tris + 0] = vert + 0;
                myTris[tris + 1] = vert + xSize + 1;
                myTris[tris + 2] = vert + 1;
                myTris[tris + 3] = vert + 1;
                myTris[tris + 4] = vert + xSize + 1;
                myTris[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        //mesh.vertices = verticles;
        mesh.vertices = myVerts.ToArray();

        mesh.triangles = triangles;
        //mesh.triangles = myTris;

        mesh.RecalculateNormals();
    }
    
    private void OnDrawGizmos()
    {
        if (myVerts == null)
        {
            return;
        }
        for (int i = 0; i < verticles.Length; i++)
        {
            Gizmos.DrawSphere(myVerts[i], .1f);
        }
    }
}
