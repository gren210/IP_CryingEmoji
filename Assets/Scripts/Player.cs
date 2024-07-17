using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;

public class Player : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera aimVirtualCamera;

    [SerializeField]
    CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    Transform playerLookAt;

    [SerializeField]
    Transform playerCamera;

    [SerializeField]
    float normalSensitivity;

    [SerializeField]
    float aimSensitivity;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        playerLookAt.position = playerCamera.position + (playerCamera.forward * 7f);
        Vector3 aimTarget = playerLookAt.position;
        RaycastHit hitInfo;
        Debug.DrawLine(playerCamera.position, playerCamera.position+ (playerCamera.forward * 999f), Color.red); // Draws raycast line in scene
        Debug.DrawLine(transform.position, transform.position + (transform.forward * 999f), Color.red);
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitInfo))
        {
            Debug.Log(hitInfo);
        }


        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);

        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
        }
        aimTarget.y = transform.position.y; // Keeps the player face the x axis only so the whole player model does not face up and down
        Vector3 aimDirection = (aimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 50f);
    }

}
