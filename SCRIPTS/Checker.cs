using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public delegate void CheckDelagate();

    public static event CheckDelagate checkerEvent = null;
    public static event CheckDelagate endCheckerEvent = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainWall"))
        {
            checkerEvent?.Invoke();
        }        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainWall"))
        {
             endCheckerEvent?.Invoke();
        }
        
    }

}
