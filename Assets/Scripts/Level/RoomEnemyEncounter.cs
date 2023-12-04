using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemyEncounter : EntityTrigger
{
    [SerializeField] EnemySpawnPoint[] spawnPoints;
    [SerializeField] int[] enemiesPerWave;

    private bool beenEnabled = false;
    private int killedEnemiesCounter = 0;
    private int previousWavesEnemyCount = 0;

    private int currentWaveIndex = 0;


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
        if(currentWaveIndex + 1 >= enemiesPerWave.Length) Destroy(gameObject);
    }

    protected override void OnPlayerEnter(Player _player)
    {
        if (beenEnabled) return;
        for (int i = 0; i < enemiesPerWave[0]; i += 1)
        {
            int spawnPointNum = Random.Range(0, spawnPoints.Length);
            GameObject enemy = spawnPoints[spawnPointNum].Spawn();
            enemy.GetComponent<Health>().OnHealthEnded.AddListener(OnEnemyDeath);
        }

        beenEnabled = true;

        print("я жив");
    }

    void OnEnemyDeath()
    {
        killedEnemiesCounter += 1;
        print("убито" + killedEnemiesCounter);

    }
}
