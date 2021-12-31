using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnPrefab;
    public void SpawnObject()
    {
        Instantiate(spawnPrefab);
    }
}
