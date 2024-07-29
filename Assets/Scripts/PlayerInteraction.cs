using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class PlayerInteraction : MonoBehaviour
{
    Player x;

    Gun currentGunPickup;

    Grenade currentGrenadePickup;

    Melee currentMeleePickup;



    // Start is called before the first frame update
    void Start()
    {
        x = gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        Debug.DrawLine(x.playerCamera.position, x.playerCamera.position+ (x.playerCamera.forward * 999f), Color.red); // Draws raycast line in scene
        //Debug.DrawLine(transform.position, transform.position + (transform.forward * 999f), Color.red);
        if (Physics.Raycast(x.playerCamera.position, x.playerCamera.forward, out hitInfo, x.interactionDistance))
        {
            if(hitInfo.transform.TryGetComponent<Gun>(out currentGunPickup))
            {
                Debug.Log(currentGunPickup);
            }
            else if (hitInfo.transform.TryGetComponent<Grenade>(out currentGrenadePickup))
            {

            }
            else if(hitInfo.transform.TryGetComponent<Melee>(out currentMeleePickup))
            {

            }
        }
    }

    void OnInteract()
    {
        if (currentGunPickup != null)
        {
            currentGunPickup.Interact(x);
        }
        if(currentGrenadePickup != null)
        {
            currentGrenadePickup.Interact(x);
        }
        if(currentMeleePickup != null)
        {
            currentMeleePickup.Interact(x);
        }
    }
}
