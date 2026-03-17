using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class CameraMovement
 * @brief Controls automatic back-and-forth camera movement.
 *
 * The camera moves forward by a specified distance and then returns
 * to its starting position. The movement repeats continuously with
 * an optional pause between movements.
 */
public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// Distance the camera moves forward (in meters).
    /// </summary>
    public float distance = 1f;      // Odleg³oœæ ruchu (1 metr)

    /// <summary>
    /// Speed at which the camera moves.
    /// </summary>
    public float moveSpeed = 1f;     // Prêdkoœæ ruchu

    /// <summary>
    /// Pause time between movements (in seconds).
    /// </summary>
    public float waitTime = 0f;    // Pauza miêdzy ruchami

    /// <summary>
    /// Stores the initial position of the camera.
    /// </summary>
    private Vector3 startPosition;

    /**
     * @brief Called once when the object is initialized.
     *
     * Saves the initial camera position and starts
     * the camera movement coroutine.
     */
    void Start()
    {
        startPosition = transform.position;
        StartCoroutine(MoveCamera());
    }

    /**
     * @brief Coroutine that continuously moves the camera forward and backward.
     *
     * The camera moves forward by the specified distance, waits for a defined time,
     * then returns to the starting position and waits again before repeating.
     *
     * @return IEnumerator Required for Unity coroutine execution.
     */
    IEnumerator MoveCamera()
    {
        while (true)
        {
            /// <summary>
            /// Move the camera forward.
            /// </summary>
            yield return MoveToPosition(startPosition + new Vector3(0, 0, distance));

            yield return new WaitForSeconds(waitTime);

            /// <summary>
            /// Move the camera back to the starting position.
            /// </summary>
            yield return MoveToPosition(startPosition);

            yield return new WaitForSeconds(waitTime);
        }
    }

    /**
     * @brief Smoothly moves the camera toward a target position.
     *
     * Uses Vector3.MoveTowards to gradually move the camera until
     * it reaches the desired position.
     *
     * @param target Target position the camera should move to.
     * @return IEnumerator Required for coroutine execution.
     */
    IEnumerator MoveToPosition(Vector3 target)
    {
        /// <summary>
        /// Continue moving until the camera is very close to the target.
        /// </summary>
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        /// <summary>
        /// Ensures the final position is exactly the target.
        /// </summary>
        transform.position = target;
    }
}