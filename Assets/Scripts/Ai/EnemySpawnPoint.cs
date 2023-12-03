using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject enemyObject;
    [SerializeField] GameObject childrenModel;


    void Awake()
    {
        childrenModel.SetActive(false);
    }

    public void Spawn()
    {
        childrenModel.SetActive(true);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        GameObject pr = Instantiate(enemyObject, pos, Quaternion.identity);
    }
}
