using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    float roateSpeed = 4.0f;

    [SerializeField]
    Vector3 rotateAxis = Vector3.up;

    void Update()
    {
        RotateHandler();
    }

    void RotateHandler()
    {
        transform.RotateAround(rotateAxis, roateSpeed * Time.deltaTime);
    }
}

