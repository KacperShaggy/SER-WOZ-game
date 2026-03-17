using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Spawner
 * @brief Responsible for periodically spawning enemy units around the map.
 *
 * This class spawns enemies at random positions on a circle around the spawner.
 * Enemies appear either on the left or right side relative to the center and
 * move toward the specified target carriage.
 */
public class Spawner : MonoBehaviour
{
    /// <summary>
    /// Prefab of the enemy that will be spawned.
    /// </summary>
    public GameObject EnemyToSpawn;

    /// <summary>
    /// Radius of the spawning circle around the spawner.
    /// </summary>
    public float Radius = 25;

    /// <summary>
    /// Target object that enemies will move toward.
    /// </summary>
    public GameObject TestCariage;

    /// <summary>
    /// Time interval between enemy spawns.
    /// </summary>
    public float spawnRate = .5f;

    /**
     * @brief Called when the spawner is initialized.
     *
     * Starts repeatedly spawning enemies at the specified spawn rate.
     */
    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnRate);
    }

    /**
     * @brief Spawns a new enemy at a random position around the spawner.
     *
     * The enemy is spawned on a circular radius around the spawner,
     * either to the left or right side, with a small random angle offset.
     * After spawning, the enemy receives a reference to the target carriage.
     */
    void SpawnEnemy()
    {
        /// <summary>
        /// Chooses a base angle: left (270°) or right (90°).
        /// </summary>
        float baseAngle = (UnityEngine.Random.value < 0.5f) ? 90f : 270f; // prawo albo lewo

        /// <summary>
        /// Adds a random variation to the base angle.
        /// </summary>
        float angle = baseAngle + UnityEngine.Random.Range(-40f, 40f);

        float rad = angle * Mathf.Deg2Rad;

        /// <summary>
        /// Calculates the spawn position on the circular radius.
        /// </summary>
        Vector3 spawnPos = transform.position + new Vector3(
            Mathf.Sin(rad) * Radius,
            0,
            Mathf.Cos(rad) * Radius
        );

        /// <summary>
        /// Instantiates the enemy and assigns its target.
        /// </summary>
        GameObject newEnemy = Instantiate(EnemyToSpawn, spawnPos, Quaternion.identity);
        newEnemy.GetComponent<Enemy>().TestCariage = TestCariage;
    }

    /**
     * @brief Called once per frame.
     *
     * Currently unused but available for future logic.
     */
    // Update is called once per frame
    void Update()
    {

    }
}