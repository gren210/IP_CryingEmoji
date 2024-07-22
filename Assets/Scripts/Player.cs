using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour
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
    private ThirdPersonController thirdPersonController;

    /// <summary>
    /// The starter assets input player script
    /// </summary>
    private StarterAssetsInputs starterAssetsInputs;

    /// <summary>
    /// The animator for the player controller.
    /// </summary>
    private Animator animator;

    public Rig rig;

    private bool isAiming;

    [HideInInspector]
    public int currentWeaponLayer;

    int previousWeaponLayer;

    float currentTimer = 0;

    public GameObject currentPrimary;

    public GameObject currentSecondary;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat("moveX", starterAssetsInputs.move.x);
        animator.SetFloat("moveY", starterAssetsInputs.move.y);
        if (isAiming)
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
        Vector3 aimDirection = (aimTarget - transform.position).normalized;
        //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 35f); // Make rotations smooth

        RaycastHit hitInfo;
        //Debug.DrawLine(playerCamera.position, playerCamera.position+ (playerCamera.forward * 999f), Color.red); // Draws raycast line in scene
        //Debug.DrawLine(transform.position, transform.position + (transform.forward * 999f), Color.red);
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitInfo))
        {
            //Debug.Log(hitInfo);
        }


        if (starterAssetsInputs.aim)
        {
            if(currentWeaponLayer == 3)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 4;
                AimState(true, previousWeaponLayer, 1f, aimSensitivity);
            }
            else if (currentWeaponLayer == 1)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 2;
                AimState(true, previousWeaponLayer, 1f, aimSensitivity);
            }
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 35f); // Make rotations smooth

        }
        else
        {
            if (currentWeaponLayer == 4)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 3;
                AimState(false, previousWeaponLayer, 0f, aimSensitivity);
            }
            else if (currentWeaponLayer == 2)
            {
                previousWeaponLayer = currentWeaponLayer;
                currentWeaponLayer = 1;
                AimState(false, previousWeaponLayer, 0f, aimSensitivity);
            }
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

    }

    void AimState(bool aimState, int layer, float layerWeight, float sens)
    {
        aimVirtualCamera.gameObject.SetActive(aimState);
        thirdPersonController.SetSensitivity(sens);
        //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(layer), layerWeight, Time.deltaTime * 12f));
        animator.SetBool("isAiming", aimState);
        isAiming = aimState;
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
            if (GameManager.instance.currentGun.reloading == false)
            {
                GameManager.instance.currentGun = currentPrimary.GetComponent<Gun>();
                currentPrimary.SetActive(true);
                currentSecondary.SetActive(false);
                animator.SetBool("isSecondary", false);
                animator.SetLayerWeight(2, 0f);
                WeaponState(3);
            }
        }
        else
        {
            GameManager.instance.currentGun = currentPrimary.GetComponent<Gun>();
            currentPrimary.SetActive(true);
            currentSecondary.SetActive(false);
            animator.SetBool("isSecondary", false);
            animator.SetLayerWeight(2, 0f);
            WeaponState(3);
        }
    }

    void OnSecondaryEquip()
    {
        //isAiming = false;
        if(GameManager.instance.currentGun != null)
        {
            if (GameManager.instance.currentGun.reloading == false)
            {
                GameManager.instance.currentGun = currentSecondary.GetComponent<Gun>();
                currentPrimary.SetActive(false);
                currentSecondary.SetActive(true);
                animator.SetBool("isSecondary", true);
                animator.SetLayerWeight(4, 0f);
                WeaponState(1);
            }
        }
        else
        {
            GameManager.instance.currentGun = currentSecondary.GetComponent<Gun>();
            currentPrimary.SetActive(false);
            currentSecondary.SetActive(true);
            animator.SetBool("isSecondary", true);
            animator.SetLayerWeight(4, 0f);
            WeaponState(1);
        }
       
    }

    void OnShoot()
    {
        if (GameManager.instance.currentGun != null)
        {
            GameManager.instance.currentGun.Shoot(this);
        }
    }

    void OnHolster()
    {
        //isAiming = false;
        if (GameManager.instance.currentGun.reloading == false)
        {
            currentPrimary.SetActive(false);
            currentSecondary.SetActive(false);
            animator.SetLayerWeight(2, 0f);
            animator.SetLayerWeight(4, 0f);
            WeaponState(0);
        }
    }

    void OnCrouch()
    {
        if (animator.GetLayerWeight(5) == 0)
        {
            //animator.SetLayerWeight(5, 1f);
        }
        else if (animator.GetLayerWeight(5) == 1)
        {
            //animator.SetLayerWeight(5, 0f);
        }


    }

    void WeaponState(int currentLayer)
    {
        AimState(false, currentWeaponLayer, 0f, normalSensitivity);
        currentTimer = 0;
        previousWeaponLayer = currentWeaponLayer;
        currentWeaponLayer = currentLayer;
    }

    void OnReload()
    {
        StartCoroutine(GameManager.instance.currentGun.Reload());
    }



}
