```csharp id = "q7m2zs"
using UnityEngine;

/**
 * @class Spawner
 * @brief Handles spawning enemies around a central point in a randomized arc.
 *
 * Enemies are spawned at a fixed radius from the spawner, either on the left or right side,
 * with a slight random angle variation. Each spawned enemy is assigned a target (carriage).
 */
public class Spawner : MonoBehaviour
{
    /** @brief Enemy prefab to spawn */
    [SerializeField]
    private GameObject enemyPrefab;

    /** @brief Distance from the spawner where enemies will appear */
    [SerializeField]
    private float spawnRadius = 25f;

    /** @brief Reference to the target object (carriage) */
    [SerializeField]
    private GameObject targetCarriage;

    /** @brief Time interval (in seconds) between enemy spawns */
    [SerializeField]
    private float spawnRate = 0.5f;

    /**
     * @brief Unity Start method.
     *
     * Starts repeated enemy spawning.
     */
    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnRate);
    }

    /**
     * @brief Spawns a single enemy at a randomized position.
     *
     * The enemy appears either on the left or right side of the spawner,
     * with a random angular offset for variation.
     */
    private void SpawnEnemy()
    {
        // Choose base direction: left (270°) or right (90°)
        float baseAngle = (Random.value < 0.5f) ? 90f : 270f;

        // Add random spread
        float angle = baseAngle + Random.Range(-40f, 40f);
        float radians = angle * Mathf.Deg2Rad;

        // Calculate spawn position
        Vector3 spawnPosition = transform.position + new Vector3(
            Mathf.Sin(radians) * spawnRadius,
            0f,
            Mathf.Cos(radians) * spawnRadius
        );

        // Instantiate enemy
        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Assign target
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.SetTarget(targetCarriage);
        }
    }
}
```
