using Cinemachine;
using System.Collections;
using UnityEngine;

public class Gun : Interactable
{
    [HideInInspector]
    public bool reloading = false;

    [SerializeField]
    float reloadTime;

    public bool secondary;

    public bool fullAuto;

    [SerializeField]
    int ammoCount;

    [SerializeField]
    int RPM;

    int currentAmmoCount;

    float currentCooldown;

    float bulletRange;

    Transform virtualCamera;

    public Transform sphereLookAt;

    CinemachineImpulseSource shakeSource;

    public int gunIndex;

    bool reloadSmooth;

    float currentTimer;

    [HideInInspector]
    public bool isShooting = false;


   

    private void Awake()
    {
        shakeSource = gameObject.GetComponent<CinemachineImpulseSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.instance.currentGun = this;
    }

    // Update is called once per frame
    void Update()
    {
        virtualCamera = GameManager.instance.currentVirtualCamera.transform;
        

        if (reloadSmooth)
        {
            currentTimer += Time.deltaTime;
            if (reloading == true)
            {
                GameManager.instance.animator.SetLayerWeight(7, Mathf.Lerp(0, 1, currentTimer/.2f));
            }
            else
            {
                GameManager.instance.animator.SetLayerWeight(7, Mathf.Lerp(1, 0, currentTimer/.2f));
            }
            if (currentTimer > .2f)
            {
                if (reloading == true)
                {
                    GameManager.instance.animator.SetLayerWeight(7, 1);
                }
                else
                {
                    GameManager.instance.animator.SetLayerWeight(7, 0);
                    GameManager.instance.readySwap = true;
                }
                currentTimer = 0;
                reloadSmooth = false;
            }
        }

    }
    public IEnumerator Reload()
    {
        //GameManager.instance.animator.SetLayerWeight(7, 1);
        reloading = true;
        reloadSmooth = true;
        GameManager.instance.readySwap = false;
        GameManager.instance.animator.SetTrigger("isReloading");
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        reloadSmooth = true;
        //GameManager.instance.animator.SetLayerWeight(7, 0);
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
            Debug.Log("okk");
            if (fullAuto && thePlayer.starterAssetsInputs.shoot && thePlayer.starterAssetsInputs.aim && thePlayer.thirdPersonController.Grounded)
            {
                Shoot(thePlayer);
                yield return new WaitForSeconds(60/RPM);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override void Interact(Player thePlayer)
    {
        base.Interact(thePlayer);

    }

}
