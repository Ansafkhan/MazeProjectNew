using UnityEngine;

// This script kills the player if they touch spikes or dangerous zone
public class SpikeKillZone : MonoBehaviour
{
    // Runs when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Print which object entered for debugging
        Debug.Log("KillZone entered by: " + other.name);

        // Only kill player, ignore other objects
        if (!other.CompareTag("Player")) return;

        // Call kill function
        Kill(other.gameObject);
    }

    // Runs every frame while something stays in the trigger
    private void OnTriggerStay(Collider other)
    {
        // If OnTriggerEnter didn't work for some reason
        if (!other.CompareTag("Player")) return;

        // Call kill function
        Kill(other.gameObject);
    }

    // This function handles killing the player
    private void Kill(GameObject player)
    {
        // Try to get Player script
        Player ph = player.GetComponent<Player>();

        // If found, call Die() function
        if (ph != null) 
            ph.Die();
        else 
            // If no Player script, just deactivate object
            player.SetActive(false);
    }
}