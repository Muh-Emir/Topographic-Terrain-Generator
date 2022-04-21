using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

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

                verticles[i] = new Vector3(x * detailScale, y, z * detailScale);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
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

        for (int i = 0; i < triangles.Length; i++)
        {
            if (verticles[i/verticles.Length / 6].y < maxY-1)
            {
                triangles[i] = 0;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticles;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
    /*
    private void OnDrawGizmos()
    {
        if (verticles == null)
        {
            return;
        }
        for (int i = 0; i < verticles.Length; i++)
        {
            Gizmos.DrawSphere(verticles[i], .1f);
        }
    }*/
}
