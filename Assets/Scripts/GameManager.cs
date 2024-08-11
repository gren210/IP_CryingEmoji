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
using UnityEngine.UI;

public class GameManager : ScriptManager
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

    public Workbench currentWorkbench;

    public TextMeshProUGUI ammoText; 

    public bool isShooting;

    public bool readySwap = true;

    public bool readyShoot = true;

    public bool readyMove = true;

    public bool isCrouch = false;

    public bool isAiming = false;

    public bool immune = false;

    public bool[] primaryBackpack = { false, false };

    public float[] primaryAmmo = { 0, 0 } ;

    public bool[] secondaryBackpack = { false, false };

    public float[] secondaryAmmo = { 0, 0 };

    public bool[] meleeBackpack = { false, false };

    public int[] grenadeBackpack = { 0, 0, 0 };

    public int[] itemBackpack = { 0, 0 };

    public bool[] upgrades = { false, false, false, false };   

    public int health = 100;

    public TextMeshProUGUI healCountText;

    public TextMeshProUGUI gearCountText;

    public int healAmount;

    public GameObject playerUI;

    public GameObject menuUI;

    public GameObject inventoryUI;

    public GameObject workbenchUI;

    public GameObject pauseUI;

    public GameObject UIassets;

    public UI workbenchUIObject;

    public Image healthbar;

    public TextMeshProUGUI ammoCount;

    public TextMeshProUGUI ammoReserve;

    public GameObject ammoSign;

    public GameObject transition;

    public TextMeshProUGUI interactText;

    public AudioClip[] music;

    public AudioSource musicSource;

    public Transform currentCheckpoint;

    public Transform[] checkpoints = {null, null, null, null, null, null, null, null};

    public int currentCheckpointIndex = -1;

    public bool[] upgradesSave = { false, false, false, false };

    public bool[] primaryBackpackSave = { false, false };

    float[] primaryAmmoSave = { 0, 0 };

    bool[] secondaryBackpackSave = { false, false };

    float[] secondaryAmmoSave = { 0, 0 };

    bool[] meleeBackpackSave = { false, false };

    int[] grenadeBackpackSave = { 0, 0, 0 };

    int[] itemBackpackSave = { 0, 0 };

    int currentPrimarySave = 5;

    int currentSecondarySave = 5;

    int currentMeleeSave = 5;

    int currentGrenadeSave = 5;


    public void SaveItems()
    {
        primaryBackpackSave = primaryBackpack;
        primaryAmmoSave = primaryAmmo;
        secondaryBackpackSave = secondaryBackpack;
        secondaryAmmoSave = secondaryAmmo;
        meleeBackpackSave = meleeBackpack;
        grenadeBackpackSave= grenadeBackpack;
        itemBackpackSave = itemBackpack;
        upgradesSave = upgrades;
        if(thePlayer.currentPrimary != null)
        {
            currentPrimarySave = thePlayer.currentPrimary.GetComponent<Gun>().gunIndex;
        }
        if (thePlayer.currentSecondary != null)
        {
            currentSecondarySave = thePlayer.currentSecondary.GetComponent<Gun>().gunIndex;
        }
        if (thePlayer.currentMelee != null)
        {
            currentMeleeSave = thePlayer.currentMelee.GetComponent<Melee>().meleeIndex;
        }
        if (thePlayer.currentGrenade != null)
        {
            currentGrenadeSave = thePlayer.currentGrenade.GetComponent<Grenade>().grenadeIndex;
        }

    }

    public void ReloadSave()
    {
        primaryBackpack = primaryBackpackSave;
        primaryAmmo = primaryAmmoSave;
        secondaryBackpack = secondaryBackpackSave;
        secondaryAmmo = secondaryAmmoSave;
        meleeBackpack = meleeBackpackSave;
        grenadeBackpack = grenadeBackpackSave;
        itemBackpack = itemBackpackSave;
        upgrades = upgradesSave;
        if (currentPrimarySave != 5)
        {
            thePlayer.currentPrimary = thePlayer.primaryWeapons[currentPrimarySave];
        }
        if (currentSecondarySave != 5)
        {
            thePlayer.currentSecondary = thePlayer.secondaryWeapons[currentSecondarySave];
            thePlayer.OnSecondaryEquip();
        }
        if (currentMeleeSave != 5)
        {
            thePlayer.currentMelee = thePlayer.meleeWeapons[currentMeleeSave];
        }
        if (currentGrenadeSave != 5)
        {
            thePlayer.currentGrenade = thePlayer.grenadeWeapons[currentGrenadeSave];
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(UIassets);
        }
        else if (instance != null && instance != this)
        {
            Destroy(UIassets);
            Destroy(gameObject);
            
        }
        musicSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        readyShoot = true;
        readySwap = true;
        readyMove = true;
        musicSource.clip = music[0];
        musicSource.Play();
        currentCheckpointIndex = -1;
}

    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            animator.SetInteger("Health", health);
        }
        healthbar.fillAmount = health / 100f;
        gearCountText.text = "Gears:" + itemBackpack[0];
        healCountText.text = "Medkits:" + itemBackpack[1];

        if (currentGun != null)
        {
            ammoText.text = "" + currentGun.currentAmmoCount;
            if (currentGun.secondary)
            {
                ammoReserve.text = "" + secondaryAmmo[currentGun.gunIndex];
            }
            else
            {
                ammoReserve.text = "" + primaryAmmo[currentGun.gunIndex];
            }
            ammoSign.SetActive(true);
        }
        else
        {
            ammoText.text = "";
            ammoReserve.text = "";
            ammoSign.SetActive(false);
        }
    }

    public void WorkbenchClose()
    {
        workbenchUI.SetActive(false);
        currentWorkbench.virtualCamera.SetActive(false);
        currentWorkbench = null;
        CursorLock(true);
    }

}
