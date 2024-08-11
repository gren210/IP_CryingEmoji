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

    public AudioClip[] zombieSounds;

    public AudioSource audioSource;

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

    protected virtual void ChangeSound(int soundIndex)
    {
        if(zombieSounds[soundIndex] != audioSource.clip)
        {
            audioSource.Stop();
            audioSource.clip = zombieSounds[soundIndex];
            audioSource.Play();
        }
    }

}
