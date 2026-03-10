using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 10f;
    public float rotationSpeed = 5f;
    public float fireRate = 1f;            // ile strza³ów na sekundê
    public GameObject bulletPrefab;        // prefab pocisku
    public Transform firePoint;            // miejsce wystrza³u

    private Transform target;
    private float fireCountdown = .5f;

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

    void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

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