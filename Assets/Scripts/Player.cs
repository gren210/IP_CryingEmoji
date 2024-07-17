using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;

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
    [SerializeField]
    Transform playerCamera;

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

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        playerLookAt.position = playerCamera.position + (playerCamera.forward * 10f); // Sets the position of the invisible dot that the player will always face.
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

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 35f); // Make rotations smooth
    }

}
