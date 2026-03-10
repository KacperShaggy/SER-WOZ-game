using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ActiveTower : MonoBehaviour
{
    public float fireNumber = 100f;            // ile strza³ów
    public float fireRate = 0.1f;            // ile strza³ów na sekundê
    public GameObject bulletPrefab;        // prefab pocisku
    public Transform firePoint;            // miejsce wystrza³u


    // Start is called before the first frame update
    void Start()
    {

    }

    Coroutine fireRoutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && fireRoutine == null)
        {
            fireRoutine = StartCoroutine(FireLoop());
        }
    }
    //Sprinkler
    IEnumerator FireLoop()
    {
        int count = 0;
        while (count < fireNumber)
        {
            count++;

            for (int i = 0; i < 2; i++) //2 bo na dwie storny
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();

                if (bulletScript != null)
                {
                    float angle = ((360f / fireNumber) + 180*i) * count * Mathf.Deg2Rad; // obliczanie k¹ta dla ka¿dego pocisku
                    bulletScript.SetDirection(new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)));
                }
            
            }

               
            

            yield return new WaitForSeconds(fireRate);
        }
        fireRoutine = null;
    }
}