```csharp id = "k9qv2n"
using System.Collections;
using UnityEngine;

/**
 * @class Bullet
 * @brief Handles bullet movement, targeting, and collision with enemies.
 *
 * The bullet moves in a specified direction and checks for nearby enemies.
 * When an enemy is within a hit radius, the bullet deals damage and destroys itself.
 */
public class Bullet : MonoBehaviour
{
    /** @brief Movement speed of the bullet */
    [SerializeField]
    private float speed = 8f;

    /** @brief Damage dealt to the enemy on hit */
    [SerializeField]
    private float damage = 40f;

    /** @brief Normalized direction vector of the bullet */
    private Vector3 direction;

    /**
     * @brief Sets bullet direction toward a target transform.
     *
     * @param target Transform of the target enemy
     */
    public void SetDirectionTo(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        direction = dir.normalized;
    }

    /**
     * @brief Sets bullet direction manually.
     *
     * @param newDirection Direction vector
     */
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    /**
     * @brief Unity Start method.
     *
     * Automatically destroys the bullet after 3 seconds.
     */
    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    /**
     * @brief Called once per frame.
     *
     * Moves the bullet forward and checks for nearby enemies to hit.
     */
    private void Update()
    {
        float distanceThisFrame = speed * Time.deltaTime;

        // Find all enemies in the scene (not optimal for performance)
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            directionToEnemy.y = 0f;

            // Check if enemy is within hit range
            if (directionToEnemy.magnitude <= 1f)
            {
                HitTarget(enemy);
                return;
            }
        }

        // Move bullet forward
        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    /**
     * @brief Applies damage to the target and destroys the bullet.
     *
     * @param target Enemy GameObject hit by the bullet
     */
    private void HitTarget(GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
```
