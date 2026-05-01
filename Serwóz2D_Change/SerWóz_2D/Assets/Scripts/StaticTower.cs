using UnityEngine;

/// <summary>
/// Represents a static tower that automatically detects enemies and fires at them.
/// </summary>
public class StaticTower : MonoBehaviour
{
    /// <summary>Maximum attack range of the tower.</summary>
    [SerializeField] private float range;

    /// <summary>Number of bullets fired per attack.</summary>
    [SerializeField] private float bulletCount;

    /// <summary>Maximum spread angle for bullets.</summary>
    [SerializeField] private float spreadAngle;

    /// <summary>Delay between consecutive attacks.</summary>
    [SerializeField] private float delay;

    /// <summary>Time marker for next allowed shot.</summary>
    [SerializeField] private float timeCounter;

    /// <summary>Prefab used to instantiate bullets.</summary>
    [SerializeField] private GameObject bulletPrefab;

    /// <summary>Reference point from which bullets are fired.</summary>
    [SerializeField] private GameObject Weapon;

    /// <summary>Initial local position of the tower (used for recoil recovery).</summary>
    public Vector3 startPosition;

    /// <summary>
    /// Initializes tower position and randomizes initial delay.
    /// </summary>
    void Start()
    {
        startPosition = transform.localPosition;
        delay = Random.Range(0.5f, 1.5f);
    }

    /// <summary>
    /// Finds the closest living enemy within range.
    /// </summary>
    /// <returns>The closest enemy GameObject if found and within range; otherwise null.</returns>
    GameObject LocateClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
            return null;

        GameObject closestEnemy = null;
        float closestDistance = 0;

        /// <summary>Initialize with any living enemy.</summary>
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Enemy>().living)
            {
                closestDistance = Vector2.Distance(transform.position, enemy.transform.position);
                closestEnemy = enemy;
            }
        }

        /// <summary>Find the closest living enemy.</summary>
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && enemy.GetComponent<Enemy>().living)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        /// <summary>Return enemy only if within range.</summary>
        if (closestDistance <= range)
        {
            return closestEnemy;
        }

        return null;
    }

    /// <summary>
    /// Updates tower behavior: targeting, aiming, and shooting.
    /// </summary>
    void Update()
    {
        /// <summary>Return smoothly to original position after recoil.</summary>
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPosition, 2 * Time.deltaTime);

        GameObject enemy = LocateClosestEnemy();
        if (enemy != null)
        {
            /// <summary>Flip tower sprite depending on enemy position.</summary>
            if (enemy.transform.position.x < transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);

            /// <summary>Calculate direction and rotation towards enemy.</summary>
            Vector3 direction = enemy.transform.position - Weapon.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion lookRotaion = Quaternion.Euler(0, 0, angle);
            Weapon.transform.rotation = lookRotaion;

            /// <summary>Handle shooting logic based on cooldown.</summary>
            if (Time.time >= timeCounter)
            {
                for (int i = 0; i < bulletCount; i++)
                {
                    /// <summary>Random spread angle.</summary>
                    float angleOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);

                    /// <summary>Base direction angle.</summary>
                    float originalAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    /// <summary>Apply spread to angle.</summary>
                    float newAngle = originalAngle + angleOffset;

                    /// <summary>Convert angle to direction vector.</summary>
                    Vector3 spreadDirection = new Vector3(
                        Mathf.Cos(newAngle * Mathf.Deg2Rad),
                        Mathf.Sin(newAngle * Mathf.Deg2Rad),
                        0f
                    );

                    /// <summary>Instantiate bullet with rotation.</summary>
                    GameObject bullet = Instantiate(
                        bulletPrefab,
                        Weapon.transform.position,
                        Quaternion.Euler(0, 0, newAngle)
                    );

                    /// <summary>Assign movement direction to bullet.</summary>
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    bulletScript.SetDirection(spreadDirection);
                }

                /// <summary>Apply recoil effect.</summary>
                transform.localPosition -= direction.normalized / 5;

                /// <summary>Set next allowed shooting time.</summary>
                timeCounter = Time.time + delay;
            }
        }
    }
}