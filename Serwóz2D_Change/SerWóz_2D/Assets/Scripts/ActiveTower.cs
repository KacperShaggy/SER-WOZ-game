using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Represents an active tower that fires bullets in random directions when triggered.
/// </summary>
public class ActiveTower : MonoBehaviour
{
    /// <summary>Prefab used to instantiate bullets.</summary>
    public GameObject bulletPrefab;

    /// <summary>Number of bullets fired during one activation.</summary>
    public int bulletCount = 64;

    /// <summary>Optional starting angle offset (currently unused).</summary>
    public float startAngleOffset = 0f;

    /// <summary>Delay between consecutive shots.</summary>
    public float delayBetweenShots = 0.05f;

    /// <summary>Original scale of the tower (used for animation).</summary>
    private Vector3 originalScale;

    /// <summary>
    /// Initializes the tower and registers button listener.
    /// </summary>
    void Start()
    {
        originalScale = transform.localScale;

        GameObject.FindGameObjectWithTag("SprinklerButton")
            .GetComponent<Button>()
            .onClick.AddListener(UseActiveTower);
    }

    /// <summary>
    /// Triggers the tower ability (bullet sprinkler).
    /// </summary>
    public void UseActiveTower()
    {
        StartCoroutine(Sprinkler());
    }

    /// <summary>
    /// Coroutine that spawns bullets in random directions with a scaling animation.
    /// </summary>
    /// <returns>IEnumerator for coroutine execution.</returns>
    IEnumerator Sprinkler()
    {
        float maxScaleMultiplier = 1.2f;

        for (int i = 0; i < bulletCount; i++)
        {
            /// <summary>Scale up effect during shooting.</summary>
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale * maxScaleMultiplier,
                0.4f
            );

            /// <summary>Generate random angle for bullet direction.</summary>
            float angle = Random.Range(0f, 360f);

            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            /// <summary>Instantiate bullet.</summary>
            GameObject bullet = Instantiate(
                bulletPrefab,
                transform.position,
                rotation
            );

            /// <summary>Calculate direction vector from angle.</summary>
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            /// <summary>Assign direction to bullet script.</summary>
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetDirection(direction.normalized);

            yield return new WaitForSeconds(delayBetweenShots);
        }

        /// <summary>Return to original scale after shooting.</summary>
        float t = 0f;
        Vector3 startScale = transform.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}