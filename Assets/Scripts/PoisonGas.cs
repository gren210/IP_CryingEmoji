using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    bool inGas = false;

    [SerializeField]
    float gasDamagePerSecond;

    float currentTimer;

    [SerializeField]
    float gasTimer;

    [SerializeField]
    GameObject gasEffect;

    // Start is called before the first frame update
    void Start()
    {
        currentTimer = 1f/gasDamagePerSecond;
        Destroy(gameObject, gasTimer);
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer -= Time.deltaTime;
        if (inGas)
        {
            if (currentTimer <= 0f)
            {
                GameManager.instance.health--;
                Debug.Log(GameManager.instance.health);
                currentTimer = 1f / gasDamagePerSecond;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inGas = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inGas = false;
        }
    }
}
