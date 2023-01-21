using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame(string input)
    {
        ModeNameController.mode = input;
        FindObjectOfType<AudioManager>().Play("ButtonClick");
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("GameScene");
    }
}
