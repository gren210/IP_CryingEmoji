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

    [SerializeField]
    float explodeRadius;

    [SerializeField]
    GameObject explodeEffect;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        myAgent = gameObject.GetComponent<NavMeshAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeSound(0);
        currentHealth = health;
        currentState = "Idle";
        nextState = currentState;
        SwitchState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        playerTarget = GameManager.instance.thePlayer.transform;
        animator.SetInteger("Health", currentHealth);

        if (currentHealth < health)
        {
            detected = true;
        }

        if (detected)
        {
            ChangeSound(1);
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
                nextState = "Death";
            }
            else if (detected)
            {
                nextState = "Chasing";
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

    IEnumerator Death()
    {
        Collider[] entities = Physics.OverlapSphere(transform.position, explodeRadius);

        foreach (Collider collider in entities)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                collider.gameObject.GetComponent<Enemy>().currentHealth -= damage;
            }
            else if (collider.gameObject.tag == "Player") //&& !GameManager.instance.isImmune)
            {
                GameManager.instance.health -= damage;
            }
        }
        Destroy(Instantiate(explodeEffect,transform.position,transform.rotation),1f);
        gameObject.SetActive(false);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    protected override void Damage()
    {
        base.Damage();
        if (GameManager.instance.health > damage)
        {
            StartCoroutine(playerTarget.gameObject.GetComponent<Player>().Stunned());
        }
    }

}
