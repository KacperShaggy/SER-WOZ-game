using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Enemy
 * @brief Represents an enemy unit that moves toward a target carriage.
 *
 * The enemy moves toward a specified target object, takes damage from bullets,
 * scales visually based on remaining health, slows down when damaged,
 * and plays particle effects upon death.
 */
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// Particle system played when the enemy dies.
    /// </summary>
    [SerializeField] private ParticleSystem deathParticles;

    /// <summary>
    /// Current health of the enemy.
    /// </summary>
    public float health = 100f;

    /// <summary>
    /// Initial movement speed of the enemy.
    /// </summary>
    public float startingSpeed = 5f;

    /// <summary>
    /// Current movement speed (changes based on health).
    /// </summary>
    private float speed;

    /// <summary>
    /// Target object the enemy moves toward.
    /// </summary>
    public GameObject TestCariage;

    /// <summary>
    /// Visual body object used for scaling effects.
    /// </summary>
    public GameObject body;

    /// <summary>
    /// Original scale of the enemy body.
    /// </summary>
    private Vector3 bodyStartSize;

    /**
     * @brief Called when the enemy object is initialized.
     *
     * Stores the initial body scale and sets the starting speed.
     */
    void Start()
    {
        bodyStartSize = body.transform.localScale;
        speed = startingSpeed;
    }

    /**
     * @brief Called once per frame.
     *
     * Moves the enemy toward the carriage, applies a constant backward
     * drift, and rotates the enemy to face the target.
     */
    void Update()
    {
        //moving to cariage
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(TestCariage.transform.position.x, transform.position.y, TestCariage.transform.position.z), speed * Time.deltaTime);
        transform.position += Vector3.back * 1f * Time.deltaTime; //znoszenie w dó³

        var targetPosition = TestCariage.transform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
    }

    /**
     * @brief Coroutine that smoothly scales the enemy body.
     *
     * Gradually interpolates the body scale from its current size
     * to the target scale over the specified duration.
     *
     * @param targetScale The desired scale of the enemy body.
     * @param duration Duration of the scaling animation.
     * @return IEnumerator Required for coroutine execution.
     */
    IEnumerator ScaleTween(Vector3 targetScale, float duration)
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
     * @brief Applies damage to the enemy.
     *
     * Reduces health, scales the body depending on damage taken,
     * slows movement speed, and triggers death if health reaches zero.
     *
     * @param damage Amount of damage dealt to the enemy.
     */
    public void TakeDamage(float damage)
    {
        health -= damage;

        float factor = (100 - health) / 100f * 2 + 1;

        Vector3 targetScale = new Vector3(
            bodyStartSize.x * factor,
            bodyStartSize.y * factor,
            bodyStartSize.z * factor);

        StartCoroutine(ScaleTween(targetScale, 0.3f)); //skalowanie cia³a w zale¿noœci od zdrowia

        speed = startingSpeed * (health / 100f); //zmniejszanie prêdkoœci w zale¿noœci od zdrowia

        if (health <= 0)
        {
            Die();
        }
    }

    /**
     * @brief Spawns and plays the death particle effect.
     *
     * Instantiates the particle system at the enemy's position
     * and destroys it after its duration.
     */
    public void EmitDeathParticles()
    {
        //if (deathParticles == null) return;

        ParticleSystem ps = Instantiate(
        deathParticles,
        transform.position,
        Quaternion.identity);

        ps.Play();

        Destroy(ps.gameObject, ps.main.duration);
    }

    /**
     * @brief Handles enemy death.
     *
     * Plays particle effects and removes the enemy from the scene.
     */
    public void Die()
    {
        EmitDeathParticles();
        Destroy(gameObject);
    }

    /**
     * @brief Detects collisions with other objects.
     *
     * If the enemy collides with the carriage, it is destroyed.
     *
     * @param collision Collision information provided by Unity.
     */
    private void OnCollisionEnter(Collision collision)
    {
        //destroying enemy when it reaches the cariage
        if (collision.gameObject == TestCariage)
        {
            Destroy(gameObject);
        }
    }
}