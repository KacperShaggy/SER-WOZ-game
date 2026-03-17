```csharp id = "c4mv9k"
using System.Collections;
using UnityEngine;

/**
 * @class CameraMovement
 * @brief Handles back-and-forth camera movement along the Z axis.
 *
 * The camera moves forward by a specified distance, waits,
 * then returns to its starting position, repeating this loop indefinitely.
 */
public class CameraMovement : MonoBehaviour
{
    /** @brief Distance (in Unity units) the camera moves forward */
    [SerializeField]
    private float distance = 1f;

    /** @brief Movement speed of the camera */
    [SerializeField]
    private float moveSpeed = 1f;

    /** @brief Time (in seconds) to wait between movements */
    [SerializeField]
    private float waitTime = 0f;

    /** @brief Initial position of the camera */
    private Vector3 startPosition;

    /**
     * @brief Unity Start method.
     *
     * Stores the initial position and starts the movement coroutine.
     */
    private void Start()
    {
        startPosition = transform.position;
        StartCoroutine(MoveCamera());
    }

    /**
     * @brief Main loop controlling camera movement.
     *
     * Moves the camera forward and backward with optional pauses.
     *
     * @return IEnumerator required for Unity coroutines
     */
    private IEnumerator MoveCamera()
    {
        while (true)
        {
            // Move forward
            yield return MoveToPosition(startPosition + new Vector3(0f, 0f, distance));

            yield return new WaitForSeconds(waitTime);

            // Move back to start
            yield return MoveToPosition(startPosition);

            yield return new WaitForSeconds(waitTime);
        }
    }

    /**
     * @brief Smoothly moves the camera to a target position.
     *
     * @param target Target world position
     * @return IEnumerator required for Unity coroutines
     */
    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Snap exactly to target to avoid floating point inaccuracies
        transform.position = target;
    }
}
```
