using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 40f;
    public Vector3 direction;


    public void SetDirectionTo(Transform _target)
    {
        direction = new Vector3(_target.position.x - transform.position.x,0, _target.position.z - transform.position.z).normalized;
    }
    public void SetDirection(Vector3 _direction)
    {
        direction = (_direction).normalized;
    }

    private void Start()
    {
        Destroy(gameObject, 3);
    }
    void Update()
    {

        float distanceThisFrame = speed * Time.deltaTime;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Vector3 directionToEnemy = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z)  - transform.position;
                if (directionToEnemy.magnitude <= 1)
                {
                    HitTarget(enemy);
                    return;
                }
            }
        }
        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    void HitTarget(GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}