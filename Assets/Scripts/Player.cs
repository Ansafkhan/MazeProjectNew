using UnityEngine; // Unity core engine
using TMPro; // TextMeshPro for UI text
using UnityEngine.UI; // UI components like Button, Image

public class Player : MonoBehaviour
{
    [Header("References")]
    public Transform trans; // Player's main transform (position, rotation)
    public Transform modelTrans; // Transform of the 3D model (for rotation)
    public CharacterController characterController; // Built-in movement controller

    public Transform cameraPivot; // Pivot for camera rotation
    public GameObject cam; // Main camera reference

    [Header("Movement")]
    public float moveSpeed = 24f; // Normal move speed
    public float timeToMaxSpeed = 0.26f; // Time to reach full speed
    public float timeToLoseMaxSpeed = 0.2f; // Time to slow down
    public float reverseMomentumLossMultiplier = 1.5f; // Slowdown when reversing

    public float sprintMultiplier = 13f; // Sprint speed multiplier
    private float baseMoveSpeed; // Store normal speed

    private Vector3 movementVelocity = Vector3.zero; // Player velocity vector

    [Header("Camera Turning")]
    public float cameraTurnSpeed = 120f; // Camera rotation speed
    private float cameraYaw; // Yaw value for camera rotation

    [Header("Death and Respawning")]
    public float respawnWaitTime = 2f; // Wait time before respawn
    public Transform spawnPointTransform; // Spawn point reference
    public float respawnInputLockTime = 0.15f; // Lock input after respawn

    private float inputLockTimer = 0f; // Timer to prevent input
    private bool dead = false; // Player death state
    private bool won = false; // Win state

    private Vector3 spawnPoint; // Spawn position
    private Quaternion spawnRotation; // Spawn rotation
    private Quaternion modelSpawnLocalRotation; // Model local rotation at spawn
    private Quaternion cameraPivotSpawnLocalRotation; // Camera local rotation at spawn

    [Header("UI")]
    public GameObject winTextObject; // Win message object
    public GameObject startTextObject; // Start game message
    private bool hasStartedMoving = false; // Check if player started moving

    // Reset function runs in editor when adding component or resetting
    private void Reset()
    {
        trans = transform; // Assign current transform

        // Try to find child named "Model"
        if (modelTrans == null)
        {
            Transform model = transform.Find("Model"); 
            if (model != null) modelTrans = model; // Assign model transform
        }

        // Get CharacterController component if not assigned
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        // Find camera pivot if not assigned
        if (cameraPivot == null)
        {
            Transform pivot = transform.Find("CameraPivot");
            if (pivot != null) cameraPivot = pivot;
        }

        // Find camera in children
        if (cam == null)
        {
            var camComp = GetComponentInChildren<Camera>(true);
            if (camComp != null) cam = camComp.gameObject;
        }
    }

    // Start is called before first frame update
    private void Start()
    {
        if (trans == null) trans = transform; // Assign transform if missing
        if (characterController == null) characterController = GetComponent<CharacterController>();

        if (cameraPivot == null)
        {
            Transform pivot = transform.Find("CameraPivot");
            if (pivot != null) cameraPivot = pivot;
        }

        if (cam == null)
        {
            var camComp = GetComponentInChildren<Camera>(true); // Get camera in children
            if (camComp != null) cam = camComp.gameObject;
        }

        // Setup spawn point
        if (spawnPointTransform != null)
        {
            spawnPoint = spawnPointTransform.position;
            spawnRotation = spawnPointTransform.rotation;
        }
        else
        {
            spawnPoint = trans.position;
            spawnRotation = trans.rotation;
        }

        if (modelTrans != null)
            modelSpawnLocalRotation = modelTrans.localRotation; // Store model rotation

        if (cameraPivot != null)
        {
            cameraPivotSpawnLocalRotation = cameraPivot.localRotation; // Store camera rotation
            cameraYaw = cameraPivot.localEulerAngles.y; // Store yaw
        }

        // Activate start text UI
        if (startTextObject != null)
            startTextObject.SetActive(true);

        // Deactivate win text at start
        if (winTextObject != null)
            winTextObject.SetActive(false);

        Time.timeScale = 1f; // Make sure game runs normally

        baseMoveSpeed = moveSpeed; // Store base speed for sprint
    }

    // Update is called once per frame
    private void Update()
    {
        // Check for Escape key to open pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LevelSelectUI menu = FindFirstObjectByType<LevelSelectUI>();
            bool isPaused = Time.timeScale == 0f;

            if (isPaused) // Resume game if already paused
            {
                if (menu != null) menu.ResumeGame();
                else Time.timeScale = 1f;
            }
            else // Pause game
            {
                if (menu != null) menu.ShowPauseMenu();
                else Time.timeScale = 0f;
            }
        }

        if (Time.timeScale == 0f) return; // Stop updating if game paused

        // Decrease input lock timer
        if (inputLockTimer > 0f)
            inputLockTimer -= Time.deltaTime;

        // Call movement function if player can move
        if (!dead && !won && inputLockTimer <= 0f)
            Movement();
    }

    // Function to handle movement
    private void Movement()
    {
        float currentSpeed = baseMoveSpeed; // Default speed
        if (Input.GetKey(KeyCode.Space)) // Sprint check
            currentSpeed *= sprintMultiplier;

        float velocityGainPerSecond = currentSpeed / timeToMaxSpeed;
        float velocityLossPerSecond = currentSpeed / timeToLoseMaxSpeed;

        // Z-axis movement input (forward/back)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (movementVelocity.z >= 0)
                movementVelocity.z = Mathf.Min(currentSpeed, movementVelocity.z + velocityGainPerSecond * Time.deltaTime);
            else
                movementVelocity.z = Mathf.Min(0, movementVelocity.z + velocityGainPerSecond * reverseMomentumLossMultiplier * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (movementVelocity.z > 0)
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - velocityGainPerSecond * reverseMomentumLossMultiplier * Time.deltaTime);
            else
                movementVelocity.z = Mathf.Max(-currentSpeed, movementVelocity.z - velocityGainPerSecond * Time.deltaTime);
        }
        else
        {
            // Gradually stop if no input
            if (movementVelocity.z > 0)
                movementVelocity.z = Mathf.Max(0, movementVelocity.z - velocityLossPerSecond * Time.deltaTime);
            else
                movementVelocity.z = Mathf.Min(0, movementVelocity.z + velocityLossPerSecond * Time.deltaTime);
        }

        // X-axis movement input (left/right)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (movementVelocity.x >= 0)
                movementVelocity.x = Mathf.Min(currentSpeed, movementVelocity.x + velocityGainPerSecond * Time.deltaTime);
            else
                movementVelocity.x = Mathf.Min(0, movementVelocity.x + velocityGainPerSecond * reverseMomentumLossMultiplier * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (movementVelocity.x > 0)
                movementVelocity.x = Mathf.Max(0, movementVelocity.x - velocityGainPerSecond * reverseMomentumLossMultiplier * Time.deltaTime);
            else
                movementVelocity.x = Mathf.Max(-currentSpeed, movementVelocity.x - velocityGainPerSecond * Time.deltaTime);
        }
        else
        {
            // Gradually stop if no input
            if (movementVelocity.x > 0)
                movementVelocity.x = Mathf.Max(0, movementVelocity.x - velocityLossPerSecond * Time.deltaTime);
            else
                movementVelocity.x = Mathf.Min(0, movementVelocity.x + velocityLossPerSecond * Time.deltaTime);
        }

        // Rotate camera according to movement
        if (cameraPivot != null)
        {
            float turnInputNormalized = 0f;
            if (currentSpeed > 0.001f)
                turnInputNormalized = movementVelocity.x / currentSpeed;

            cameraYaw += turnInputNormalized * cameraTurnSpeed * Time.deltaTime; // Update yaw
            cameraPivot.localRotation = Quaternion.Euler(0f, cameraYaw, 0f); // Apply rotation
        }

        // Stop if no movement
        if (characterController == null) return;
        if (movementVelocity.x == 0f && movementVelocity.z == 0f) return;

        // Camera-relative movement
        Vector3 moveWorld;
        if (cameraPivot != null)
        {
            Vector3 camForward = cameraPivot.forward;
            Vector3 camRight = cameraPivot.right;
            camForward.y = 0f; // Ignore vertical rotation
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            moveWorld = (camRight * movementVelocity.x) + (camForward * movementVelocity.z);
        }
        else
        {
            moveWorld = new Vector3(movementVelocity.x, 0f, movementVelocity.z);
        }

        characterController.Move(moveWorld * Time.deltaTime); // Move character

        // Rotate model to face movement
        if (modelTrans != null && moveWorld.sqrMagnitude > 0.0001f)
        {
            Quaternion targetModelRotation = Quaternion.LookRotation(moveWorld);
            modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, targetModelRotation, 0.18f); // Smooth rotation
        }

        // Hide start text once player moves
        if (!hasStartedMoving && (Mathf.Abs(movementVelocity.x) > 0.01f || Mathf.Abs(movementVelocity.z) > 0.01f))
        {
            hasStartedMoving = true;
            if (startTextObject != null)
                startTextObject.SetActive(false);
        }
    }

    // Player death
    public void Die()
    {
        if (dead) return; // Already dead
        dead = true;
        movementVelocity = Vector3.zero; // Stop movement
        if (characterController != null)
            characterController.enabled = false; // Disable controller
        if (modelTrans != null)
            modelTrans.gameObject.SetActive(false); // Hide model
        Invoke(nameof(Respawn), respawnWaitTime); // Call Respawn after wait
    }

    // Respawn player
    public void Respawn()
    {
        if (trans == null) trans = transform;
        dead = true;
        if (characterController != null) characterController.enabled = false;
        movementVelocity = Vector3.zero;

        if (spawnPointTransform != null)
        {
            spawnPoint = spawnPointTransform.position;
            spawnRotation = spawnPointTransform.rotation;
        }

        trans.SetPositionAndRotation(spawnPoint, spawnRotation);

        if (modelTrans != null)
        {
            modelTrans.gameObject.SetActive(true);
            modelTrans.localRotation = modelSpawnLocalRotation; // Reset rotation
        }

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = cameraPivotSpawnLocalRotation; // Reset camera
            cameraYaw = cameraPivot.localEulerAngles.y;
        }

        inputLockTimer = respawnInputLockTime; // Lock input briefly

        if (characterController != null)
            characterController.enabled = true;

        dead = false;
    }

    // Collision handling
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Hazard")) Die(); // Hit hazard
        if (hit.collider.CompareTag("Goal")) Win(); // Reached goal
    }

    // Win function
    public void Win()
    {
        if (won) return; // Already won
        won = true;
        movementVelocity = Vector3.zero;
        Time.timeScale = 1f; // Make sure game is running
        if (winTextObject != null)
            winTextObject.SetActive(true); // Show win text
        Debug.Log("You Win!"); // Debug message
    }
}