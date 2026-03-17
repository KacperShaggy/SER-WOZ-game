using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/**
 * @class ActiveTower
 * @brief Controls an active tower that fires bullets in a patterned sequence.
 *
 * When the player presses the Q key, a coroutine starts that continuously
 * spawns bullets in opposite directions, creating a sprinkler-like firing pattern.
 */
public class ActiveTower : MonoBehaviour
{
    /// <summary>
    /// Maximum number of shots fired during one firing sequence.
    /// </summary>
    public float fireNumber = 100f;            // ile strza³ów

    /// <summary>
    /// Time delay between shots in seconds.
    /// </summary>
    public float fireRate = 0.1f;            // ile strza³ów na sekundê

    /// <summary>
    /// Prefab of the bullet that will be instantiated when firing.
    /// </summary>
    public GameObject bulletPrefab;        // prefab pocisku

    /// <summary>
    /// Transform representing the point from which bullets are fired.
    /// </summary>
    public Transform firePoint;            // miejsce wystrza³u


    /**
     * @brief Called once when the object is initialized.
     *
     * Currently unused but can be used for initialization logic.
     */
    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Reference to the currently running firing coroutine.
    /// Prevents multiple firing routines from running simultaneously.
    /// </summary>
    Coroutine fireRoutine;

    /**
     * @brief Called once per frame.
     *
     * Detects when the Q key is pressed. If no firing coroutine is active,
     * it starts the FireLoop coroutine.
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && fireRoutine == null)
        {
            fireRoutine = StartCoroutine(FireLoop());
        }
    }

    /**
     * @brief Coroutine responsible for spawning bullets in a pattern.
     *
     * Bullets are spawned in two opposite directions. Each shot changes
     * the angle, creating a rotating sprinkler-like firing pattern.
     *
     * @return IEnumerator Required for Unity coroutine execution.
     */
    //Sprinkler
    IEnumerator FireLoop()
    {
        int count = 0;

        /// <summary>
        /// Loop generating bullets until the maximum number of shots is reached.
        /// </summary>
        while (count < fireNumber)
        {
            count++;

            /// <summary>
            /// Creates two bullets fired in opposite directions.
            /// </summary>
            for (int i = 0; i < 2; i++) //2 bo na dwie storny
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();

                if (bulletScript != null)
                {
                    /**
                     * @brief Calculates the firing angle for the bullet.
                     *
                     * The angle depends on the current shot number and determines
                     * the direction in which the bullet will travel.
                     */
                    float angle = ((360f / fireNumber) + 180 * i) * count * Mathf.Deg2Rad; // obliczanie k¹ta dla ka¿dego pocisku

                    /// <summary>
                    /// Sets the movement direction of the bullet.
                    /// </summary>
                    bulletScript.SetDirection(new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)));
                }

            }

            /// <summary>
            /// Waits for the specified fireRate before spawning the next bullets.
            /// </summary>
            yield return new WaitForSeconds(fireRate);
        }

        /// <summary>
        /// Resets the coroutine reference after the firing sequence finishes.
        /// </summary>
        fireRoutine = null;
    }
}