using UnityEngine;

/**
 * @class GroundRow
 * @brief Controls the movement and lifecycle of a ground row segment.
 *
 * The ground row moves backward continuously to simulate map movement.
 * After travelling a certain distance, it triggers the creation of a new
 * ground row and eventually destroys itself once it exceeds its lifetime distance.
 */
public class GroundRow : MonoBehaviour
{
    /// <summary>
    /// Size of a single node in the ground grid.
    /// Determines when the next row should be generated.
    /// </summary>
    private float nodeSize = 2f;

    /// <summary>
    /// Movement speed of the ground row.
    /// </summary>
    //wolalbym zeby to nie bylo public
    public float speed = 5f;

    /// <summary>
    /// Reference to the map generator responsible for creating new rows.
    /// </summary>
    public MapGenerating mapGenerating;

    /// <summary>
    /// Starting position of this ground row.
    /// </summary>
    private Vector3 startPosition;

    /// <summary>
    /// Distance the ground row has travelled since spawning.
    /// </summary>
    private float distanceTravelled = 0f;

    /// <summary>
    /// Maximum distance the ground row should travel before being destroyed.
    /// </summary>
    public float distanceToTravel;

    /// <summary>
    /// Flag indicating whether the next ground row has already been created.
    /// Prevents multiple creations.
    /// </summary>
    private bool createdComplete = false;

    /**
     * @brief Called once when the object is initialized.
     *
     * Stores the initial position of the ground row.
     */
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    /**
     * @brief Called once per frame.
     *
     * Moves the ground row backwards, tracks the distance travelled,
     * triggers creation of the next ground row when needed,
     * and destroys this row after it travels beyond its allowed distance.
     */
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        distanceTravelled = Vector3.Distance(startPosition, transform.position);

        /**
         * @brief Creates a new ground row after travelling one node size.
         */
        if (!createdComplete && distanceTravelled >= nodeSize)
        {
            createdComplete = true;

            //utworz nowy node
            mapGenerating.CreateGroundRow();
        }

        /**
         * @brief Destroys the ground row after exceeding its travel distance.
         */
        if (distanceTravelled >= distanceToTravel)
        {
            Destroy(gameObject);
        }
    }
}