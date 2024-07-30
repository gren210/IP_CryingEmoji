using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM1 : MonoBehaviour
{
    Transform playerTarget;

    public Animator animator;

    NavMeshAgent myAgent;

    string currentState;

    string nextState;

    public bool detected;

    public float attackRange;

    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        playerTarget = GameManager.instance.thePlayer.transform;
        animator = gameObject.GetComponent<Animator>();
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
        if (detected && Vector3.Distance(playerTarget.position, gameObject.transform.position) <= attackRange)
        {
            nextState = "Attack";
            animator.SetBool("Attack", true);
        }
        else if (detected)
        {
            nextState = "Chasing";
            animator.SetBool("Attack", false);
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
            if (playerTarget != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Scream"))
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack"))
                {
                    myAgent.SetDestination(playerTarget.position);
                }
            }
        }
        SwitchState();
    }

    IEnumerator Attack()
    {
        yield return new WaitForEndOfFrame();
        while (currentState == "Attack")
        {
            yield return new WaitForSeconds(1.313f);
        }
        SwitchState();
    }

    public void Damage()
    {
        GameManager.instance.health -= damage;
    }

}
