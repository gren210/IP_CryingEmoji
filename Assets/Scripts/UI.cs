using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class UI : MonoBehaviour
{
    Player thePlayer;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = GameManager.instance.thePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchPrimary(int gunIndex)
    {
        bool backpack = GameManager.instance.primaryBackpack[gunIndex];
        if (backpack && GameManager.instance.readySwap)
        {
            thePlayer.ActiveWeapons(false, false, false, false);
            thePlayer.currentPrimary = thePlayer.primaryWeapons[gunIndex];
            thePlayer.OnPrimaryEquip();
        }
    }

    public void SwitchSecondary(int gunIndex)
    {
        bool backpack = GameManager.instance.secondaryBackpack[gunIndex];
        if (backpack && GameManager.instance.readySwap)
        {
            thePlayer.ActiveWeapons(false, false, false, false);
            thePlayer.currentSecondary = thePlayer.secondaryWeapons[gunIndex];
            thePlayer.OnSecondaryEquip(); 
        }
    }

    public void SwitchMelee(int meleeIndex)
    {
        bool backpack = GameManager.instance.meleeBackpack[meleeIndex];
        if (backpack && GameManager.instance.readySwap)
        {
            thePlayer.ActiveWeapons(false, false, false, false);
            thePlayer.currentMelee = thePlayer.meleeWeapons[meleeIndex];
            thePlayer.OnMelee();
        }
    }

    public void SwitchGrenade(int grenadeIndex)
    {
        int backpack = GameManager.instance.grenadeBackpack[grenadeIndex];
        if (backpack > 0 && GameManager.instance.readySwap)
        {
            thePlayer.ActiveWeapons(false, false, false, false);
            thePlayer.currentGrenade = thePlayer.grenadeWeapons[grenadeIndex];
            thePlayer.OnGrenade();
        }
    }



}
