using Cinemachine;
using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;

public class Gun : Interactable
{
    Player player;

    [HideInInspector]
    public bool reloading = false;

    [SerializeField]
    float reloadTime;

    public bool secondary;

    public bool fullAuto;

    public float ammoCount;

    public float RPM;

    [HideInInspector]
    public float currentAmmoCount;

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

    public bool interactable;

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

    public float swayAmplitude;

    public float swaySpeed;

    public bool silenced = false;

    public GameObject[] attachmentObjects = { null, null, null, null };

    bool[] attachments = { false, false, false, false };

    private void Awake()
    {
        shakeSource = gameObject.GetComponent<CinemachineImpulseSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.thePlayer;
        currentAmmoCount = ammoCount;
        if (!interactable)
        {
            ShakeCamera(swayAmplitude, swaySpeed, player.aimVirtualCamera);
            ShakeCamera(swayAmplitude, swaySpeed, player.crouchAimVirtualCamera);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.currentVirtualCamera != null)
        {
            virtualCamera = GameManager.instance.currentVirtualCamera.transform;
        }

        if (reloadSmooth)
        {
            currentTimer += Time.deltaTime;
            if (reloading == true)
            {
                player.animator.SetLayerWeight(7, Mathf.Lerp(0, 1, currentTimer/.2f));
            }
            else
            {
                player.animator.SetLayerWeight(7, Mathf.Lerp(1, 0, currentTimer/.2f));
            }
            if (currentTimer > .2f)
            {
                if (reloading == true)
                {
                    player.animator.SetLayerWeight(7, 1);
                }
                else
                {
                    player.animator.SetLayerWeight(7, 0);
                    GameManager.instance.readySwap = true;
                }
                currentTimer = 0;
                reloadSmooth = false;
            }
        }

        if (!interactable)
        {
            ShakeCamera(swayAmplitude, swaySpeed, player.aimVirtualCamera);
            ShakeCamera(swayAmplitude, swaySpeed, player.crouchAimVirtualCamera);
        }



    }
    public IEnumerator Reload()
    {
        if (currentAmmoCount != ammoCount && ((secondary && GameManager.instance.secondaryAmmo[gunIndex] != 0) || (!secondary && GameManager.instance.primaryAmmo[gunIndex] != 0)))
        {
            reloading = true;
            reloadSmooth = true;
            GameManager.instance.readySwap = false;
            player.animator.SetTrigger("isReloading");
            yield return new WaitForSeconds(reloadTime);
            if (secondary)
            {
                GameManager.instance.secondaryAmmo[gunIndex] = RefillAmmo(GameManager.instance.secondaryAmmo[gunIndex]);
            }
            else
            {
                GameManager.instance.primaryAmmo[gunIndex] = RefillAmmo(GameManager.instance.primaryAmmo[gunIndex]);
            }
            reloading = false;
            reloadSmooth = true;
        }
    }

    float RefillAmmo(float ammoReserve) 
    {
        if (ammoReserve < (ammoCount - currentAmmoCount))
        {
            currentAmmoCount += ammoReserve;
            ammoReserve = 0;
        }
        else
        {
            ammoReserve -= (ammoCount - currentAmmoCount);
            currentAmmoCount = ammoCount;
        }
        Debug.Log(ammoCount - currentAmmoCount);
        return ammoReserve;
    }

    public void Shoot(Player thePlayer)
    {
        currentAmmoCount -= 1;
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
            if (fullAuto && currentAmmoCount > 0 && !reloading && thePlayer.starterAssetsInputs.shoot && thePlayer.starterAssetsInputs.aim && thePlayer.thirdPersonController.Grounded)
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
