```csharp id = "m2x8qp"
using UnityEngine;

/**
 * @class GroundRow
 * @brief Controls movement and lifecycle of a ground segment.
 *
 * The object moves backward over time, spawns a new ground row after
 * traveling a certain distance, and destroys itself after exceeding
 * a defined travel limit.
 */
public class GroundRow : MonoBehaviour
{
    /** @brief Size of a single ground node (distance before spawning next row) */
    [SerializeField]
    private float nodeSize = 2f;

    /** @brief Movement speed of the ground row */
    [SerializeField]
    private float speed = 5f;

    /** @brief Reference to the map generator responsible for spawning new rows */
    [SerializeField]
    private MapGenerating mapGenerating;

    /** @brief Maximum distance the row can travel before being destroyed */
    [SerializeField]
    private float distanceToTravel = 20f;

    /** @brief Starting position of the row */
    private Vector3 startPosition;

    /** @brief Distance already traveled */
    private float distanceTravelled = 0f;

    /** @brief Flag to ensure new row is created only once */
    private bool hasSpawnedNext = false;

    /**
     * @brief Unity Start method.
     *
     * Stores the initial position of the object.
     */
    private void Start()
    {
        startPosition = transform.position;
    }

    /**
     * @brief Called once per frame.
     *
     * Moves the row backward, tracks distance traveled,
     * spawns a new row when needed, and destroys itself after exceeding the limit.
     */
    private void Update()
    {
        // Move backward
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // Calculate traveled distance
        distanceTravelled = Vector3.Distance(startPosition, transform.position);

        // Spawn next row once
        if (!hasSpawnedNext && distanceTravelled >= nodeSize)
        {
            hasSpawnedNext = true;

            if (mapGenerating != null)
            {
                mapGenerating.CreateGroundRow();
            }
        }

        // Destroy after exceeding max distance
        if (distanceTravelled >= distanceToTravel)
        {
            Destroy(gameObject);
        }
    }
}
```
