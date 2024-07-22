using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : ScriptManager
{
    [HideInInspector]
    public bool reloading = false;

    [SerializeField]
    float reloadTime;

    [SerializeField]
    int ammoCount;

    [SerializeField]
    int RPM;

    int currentAmmoCount;

    float currentCooldown;

    float bulletRange;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.currentGun = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Reload()
    {
        GameManager.instance.animator.SetLayerWeight(6, 1);
        reloading = true;
        GameManager.instance.animator.SetTrigger("isReloading");
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        GameManager.instance.animator.SetLayerWeight(6, 0);
    }

    public void Shoot(Player thePlayer)
    {
        RaycastHit hitInfo;
        if (currentCooldown <= 0f)
        {
            bool hit = Physics.Raycast(thePlayer.playerCamera.position, thePlayer.playerCamera.forward, out hitInfo, bulletRange);
            StartCoroutine(ShakeCameraOverTime(1f, 1f, .25f));
        }



    }
}
