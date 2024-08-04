using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Melee : Interactable
{
    private Animator animator;

    public float cooldown;

    private int hitCount;

    float lastClickedTime;

    public float comboDelay;

    public bool readySwing;

    public int meleeIndex;

    public bool interactable;

    public int damage;

    public float attackRange;

    public BoxCollider damageCollider;

    // Start is called before the first frame update
    void Start()
    {
        animator = GameManager.instance.animator;
        readySwing = true;
        lastClickedTime = 0;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactable)
        {
            lastClickedTime += Time.deltaTime;
            if (lastClickedTime >= cooldown)
            {
                hitCount = 0;
                animator.SetInteger("hitCount", hitCount);
            }
        }
    }

    public IEnumerator MeleeAttack()
    {
        if (readySwing)
        {
            GameManager.instance.readySwap = false;
            GameManager.instance.readyMove = false;
            lastClickedTime = 0;
            readySwing = false;
            hitCount++;
            animator.SetInteger("hitCount", hitCount);
            yield return new WaitForSeconds(comboDelay);
            readySwing = true;
            GameManager.instance.readySwap = true;
            GameManager.instance.readyMove = true;
        }

        if (hitCount == 3)
        {
            hitCount = 0;
            readySwing = false;
            yield return new WaitForSeconds(cooldown - comboDelay);
            animator.SetInteger("hitCount", hitCount);
            readySwing = true;
        }
    }

    public override void Interact(Player thePlayer)
    {
        base.Interact(thePlayer);
        GameManager.instance.meleeBackpack[meleeIndex] = true;
        if(thePlayer.currentMelee != null)
        {
            thePlayer.meleeWeapons[thePlayer.currentMelee.GetComponent<Melee>().meleeIndex].SetActive(false);
        }
        thePlayer.currentMelee = thePlayer.meleeWeapons[meleeIndex];
        thePlayer.OnMelee();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.tag);
        if (other.transform.tag == "Enemy")
        {
            Debug.Log("ok");
            other.gameObject.GetComponent<Enemy>().currentHealth -= damage;
        }
    }
}
