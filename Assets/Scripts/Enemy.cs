using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    Transform playerTarget;

    NavMeshAgent myAgent;

    string currentState;

    string nextState;

    bool detected;

    // Start is called before the first frame update
    void Start()
    {
        playerTarget = GameManager.instance.thePlayer.transform;
        myAgent = gameObject.GetComponent<NavMeshAgent>();
        currentState = "Idle";
        nextState = currentState;
        SwitchState();

    }

    // Update is called once per frame
    void Update()
    {
        if(currentState != nextState)
        {
            currentState = nextState;
        }
    }
    void SwitchState()
    {
        StartCoroutine(currentState);
    }

    public void UpdateTarget(Transform newTarget)
    {
        if(newTarget == null)
        {
            nextState = "Idle";

        }
        else
        {
            nextState = "Chasing";
        }
        playerTarget = newTarget;
    }

    IEnumerator Idle()
    {
        while(currentState == "Idle")
        {
            yield return new WaitForEndOfFrame();
        }
        SwitchState();
    }

    IEnumerator Chasing()
    {
        while(currentState == "Chasing")
        {
            yield return new WaitForEndOfFrame();
            if(playerTarget != null)
            {
                myAgent.SetDestination(playerTarget.position);
            }
        }
        SwitchState();
    }

}
