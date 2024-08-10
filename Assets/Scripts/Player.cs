using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class Player : ScriptManager
{
    /// <summary>
    /// The virtual camera for aiming.
    /// </summary>
    public CinemachineVirtualCamera aimVirtualCamera;

    /// <summary>
    /// The standard virtual camera.
    /// </summary>
    public CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// The crouching virtual camera.
    /// </summary>
    public CinemachineVirtualCamera crouchVirtualCamera;

    /// <summary>
    /// The crouch aiming virtual camera.
    /// </summary>
    public CinemachineVirtualCamera crouchAimVirtualCamera;

    /// <summary>
    /// The current aiming virtual camera.
    /// </summary>
    public CinemachineVirtualCamera currentAimVirtualCamera;

    /// <summary>
    /// The current virtual camera.
    /// </summary>
    public CinemachineVirtualCamera currentVirtualCamera;

    /// <summary>
    /// The transform of the invisible dot the player will always face.
    /// </summary>
    [SerializeField]
    Transform playerLookAt;

    /// <summary>
    /// The transform of the player's main camera
    /// </summary>
    public Transform playerCamera;

    [SerializeField]
    GameObject flashlight;

    /// <summary>
    /// The standard player camera sensitivity
    /// </summary>
    [SerializeField]
    float normalSensitivity;

    /// <summary>
    /// The aiming player camera sensitivity
    /// </summary>
    [SerializeField]
    float aimSensitivity;

    /// <summary>
    /// The third person controller player script
    /// </summary>
    [HideInInspector]
    public ThirdPersonController thirdPersonController;

    /// <summary>
    /// The starter assets input player script
    /// </summary>
    [HideInInspector]
    public StarterAssetsInputs starterAssetsInputs;

    /// <summary>
    /// The animator for the player controller.
    /// </summary>
    [HideInInspector]
    public Animator animator;

    [SerializeField]
    GameObject deathCam;

    CinemachineBrain cinemachineBrain;

    [HideInInspector]
    public PlayerInput playerInput;

    public float interactionDistance = 1000f;

    public Rig rig;

    private bool isAiming;

    [HideInInspector]
    public int currentWeaponLayer;

    int previousWeaponLayer;

    float currentTimer = 0;

    public GameObject currentPrimary;

    public GameObject currentSecondary;

    public GameObject currentGrenade;

    public GameObject currentMelee;

    float currentCrouchTimer;

    float currentStunTimer;

    private bool isCrouch = false;

    private bool isCrouching = false;

    private int crouchCount = 0;

    private Coroutine currentCoroutine = null;

    private Vector3 aimDirection;

    public float weaponSwapDelay;

    public float stunDuration;

    public GameObject[] primaryWeapons;

    public GameObject[] secondaryWeapons;

    public GameObject[] meleeWeapons;

    public GameObject[] grenadeWeapons;



    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        cinemachineBrain = playerCamera.gameObject.GetComponent<CinemachineBrain>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        GameManager.instance.thePlayer = this;
        GameManager.instance.animator = animator;
        GameManager.instance.immune = false;
        currentVirtualCamera = virtualCamera;
        currentAimVirtualCamera = aimVirtualCamera;
        CursorLock(true);
        GameManager.instance.playerUI.SetActive(true);
    }

    private void Update()
    {
        animator.SetFloat("moveX", starterAssetsInputs.move.x);
        animator.SetFloat("moveY", starterAssetsInputs.move.y);

        if (GameManager.instance.isShooting)
        {
            //Vector2 rng = Random.insideUnitCircle;
            //Vector3 cameraRNG = new Vector3(rng.x, rng.y, 1f);
            //Debug.Log(cameraRNG);
            //playerLookAt.position = new Vector3(playerLookAt.position.x + rng.x * 10, playerLookAt.position.y + rng.y * 10, playerLookAt.position.z);
            //playerCamera.LookAt(playerLookAt.position);
            //Debug.Log(playerLookAt.position);
            //GameManager.instance.isShooting = false;
        }
        if (isAiming && thirdPersonController.Grounded && GameManager.instance.readySwap)
        {
            playerLookAt.position = playerCamera.position + (playerCamera.forward * 10f);
            rig.weight = 1;
        }
        else
        {
            playerLookAt.position = playerCamera.position + (playerCamera.forward * 10f); // Sets the position of the invisible dot that the player will always face.
            rig.weight = 0;
        }
        flashlight.transform.LookAt(playerLookAt);
        Vector3 aimTarget = playerLookAt.position;
        aimTarget.y = transform.position.y; // Keeps the player face the x axis only so the whole player model does not face up and down
        aimDirection = (aimTarget - transform.position).normalized;

        if (starterAssetsInputs.aim && thirdPersonController.Grounded && GameManager.instance.currentMelee == null && GameManager.instance.readySwap)
        {
            if (currentWeaponLayer == 2)
            {
                SetLayer(4,currentAimVirtualCamera,true,aimSensitivity);
            }
            else if (currentWeaponLayer == 1)
            {
                SetLayer(3, currentAimVirtualCamera, true, aimSensitivity);
            }
            else
            {
                currentAimVirtualCamera.gameObject.SetActive(true);
                thirdPersonController.SetSensitivity(aimSensitivity);
            }
            GameManager.instance.currentVirtualCamera = currentAimVirtualCamera;
            FaceForward(); // Make rotations smooth

        }
        else
        {
            if (currentWeaponLayer == 4)
            {
                SetLayer(2, currentAimVirtualCamera, false, aimSensitivity);
            }
            else if (currentWeaponLayer == 3)
            {
                SetLayer(1, currentAimVirtualCamera, false, aimSensitivity);
            }
            else
            {
                currentAimVirtualCamera.gameObject.SetActive(false);
                thirdPersonController.SetSensitivity(normalSensitivity);
            }
            GameManager.instance.currentVirtualCamera = currentVirtualCamera;
        }


        if (previousWeaponLayer != currentWeaponLayer)
        {
            currentTimer += Time.deltaTime;
            animator.SetLayerWeight(currentWeaponLayer, Mathf.Lerp(0, 1f, currentTimer/ .2f));
            animator.SetLayerWeight(previousWeaponLayer, Mathf.Lerp(1, 0f, currentTimer / .2f));
            if (currentTimer >= .2f)
            {
                animator.SetLayerWeight(currentWeaponLayer, 1);
                animator.SetLayerWeight(previousWeaponLayer, 0);
                previousWeaponLayer = currentWeaponLayer;
                currentTimer = 0;
                GameManager.instance.readySwap = true;
            }
        }

        if (isCrouch)
        {
            currentCrouchTimer += Time.deltaTime;
            animator.SetLayerWeight(6, Mathf.Lerp(animator.GetLayerWeight(6), crouchCount%2, currentCrouchTimer / .15f));
            if (currentCrouchTimer >= .15f)
            {
                animator.SetLayerWeight(6, crouchCount%2);
                currentCrouchTimer = 0;
                isCrouch = false;
            }
        }

        if(isCrouching || starterAssetsInputs.aim)
        {
            thirdPersonController.SprintSpeed = 2;
        }
        else
        {
            thirdPersonController.SprintSpeed = 5.335f;
        }

        if (animator.GetLayerWeight(8) != 0)
        {
            currentStunTimer += Time.deltaTime;
            animator.SetLayerWeight(8, Mathf.Lerp(1, 0, currentStunTimer / stunDuration));
            if (currentStunTimer >= stunDuration)
            {
                animator.SetLayerWeight(8, 0);
                currentStunTimer = 0;
            }
        }

        GameManager.instance.isCrouch = isCrouching;
        GameManager.instance.isAiming = starterAssetsInputs.aim;
        GameManager.instance.thirdPersonController = thirdPersonController;

        
    }

    public IEnumerator Stunned()
    {
        animator.SetLayerWeight(8, 1);
        //SetLayer(8, 1)
        float moveSpeed = thirdPersonController.MoveSpeed;
        float sprintSpeed = thirdPersonController.SprintSpeed;
        animator.SetBool("stunned", true);
        //gameObject.GetComponent<CharacterController>().enabled = false;
        thirdPersonController.MoveSpeed = 0;
        thirdPersonController.SprintSpeed = 0;
        GameManager.instance.readySwap = false;
        yield return new WaitForSeconds(stunDuration);
        thirdPersonController.MoveSpeed = moveSpeed;
        thirdPersonController.SprintSpeed = sprintSpeed;
        animator.SetBool("stunned", false);

        //gameObject.GetComponent<CharacterController>().enabled = true;
        GameManager.instance.readySwap = true;

    }

    public void FaceForward()
    {
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 35f);
    }

    void SetLayer(int weaponLayerSwitch, CinemachineVirtualCamera virtualCamera, bool isAim, float sens)
    {
        previousWeaponLayer = currentWeaponLayer;
        currentWeaponLayer = weaponLayerSwitch;
        AimState(virtualCamera, isAim, sens);
        
    }
    void CameraSwap(bool isSwap, CinemachineVirtualCamera normal, CinemachineVirtualCamera aim)
    {
        currentVirtualCamera.gameObject.SetActive(isSwap);
        currentAimVirtualCamera.gameObject.SetActive(isSwap);
        currentVirtualCamera = normal;
        currentAimVirtualCamera = aim;
        currentVirtualCamera.gameObject.SetActive(!isSwap);
        currentAimVirtualCamera.gameObject.SetActive(!isSwap);
        if (starterAssetsInputs.aim)
        {
            GameManager.instance.currentVirtualCamera = currentAimVirtualCamera;
        }
        else
        {
            GameManager.instance.currentVirtualCamera = currentVirtualCamera;
        }
    }

    void CameraState(CinemachineVirtualCamera virtualCamera,bool aimState, float sens)
    {
        virtualCamera.gameObject.SetActive(aimState);
        thirdPersonController.SetSensitivity(sens);
    }

    void AimState(CinemachineVirtualCamera virtualCamera, bool aimState, float sens)
    {
        CameraState(virtualCamera, aimState, sens);
        animator.SetBool("isAiming", aimState);
        isAiming = aimState;
    }

    void WeaponState(int currentLayer)
    {
        AimState(aimVirtualCamera, false, normalSensitivity);
        currentTimer = 0;
        previousWeaponLayer = currentWeaponLayer;
        currentWeaponLayer = currentLayer;
    }

    public void ActiveWeapons(bool primary, bool secondary, bool grenade, bool melee)
    {
        if (currentPrimary != null)
        {
            currentPrimary.SetActive(primary);
        }
        if (currentSecondary != null)
        {
            currentSecondary.SetActive(secondary);
        }
        if (currentGrenade != null)
        {
            currentGrenade.SetActive(grenade);
        }
        if (currentMelee != null)
        {
            currentMelee.SetActive(melee);
        }
    }

    IEnumerator EquipState(bool primary, bool secondary, bool grenade, bool melee, int setLayer, int weaponState)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        ActiveWeapons(primary, secondary, grenade, melee);
        animator.SetBool("isSecondary", secondary);
        animator.SetLayerWeight(setLayer, 0f);
        WeaponState(weaponState);
        GameManager.instance.readySwap = false;
        yield return new WaitForSeconds(weaponSwapDelay);
        GameManager.instance.readySwap = true;
    }

    void OnFlashlight()
    {
        if (flashlight.activeSelf == false)
        {
            flashlight.SetActive(true);
        }
        else
        {
            flashlight.SetActive(false);
        }
    }

    public void WeaponSwitch(Gun gun, Grenade grenade, Melee melee)
    {
        GameManager.instance.currentGun = gun;
        GameManager.instance.currentGrenade = grenade;
        GameManager.instance.currentMelee = melee;
    }

    public void OnPrimaryEquip()
    {
        if (GameManager.instance.readySwap && !cinemachineBrain.IsBlending && currentPrimary != null)
        {
            WeaponSwitch(currentPrimary.GetComponent<Gun>(), null,null);
            StartCoroutine(EquipState(true, false, false, false, 3, 2));
            if (GameManager.instance.currentGun.fullAuto)
            { 
                currentCoroutine = StartCoroutine(GameManager.instance.currentGun.FullAutoShoot(this));
            }
        }
    }

    public void OnSecondaryEquip()
    {
        if(GameManager.instance.readySwap && !cinemachineBrain.IsBlending && currentSecondary != null)
        {
            WeaponSwitch(currentSecondary.GetComponent<Gun>(), null, null);
            StartCoroutine(EquipState(false, true, false, false, 4, 1));
        }
    }

    public void OnMelee()
    {
        if (GameManager.instance.readySwap && !cinemachineBrain.IsBlending && currentMelee != null)
        {
            WeaponSwitch(null, null, currentMelee.GetComponent<Melee>());
            StartCoroutine(EquipState(false, false, false, true, 3, 5));
        }
    }

    public void OnGrenade()
    {
        if (GameManager.instance.readySwap && !cinemachineBrain.IsBlending && currentGrenade != null)
        {
            if (GameManager.instance.grenadeBackpack[currentGrenade.GetComponent<Grenade>().grenadeIndex] > 0)
            {
                WeaponSwitch(null, currentGrenade.GetComponent<Grenade>(), null);
                StartCoroutine(EquipState(false, false, true, false, 3, 0));
                AimState(currentAimVirtualCamera, false, normalSensitivity);
            }
        }

    }

    void OnSingleShot()
    {
        if (GameManager.instance.currentGrenade != null)
        {
            if (GameManager.instance.readyShoot)
            {
                StartCoroutine(GameManager.instance.currentGrenade.GrenadeThrow(this));
                GameManager.instance.readyShoot = false;
            }
        }
        else if (GameManager.instance.currentGun != null)
        {
            if (GameManager.instance.currentGun.currentAmmoCount <= 0)
            {

            }
            else if (!GameManager.instance.currentGun.fullAuto)
            {
                GameManager.instance.currentGun.Shoot(this);
            }
        }
        else if (GameManager.instance.currentMelee != null)
        {
            if (GameManager.instance.currentMelee.readySwing && thirdPersonController.Grounded && !isCrouching)
            {
                StartCoroutine(GameManager.instance.currentMelee.MeleeAttack());
            }
        }
    }

    public void OnHolster()
    {
        if (GameManager.instance.readySwap && (currentPrimary != null || currentSecondary != null || currentGrenade != null || currentMelee != null)) // check if currentprimary secondary melee grenade is false null &&
        {
            WeaponSwitch(null,null,null);   
            StartCoroutine(EquipState(false, false, false, false, 1, 0));
        }
    }

    void OnCrouch()
    {
        if (currentCrouchTimer == 0)
        {
            isCrouch = !isCrouch;
            isCrouching = !isCrouching;
            crouchCount++;
        }
        if (!isCrouching)
        {
            CameraSwap(false, virtualCamera, aimVirtualCamera);

        }
        else
        {
            CameraSwap(false, crouchVirtualCamera, crouchAimVirtualCamera);
        }
    }

    void OnReload()
    {
        if(GameManager.instance.currentGun != null)
        {
            StartCoroutine(GameManager.instance.currentGun.Reload());
        }
    }

    void Death()
    {
        GameManager.instance.immune = true;
        animator.SetLayerWeight(9, 1);
        playerInput.SwitchCurrentActionMap("UI");
        gameObject.GetComponent<CharacterController>().enabled = false;
        starterAssetsInputs.enabled = false;
        WeaponSwitch(null, null, null);
        StartCoroutine(EquipState(false, false, false, false, 1, 0));
        playerCamera.gameObject.SetActive(false);
        deathCam.SetActive(true);
        for(int i = 1; i < animator.layerCount - 2; i++)
        {
            animator.SetLayerWeight(i, 0);
        }
    }

    void OnBackpack()
    {
        if (!starterAssetsInputs.aim)
        {
            if (GameManager.instance.inventoryUI.activeSelf == true)
            {
                playerInput.SwitchCurrentActionMap("Player");
                GameManager.instance.inventoryUI.SetActive(false);
                CursorLock(true);
            }
            else
            {
                playerInput.SwitchCurrentActionMap("UI");
                GameManager.instance.inventoryUI.SetActive(true);
                CursorLock(false);
            }
        }
    }

}
