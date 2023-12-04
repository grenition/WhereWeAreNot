using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnTrigger : EntityTrigger
{
    [SerializeField] bool enableOnce = true;
    [SerializeField] EnemySpawnPoint[] spawnPoints;



    protected override void OnPlayerEnter(Player _player)
    {
        
        foreach (EnemySpawnPoint enemySpawnPoint in spawnPoints)
        {
            enemySpawnPoint.Spawn();
        }

        if(enableOnce) Destroy(gameObject);
    }
}
