using UnityEngine;

// This script makes an AI enemy wander inside a region randomly
[RequireComponent(typeof(Rigidbody))]
public class WandererAI : MonoBehaviour
{
    // Movement speed of AI
    public float moveSpeed = 2f;

    // Time before changing direction
    public float directionChangeTime = 2f;

    // Reference to Rigidbody
    private Rigidbody rb;

    // Current move direction
    private Vector3 moveDirection;

    // Timer for direction changes
    private float timer;

    // Bounds of allowed movement region
    private Bounds bounds;

    // True if region is assigned
    private bool hasRegion;

    // Awake runs when object is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Disable gravity so AI floats
        rb.useGravity = false;

        // Freeze rotation X and Z so AI doesn't tip over
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Assign movement region
    public void SetRegion(WanderRegion region)
    {
        bounds = region.GetBounds();
        hasRegion = true;

        // Pick initial random direction
        PickNewDirection();
    }

    // Update runs every frame
    void Update()
    {
        if (!hasRegion) return;

        // Increment timer
        timer += Time.deltaTime;

        // Pick new direction when time exceeded
        if (timer >= directionChangeTime)
        {
            PickNewDirection();
            timer = 0f;
        }
    }

    // FixedUpdate runs at physics interval
    void FixedUpdate()
    {
        if (!hasRegion) return;

        // Move AI
        rb.linearVelocity = moveDirection * moveSpeed;

        // Check next position
        Vector3 nextPos = transform.position + rb.linearVelocity * Time.fixedDeltaTime;
        if (!InsideXZ(bounds, nextPos))
        {
            // Turn AI back inside region
            TurnBackInside();
        }
    }

    // Check if position is inside XZ bounds
    bool InsideXZ(Bounds b, Vector3 p)
    {
        return p.x >= b.min.x && p.x <= b.max.x &&
               p.z >= b.min.z && p.z <= b.max.z;
    }

    // Turn AI toward center if out of bounds
    void TurnBackInside()
    {
        Vector3 toCenter = bounds.center - transform.position;
        toCenter.y = 0f;
        moveDirection = toCenter.normalized;
        timer = 0f;
    }

    // Pick a random direction on XZ plane
    void PickNewDirection()
    {
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        moveDirection = new Vector3(x, 0f, z).normalized;
    }

    // Collision detection
    void OnCollisionEnter(Collision collision)
    {
        // If collided with player, kill player
        if (collision.gameObject.CompareTag("Player"))
        {
            var ph = collision.gameObject.GetComponent<Player>();
            if (ph != null) ph.Die();
            else collision.gameObject.SetActive(false); // fallback
            return;
        }

        // If collided with wall or obstacle, pick new direction
        PickNewDirection();
    }
}