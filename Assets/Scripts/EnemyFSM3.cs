using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyFSM3 : Enemy
{
    Transform playerTarget;

    [HideInInspector]
    public Animator animator;

    NavMeshAgent myAgent;

    string currentState;

    string nextState;

    bool stunned;

    [SerializeField]
    GameObject gasEffect;

    bool hasGas = false;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        myAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        currentState = "Idle";
        nextState = currentState;
        SwitchState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTarget != GameManager.instance.thePlayer.transform)
        {
            playerTarget = GameManager.instance.thePlayer.transform;
        }

        animator.SetInteger("Health", currentHealth);

        if (currentHealth < health)
        {
            detected = true;
        }

        if (detected)
        {
            animator.SetTrigger("Detected");
        }

        if (currentState != nextState)
        {
            currentState = nextState;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Roar"))
        {
            //transform.forward = Vector3.Lerp(transform.forward, playerTarget.position, Time.deltaTime * 3f);
            transform.LookAt(playerTarget.position);
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Roar") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Idle"))
        {
            if (detected && Vector3.Distance(playerTarget.position, gameObject.transform.position) <= attackRange)
            {
                nextState = "Attack";
            }
            else if (detected)
            {
                nextState = "Chasing";
            }
            //else if (stunned)
            //{
                //nextState = "Stunned";
            //}
        }
        if (currentHealth <= 0)
        {
            nextState = "Death";
        }
    }

    IEnumerator Idle()
    {
        while(currentState == "Idle")
        {
            yield return new WaitForEndOfFrame();
        }
        SwitchState(currentState);
    }

    //IEnumerator Stunned()
    //{
        //while (currentState == "Stunned")
        //{
            //yield return new WaitForEndOfFrame();
        //}
        //SwitchState(currentState);
    //}

    IEnumerator Chasing()
    {
        while (currentState == "Chasing")
        {
            yield return new WaitForEndOfFrame();
            if (playerTarget != null)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Roar"))
                {
                    myAgent.SetDestination(playerTarget.position);
                }
            }
        }
        SwitchState(currentState);
    }

    IEnumerator Attack()
    {
        if (hasGas)
        {
            animator.SetBool("Gas", true);
            hasGas = false;
        }
        else 
        {
            animator.SetBool("Attack", true);
            hasGas = true;
        }
        while (currentState == "Attack" || (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Gas")))
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Attack", false);
        animator.SetBool("Gas", false);
        SwitchState(currentState);
    }

    IEnumerator Death()
    {
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        myAgent.enabled = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void Gas()
    {
        Instantiate(gasEffect, transform.position, transform.rotation);
    }

    protected override void Damage()
    {
        if(!GameManager.instance.immune)
        {
            base.Damage();
            if (GameManager.instance.health > damage)
            {
                StartCoroutine(playerTarget.gameObject.GetComponent<Player>().Stunned());
            }
        }
    }

}
