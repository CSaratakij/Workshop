using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float followRate = 1.0f;

    Vector3 offset;

    void Awake()
    {
        Initialize();
    }

    void LateUpdate()
    {
        FollowTarget();
    }

    void Initialize()
    {
        if (target == null) {
            Debug.LogError("Cannot find the target to follow...");
        }

        offset = transform.position - target.position;
    }

    void FollowTarget()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followRate);
    }
}

