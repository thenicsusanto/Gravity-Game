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
        StartCoroutine(DelaySceneLoad());
    }

    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("GameScene");
    }
}
