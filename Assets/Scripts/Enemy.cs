using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected virtual void Damage(int damage)
    {
        GameManager.instance.health -= damage;
        Debug.Log(GameManager.instance.health);
    }

    protected virtual void SwitchState(string currentState)
    {
        StartCoroutine(currentState);
    }

}
