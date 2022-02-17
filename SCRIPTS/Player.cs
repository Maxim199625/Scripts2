using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Transform[] childs;
    [HideInInspector] public int maxX, maxY;

    private void Awake()
    {
        childs = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            
            maxX = Mathf.Max(Mathf.RoundToInt(childs[i].localPosition.x), maxX);
            maxY = Mathf.Max(Mathf.RoundToInt(childs[i].localPosition.y), maxY);
        }
    }


}
