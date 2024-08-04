using Cinemachine;
using StarterAssets;
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

    public float bulletRange;

    public int damage;

    Transform virtualCamera;

    public Transform sphereLookAt;

    CinemachineImpulseSource shakeSource;

    public int gunIndex;

    bool reloadSmooth;

    float currentTimer;

    [HideInInspector]
    public bool isShooting = false;

    [SerializeField]
    GameObject gunAudio;

    Enemy currentEnemy;

    [SerializeField]
    GameObject bloodVFX;

    [SerializeField]
    GameObject smokeVFX;

    [SerializeField]
    GameObject muzzleVFX;

    [SerializeField]
    Transform muzzle;
   

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
        reloading = true;
        reloadSmooth = true;
        GameManager.instance.readySwap = false;
        GameManager.instance.animator.SetTrigger("isReloading");
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
        reloadSmooth = true;
    }

    public void Shoot(Player thePlayer)
    {
        
        RaycastHit hitInfo;
        GameObject smoke = Instantiate(smokeVFX, muzzle.position, muzzle.rotation);
        smoke.transform.SetParent(muzzle);
        Destroy(smoke, .5f);
        if (currentCooldown <= 0f && thePlayer.starterAssetsInputs.aim)
        {
            GameObject gunshot = Instantiate(gunAudio);
            Destroy(gunshot, 1f); 
            shakeSource.GenerateImpulseWithForce(.04f);
            GameManager.instance.isShooting = true;
            bool hit = Physics.Raycast(thePlayer.playerCamera.position, thePlayer.playerCamera.forward, out hitInfo, bulletRange);
            if (hit)
            {
                if(hitInfo.collider.TryGetComponent<Enemy>(out currentEnemy))
                {
                    Debug.Log("hell");
                    currentEnemy.TakeDamage(damage);
                    Destroy(Instantiate(bloodVFX, hitInfo.point,hitInfo.transform.rotation), 1f);
                }
            }
        }
    }

    public IEnumerator FullAutoShoot(Player thePlayer)
    {
        while (true)
        {
            if (fullAuto && !reloading && thePlayer.starterAssetsInputs.shoot && thePlayer.starterAssetsInputs.aim && thePlayer.thirdPersonController.Grounded)
            {
                Shoot(thePlayer);
                float shotDelay = 60f / RPM;
                yield return new WaitForSeconds(shotDelay);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override void Interact(Player thePlayer)
    {
        base.Interact(thePlayer);
        if (!secondary)
        {
            GameManager.instance.primaryBackpack[gunIndex] = true;
            if (thePlayer.currentPrimary != null)
            {
                thePlayer.primaryWeapons[thePlayer.currentPrimary.GetComponent<Gun>().gunIndex].SetActive(false);
            }
            thePlayer.currentPrimary = thePlayer.primaryWeapons[gunIndex];
            thePlayer.OnPrimaryEquip();
        }
        else
        {
            GameManager.instance.secondaryBackpack[gunIndex] = true;
            if (thePlayer.currentSecondary != null)
            {
                thePlayer.secondaryWeapons[thePlayer.currentSecondary.GetComponent<Gun>().gunIndex].SetActive(false);
            }
            thePlayer.currentSecondary = thePlayer.secondaryWeapons[gunIndex];
            thePlayer.OnSecondaryEquip();
        }
        Destroy(gameObject);
    }

}
