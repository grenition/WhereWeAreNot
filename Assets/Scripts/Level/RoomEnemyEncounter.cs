using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomEnemyEncounter : EntityTrigger
{
    [SerializeField] EnemySpawnPoint[] spawnPoints;
    [SerializeField] int[] enemiesPerWave;

    private bool beenEnabled = false;
    private int killedEnemiesCounter = 0;
    private int previousWavesEnemyCount = 0;

    private int currentWaveIndex = 0;

    public UnityEvent onEncounterStart;
    public UnityEvent onEncounterEnd;


    void FixedUpdate()
    {
        if (beenEnabled)
        {
            if (killedEnemiesCounter >= enemiesPerWave[currentWaveIndex] + previousWavesEnemyCount)
            {
                previousWavesEnemyCount += enemiesPerWave[currentWaveIndex];
                currentWaveIndex += 1;
                for (int i = 0; i < enemiesPerWave[currentWaveIndex]; i += 1)
                {
                    int spawnPointNum = Random.Range(0, spawnPoints.Length);
                    GameObject enemy = spawnPoints[spawnPointNum].Spawn();
                    enemy.GetComponent<Health>().OnHealthEnded.AddListener(OnEnemyDeath);
                }
            }
        }
        if (currentWaveIndex + 1 >= enemiesPerWave.Length)
        {
            onEncounterEnd.Invoke();
            Destroy(gameObject);
        }
    }

    protected override void OnPlayerEnter(Player _player)
    {
        if (beenEnabled) return;
        onEncounterStart.Invoke();
        for (int i = 0; i < enemiesPerWave[0]; i += 1)
        {
            int spawnPointNum = Random.Range(0, spawnPoints.Length);
            GameObject enemy = spawnPoints[spawnPointNum].Spawn();
            enemy.GetComponent<Health>().OnHealthEnded.AddListener(OnEnemyDeath);
        }

        beenEnabled = true;

    }

    void OnEnemyDeath()
    {
        killedEnemiesCounter += 1;
        print("убито" + killedEnemiesCounter);
    }
}
