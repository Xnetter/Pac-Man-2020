using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameOver : MonoBehaviour
{
    float gameOverTimer;
    private static float gameOverTimerLimit = 100;
     
    void Start () 
    {
        gameOverTimer = gameOverTimerLimit;
    }
 
    void CheckTimer ()
    {
        gameOverTimer -= 1;
        if (gameOverTimer == 0)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    void FixedUpdate ()
    {
        CheckTimer();
    }
}
