using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Melee : MonoBehaviour
{
    private Animator animator;

    public float cooldown;

    private int hitCount;

    float lastClickedTime;

    public float comboDelay;

    public bool readySwing;

    // Start is called before the first frame update
    void Start()
    {
        animator = GameManager.instance.animator;
        readySwing = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        lastClickedTime += Time.deltaTime;
        if (lastClickedTime >= cooldown)
        {
            hitCount = 0;
            animator.SetInteger("hitCount", hitCount);
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
}
