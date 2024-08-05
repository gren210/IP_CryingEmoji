using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyFSM2 : Enemy
{
    Transform playerTarget;

    [HideInInspector]
    public Animator animator;

    NavMeshAgent myAgent;

    string currentState;

    string nextState;

    bool stunned;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        playerTarget = GameManager.instance.thePlayer.transform;
        animator = gameObject.GetComponent<Animator>();
        myAgent = gameObject.GetComponent<NavMeshAgent>();
        currentState = "Idle";
        nextState = currentState;
        SwitchState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("Health", currentHealth);

        if (currentHealth < health)
        {
            detected = true;
        }

        if (detected)
        {
            animator.SetTrigger("Detected");
        }

        if(currentState != nextState)
        {
            currentState = nextState;
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Scream"))
        {
            transform.LookAt(playerTarget.position);
        }

        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Scream") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Idle"))
        {
            if (detected && Vector3.Distance(playerTarget.position, gameObject.transform.position) <= attackRange)
            {
                nextState = "Attack";
            }
            else if (detected)
            {
                nextState = "Chasing";
            }
            else if (stunned)
            {
                nextState = "Stunned";
            }
        }
        if (currentHealth <= 0)
        {
            nextState = "Death";
        }
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
        SwitchState(currentState);
    }

    IEnumerator Stunned()
    {
        while (currentState == "Stunned")
        {
            yield return new WaitForEndOfFrame();
        }
        SwitchState(currentState);
    }

    IEnumerator Chasing()
    {
        while (currentState == "Chasing")
        {
            yield return new WaitForEndOfFrame();
            if (playerTarget != null)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Scream"))
                {
                    myAgent.SetDestination(playerTarget.position);
                }
            }
        }
        SwitchState(currentState);
    }

    IEnumerator Attack()
    {
        animator.SetBool("Attack", true);
        while (currentState == "Attack")
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Attack", false);
        SwitchState(currentState);
    }

    IEnumerator Death()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        myAgent.enabled = false;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    protected override void Damage()
    {
        base.Damage();
    }

}
