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

    public int health = 100;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (animator != null)
        {
            animator.SetInteger("Health", health);
        }
        healthbar.fillAmount = health / 100f;

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
