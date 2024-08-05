using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Interactable
{
    public bool isSecondary;

    public int ammoGunIndex;

    public int ammoCount;

    public override void Interact(Player thePlayer)
    {
        base.Interact(thePlayer);
        if(isSecondary )
        {
            GameManager.instance.secondaryAmmo[ammoGunIndex] += ammoCount;
        }
        else
        {
            GameManager.instance.primaryAmmo[ammoGunIndex] += ammoCount;
        }
        Destroy(gameObject);
    }
}
