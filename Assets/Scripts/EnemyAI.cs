using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    Transform playerTarget;

    NavMeshAgent myAgent;

    // Start is called before the first frame update
    void Start()
    {
        playerTarget = GameManager.instance.thePlayer.transform;
        myAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        myAgent.SetDestination(playerTarget.position);
    }
}
