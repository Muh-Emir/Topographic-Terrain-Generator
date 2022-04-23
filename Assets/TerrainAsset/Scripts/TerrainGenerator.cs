using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    public List<Vector3> myVerts;
    public List<int> myTris;
    public Color[] myColors;

    public float cellSize;
    public int xSize = 20;
    public int zSize = 20;

    public float noiseDetail;
    public float noiseSize;

    public Gradient myGradient;
    float minHeight, maxHeight;

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
        CalculateTris();
        CreateColors();
    }

    void CalculateVerts()
    {
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = (int)(Mathf.PerlinNoise(x * noiseDetail, z * noiseDetail) * noiseSize);

                Vector2 prevY = new Vector2((int)(Mathf.PerlinNoise((x - 1) * noiseDetail, z * noiseDetail) * noiseSize),
                    (int)(Mathf.PerlinNoise(x * noiseDetail, (z - 1) * noiseDetail) * noiseSize));

                Vector2 nextY = new Vector2((int)(Mathf.PerlinNoise((x + 1) * noiseDetail, z * noiseDetail) * noiseSize),
                     (int)(Mathf.PerlinNoise(x * noiseDetail, (z + 1) * noiseDetail) * noiseSize));

                if (prevY.x != y && nextY.x != y)
                {
                    y = prevY.x;
                }

                if (prevY.y != y && nextY.y != y)
                {
                    y = prevY.y;
                }

                if (i - 1 >= 0 && i - 1 >= xSize && x > 0)
                {
                    if (myVerts[i - 1].y != y && x != xSize - 1)
                    {
                        myVerts.Add(new Vector3((x - 1) * cellSize, y, z * cellSize));
                    }

                    if (myVerts[i / zSize].y != y && z != zSize - 1)
                    {
                        myVerts.Add(new Vector3(x * cellSize, y, (z - 1) * cellSize));
                    }
                }

                myVerts.Add(new Vector3(x * cellSize, y, z * cellSize));

                if (y != nextY.x && x < xSize)
                {
                    myVerts.Add(new Vector3((x + 1) * cellSize, y, z * cellSize));
                }
                i++;

                if (maxHeight < y)
                    maxHeight = y;

                if (minHeight > y)
                    minHeight = y;
            }
        }        
    }

    void CalculateTris()
    {
        int isXpos = 0;
        int isZpos = 0;
        int isCross = 0;

        for (int i = 0; i < myVerts.Count; i++)
        {
            isCross = 0;
            for (int j = 0; j < myVerts.Count; j++)
            {
                if (myVerts[i].y == myVerts[j].y)
                {
                    if (myVerts[i].x + cellSize == myVerts[j].x && myVerts[i].z == myVerts[j].z)
                    {
                        //saðýndaki
                        isXpos = j;
                    }

                    if (myVerts[i].x == myVerts[j].x && myVerts[i].z + cellSize == myVerts[j].z)
                    {
                        //önündeki
                        isZpos = j;
                    }

                    if (myVerts[i].x + cellSize == myVerts[j].x && myVerts[i].z + cellSize == myVerts[j].z)
                    {
                        isCross = j;
                    }
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

    void CreateColors()
    {
        myColors = new Color[myVerts.Count];

        for (int i = 0; i < myVerts.Count; i++)
        {
            float height = Mathf.InverseLerp(minHeight, maxHeight, myVerts[i].y);
            myColors[i] = myGradient.Evaluate(height);
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = myVerts.ToArray();
        mesh.triangles = myTris.ToArray();
        mesh.colors = myColors;

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
