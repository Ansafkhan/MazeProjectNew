using UnityEngine;

// This script makes a shooter object fire bullets at the player
public class ShooterHazard : MonoBehaviour
{
    [Header("References")]

    // Reference to player transform
    public Transform player;

    // Point from where bullet will spawn
    public Transform firePoint;

    // Bullet prefab object
    public GameObject bulletPrefab;

    [Header("Shooting")]

    // Time between each bullet
    public float fireInterval = 1.2f;

    // Speed of bullet
    public float bulletSpeed = 12f;

    // How long bullet stays before destroying
    public float bulletLifeTime = 4f;

    [Header("Aim")]

    // If true, shooter aims at player
    public bool aimAtPlayer = true;

    // Height offset so bullet aims near chest/head
    public float aimHeightOffset = 0.8f;

    // Timer to control shooting delay
    private float fireTimer = 0f;

    // Start runs when object is created
    private void Start()
    {
        // If player not assigned in Inspector, find by tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        // If firePoint not assigned, find child named FirePoint
        if (firePoint == null)
        {
            Transform fp = transform.Find("FirePoint");
            if (fp != null)
                firePoint = fp;
        }
    }

    // Update runs every frame
    private void Update()
    {
        // If something is missing, do nothing
        if (bulletPrefab == null || firePoint == null || player == null)
            return;

        // Reduce timer every frame
        fireTimer -= Time.deltaTime;

        // When timer reaches zero, shoot
        if (fireTimer <= 0f)
        {
            Shoot();

            // Reset timer
            fireTimer = fireInterval;
        }
    }

    // This function shoots a bullet
    private void Shoot()
    {
        Vector3 direction;

        // If aiming at player
        if (aimAtPlayer)
        {
            // Aim slightly above player position
            Vector3 targetPos = player.position + Vector3.up * aimHeightOffset;

            // Calculate direction towards player
            direction = (targetPos - firePoint.position).normalized;
        }
        else
        {
            // Otherwise shoot forward
            direction = firePoint.forward;
        }

        // Rotate shooter to face player (only horizontal rotation)
        Vector3 flatDir = direction;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(flatDir);
        }

        // Create bullet object
        GameObject bulletObj = Instantiate(
                                    bulletPrefab,
                                    firePoint.position,
                                    Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f)
                                );

        // Ignore collision between bullet and shooter
        Collider bulletCol = bulletObj.GetComponent<Collider>();
        Collider[] shooterCols = GetComponentsInChildren<Collider>();

        if (bulletCol != null)
        {
            foreach (Collider c in shooterCols)
            {
                Physics.IgnoreCollision(bulletCol, c);
            }
        }

        // Get HazardBullet script from bullet
        HazardBullet bullet = bulletObj.GetComponent<HazardBullet>();

        // If script found, initialize bullet
        if (bullet != null)
        {
            bullet.Init(direction, bulletSpeed, bulletLifeTime);
        }

        // Print message in console
        Debug.Log("Shooter fired bullet");
    }
}