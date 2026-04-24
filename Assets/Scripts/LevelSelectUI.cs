using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    // This static variable makes sure only one object exists
    private static LevelSelectUI instance;

    // This stores which scene is currently active
    // 0 means Level 1
    // 1 means Level 2
    private int currentScene = 0;

    // This stores the preview camera object
    private GameObject levelViewCamera;

    // This is used for loading scene in background
    private AsyncOperation loadOp;

    // This bool decides if menu UI should be hidden or shown
    private bool hideUI = false;

    void Start()
    {
        // If another object already exists, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Assign this object as instance
        instance = this;

        // This makes object stay when changing scenes
        DontDestroyOnLoad(gameObject);

        // Find preview camera when game starts
        FindLevelViewCamera();
    }

    void Update()
    {
        // Check if scene loading is finished
        if (loadOp != null && loadOp.isDone)
        {
            // Clear loading variable
            loadOp = null;

            // After loading new scene, find camera again
            FindLevelViewCamera();
        }
    }

    // This function finds a GameObject named "Level View Camera"
    private void FindLevelViewCamera()
    {
        levelViewCamera = GameObject.Find("Level View Camera");

        // If camera not found, show error
        if (levelViewCamera == null)
            Debug.LogError("No GameObject named 'Level View Camera' found in this scene!");
        else
            // If found, activate it
            levelViewCamera.SetActive(true);
    }

    // This function checks if scene is currently loading
    private bool IsLoading() => loadOp != null && !loadOp.isDone;

    // This function loads scene using build index
    private void LoadScene(int buildIndex)
    {
        // If already loading, do nothing
        if (IsLoading()) return;

        // Make sure game is not paused
        Time.timeScale = 1f;

        // Show menu while loading
        hideUI = false;

        // Store which scene we are loading
        currentScene = buildIndex;

        // Load scene in background
        loadOp = SceneManager.LoadSceneAsync(buildIndex);
    }

    // This function starts the selected level
    private void PlayCurrentLevel()
    {
        // If loading, don't allow play
        if (IsLoading()) return;

        // Always unpause game before playing
        Time.timeScale = 1f;

        // Turn off preview camera
        if (levelViewCamera != null)
        {
            levelViewCamera.SetActive(false);

            var previewCam = levelViewCamera.GetComponent<Camera>();
            if (previewCam != null)
                previewCam.enabled = false;
        }

        // Find Player object in scene
        GameObject playerObj = GameObject.Find("Player");

        // If player not found, show error
        if (playerObj == null)
        {
            Debug.LogError("Couldn't find GameObject named 'Player'.");
            return;
        }

        // Get Player script from player object
        Player p = playerObj.GetComponent<Player>();

        // If script missing, show error
        if (p == null)
        {
            Debug.LogError("Player object does not have Player.cs attached.");
            return;
        }

        // Enable player movement script
        p.enabled = true;

        // Check if gameplay camera is assigned
        if (p.cam == null)
        {
            Debug.LogError("Player.cam is not assigned.");
            return;
        }

        // Find all cameras in scene
        Camera[] cams = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        // Disable all cameras first
        foreach (Camera c in cams)
            c.gameObject.SetActive(false);

        // Enable player camera
        p.cam.SetActive(true);

        // Get camera component
        Camera playerCam = p.cam.GetComponent<Camera>();

        // If camera component not found, search inside children
        if (playerCam == null)
            playerCam = p.cam.GetComponentInChildren<Camera>(true);

        // If camera found, enable it
        if (playerCam != null)
        {
            playerCam.enabled = true;

            // Set this camera as MainCamera
            playerCam.tag = "MainCamera";
        }

        // Hide menu when gameplay starts
        hideUI = true;
    }

    // This function shows pause menu
    public void ShowPauseMenu()
    {
        // Show menu UI
        hideUI = false;

        // Stop game time
        Time.timeScale = 0f;
    }

    // This function resumes the game
    public void ResumeGame()
    {
        // Start game time again
        Time.timeScale = 1f;

        // Hide menu UI
        hideUI = true;
    }

    // This function draws GUI on screen
    void OnGUI()
    {
        // If game is paused, always show pause menu
        if (Time.timeScale == 0f)
        {
            DrawPauseMenu();
            return;
        }

        // If gameplay is running, hide menu
        if (hideUI) return;

        // If loading, show loading message
        if (IsLoading())
        {
            GUI.Box(new Rect(20, 20, 260, 60), "");
            GUI.Label(new Rect(35, 40, 240, 30), "Loading... please wait");
            return;
        }

        // Otherwise show main menu
        DrawMainMenu();
    }

    // This function draws main menu buttons
    private void DrawMainMenu()
    {
        float panelW = 320f;
        float panelH = 260f;
        float x = 20f;
        float y = 20f;

        GUI.Box(new Rect(x, y, panelW, panelH), "");
        GUILayout.BeginArea(new Rect(x + 12, y + 12, panelW - 24, panelH - 24));

        // Game title
        GUILayout.Label("3D MAZE GAME");
        GUILayout.Space(10);

        // If currently in level 2 preview
        if (currentScene == 1)
        {
            GUILayout.Label("Previewing Level 2");
            GUILayout.Space(10);

            // Button to play level 2
            if (GUILayout.Button("PLAY LEVEL 2", GUILayout.Height(40)))
                PlayCurrentLevel();

            GUILayout.Space(10);

            // Button to go back to level 1
            if (GUILayout.Button("BACK TO LEVEL 1 MENU", GUILayout.Height(40)))
                LoadScene(0);

            GUILayout.EndArea();
            return;
        }

        GUILayout.Label("Select a level:");
        GUILayout.Space(10);

        // Button to play level 1
        if (GUILayout.Button("PLAY LEVEL 1", GUILayout.Height(40)))
            PlayCurrentLevel();

        GUILayout.Space(10);

        // Button to preview level 2
        if (GUILayout.Button("PREVIEW LEVEL 2", GUILayout.Height(40)))
            LoadScene(1);

        GUILayout.EndArea();
    }

    // This function draws pause menu
    private void DrawPauseMenu()
    {
        float panelW = 320f;
        float panelH = 220f;
        float x = 20f;
        float y = 20f;

        GUI.Box(new Rect(x, y, panelW, panelH), "");
        GUILayout.BeginArea(new Rect(x + 12, y + 12, panelW - 24, panelH - 24));

        GUILayout.Label("PAUSED");
        GUILayout.Space(10);

        // Resume button
        if (GUILayout.Button("RESUME", GUILayout.Height(40)))
            ResumeGame();

        GUILayout.Space(10);

        // Go back to level 1 menu
        if (GUILayout.Button("BACK TO LEVEL 1 MENU", GUILayout.Height(40)))
        {
            Time.timeScale = 1f;
            hideUI = false;
            LoadScene(0);
        }

        GUILayout.EndArea();
    }

    // This function hides the menu manually
    public void HideMenuUI()
    {
        hideUI = true;
    }
}