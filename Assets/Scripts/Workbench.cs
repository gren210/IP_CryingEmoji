using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : Interactable
{
    public GameObject virtualCamera;

    GameObject workbenchUI;

    public Transform gunTransform;

    [HideInInspector]
    public int[] currentUpgrade = {0, 0, 0, 0};

    public string[] currentUpgradeText;

    // Start is called before the first frame update
    void Start()
    {
        workbenchUI = GameManager.instance.workbenchUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(Player thePlayer)
    {
        base.Interact(thePlayer);
        virtualCamera.SetActive(true);
        StartCoroutine(WorkbenchOpen());
        CursorLock(false);
        GameManager.instance.currentWorkbench = this;
    }

    public IEnumerator WorkbenchOpen()
    {
        yield return new WaitForSeconds(.5f);
        workbenchUI.SetActive(true);
        GameManager.instance.workbenchUIObject.UpgradeSelect(0);
    }

    

}
