/*
 * Author: Thaqif Adly Bin Mazalan
 * Date: 17/7/24
 * Description: The Game Manager which handles important game information and persists through scenes.
 */

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// An instance of the GameManager so that all scripts can access the GameManager.
    /// </summary>
    public static GameManager instance;

    public GameObject playerCamera;
    
    /// <summary>
    /// The current virtual camera that is being used by the player.
    /// </summary>
    public CinemachineVirtualCamera currentVirtualCamera;


    public Animator animator;

    public Gun currentGun;

    public Grenade currentGrenade;

    public bool isShooting;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
