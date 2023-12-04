using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnController : MonoBehaviour
{
    public static RespawnController Instance { get; private set; }
    public static Vector3 SpawnPoint
    {
        get
        {
            if (Instance != null)
                return Instance.spawnPoint;
            return Vector3.zero;
        }
        set
        {
            if (Instance != null)
                Instance.spawnPoint = value;
        }
    }

    [SerializeField] private Transform startPlayerRespawnPoint;
    private Vector3 spawnPoint = Vector3.zero;
    private int attachedSceneIndex = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (startPlayerRespawnPoint != null)
                spawnPoint = startPlayerRespawnPoint.position;
            DontDestroyOnLoad(gameObject);
            attachedSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }
        else if (this != Instance)
        {
            print("destroying controller");
            Destroy(gameObject);
            return;
        }
    }

    private void SceneManager_activeSceneChanged(Scene lastScene, Scene newScene)
    {
        if (this != Instance)
            return;

        if (attachedSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            Destroy(gameObject);
            return;
        }

        Player.Instance.Movement.Teleport(spawnPoint, Quaternion.identity);
    }

    public static void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
