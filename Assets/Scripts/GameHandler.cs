using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public bool gameOver;
    public Transform gameOverPanel;
    public void GameOver()
    {
        gameOver = true;
        gameOverPanel.gameObject.SetActive(transform);
        Time.timeScale = 0f;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;

    }

    public void ReloadScene()
    {
        int buildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex);
        Time.timeScale = 1f;
    }
}
