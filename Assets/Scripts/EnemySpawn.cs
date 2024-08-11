/*
 * Author: Thaqif Adly Bin Mazalan
 * Date: 30/6/24
 * Description: This script facilitates the spawning of enemies. 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour
{
    /// <summary>
    /// The enemy prefab that will spawn.
    /// </summary>
    [SerializeField]
    GameObject enemyAsset;

    /// <summary>
    /// Bool that checks whether spawning should start.
    /// </summary>
    public bool startSpawning;

    /// <summary>
    /// Bool that checks whether the enemies should spawn over time.
    /// </summary>
    public bool isSpawnOverTime;

    /// <summary>
    /// The time interval between each spawn.
    /// </summary>
    [SerializeField]
    float spawnTime;

    /// <summary>
    /// Timer that is used in the updated function.
    /// </summary>
    float currentTimer = 0;

    /// <summary>
    /// Bool that checks whether the spawned enemy should instantly detect the player.
    /// </summary>
    public bool instantDetect;

    /// <summary>
    /// This is a reference to the enemy component of the spawned enemy.
    /// </summary>
    Enemy notBoss;

    /// <summary>
    /// Bool that checks whether an enemy has been spawned.
    /// </summary>
    [HideInInspector]
    public bool hasSpawned;

    /// <summary>
    /// Checks whether the spawn is for the boss fight.
    /// </summary>
    [SerializeField]
    bool bossFight;

    void Start()
    {
        currentTimer = 0;
        hasSpawned = false;
    }

    void Update()
    {
        // This code is checking if it spawns over time, the code will run and spawn an enemy everytime the timer is up.
        if (isSpawnOverTime)
        {
            if (startSpawning)
            {
                if (currentTimer <= 0)
                {
                    SpawnEnemy();
                    currentTimer = spawnTime;
                }
                currentTimer -= Time.deltaTime;
            }
        }

        // Else, the spawner will spawn only one enemy.
        else if (startSpawning && !hasSpawned)
        {
            SpawnEnemy();
            hasSpawned = true;
        }
    }

    /// <summary>
    /// Spawns an enemy at the current position.
    /// </summary>
    public void SpawnEnemy()
    {
        hasSpawned = false;
        GameObject enemy = Instantiate(enemyAsset, gameObject.transform.position, gameObject.transform.rotation);
        if (enemy.TryGetComponent<Enemy>(out notBoss))
        {
            if (instantDetect)
            {
                notBoss.detected = true;
            }
        }
    }
}