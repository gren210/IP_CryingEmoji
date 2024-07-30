using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField]
    EnemyFSM1 attachedEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            attachedEnemy.UpdateTarget(other.transform);
            attachedEnemy.animator.SetTrigger("Detected");
            attachedEnemy.detected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
            //attachedEnemy.UpdateTarget(null);
        //}
    }
}
