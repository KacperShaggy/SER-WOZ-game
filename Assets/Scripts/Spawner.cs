using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject EnemyToSpawn;
    public float Radius = 25;
    public GameObject TestCariage;
    public float spawnRate = .5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnRate);
    }

    void SpawnEnemy()
    {
        float baseAngle = (UnityEngine.Random.value < 0.5f) ? 90f : 270f; // prawo albo lewo
        float angle = baseAngle + UnityEngine.Random.Range(-40f, 40f);

        float rad = angle * Mathf.Deg2Rad;

        Vector3 spawnPos = transform.position + new Vector3(
            Mathf.Sin(rad) * Radius,
            0,
            Mathf.Cos(rad) * Radius
        );

        GameObject newEnemy = Instantiate(EnemyToSpawn, spawnPos, Quaternion.identity);
        newEnemy.GetComponent<Enemy>().TestCariage = TestCariage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
