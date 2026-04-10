using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject blueBulletPrefab;

    public float initialMin = 5f;
    public float initialMax = 10f;

    public float minSpawnRate = 0.8f;

    private Transform target;
    private float timeAfterSpawn;
    private float spawnRate;

    private float currentMin;
    private float currentMax;

    void Start()
    {
        timeAfterSpawn = 0f;

        currentMin = initialMin;
        currentMax = initialMax;
        spawnRate = Random.Range(currentMin, currentMax);

        target = FindFirstObjectByType<PlayerController>().transform;
    }

    void Update()
    {
        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;

            GameObject prefabToSpawn = Random.value < 0.5f ? bulletPrefab : blueBulletPrefab;

            GameObject bullet = Instantiate(prefabToSpawn, transform.position, transform.rotation);
            bullet.transform.LookAt(target);

            currentMin = Mathf.Max(minSpawnRate, currentMin - 0.3f);
            currentMax = Mathf.Max(minSpawnRate + 3f, currentMax - 0.3f);

            spawnRate = Random.Range(currentMin, currentMax);
        }
    }
}