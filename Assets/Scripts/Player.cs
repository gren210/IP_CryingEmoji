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
    [SerializeField]
    CinemachineVirtualCamera aimVirtualCamera;

    /// <summary>
    /// The standard virtual camera.
    /// </summary>
    [SerializeField]
    CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// The crouching virtual camera.
    /// </summary>
    [SerializeField]
    CinemachineVirtualCamera crouchVirtualCamera;

    /// <summary>
    /// The crouch aiming virtual camera.
    /// </summary>
    [SerializeField]
    CinemachineVirtualCamera crouchAimVirtualCamera;

    /// <summary>
    /// The current aiming virtual camera.
    /// </summary>
    [SerializeField]
    CinemachineVirtualCamera currentAimVirtualCamera;

    /// <summary>
    /// The current virtual camera.
    /// </summary>
    [SerializeField]
    CinemachineVirtualCamera currentVirtualCamera;

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

    private bool isCrouch = false;

    private bool isCrouching = false;

    private int crouchCount = 0;

    private Coroutine currentCoroutine = null;

    private Vector3 aimDirection;

    public float weaponSwapDelay;



    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        currentVirtualCamera = virtualCamera;
        currentAimVirtualCamera = aimVirtualCamera;
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
        //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 35f); // Make rotations smooth

        RaycastHit hitInfo;
        //Debug.DrawLine(playerCamera.position, playerCamera.position+ (playerCamera.forward * 999f), Color.red); // Draws raycast line in scene
        //Debug.DrawLine(transform.position, transform.position + (transform.forward * 999f), Color.red);
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitInfo, interactionDistance))
        {
            //Debug.Log(hitInfo);
        }

        if (starterAssetsInputs.aim && thirdPersonController.Grounded && GameManager.instance.currentMelee == null && GameManager.instance.readySwap)
        {
            if (currentWeaponLayer == 3)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 4;
                AimState(currentAimVirtualCamera,true, previousWeaponLayer, 1f, aimSensitivity);
            }
            else if (currentWeaponLayer == 1)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 2;
                AimState(currentAimVirtualCamera, true, previousWeaponLayer, 1f, aimSensitivity);
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
            ShakeCamera(0f, 0f);
            if (currentWeaponLayer == 4)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 3;
                AimState(currentAimVirtualCamera, false, previousWeaponLayer, 0f, aimSensitivity);
            }
            else if (currentWeaponLayer == 2)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 1;
                AimState(currentAimVirtualCamera, false, previousWeaponLayer, 0f, aimSensitivity);
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

        GameManager.instance.isCrouch = isCrouching;
        GameManager.instance.isAiming = starterAssetsInputs.aim;
        GameManager.instance.thirdPersonController = thirdPersonController;
    }

    public void FaceForward()
    {
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 35f);
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

    void AimState(CinemachineVirtualCamera virtualCamera, bool aimState, int layer, float layerWeight, float sens)
    {
        CameraState(virtualCamera, aimState, sens);
        animator.SetBool("isAiming", aimState);
        isAiming = aimState;
    }

    void WeaponState(int currentLayer)
    {
        AimState(aimVirtualCamera, false, currentWeaponLayer, 0f, normalSensitivity);
        currentTimer = 0;
        previousWeaponLayer = currentWeaponLayer;
        currentWeaponLayer = currentLayer;
    }

    IEnumerator EquipState(bool primary, bool secondary, bool grenade, bool melee, int setLayer, int weaponState)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentPrimary.SetActive(primary);
        currentSecondary.SetActive(secondary);
        currentGrenade.SetActive(grenade);
        currentMelee.SetActive(melee);
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

    void OnPrimaryEquip()
    {
        //isAiming = false;
        if (GameManager.instance.currentGun != null)
        {
            if (!GameManager.instance.currentGun.reloading && GameManager.instance.readySwap)
            {
                GameManager.instance.currentGun = currentPrimary.GetComponent<Gun>();
                StartCoroutine(EquipState(true, false, false, false, 2, 3));
                GameManager.instance.currentGrenade = null;
                GameManager.instance.currentMelee = null;
                if (GameManager.instance.currentGun.fullAuto)
                {
                    currentCoroutine = StartCoroutine(GameManager.instance.currentGun.FullAutoShoot(this));
                }
            }
        }
        else if (GameManager.instance.readySwap)
        {
            GameManager.instance.currentGun = currentPrimary.GetComponent<Gun>();
            StartCoroutine(EquipState(true, false, false, false, 2, 3));
            GameManager.instance.currentGrenade = null;
            GameManager.instance.currentMelee = null;
            if (GameManager.instance.currentGun.fullAuto)
            {
                currentCoroutine = StartCoroutine(GameManager.instance.currentGun.FullAutoShoot(this));
            }
        }
    }

    void OnSecondaryEquip()
    {
        //isAiming = false;
        if(GameManager.instance.currentGun != null)
        {
            if (!GameManager.instance.currentGun.reloading && GameManager.instance.readySwap)
            {
                GameManager.instance.currentGun = currentSecondary.GetComponent<Gun>();
                StartCoroutine(EquipState(false, true, false, false, 4, 1));
                GameManager.instance.currentGrenade = null;
            }
        }
        else if (GameManager.instance.readySwap)
        {
            GameManager.instance.currentGun = currentSecondary.GetComponent<Gun>();
            StartCoroutine(EquipState(false, true, false, false, 4, 1));
            GameManager.instance.currentGrenade = null;
            GameManager.instance.currentMelee = null;
        }
    }

    void OnMelee()
    {
        if (GameManager.instance.readySwap)
        {
            GameManager.instance.currentMelee = currentMelee.GetComponent<Melee>();
            StartCoroutine(EquipState(false, false, false, true, 2, 5));
            animator.SetLayerWeight(4, 0f);
            GameManager.instance.currentGun = null;
            GameManager.instance.currentGrenade = null;
        }
    }

    void OnGrenade()
    {
        if (GameManager.instance.readySwap)
        {
            GameManager.instance.currentGrenade = currentGrenade.GetComponent<Grenade>();
            StartCoroutine(EquipState(false, false, true, false, 2, 0));
            animator.SetLayerWeight(4, 0f);
            GameManager.instance.currentGun = null;
            GameManager.instance.currentMelee = null;
            AimState(currentAimVirtualCamera, false, 3, 0, normalSensitivity);
        }

    }

    void OnSingleShot()
    {
        if (GameManager.instance.currentGrenade != null)
        {
            Debug.Log(GameManager.instance.readyShoot);
            if (GameManager.instance.readyShoot)
            {
                Debug.Log("ball");
                StartCoroutine(GameManager.instance.currentGrenade.GrenadeThrow(this));
                GameManager.instance.readyShoot = false;
            }
        }
        else if (GameManager.instance.currentGun != null)
        {
            //GameManager.instance.currentGun.isShooting = true;
            if (starterAssetsInputs.shoot && !GameManager.instance.currentGun.fullAuto)
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

    void OnHolster()
    {
        //isAiming = false;
        if (GameManager.instance.readySwap)
        {
            StartCoroutine(EquipState(false, false, false, false, 1, 0));
            animator.SetLayerWeight(4, 0f);
            GameManager.instance.currentGun = null;
            GameManager.instance.currentGrenade = null;
        }
    }

    void OnCrouch()
    {
        if (currentCrouchTimer == 0)
        {
            isCrouch = !isCrouch;
            isCrouching = !isCrouching;
            Debug.Log(isCrouch);
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
}
