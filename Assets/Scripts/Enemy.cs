using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathParticles;

    public float health = 100f;
    public float startingSpeed = 5f;
    private float speed;
    public GameObject TestCariage;
    public GameObject body;
    private Vector3 bodyStartSize;
    void Start()
    {
        bodyStartSize = body.transform.localScale;
        speed = startingSpeed;
    }

    void Update()
    {
        //moving to cariage
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(TestCariage.transform.position.x, transform.position.y, TestCariage.transform.position.z), speed * Time.deltaTime);
        transform.position += Vector3.back * 1f * Time.deltaTime; //znoszenie w dó³

        var targetPosition = TestCariage.transform.position;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
    }
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

    public void Die()
    {
        EmitDeathParticles();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //destroying enemy when it reaches the cariage
        if (collision.gameObject == TestCariage)
        {
            Destroy(gameObject);
        }
    }
}
