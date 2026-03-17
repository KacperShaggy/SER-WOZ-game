```csharp id = "n8x4vr"
using UnityEngine;

/**
 * @class Tower
 * @brief Controls targeting, rotation, and shooting behavior of a tower.
 *
 * The tower searches for the closest enemy within range, rotates toward it,
 * and fires projectiles at a fixed rate.
 */
public class Tower : MonoBehaviour
{
    /** @brief Maximum attack range */
    [SerializeField]
    private float range = 10f;

    /** @brief Rotation speed toward target */
    [SerializeField]
    private float rotationSpeed = 5f;

    /** @brief Shots per second */
    [SerializeField]
    private float fireRate = 1f;

    /** @brief Bullet prefab */
    [SerializeField]
    private GameObject bulletPrefab;

    /** @brief Bullet spawn point */
    [SerializeField]
    private Transform firePoint;

    /** @brief Current target */
    private Transform target;

    /** @brief Time remaining until next shot */
    private float fireCountdown = 0f;

    /**
     * @brief Called once per frame.
     *
     * Handles target acquisition, rotation, and shooting.
     */
    private void Update()
    {
        FindClosestEnemy();

        if (target == null) return;

        RotateTowardsTarget();

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate; // FIX: proper fire rate usage
        }

        fireCountdown -= Time.deltaTime;
    }

    /**
     * @brief Finds the closest enemy within range.
     *
     * Searches all objects tagged as "Enemy" and selects the nearest one.
     */
    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance && distance <= range)
            {
                shortestDistance = distance;
                nearestTarget = enemy.transform;
            }
        }

        target = nearestTarget;
    }

    /**
     * @brief Smoothly rotates the tower toward the current target.
     */
    private void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    /**
     * @brief Spawns a bullet and directs it toward the target.
     */
    private void Shoot()
    {
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirectionTo(target);
        }
    }
}
```
