using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { Spawning, Waiting, Counting, waveFinished};

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

    public GameObject planetPrefab;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;
    private float searchCountdown = 1f;
    public float spawnRate;

    public TextMeshProUGUI waveCountdownText;
    public TextMeshProUGUI enemiesLeftText;

    public SpawnState state = SpawnState.Counting;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
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
                GenerateWave();
                StartCoroutine(SpawnWave());
            }
        } else {
            waveCountdown -= Time.deltaTime;
            waveCountdownText.text = "Wave spawning in " + (int)waveCountdown;
        }
    }

    IEnumerator WaveCompleted()
    {
        waveCountdownText.enabled = true;
        waveCountdownText.text = "Wave Completed!";
        state = SpawnState.waveFinished;
        yield return new WaitForSeconds(3.5f);
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
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave()
    {
        state = SpawnState.Spawning;
        spawnRate = UnityEngine.Random.Range(0.5f, 2.5f);
        int enemyToSpawnCount = enemiesToSpawn.Count;
        Debug.Log(enemyToSpawnCount);
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
        Debug.Log("Spawning enemy: " + enemy.name);
        Vector3 randomPoint = Random.onUnitSphere * 25;

        Transform obj = Instantiate(enemy, randomPoint, Quaternion.identity).transform;
        Vector3 gravityUp = (obj.position - transform.position).normalized;
        Vector3 localUp = obj.transform.up;
        obj.rotation = Quaternion.FromToRotation(localUp, gravityUp) * obj.rotation;
    }

    public void GenerateWave()
    {
        waveValue = Mathf.Round(50/(1 + 20 * Mathf.Pow(2.718f, (float)(-0.30*currentWave))));
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

