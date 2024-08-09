using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NPC : Enemy
{
    [HideInInspector]
    public Animator animator;

    NavMeshAgent myAgent;

    Player playerTarget;

    string nextState;

    string currentState;

    public GameObject[] idleTargets;

    public float idleTime;

    int target = 0;

    int newTarget = 0;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        myAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = "Idle";
        nextState = currentState;
        SwitchState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != nextState)
        {
            currentState = nextState;
        }
    }

    IEnumerator Idle()
    {
        yield return new WaitForSeconds(idleTime);
        nextState = "Walk";
        while(currentState != nextState)
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("isWalk",true);
        SwitchState(currentState);
    }

    IEnumerator Walk()
    {
        while(target == newTarget)
        {
            newTarget = Random.Range(0, idleTargets.Length);
        }
        target = newTarget;
        myAgent.SetDestination(idleTargets[target].transform.position);
        while (Vector3.Distance(gameObject.transform.position, idleTargets[target].transform.position) > 1)
        {
            yield return new WaitForEndOfFrame();
        }
        nextState = "Idle";
        while (currentState != nextState)
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("isWalk", false);
        SwitchState(currentState);
    }

}
