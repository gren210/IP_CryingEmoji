using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM1 : Enemy
{
    Transform playerTarget;

    [HideInInspector]
    public Animator animator;

    NavMeshAgent myAgent;

    string currentState;

    string nextState;

    public bool detected;

    public float attackRange;

    public float health;

    public int damage;

    bool stunned;

    // Start is called before the first frame update
    void Start()
    {
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
        if(currentState != nextState)
        {
            currentState = nextState;
        }
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Scream"))
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
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack"))
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

    protected override void Damage(int damage)
    {
        base.Damage(damage);
    }

}
