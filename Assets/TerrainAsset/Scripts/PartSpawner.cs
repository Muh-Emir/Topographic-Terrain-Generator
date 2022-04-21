using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public GameObject partPref;
    public float spawnDistance;
    public int spawnCount;

    public List<GameObject> partList;

    private void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var spawnedPart = Instantiate(partPref, new Vector3(0, 0, partList.Count * spawnDistance), Quaternion.identity, transform);
            partList.Add(spawnedPart);
        }
    }
}