using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    const int OBSERV_RATE = 3;

    [Header("General Setting")]
    [SerializeField]
    Transform spawnTarget;

    [SerializeField]
    Transform avoider;

    [SerializeField]
    float spawnDelay = 2.0f;

    [SerializeField]
    Vector3 spawnOffset = new Vector3(0.0f, 0.5f, 0.0f);

    bool currentTargetStatus = false;
    bool previousTargetStatus = false;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        if (Time.frameCount % OBSERV_RATE == 0) {
            SpawnHandler();
        }
    }

    void Initialize()
    {
        currentTargetStatus = spawnTarget.gameObject.activeSelf;
        previousTargetStatus = currentTargetStatus;

        bool isNoSpawnPoint = (transform.childCount <= 0);

        if (isNoSpawnPoint) {
            Debug.LogError("No spawn point found in the child of this spawner.");
        }
    }

    void SpawnHandler()
    {
        previousTargetStatus = currentTargetStatus;
        currentTargetStatus = spawnTarget.gameObject.activeSelf;

        bool shouldStartRespawn = !currentTargetStatus && (currentTargetStatus != previousTargetStatus);

        if (shouldStartRespawn) {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnDelay);

        float distance = float.MinValue;
        Transform pickSpawnPoint = null;

        foreach (Transform spawnPoint in transform) {
            var currentDistance = (spawnPoint.position - avoider.position).sqrMagnitude;

            if (currentDistance > distance) {
                distance = currentDistance;
                pickSpawnPoint = spawnPoint;
            }
        }

        spawnTarget.position = (pickSpawnPoint.position + spawnOffset);
        spawnTarget.gameObject.SetActive(true);
    }
}

