using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    Transform target;

    NavMeshAgent agent;

    void Awake()
    {
        Initialize();
    }
    
    void Update()
    {
        MoveHandler();
    }

    void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void MoveHandler()
    {
        float distanceSqr = (target.position - transform.position).sqrMagnitude;
        agent.SetDestination(target.position);
    }
}

