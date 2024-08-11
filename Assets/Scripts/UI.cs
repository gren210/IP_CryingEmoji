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
        GameManager.instance.transition.GetComponent<Animator>().SetBool("Transition", true);
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

    public void Resume()
    {
        Time.timeScale = 1;
        GameManager.instance.pauseUI.SetActive(false);
        GameManager.instance.thePlayer.playerInput.SwitchCurrentActionMap("Player");
        if (GameManager.instance.currentWorkbench == null && GameManager.instance.inventoryUI.activeSelf == false)
        {
            CursorLock(true);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        GameManager.instance.pauseUI.SetActive(false);
        GameManager.instance.playerUI.SetActive(false);
        GameManager.instance.inventoryUI.SetActive(false);
        GameManager.instance.workbenchUI.SetActive(false);
        StartCoroutine(SceneLoad(SceneManager.GetActiveScene().buildIndex));
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        GameManager.instance.pauseUI.SetActive(false);
        GameManager.instance.playerUI.SetActive(false);
        GameManager.instance.inventoryUI.SetActive(false);
        GameManager.instance.workbenchUI.SetActive(false);
        StartCoroutine(SceneLoad(0));
    }

    public void SwitchPrimary(int gunIndex)
    {
        bool backpack = GameManager.instance.primaryBackpack[gunIndex];
        Debug.Log(backpack);
        Debug.Log(GameManager.instance.readySwap);
        if (backpack && GameManager.instance.readySwap)
        {
            Debug.Log("hello");
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
        GameManager.instance.thePlayer.Upgrade1();
        GameManager.instance.thePlayer.Upgrade2();
        GameManager.instance.thePlayer.Upgrade3();
        GameManager.instance.thePlayer.Upgrade4();
        GameManager.instance.upgrades[currentUpgrade] = true;

        if (GameManager.instance.upgrades[currentUpgrade])
        {
            upgradeInfo.SetActive(false);
        }
        else
        {
            upgradeInfo.SetActive(true);
        }
    }

    public void Heal()
    {
        if (GameManager.instance.itemBackpack[1] > 0)
        {
            GameManager.instance.health += GameManager.instance.healAmount;
        }
    }

}
