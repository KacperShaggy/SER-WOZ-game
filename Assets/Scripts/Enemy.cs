```csharp id = "z8r4tn"
using System.Collections;
using UnityEngine;

/**
 * @class Enemy
 * @brief Controls enemy behavior including movement, health, scaling, and death effects.
 *
 * The enemy moves toward a target (carriage), gradually slows down as it loses health,
 * visually scales based on damage taken, and plays particle effects upon death.
 */
public class Enemy : MonoBehaviour
{
    /** @brief Particle system played when the enemy dies */
    [SerializeField]
    private ParticleSystem deathParticles;

    /** @brief Current health of the enemy */
    [SerializeField]
    private float health = 100f;

    /** @brief Initial movement speed */
    [SerializeField]
    private float startingSpeed = 5f;

    /** @brief Reference to the target object (carriage) */
    [SerializeField]
    private GameObject targetCarriage;

    /** @brief Visual body of the enemy (used for scaling effect) */
    [SerializeField]
    private GameObject body;

    /** @brief Current movement speed */
    private float speed;

    /** @brief Initial scale of the body */
    private Vector3 bodyStartSize;

    /**
     * @brief Unity Start method.
     *
     * Initializes speed and stores initial body scale.
     */
    private void Start()
    {
        bodyStartSize = body.transform.localScale;
        speed = startingSpeed;
    }

    /**
     * @brief Called once per frame.
     *
     * Handles movement toward the target and rotation.
     */
    private void Update()
    {
        if (targetCarriage == null) return;

        Vector3 targetPosition = targetCarriage.transform.position;
        targetPosition.y = transform.position.y;

        // Move toward carriage
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(targetPosition.x, transform.position.y, targetPosition.z),
            speed * Time.deltaTime
        );

        // Constant backward drift (optional effect)
        transform.position += Vector3.back * Time.deltaTime;

        // Rotate toward target
        transform.LookAt(targetPosition);
    }

    /**
     * @brief Smoothly scales the enemy body over time.
     *
     * @param targetScale Target scale of the body
     * @param duration Duration of the scaling animation
     * @return IEnumerator for coroutine handling
     */
    private IEnumerator ScaleTween(Vector3 targetScale, float duration)
    {
        Vector3 startScale = body.transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            body.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        body.transform.localScale = targetScale;
    }

    /**
     * @brief Applies damage to the enemy and updates its state.
     *
     * Reduces health, scales the body visually, and slows movement.
     * Triggers death if health reaches zero.
     *
     * @param damage Amount of damage dealt
     */
    public void TakeDamage(float damage)
    {
        health -= damage;

        // Calculate scaling factor based on missing health
        float factor = (100f - health) / 100f * 2f + 1f;

        Vector3 targetScale = new Vector3(
            bodyStartSize.x * factor,
            bodyStartSize.y * factor,
            bodyStartSize.z * factor
        );

        StartCoroutine(ScaleTween(targetScale, 0.3f));

        // Reduce speed proportionally to health
        speed = startingSpeed * Mathf.Clamp01(health / 100f);

        if (health <= 0f)
        {
            Die();
        }
    }

    /**
     * @brief Spawns and plays death particle effects.
     */
    private void EmitDeathParticles()
    {
        if (deathParticles == null) return;

        ParticleSystem ps = Instantiate(
            deathParticles,
            transform.position,
            Quaternion.identity
        );

        ps.Play();
        Destroy(ps.gameObject, ps.main.duration);
    }

    /**
     * @brief Handles enemy death.
     *
     * Plays effects and removes the enemy from the scene.
     */
    private void Die()
    {
        EmitDeathParticles();
        Destroy(gameObject);
    }

    /**
     * @brief Unity collision callback.
     *
     * Destroys the enemy when it reaches the target carriage.
     *
     * @param collision Collision data
     */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == targetCarriage)
        {
            Destroy(gameObject);
        }
    }
}
```
