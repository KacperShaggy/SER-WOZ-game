```csharp
using System.Collections;
using UnityEngine;

/**
 * @class ActiveTower
 * @brief Controls an active tower that fires bullets in a circular "sprinkler" pattern.
 *
 * When the user presses the Q key, the tower starts firing bullets over time.
 * Bullets are evenly distributed in a circular pattern and fired in two opposite directions.
 */
public class ActiveTower : MonoBehaviour
{
    /** @brief Total number of bullets fired in one sequence */
    [SerializeField]
    private float fireNumber = 100f;

    /** @brief Time delay (in seconds) between shots */
    [SerializeField]
    private float fireRate = 0.1f;

    /** @brief Bullet prefab to instantiate */
    [SerializeField]
    private GameObject bulletPrefab;

    /** @brief Transform representing the firing position */
    [SerializeField]
    private Transform firePoint;

    /** @brief Reference to the currently running firing coroutine */
    private Coroutine fireRoutine;

    /**
     * @brief Called once per frame.
     *
     * Listens for Q key press and starts the firing coroutine
     * if it is not already running.
     */
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && fireRoutine == null)
        {
            fireRoutine = StartCoroutine(FireLoop());
        }
    }

    /**
     * @brief Main firing loop for the sprinkler pattern.
     *
     * Spawns bullets at fixed intervals and assigns their direction
     * based on a calculated angle. Each step fires bullets in two opposite directions.
     *
     * @return IEnumerator required for Unity coroutines
     */
    private IEnumerator FireLoop()
    {
        int count = 0;

        while (count < fireNumber)
        {
            count++;

            // Fire in two opposite directions
            for (int i = 0; i < 2; i++)
            {
                GameObject bullet = Instantiate(
                    bulletPrefab,
                    firePoint.position,
                    firePoint.rotation
                );

                Bullet bulletScript = bullet.GetComponent<Bullet>();

                if (bulletScript != null)
                {
                    // Calculate angle for current bullet
                    float angle = ((360f / fireNumber) + 180f * i) * count * Mathf.Deg2Rad;

                    Vector3 direction = new Vector3(
                        Mathf.Sin(angle),
                        0f,
                        Mathf.Cos(angle)
                    );

                    bulletScript.SetDirection(direction);
                }
            }

            yield return new WaitForSeconds(fireRate);
        }

        fireRoutine = null;
    }
}
```
