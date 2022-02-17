using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBox : MonoBehaviour
{
    public static bool isGameOver;
    public delegate void GameOver();
    public static event GameOver gameOverEvent;

    private void Awake()
    {
        isGameOver = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") && !isGameOver)
        {
            isGameOver = true;
            gameOverEvent?.Invoke();
        }
    }
}
