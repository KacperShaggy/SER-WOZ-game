using UnityEngine;

/// <summary>
/// Represents a projectile that travels in a given direction and deals damage to enemies.
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>Damage dealt to enemy on hit.</summary>
    [SerializeField] private float damage = 2f;

    /// <summary>Movement speed of the bullet.</summary>
    [SerializeField] private float speed = 3f;

    /// <summary>Normalized movement direction of the bullet.</summary>
    private Vector2 direction;

    /// <summary>
    /// Sets the movement direction of the bullet.
    /// </summary>
    /// <param name="_direction">Direction vector to assign.</param>
    public void SetDirection(Vector2 _direction)
    {
        direction = _direction.normalized;
    }

    /// <summary>
    /// Initializes bullet lifetime.
    /// </summary>
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    /// <summary>
    /// Handles collision with enemies and applies damage.
    /// </summary>
    /// <param name="other">Collider that was hit.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage, direction);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Moves the bullet every frame in its assigned direction.
    /// </summary>
    void Update()
    {
        transform.position += (Vector3)direction * Time.deltaTime * speed;
    }
}