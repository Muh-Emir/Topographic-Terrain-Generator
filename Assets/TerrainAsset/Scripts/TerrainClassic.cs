using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainClassic : MonoBehaviour
{
    Mesh mesh;

    public Vector3[] verticles;
    public int[] triangles;
    public Color[] myColors;

    public int xSize = 20;
    public int zSize = 20;
    public float noiseDetail;
    public float noiseSize;

    public Gradient myGradient;
    float minHeight, maxHeight;

    void Start()
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
                float y = Mathf.PerlinNoise(x * noiseDetail, z * noiseDetail) * noiseSize;
                verticles[i] = new Vector3(x, y, z);

                if (maxHeight < y)
                    maxHeight = y;

                if (minHeight > y)
                    minHeight = y;

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

        myColors = new Color[verticles.Length];

        for (int i = 0; i < verticles.Length; i++)
        {
            float height = Mathf.InverseLerp(minHeight, maxHeight, verticles[i].y);
            myColors[i] = myGradient.Evaluate(height);
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticles;
        mesh.triangles = triangles;
        mesh.colors = myColors;

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
