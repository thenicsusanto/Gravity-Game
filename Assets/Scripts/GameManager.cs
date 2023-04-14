using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int enemiesAlive;
    public int coins;
    public GameObject swordContainer;
    public string mode;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
