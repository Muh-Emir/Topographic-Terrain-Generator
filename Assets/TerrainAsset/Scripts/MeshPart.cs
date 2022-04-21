using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshPart : MonoBehaviour
{
    Mesh mesh;

    public GameObject myCube;

    public Vector3[] verticles;
    public int[] triangles;

    public int rotStep;

    public float minPos = 1;
    public float noiseDetail;
    public float noiseSize;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
        //CreateShape();
        //UpdateMesh();
    }

    void Update()
    {

    }

    IEnumerator CreateShape()
    {
        verticles = new Vector3[360 / rotStep];

        float posY = 0;

        for (int i = 0, y = 0; y < 360; y+=rotStep)
        {
            transform.rotation = Quaternion.Euler(0, y, 0);
            yield return new WaitForSeconds(.001f);
            posY = Mathf.PerlinNoise(i*3 * noiseDetail, transform.position.z * noiseDetail) * noiseSize;

            if (minPos > posY)
                posY = minPos;

            Vector3 newPos = new Vector3(0, 0, posY);

            verticles[i] = transform.TransformPoint(newPos) - transform.position;

            i++;
        }
        Debug.Log(minPos);
        verticles[0] = transform.TransformPoint(Vector3.zero) - transform.position;

        triangles = new int[359/rotStep * 3];
        int vert = 0;
        int tris = 0;

        for (int i = 0; i < 359/rotStep-1; i++)
        {
            triangles[tris + 0] = 0;
            triangles[tris + 1] = vert + 1;
            triangles[tris + 2] = vert + 2;

            vert++;
            tris += 3;
        }
        triangles[tris + 0] = 0;
        triangles[tris + 1] = vert + 1;
        triangles[tris + 2] = 1;

        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verticles;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

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
    }
}