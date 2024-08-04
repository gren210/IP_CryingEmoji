using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public int currentHealth;

    public int health = 0;

    public int damage;

    public float attackRange;

    public bool detected;

    protected virtual void Damage()
    {
        GameManager.instance.health -= damage;
        Debug.Log(GameManager.instance.health);
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    protected virtual void SwitchState(string currentState)
    {
        StartCoroutine(currentState);
    }

}
