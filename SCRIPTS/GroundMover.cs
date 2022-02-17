using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMover : MonoBehaviour
{
    public static float speed = 1.2f;

    void FixedUpdate()
    {
        transform.Translate(Vector3.back * speed);
    }
}
