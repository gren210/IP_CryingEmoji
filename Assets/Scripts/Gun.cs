using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : ScriptManager
{
    [HideInInspector]
    public bool reloading = false;

    [SerializeField]
    float reloadTime;

    public bool fullAuto;

    [SerializeField]
    int ammoCount;

    [SerializeField]
    int RPM;

    int currentAmmoCount;

    float currentCooldown;

    float bulletRange;

    [HideInInspector]
    public bool readySwap;

    Transform virtualCamera;

    public Transform sphereLookAt;

    CinemachineImpulseSource shakeSource;

    [HideInInspector]
    public bool isShooting = false;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.currentGun = this;
        shakeSource = gameObject.GetComponent<CinemachineImpulseSource>();
        if (fullAuto)
        {
            StartCoroutine(FullAutoShoot(GameManager.instance.thePlayer));
        }
    }

    // Update is called once per frame
    void Update()
    {
        virtualCamera = GameManager.instance.currentVirtualCamera.transform;
        if (!reloading)
        {
            readySwap = true;
        }
        else
        {
            readySwap = false;
        }
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
            //StartCoroutine(ShakeCameraOverTime(.7f, .7f, .1f));
            shakeSource.GenerateImpulseWithForce(.04f);
            GameManager.instance.isShooting = true;
        }
    }

    public IEnumerator FullAutoShoot(Player thePlayer)
    {
        while (true)
        {
            while (isShooting)
            {
                Shoot(thePlayer);
                Debug.Log("okk");
                yield return new WaitForSeconds(60/RPM);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
