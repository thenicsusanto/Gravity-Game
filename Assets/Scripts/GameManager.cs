using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int enemiesAlive;
    public int coins;
    public GameObject swordContainer;

    private void Awake()
    {
        swordContainer = GameObject.FindGameObjectWithTag("SwordContainer");
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
}
