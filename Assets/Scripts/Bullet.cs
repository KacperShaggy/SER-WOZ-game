using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Bullet
 * @brief Represents a projectile fired by a tower.
 *
 * The bullet moves in a specified direction and checks for collisions
 * with enemies. When it detects an enemy within a certain distance,
 * it deals damage and destroys itself.
 */
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Movement speed of the bullet.
    /// </summary>
    public float speed = 8f;

    /// <summary>
    /// Amount of damage dealt to an enemy on hit.
    /// </summary>
    public float damage = 40f;

    /// <summary>
    /// Direction in which the bullet travels.
    /// </summary>
    public Vector3 direction;


    /**
     * @brief Sets the bullet direction toward a target transform.
     *
     * Calculates a normalized direction vector from the bullet's
     * current position to the target position (on the XZ plane).
     *
     * @param _target Transform of the target object.
     */
    public void SetDirectionTo(Transform _target)
    {
        direction = new Vector3(_target.position.x - transform.position.x, 0, _target.position.z - transform.position.z).normalized;
    }

    /**
     * @brief Sets the bullet movement direction manually.
     *
     * @param _direction Vector representing the desired direction.
     */
    public void SetDirection(Vector3 _direction)
    {
        direction = (_direction).normalized;
    }

    /**
     * @brief Called when the bullet is created.
     *
     * Automatically destroys the bullet after 3 seconds
     * to prevent unused objects from remaining in the scene.
     */
    private void Start()
    {
        Destroy(gameObject, 3);
    }

    /**
     * @brief Called once per frame.
     *
     * Moves the bullet forward and checks if any enemy is within
     * a small distance. If an enemy is detected, the bullet
     * damages the enemy and destroys itself.
     */
    void Update()
    {

        float distanceThisFrame = speed * Time.deltaTime;

        /// <summary>
        /// Retrieves all objects tagged as "Enemy".
        /// </summary>
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        /// <summary>
        /// Checks each enemy to see if it is within hit range.
        /// </summary>
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Vector3 directionToEnemy = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z) - transform.position;

                /// <summary>
                /// If the enemy is close enough, the bullet hits the target.
                /// </summary>
                if (directionToEnemy.magnitude <= 1)
                {
                    HitTarget(enemy);
                    return;
                }
            }
        }

        /// <summary>
        /// Moves the bullet forward based on its direction and speed.
        /// </summary>
        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    /**
     * @brief Handles bullet collision with an enemy.
     *
     * Retrieves the Enemy component from the target,
     * applies damage, and destroys the bullet.
     *
     * @param target The enemy GameObject that was hit.
     */
    void HitTarget(GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}