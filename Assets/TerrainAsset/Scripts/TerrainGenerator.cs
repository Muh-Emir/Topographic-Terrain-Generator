using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    public List<Vector3> myVerts;
    public List<int> myTris;

    public int xSize = 20;
    public int zSize = 20;

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
        CalculateMyTris();
    }

    void CalculateVerts()
    {

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = (int)(Mathf.PerlinNoise(x * noiseDetail, z * noiseDetail) * noiseSize);
                float nextxY = (int)(Mathf.PerlinNoise((x + 1) * noiseDetail, z * noiseDetail) * noiseSize);
                float nextzY = (int)(Mathf.PerlinNoise(x * noiseDetail, (z + 1) * noiseDetail) * noiseSize);

                

                if (i - 1 >= 0 && i - 1 >= xSize && x > 0)
                {
                    if (myVerts[i - 1].y != y && x != xSize - 1)
                    {
                        myVerts.Add(new Vector3(x - 1, y, z));
                    }

                    if (myVerts[i / zSize].y != y && z != zSize - 1)
                    {
                        myVerts.Add(new Vector3(x, y, z - 1));
                    }
                }

                myVerts.Add(new Vector3(x, y, z));
                if (y != nextxY)
                {
                    myVerts.Add(new Vector3(x + 1, y, z));
                }
                i++;
            }
        }
    }

    void CalculateMyTris()
    {
        int isXpos = 0;
        int isZpos = 0;
        int isCross = 0;

        for (int i = 0; i < myVerts.Count; i++)
        {
            for (int j = 0; j < myVerts.Count; j++)
            {
                if (myVerts[i].x + 1 == myVerts[j].x && myVerts[i].y == myVerts[j].y && myVerts[i].z == myVerts[j].z)
                {
                    //saðýndaki
                    isXpos = j;
                }

                if (myVerts[i].x == myVerts[j].x && myVerts[i].y == myVerts[j].y && myVerts[i].z + 1 == myVerts[j].z)
                {
                    //önündeki
                    isZpos = j;
                }

                if (myVerts[i].x + 1 == myVerts[j].x && myVerts[i].y == myVerts[j].y && myVerts[i].z + 1 == myVerts[j].z)
                {
                    isCross = j;
                }
            }

            myTris.Add(i);//0
            myTris.Add(isZpos);//1
            myTris.Add(isXpos);//2
            if (isCross != 0)
            {
                myTris.Add(isZpos);//1
                myTris.Add(isCross);//3
                myTris.Add(isXpos);//2
            }

        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        //mesh.vertices = verticles;
        mesh.vertices = myVerts.ToArray();

        //mesh.triangles = triangles;
        mesh.triangles = myTris.ToArray();

        mesh.RecalculateNormals();
    }
    
    private void OnDrawGizmos()
    {
        if (myVerts == null)
        {
            return;
        }
        for (int i = 0; i < myVerts.Count; i++)
        {
            Gizmos.DrawSphere(myVerts[i], .1f);
        }
    }
}
