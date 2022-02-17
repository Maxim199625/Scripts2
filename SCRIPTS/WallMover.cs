using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour
{
    private float speed;
    private void Awake()
    {
        LevelController.spawnEvent += SetValues;
        Invoke("DestroySelf", 8f);
    }
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.back);
    }
    private void SetValues()
    {
        if(LevelController.currentWall == 1)
        {
            speed = LevelController.currentSpeed / 1.3f;
        }
        else
           speed = LevelController.currentSpeed;
    }
    private void OnDestroy()
    {
        LevelController.spawnEvent -= SetValues;
    }
}
