using UnityEngine;
using TMPro;
using System.Collections;

// This script manages UI messages like start text and win text
public class UIManager : MonoBehaviour
{
    [Header("UI")]

    // Text shown at level start
    public TMP_Text startText;

    // Text shown when player wins
    public TMP_Text winText;

    [Header("Start Message")]

    // How long start text is visible
    public float startTextDuration = 3f;

    // Runs when scene starts
    private void Start()
    {
        // Hide win text at start
        if (winText != null) 
            winText.gameObject.SetActive(false);

        // Show start text at beginning
        if (startText != null)
        {
            startText.gameObject.SetActive(true);

            // Hide start text after delay
            StartCoroutine(HideStartTextAfterDelay());
        }
    }

    // Coroutine to hide start text after set time
    IEnumerator HideStartTextAfterDelay()
    {
        // Wait for duration
        yield return new WaitForSeconds(startTextDuration);

        // Disable start text
        startText.gameObject.SetActive(false);
    }

    // Show win text when player wins
    public void ShowWin()
    {
        if (winText != null) 
            winText.gameObject.SetActive(true);
    }
}