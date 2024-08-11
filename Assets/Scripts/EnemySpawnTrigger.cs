/*
 * Author: Thaqif Adly Bin Mazalan
 * Date: 30/6/24
 * Description: Script that handles spawning of enemies when the player enters the trigger.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTrigger : ScriptManager
{
    /// <summary>
    /// Array of enemies to be spawned by the trigger.
    /// </summary>
    [SerializeField]
    GameObject[] enemies;

    /// <summary>
    /// Checks if enemies will instantly detect the player upon spawning.
    /// </summary>
    [SerializeField]
    bool instantDetect;

    /// <summary>
    /// Checks whether the trigger is for after the boss sequence.
    /// </summary>
    [SerializeField]
    bool bossTrigger;

    /// <summary>
    /// Runs when another collider enters the trigger.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (bossTrigger)
        {
            //GameManager.instance.objectiveText.text = GameManager.instance.objectiveStrings[9];
            //ChangeMusic(10); 
        }

        if (other.gameObject.tag == "Player")
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<EnemySpawn>().isSpawnOverTime)
                {
                    enemy.GetComponent<EnemySpawn>().startSpawning = true;
                }
                else
                {
                    enemy.GetComponent<EnemySpawn>().SpawnEnemy();
                    Destroy(enemy, 0.1f); // Destroy the spawner after a short delay.
                }

                if (instantDetect)
                {
                    enemy.GetComponent<EnemySpawn>().instantDetect = true;
                }
            }
            Destroy(gameObject); // Destroy the trigger object after it has been activated.
        }
    }
}
