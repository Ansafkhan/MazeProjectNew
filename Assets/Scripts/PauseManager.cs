using UnityEngine;

// This script is used to pause and resume the game
public class PauseManager : MonoBehaviour
{
    // This is the UI panel that appears when the game is paused
    public GameObject pausePanel;

    // This function is called when we want to pause the game
    public void PauseGame()
    {
        // Setting timeScale to 0 will stop the game
        Time.timeScale = 0f;

        // This will show the pause panel on the screen
        pausePanel.SetActive(true);
    }

    // This function is called when we want to resume the game
    public void ResumeGame()
    {
        // Setting timeScale back to 1 will start the game again
        Time.timeScale = 1f;

        // This will hide the pause panel
        pausePanel.SetActive(false);
    }
}