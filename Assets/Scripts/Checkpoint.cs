using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform checkpointTarget;

    public int checkpointIndex;

    public bool isList;
    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.instance.checkpoints[checkpointIndex] = checkpointTarget;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.SaveItems();
        GameManager.instance.currentCheckpointIndex = checkpointIndex;
        Debug.Log("Saved");
        Destroy(gameObject);
    }
}
