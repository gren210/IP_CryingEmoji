using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    public int damage;

    public float attackRange;

    protected virtual void Damage()
    {
        GameManager.instance.health -= damage;
        Debug.Log(GameManager.instance.health);
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
    }

    protected virtual void SwitchState(string currentState)
    {
        StartCoroutine(currentState);
    }

}
