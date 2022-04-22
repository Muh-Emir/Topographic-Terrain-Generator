using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    public List<Vector3> myVerts;
    public List<int> myTris;
    public Vector3[] verticles;
    public int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

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
        CalculateMyTris();
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
        }
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
    /*
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
    }*/
}
