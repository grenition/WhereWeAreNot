using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController
{
    public static void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public static void LoadGame()
    {
        SceneManager.LoadScene("Prototyphing 1");
    }
}
