using UnityEngine;

// This makes sure the object has a Rigidbody component
[RequireComponent(typeof(Rigidbody))]
public class HazardBullet : MonoBehaviour
{
    // Reference to Rigidbody component
    private Rigidbody rb;

    // Awake runs when object is created
    private void Awake()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Disable gravity so bullet does not fall down
        rb.useGravity = false;

        // Use continuous collision to avoid missing fast collisions
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // This function is used to initialize the bullet
    public void Init(Vector3 direction, float speed, float lifeTime)
    {
        // Set bullet velocity in given direction
        rb.linearVelocity = direction.normalized * speed;

        // Destroy bullet automatically after some time
        Destroy(gameObject, lifeTime);
    }

    // This runs when bullet touches something
    private void OnTriggerEnter(Collider other)
    {
        // If the other object is a trigger, ignore it
        if (other.isTrigger) return;

        // Try to get Player script directly from object
        Player player = other.GetComponent<Player>();

        // If not found, check parent object
        if (player == null)
            player = other.GetComponentInParent<Player>();

        // If Player found, kill player
        if (player != null)
        {
            player.Die();

            // Destroy bullet after hitting player
            Destroy(gameObject);
            return;
        }

        // If bullet hits wall, floor, or anything else
        // Destroy the bullet
        Destroy(gameObject);
    }
}