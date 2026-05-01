using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents an enemy that targets train carriages, takes damage, and handles death behavior.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>Main visual body of the enemy.</summary>
    [SerializeField] private GameObject body;

    /// <summary>Shadow object attached to the enemy.</summary>
    [SerializeField] private GameObject shadow;

    /// <summary>Initial health value.</summary>
    [SerializeField] private float StartedHealth;

    /// <summary>Current health value.</summary>
    private float health;

    /// <summary>Particle effect spawned on death.</summary>
    [SerializeField] private GameObject DeathParticle;

    /// <summary>Coin prefab dropped on death.</summary>
    [SerializeField] private GameObject Coin;

    /// <summary>Initial movement speed.</summary>
    [SerializeField] private float StartedSpeed;

    /// <summary>Current movement speed.</summary>
    private float speed;

    /// <summary>Target carriage that the enemy is chasing.</summary>
    private GameObject targetCarriage;

    /// <summary>Shared list of active enemies.</summary>
    public List<GameObject> enemiesList;

    /// <summary>Target position after attaching to carriage.</summary>
    private Vector3 deathDestination;

    /// <summary>Indicates if death sequence has already started.</summary>
    private bool alive = true;

    /// <summary>Indicates if enemy is actively moving.</summary>
    public bool living = true;

    /// <summary>
    /// Initializes enemy stats and selects the closest carriage as target.
    /// </summary>
    void Start()
    {
        Invoke(nameof(DestroyEnemyAfterTime), 10f);

        health = StartedHealth;
        speed = StartedSpeed;

        /// <summary>Find all carriages and select the closest one.</summary>
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

        /// <summary>Set initial rotation towards target.</summary>
        Vector3 destination = new Vector3(
            targetCarriage.transform.position.x,
            targetCarriage.transform.position.y,
            transform.position.z
        );

        Vector3 direction = destination - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Applies damage to the enemy and updates its state.
    /// </summary>
    /// <param name="damage">Damage value.</param>
    /// <param name="direction">Knockback direction.</param>
    public void TakeDamage(float damage, Vector2 direction)
    {
        health -= damage;

        /// <summary>Adjust speed and body scale based on remaining health.</summary>
        float bodyMultiplier = (1f - health / StartedHealth);
        float speedMultiplier = 1f + bodyMultiplier * 2;
        speed = StartedSpeed / speedMultiplier;

        Vector3 targetScale = new Vector3(
            1 + 1f * bodyMultiplier,
            1 + 0.3f * bodyMultiplier,
            1f
        );

        float lerpSpeed = 8f;

        body.transform.localScale = Vector3.Lerp(
            body.transform.localScale,
            targetScale,
            Time.deltaTime * lerpSpeed
        );

        /// <summary>Apply knockback.</summary>
        transform.position += (Vector3)direction / 50;

        /// <summary>Handle death.</summary>
        if (health <= 0 && alive)
        {
            alive = false;

            /// <summary>Spawn death particle effect.</summary>
            GameObject newParticle = Instantiate(
                DeathParticle,
                transform.position,
                Quaternion.identity
            );

            newParticle.GetComponent<ParticleSystem>().Emit(20);

            /// <summary>Spawn random coins.</summary>
            int coinNumber = Random.Range(0, 3);

            for (int i = 0; i < coinNumber; i++)
            {
                GameObject newCoin = Instantiate(
                    Coin,
                    transform.position + new Vector3(
                        Random.Range(-.5f, .5f),
                        Random.Range(-.5f, .5f)
                    ),
                    Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))
                );
            }

            enemiesList.Remove(this.gameObject);

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Destroys enemy after a fixed time if still alive.
    /// </summary>
    private void DestroyEnemyAfterTime()
    {
        if (enemiesList.Contains(this.gameObject))
            enemiesList.Remove(this.gameObject);

        Destroy(gameObject);
    }

    /// <summary>
    /// Handles collision with carriage.
    /// </summary>
    /// <param name="other">Collider encountered.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Carriage"))
        {
            /// <summary>Attach to carriage.</summary>
            transform.parent = other.transform;

            Destroy(shadow);

            Vector3 destination = new Vector3(
                targetCarriage.transform.position.x,
                targetCarriage.transform.position.y,
                transform.localPosition.z
            );

            Vector3 direction = (destination - transform.localPosition).normalized;

            deathDestination = transform.localPosition + direction / 3;

            /// <summary>Apply damage to train.</summary>
            other.transform.parent.GetComponent<Train>().TakeDamage(10);

            living = false;

            transform.GetComponent<CircleCollider2D>().enabled = false;

            /// <summary>Change appearance to "dead" state.</summary>
            foreach (Transform child in transform)
            {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                }
            }

            CancelInvoke(nameof(DestroyEnemyAfterTime));
            enemiesList.Remove(this.gameObject);
        }
    }

    /// <summary>
    /// Updates movement and behavior each frame.
    /// </summary>
    void Update()
    {
        if (living)
        {
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

            /// <summary>Apply global left movement (world scrolling).</summary>
            transform.position += Vector3.left * Time.deltaTime;

            /// <summary>Rotate towards movement direction.</summary>
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
        else
        {
            /// <summary>Move toward final death position.</summary>
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                deathDestination,
                5 * Time.deltaTime
            );
        }
    }
}