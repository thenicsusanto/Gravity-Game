using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { Spawning, Waiting, Counting, waveFinished, WonGame};

    [System.Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public int cost;
    }

    public List<Enemy> enemies = new List<Enemy>();
    public int currentWave;
    private float waveValue;
    public int spawnedEnemies;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();

    //public GameObject planetPrefab;
    //public GameObject meleeEnemy;
    //public GameObject rangedEnemy;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;
    private float searchCountdown = 1f;
    public float spawnRate;
    private float waveMultiplier;

    public TextMeshProUGUI waveCountdownText;
    public TextMeshProUGUI enemiesLeftText;
    public TextMeshProUGUI currentWaveText;

    public SpawnState state = SpawnState.Counting;

    public PlayerController playerController;

    public Button shopButton;
    public GameObject shop;

    public float timeElapsed;
    public GameHandler gameHandler;

    public string mode;

    void Awake()
    {
        //GameManager.Instance.swordContainer = GameObject.FindGameObjectWithTag("SwordContainer");
        FindObjectOfType<AudioManager>().Stop("MainMenuBackgroundMusic");
        FindObjectOfType<AudioManager>().Play("BackgroundMusic");
        
        waveCountdown = timeBetweenWaves;
    }

    private void Start()
    {
        mode = ModeNameController.mode;
        GameManager.Instance.enemiesAlive = 0;
        GameManager.Instance.coins = 0;

        if (mode == "Normal")
        {
            waveMultiplier = 4;
        } else if(mode == "Endless")
        {
            waveMultiplier = 2;
        }

        GameManager.Instance.swordContainer = GameObject.FindGameObjectWithTag("SwordContainer");
    }

    void Update()
    {
        if(state != SpawnState.WonGame || gameHandler.gameOver == false)
        {
            timeElapsed += Time.deltaTime;
        }

        if(state == SpawnState.Waiting || state == SpawnState.Spawning)
        {
            if(EnemyIsAlive() == false)
            {
                StartCoroutine(WaveCompleted());
            } else
            {
                enemiesLeftText.text = "Enemies Alive: " + GameManager.Instance.enemiesAlive.ToString();
                return;
            }
        }

        if(waveCountdown <= 0)
        {
            if(state == SpawnState.Counting)
            {
                waveCountdownText.enabled = false;
                currentWaveText.text = "Wave: " + currentWave.ToString();
                GenerateWave();
                StartCoroutine(SpawnWave());
            }
        } else {
            waveCountdown -= Time.deltaTime;
            waveCountdownText.text = "Wave spawning in " + (int)waveCountdown;
        }

        if(state == SpawnState.waveFinished)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                StopAllCoroutines();
                state = SpawnState.Counting;
                waveCountdown = timeBetweenWaves;
                currentWave++;              
            }
        }

        if(state != SpawnState.Counting)
        {
            shopButton.interactable = false;
        } else
        {
            shopButton.interactable = true;
        }

        if(shop.activeInHierarchy == true && state != SpawnState.Counting)
        {
            shop.SetActive(false);
        }
    }

    IEnumerator WaveCompleted()
    {
        Debug.Log(currentWave);
        if(mode == "Normal" && currentWave == 25)
        {
            state = SpawnState.WonGame;
            gameHandler.GameOverWin();
            yield return null;
        }
        waveCountdownText.enabled = true;
        waveCountdownText.text = "Wave Completed!";
        state = SpawnState.waveFinished;
        
        if(mode == "Endless")
        {
            playerController.maxHealthPlayer += 7;
            playerController.currentHealthPlayer += 7;
            playerController.GetComponentInChildren<SwordCollider>().damageToTake = 2 * currentWave;
        }
        
        yield return new WaitForSeconds(4f);
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;
        currentWave++;
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && GameManager.Instance.enemiesAlive == 0)
            {
                GameManager.Instance.enemiesAlive = 0;
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave()
    {
        state = SpawnState.Spawning;
        spawnRate = UnityEngine.Random.Range(0.4f, 2.5f);
        int enemyToSpawnCount = enemiesToSpawn.Count;
        //Spawn
        for(int i=0; i<enemyToSpawnCount; i++)
        {
            SpawnEnemy(enemiesToSpawn[0]); // spawn first enemy in our list
            enemiesToSpawn.RemoveAt(0); // and remove it
            GameManager.Instance.enemiesAlive++;
            yield return new WaitForSeconds(spawnRate);
        }
        state = SpawnState.Waiting;
        yield break;
    }

    void SpawnEnemy(GameObject enemy)
    {
        Vector3 randomPoint = Random.onUnitSphere * 11;

        Transform obj = Instantiate(enemy, randomPoint, Quaternion.identity).transform;
        if(obj.GetComponent<RangedEnemyController>() == true)
        {
            obj.GetComponent<RangedEnemyController>().maxHealthEnemy += (3 * currentWave);
            obj.GetComponent<RangedEnemyController>().currentHealthEnemy += (3 * currentWave);
            obj.GetComponent<RangedEnemyController>().damage += (2 * currentWave);
        } else
        {
            obj.GetComponent<MeleeEnemyController>().maxHealthEnemy += (3 * currentWave);
            obj.GetComponent<MeleeEnemyController>().currentHealthEnemy += (3 * currentWave);
            obj.GetComponent<MeleeEnemyController>().attackDamage += (2 * currentWave);
        }
        Vector3 gravityUp = (obj.position - transform.position).normalized;
        Vector3 localUp = obj.transform.up;
        obj.rotation = Quaternion.FromToRotation(localUp, gravityUp) * obj.rotation;
    }

    public void GenerateWave()
    {
        waveValue = waveMultiplier * currentWave;
        GenerateEnemies();
    }

    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate
        // 
        // in a loop grab a random enemy 
        // see if we can afford it
        // if we can, add it to our list, and deduct the cost.

        // repeat... 

        //  -> if we have no points left, leave the loop

        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0 || generatedEnemies.Count < 50)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].cost;

            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }
}

