using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { Spawning, Waiting, Counting};
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float spawnRate;
    }

    public GameObject planetPrefab;
    public GameObject enemyPrefab;
    public Wave[] waves;
    private int nextWave = 0;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;
    private float searchCountdown = 1f;
    public TextMeshProUGUI waveCountdownText;

    private SpawnState state = SpawnState.Counting;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        if(state == SpawnState.Waiting)
        {
            if(EnemyIsAlive() == false)
            {
                StartCoroutine(WaveCompleted());
            } else
            {
                return;
            }

        }
        if(waveCountdown <= 0)
        {
            if(state != SpawnState.Spawning && state == SpawnState.Counting)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
                waveCountdownText.enabled = false;
            }
        } else
        {
            waveCountdown -= Time.deltaTime;
            waveCountdownText.text = "Wave spawning in " + (int)waveCountdown;
        }
    }

    IEnumerator WaveCompleted()
    {
        
        waveCountdownText.enabled = true;
        waveCountdownText.text = "Wave Completed!";
        yield return new WaitForSeconds(3.5f);
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;
        nextWave++;
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

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("Spawning wave: " + wave.name);
        state = SpawnState.Spawning;
        //Spawn
        for(int i=0; i<wave.count; i++)
        {
            SpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(1f / wave.spawnRate);
        }
        Debug.Log(wave.name);
        state = SpawnState.Waiting;
        yield break;
    }

    void SpawnEnemy(Transform enemy)
    {
        Debug.Log("Spawning enemy:" + enemy.name);
        Vector3 randomPoint = Random.onUnitSphere * 25;

        Transform obj = Instantiate(enemy, randomPoint, Quaternion.identity).transform;
        Vector3 gravityUp = (obj.position - transform.position).normalized;
        Vector3 localUp = obj.transform.up;
        obj.rotation = Quaternion.FromToRotation(localUp, gravityUp) * obj.rotation;
    }
}

