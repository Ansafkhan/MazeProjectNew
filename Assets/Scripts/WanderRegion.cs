using UnityEngine;

// This script defines a region where AI wanderers can move and spawns them
public class WanderRegion : MonoBehaviour
{
    [Header("Spawn")]

    // Prefab of the AI wanderer
    public GameObject wandererPrefab;

    // How many wanderers to spawn
    public int spawnCount = 1;

    // Y position to spawn at
    public float ySpawn = 0.5f;

    // Reference to BoxCollider defining region
    private BoxCollider box;

    // Awake runs when object is created
    void Awake()
    {
        box = GetComponent<BoxCollider>();

        // Make sure BoxCollider is assigned
        if (!box) 
            Debug.LogError("WanderRegion needs a BoxCollider.");
    }

    // Start runs once at beginning
    void Start()
    {
        // Spawn the specified number of wanderers
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnWanderer();
        }
    }

    // Return bounds of the region
    public Bounds GetBounds() => box.bounds;

    // Pick a random position inside the bounds
    Vector3 RandomPointInBounds(Bounds b)
    {
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        return new Vector3(x, ySpawn, z);
    }

    // Spawn a wanderer inside this region
    void SpawnWanderer()
    {
        if (!wandererPrefab) return;

        // Pick random spawn position
        Vector3 pos = RandomPointInBounds(GetBounds());

        // Instantiate wanderer
        var w = Instantiate(wandererPrefab, pos, Quaternion.identity);

        // Tell wanderer which region it belongs to
        var ai = w.GetComponent<WandererAI>();
        if (ai != null) 
            ai.SetRegion(this);
    }
}