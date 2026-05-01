using UnityEngine;
using System.Collections;

/// <summary>
/// Represents a collectible coin that moves toward a target carriage and increases player currency.
/// </summary>
public class Coin : MonoBehaviour
{
    /// <summary>Target carriage the coin is attracted to.</summary>
    private GameObject targetCarriage;

    /// <summary>Movement speed of the coin.</summary>
    private float speed = 10;

    /// <summary>Delay before coin starts moving.</summary>
    public float duration = 0.2f;

    /// <summary>Reference to Train for updating coin count.</summary>
    public Train train;

    /// <summary>Target scale after spawn animation.</summary>
    private Vector3 targetScale;

    /// <summary>Delay timer before movement starts.</summary>
    public float delay = 2f;

    /// <summary>Internal timer for movement delay.</summary>
    private float timer = 0f;

    /// <summary>Indicates whether coin can start moving.</summary>
    private bool canMove = false;

    /// <summary>
    /// Initializes coin, finds target, sets rotation and plays spawn animation.
    /// </summary>
    void Start()
    {
        /// <summary>Get reference to Train object.</summary>
        train = GameObject.FindGameObjectWithTag("Train").GetComponent<Train>();

        /// <summary>Find closest carriage as target.</summary>
        GameObject[] carriages = GameObject.FindGameObjectsWithTag("Carriage");

        float d = Vector2.Distance(transform.position, carriages[0].transform.position);
        targetCarriage = carriages[0];

        foreach (GameObject carriage in carriages)
        {
            float dist = Vector2.Distance(transform.position, carriage.transform.position);
            if (dist < d)
            {
                targetCarriage = carriage;
            }
        }

        /// <summary>Rotate coin toward target carriage.</summary>
        Vector3 destination = new Vector3(
            targetCarriage.transform.position.x,
            targetCarriage.transform.position.y,
            transform.position.z
        );

        Vector3 direction = destination - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        /// <summary>Setup spawn scaling animation.</summary>
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;

        StartCoroutine(Grow());
    }

    /// <summary>
    /// Smoothly scales coin from zero to full size.
    /// </summary>
    IEnumerator Grow()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    /// <summary>
    /// Handles coin pickup when colliding with a carriage.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Carriage"))
        {
            train.coinNumber += 1;
            train.coinNumberText.text = $"Coins: {train.coinNumber}";

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates coin movement and delayed activation.
    /// </summary>
    void Update()
    {
        /// <summary>Wait before enabling movement.</summary>
        if (!canMove)
        {
            timer += Time.deltaTime;

            if (timer >= delay)
                canMove = true;

            return;
        }

        /// <summary>Move toward target carriage.</summary>
        Vector3 destination = new Vector3(
            targetCarriage.transform.position.x,
            targetCarriage.transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            speed * Time.deltaTime
        );

        /// <summary>Global left drift.</summary>
        transform.position += Vector3.left * Time.deltaTime;

        /// <summary>Rotate toward movement direction.</summary>
        Vector3 direction = destination - transform.position;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                1 * Time.deltaTime
            );
        }
    }
}