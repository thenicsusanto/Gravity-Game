using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardToggle : MonoBehaviour
{
    public GameObject normalModePanel;   // Assign the NormalModePanel GameObject
    public GameObject endlessModePanel;  // Assign the EndlessModePanel GameObject
    public TextMeshProUGUI toggleButtonText;  // Assign the Text component of the ToggleButton

    private bool isShowingNormal = true;  // Tracks which leaderboard is currently visible

    void Start()
    {
        // Set the initial state
        ShowNormalMode();
    }

    // This function toggles between showing the normal and endless mode leaderboards
    public void ToggleLeaderboard()
    {
        if (isShowingNormal)
        {
            ShowEndlessMode();
        }
        else
        {
            ShowNormalMode();
        }
    }

    // Shows the normal mode leaderboard and hides the endless mode leaderboard
    private void ShowNormalMode()
    {
        normalModePanel.SetActive(true);
        endlessModePanel.SetActive(false);
        toggleButtonText.text = "Endless";  // Update button text
        isShowingNormal = true;
    }

    // Shows the endless mode leaderboard and hides the normal mode leaderboard
    private void ShowEndlessMode()
    {
        normalModePanel.SetActive(false);
        endlessModePanel.SetActive(true);
        toggleButtonText.text = "Normal";  // Update button text
        isShowingNormal = false;
    }
}
