using UnityEngine;

// This script is for hazards (like spikes, fire, etc.)
public class Hazard : MonoBehaviour
{
    [Header("Sound")]
    public AudioClip deathSound; // Sound that will play when player dies
    [Range(0f, 1f)] public float deathSoundVolume = 1f; // Volume of the death sound

    private int playerLayer; // To store the layer number of Player

    private void Awake()
    {
        // Get the layer number of "Player"
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        // This function runs when something enters the trigger
        // Check if the object that touched is the Player
        if (other.gameObject.layer == playerLayer)
        {
            // Get the Player script from that object
            Player p = other.GetComponent<Player>();

            // If Player script is found
            if (p != null)
            {
                // Play death sound at player's position
                if (deathSound != null)
                {
                    AudioSource.PlayClipAtPoint(deathSound, other.transform.position, deathSoundVolume);
                }

                // Call Die() function from Player script
                p.Die();
            }
        }
    }
}