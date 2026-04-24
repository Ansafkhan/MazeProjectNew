using UnityEngine;

// This script controls spikes that rise and fall periodically
public class SpikeTrap : MonoBehaviour
{
    // The spikes object to move
    public Transform spikes;

    // Y position when spikes are hidden
    public float downHeight = 0f;

    // Y position when spikes are active
    public float upHeight = 0.6f;

    // Time taken to move up or down
    public float moveDuration = 0.25f;

    // How long spikes stay hidden
    public float timeDown = 1.0f;

    // How long spikes stay up
    public float timeUp = 1.0f;

    // States of the trap
    private enum TrapState { Down, Rising, Up, Falling }

    // Current state
    private TrapState state = TrapState.Down;

    // Timer for lerping movement
    private float t;

    // Starting Y of current movement
    private float startY;

    // Target Y of current movement
    private float targetY;

    // Runs when object starts
    void Start()
    {
        // Make sure spikes are assigned
        if (spikes == null)
        {
            Debug.LogError("SpikeTrap: Assign spikes Transform in Inspector!");
            enabled = false;
            return;
        }

        // Start with spikes down
        SetLocalY(downHeight);
        state = TrapState.Down;

        // Start rising after timeDown seconds
        Invoke(nameof(BeginRising), timeDown);
    }

    // Runs every frame
    void Update()
    {
        // Only move if spikes are Rising or Falling
        if (state == TrapState.Rising || state == TrapState.Falling)
        {
            // Increase lerp timer
            t += Time.deltaTime / moveDuration;

            // Calculate new Y position
            float newY = Mathf.Lerp(startY, targetY, t);
            SetLocalY(newY);

            // When movement completes
            if (t >= 1f)
            {
                // Snap exactly to target Y
                SetLocalY(targetY);

                if (state == TrapState.Rising)
                {
                    // Spikes reached top, wait Up duration
                    state = TrapState.Up;
                    Invoke(nameof(BeginFalling), timeUp);
                }
                else
                {
                    // Spikes reached bottom, wait Down duration
                    state = TrapState.Down;
                    Invoke(nameof(BeginRising), timeDown);
                }
            }
        }
    }

    // Start moving spikes up
    void BeginRising()
    {
        state = TrapState.Rising;
        t = 0f;
        startY = spikes.localPosition.y;
        targetY = upHeight;
    }

    // Start moving spikes down
    void BeginFalling()
    {
        state = TrapState.Falling;
        t = 0f;
        startY = spikes.localPosition.y;
        targetY = downHeight;
    }

    // Helper to set spikes Y position
    void SetLocalY(float y)
    {
        Vector3 p = spikes.localPosition;
        p.y = y;
        spikes.localPosition = p;
    }

    // Check if spikes are dangerous (up or moving up)
    public bool IsDangerous()
    {
        return state == TrapState.Up || state == TrapState.Rising;
    }
}