using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class Tower
 * @brief Represents a defensive tower that detects enemies and shoots at them.
 *
 * The tower continuously searches for the closest enemy within its range,
 * rotates toward the target, and fires bullets at a specified rate.
 */
public class Tower : MonoBehaviour
{
    /// <summary>
    /// Maximum distance at which the tower can detect and target enemies.
    /// </summary>
    public float range = 10f;

    /// <summary>
    /// Speed at which the tower rotates toward its target.
    /// </summary>
    public float rotationSpeed = 5f;

    /// <summary>
    /// Time between shots (shots per second logic).
    /// </summary>
    public float fireRate = 1f;            // ile strza³ów na sekundê

    /// <summary>
    /// Prefab of the bullet that will be fired.
    /// </summary>
    public GameObject bulletPrefab;        // prefab pocisku

    /// <summary>
    /// Transform representing the position where bullets are spawned.
    /// </summary>
    public Transform firePoint;            // miejsce wystrza³u

    /// <summary>
    /// Current enemy target.
    /// </summary>
    private Transform target;

    /// <summary>
    /// Countdown timer controlling the firing interval.
    /// </summary>
    private float fireCountdown = .5f;

    /**
     * @brief Called once per frame.
     *
     * Searches for the closest enemy, rotates the tower toward it,
     * and fires bullets when the cooldown timer reaches zero.
     */
    void Update()
    {
        FindClosestEnemy();

        if (target != null)
        {
            RotateTowardsTarget();

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = .5f;
            }

            fireCountdown -= Time.deltaTime;
        }
    }

    /**
     * @brief Finds the closest enemy within the tower's range.
     *
     * Searches all objects tagged as "Enemy" and selects the nearest
     * one that is within the defined attack range.
     */
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        target = nearestEnemy != null ? nearestEnemy.transform : null;
    }

    /**
     * @brief Rotates the tower smoothly toward the current target.
     *
     * Uses Quaternion interpolation to gradually align the tower
     * with the enemy's position.
     */
    void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    /**
     * @brief Fires a bullet toward the current target.
     *
     * Instantiates a bullet prefab at the fire point and
     * sets its direction toward the enemy target.
     */
    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirectionTo(target);
        }
    }
}