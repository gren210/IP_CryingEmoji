/*
 * Author: Thaqif Adly Bin Mazalan
 * Date: 17/7/24
 * Description: The Game Manager which handles important game information and persists through scenes.
 */

using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public ThirdPersonController thirdPersonController;

    public Player thePlayer;

    public Animator animator;

    public Gun currentGun;

    public Grenade currentGrenade;

    public Melee currentMelee;

    public TextMeshProUGUI ammoText; 

    public bool isShooting;

    public bool readySwap = true;

    public bool readyShoot = true;

    public bool readyMove = true;

    public bool isCrouch = false;

    public bool isAiming = false;

    public bool[] primaryBackpack = { false, false, false };

    public int[] primaryAmmo = { 0, 0, 0 } ;

    public bool[] secondaryBackpack = { false, false };

    public int[] secondaryAmmo = { 0, 0 };

    public bool[] meleeBackpack = { false, false, false };

    public int[] grenadeBackpack = { 0, 0, 0 };

    public int health = 100;

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
        readyShoot = true;
        readySwap = true;
        readyMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("Health", health);

        if(currentGun != null)
        {
            ammoText.text = "" + currentGun.currentAmmoCount;
        }
    }


}
