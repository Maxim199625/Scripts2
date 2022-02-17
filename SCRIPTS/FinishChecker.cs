using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishChecker : MonoBehaviour
{
    private LevelController levelCont;
    public static int countWallSpawns;

    private void Awake()
    {
        levelCont = FindObjectOfType<LevelController>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainWall") && !PlayerBox.isGameOver)
        {
            countWallSpawns--;
            if(countWallSpawns > 0)
            {
                levelCont.Spawn();
            }
            else
            {
                LevelController.FinishLevel();
            }
            
        }
    }
}
