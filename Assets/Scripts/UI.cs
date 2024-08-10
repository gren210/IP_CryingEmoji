using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UI : ScriptManager
{
    Player thePlayer;

    Workbench workbench;

    public Animator animator;

    public Canvas transitionCanvas;

    [SerializeField]
    TextMeshProUGUI upgradeText;

    [SerializeField]
    GameObject upgradeInfo;

    [SerializeField]
    int currentUpgrade;

    [SerializeField]
    bool[] upgraded = { false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = GameManager.instance.thePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        workbench = GameManager.instance.currentWorkbench;
    }

    IEnumerator Transition(int index)
    {
        animator.SetBool("Transition",true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(index);
    }

    public void StartGame()
    {
        StartCoroutine(Transition(1));
        
    }

    public void QuitGame()
    {
        Application.Quit();
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

    public void UpgradeSelect(int upgradeIndex)
    {
        upgradeText.text = "" + workbench.currentUpgradeText[upgradeIndex];
        currentUpgrade = upgradeIndex;
        if (upgraded[upgradeIndex])
        {
            upgradeInfo.SetActive(false);
        }
        else
        {
            upgradeInfo.SetActive(true);
        }
    }

    public void Upgrade()
    {
        if(currentUpgrade == 0)
        {
            foreach (GameObject gun in GameManager.instance.thePlayer.primaryWeapons)
            {
                gun.GetComponent<Gun>().RPM = gun.GetComponent<Gun>().RPM * 1.2f;
            }
            foreach (GameObject gun in GameManager.instance.thePlayer.secondaryWeapons)
            {
                gun.GetComponent<Gun>().RPM = gun.GetComponent<Gun>().RPM * 1.2f;
            }
        }
        else if(currentUpgrade == 1)
        {
            foreach (GameObject gun in GameManager.instance.thePlayer.primaryWeapons)
            {
                gun.GetComponent<Gun>().swayAmplitude = gun.GetComponent<Gun>().swayAmplitude / 2;
                gun.GetComponent<Gun>().swaySpeed = gun.GetComponent<Gun>().swaySpeed / 2;
            }
            foreach (GameObject gun in GameManager.instance.thePlayer.secondaryWeapons)
            {
                gun.GetComponent<Gun>().swayAmplitude = gun.GetComponent<Gun>().swayAmplitude / 2;
                gun.GetComponent<Gun>().swaySpeed = gun.GetComponent<Gun>().swaySpeed / 2;
            }
        }
        else if (currentUpgrade == 2)
        {
            foreach (GameObject gun in GameManager.instance.thePlayer.primaryWeapons)
            {
                gun.GetComponent<Gun>().ammoCount = gun.GetComponent<Gun>().ammoCount * 1.5f;
            }
            foreach (GameObject gun in GameManager.instance.thePlayer.secondaryWeapons)
            {
                gun.GetComponent<Gun>().ammoCount = gun.GetComponent<Gun>().ammoCount * 1.5f;
            }
        }
        else if (currentUpgrade == 3)
        {
            foreach (GameObject gun in GameManager.instance.thePlayer.primaryWeapons)
            {
                gun.GetComponent<Gun>().silenced = true;
            }
            foreach (GameObject gun in GameManager.instance.thePlayer.secondaryWeapons)
            {
                gun.GetComponent<Gun>().silenced = true;
            }
        }
        upgraded[currentUpgrade] = true;

        if (upgraded[currentUpgrade])
        {
            upgradeInfo.SetActive(false);
        }
        else
        {
            upgradeInfo.SetActive(true);
        }
    }

    

}
