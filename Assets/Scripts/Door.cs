/*
 * Author: Thaqif Adly Bin Mazalan
 * Date: 30/6/24
 * Description: Handles the functionality of doors
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Interactable
{
    /// <summary>
    /// Indicates whether the door is opened.
    /// </summary>
    [HideInInspector]
    public bool opened;

    /// <summary>
    /// Indicates whether the door is locked.
    /// </summary>
    public bool locked;

    /// <summary>
    /// Determines if the door can be interacted with.
    /// </summary>
    public bool isInteractable;

    /// <summary>
    /// Duration for the door to open or close.
    /// </summary>
    public float openDuration;

    /// <summary>
    /// Current duration of the rotation.
    /// </summary>
    float currentDuration;

    /// <summary>
    /// Audio source for the door's opening sound.
    /// </summary>
    [SerializeField]
    AudioSource openingSound;

    /// <summary>
    /// Indicates whether the door is currently opening.
    /// </summary>
    [HideInInspector]
    public bool opening = false;

    /// <summary>
    /// Indicates whether the door is currently closing.
    /// </summary>
    [HideInInspector]
    public bool closing = false;

    /// <summary>
    /// Rotation values for the door.
    /// </summary>
    public float rotationX;
    public float rotationY;
    public float rotationZ;

    /// <summary>
    /// Position values for the door.
    /// </summary>
    public float positionX;
    public float positionY;
    public float positionZ;

    /// <summary>
    /// Vector3 variables for lerping.
    /// </summary>
    Vector3 startRotation;
    Vector3 targetRotation;
    Vector3 startPosition;
    Vector3 targetPosition;

    /// <summary>
    /// Determines if there is an extraction sequence for level 4.
    /// </summary>
    [SerializeField]
    bool extraction;

    /// <summary>
    /// Time for extraction sequence.
    /// </summary>
    [SerializeField]
    float extractionTime;

    /// <summary>
    /// Timer for extraction sequence.
    /// </summary>
    [HideInInspector]
    public float currentExtractionTimer;

    /// <summary>
    /// Determines if extraction is over.
    /// </summary>
    bool extracted = false;

    // Start is called before the first frame update
    void Start()
    {
        currentExtractionTimer = extractionTime;
    }

    // Update is called once per frame
    void Update()
    {

        if (opening)
        {
            currentDuration += Time.deltaTime;
            float t = currentDuration / openDuration;
            if (rotationX + rotationY + rotationZ != 0f)
            {
                transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
            }
            if (positionX + positionY + positionZ != 0f)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            }

            if (currentDuration >= openDuration)
            {
                currentDuration = 0f;
                transform.eulerAngles = targetRotation;
                transform.position = targetPosition;
                opening = false;
                opened = true;
                openingSound.Stop();
            }
        }

        if (closing)
        {
            currentDuration += Time.deltaTime;
            float t = currentDuration / openDuration;
            if (rotationX + rotationY + rotationZ != 0f)
            {
                transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
            }
            if (positionX + positionY + positionZ != 0f)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            }

            if (currentDuration >= openDuration)
            {
                currentDuration = 0f;
                transform.eulerAngles = targetRotation;
                transform.position = targetPosition;
                closing = false;
                opened = false;
                openingSound.Stop();
            }
        }
    }

    /// <summary>
    /// Opens the door with animation.
    /// </summary>
    public void OpenDoor()
    {
        if (!opening)
        {
            openingSound.Play();
            startRotation = transform.eulerAngles;
            targetRotation = startRotation;
            targetRotation.x += rotationX;
            targetRotation.y += rotationY;
            targetRotation.z += rotationZ;

            startPosition = transform.position;
            targetPosition = startPosition;
            targetPosition.x += positionX;
            targetPosition.y += positionY;
            targetPosition.z += positionZ;

            opening = true;
        }
    }

    /// <summary>
    /// Closes the door with animation.
    /// </summary>
    public void CloseDoor()
    {
        if (!closing)
        {
            openingSound.Play();
            startRotation = transform.eulerAngles;
            targetRotation = startRotation;
            targetRotation.x -= rotationX;
            targetRotation.y -= rotationY;
            targetRotation.z -= rotationZ;

            startPosition = transform.position;
            targetPosition = startPosition;
            targetPosition.x -= positionX;
            targetPosition.y -= positionY;
            targetPosition.z -= positionZ;

            closing = true;
        }
    }

    /// <summary>
    /// Handles interaction with the player.
    /// </summary>
    /// <param name="thePlayer">The player interacting with the door.</param>
    public override void Interact(Player thePlayer)
    {
        if (isInteractable && !locked)
        {
            base.Interact(thePlayer);
            if (opened)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }
    }
}

