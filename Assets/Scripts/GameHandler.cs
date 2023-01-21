using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameHandler : MonoBehaviour
{
    public bool gameOver;
    public Transform gameOverPanel;
    public Transform gameWinPanel;
    public TextMeshProUGUI timeElapsedTextWin;
    public TextMeshProUGUI totalCoinsTextWin;
    public TextMeshProUGUI timeElapsedTextLoss;
    public TextMeshProUGUI totalCoinsTextLoss;
    public void GameOverLoss()
    {
        gameOver = true;
        gameOverPanel.gameObject.SetActive(transform);
        DisplayTime(FindObjectOfType<WaveSpawner>().timeElapsed);
        totalCoinsTextLoss.text = "Coins: " + GameManager.Instance.coins;
        Time.timeScale = 0f;
    }

    public void GameOverWin()
    {
        gameOver = true;
        gameWinPanel.gameObject.SetActive(transform);
        DisplayTime(FindObjectOfType<WaveSpawner>().timeElapsed);
        totalCoinsTextWin.text = "Coins: " + GameManager.Instance.coins;
        Time.timeScale = 0f;
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeElapsedTextLoss.text = "Time Elasped: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        timeElapsedTextWin.text = "Time Elasped: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        FindObjectOfType<AudioManager>().Play("MainMenuBackgroundMusic");
    }

    public void ReloadGame(string input)
    {
        
        ModeNameController.mode = input;
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
        FindObjectOfType<AudioManager>().Play("ButtonClick");
        Debug.Log("Scene loaded");
    }
}
