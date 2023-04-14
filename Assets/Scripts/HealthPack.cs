using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float speed;
    public PlayerController player;
    public WaveSpawner waveSpawner;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition += transform.up * 2;
        player = FindObjectOfType<PlayerController>();
        waveSpawner = FindObjectOfType<WaveSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (waveSpawner.currentWave > 25)
            {
                player.GainHealthPlayer(50 + (5 * waveSpawner.currentWave));
                AudioManager.instance.Play("HealthRegen");
            }
            else
            {
                player.GainHealthPlayer(50 + (3 * waveSpawner.currentWave));
                AudioManager.instance.Play("HealthRegen");
            }
            Destroy(gameObject);
        }
    }
}
