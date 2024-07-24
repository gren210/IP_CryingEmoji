using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    /// <summary>
    /// The distance the grenade is thrown.
    /// </summary>
    [SerializeField]
    float grenadeDistance;

    /// <summary>
    /// The delay before the grenade explodes.
    /// </summary>
    public float delay = 3f;

    /// <summary>
    /// Timer to track the grenade's delay.
    /// </summary>
    float timer;

    /// <summary>
    /// The particle system for the grenade explosion effect.
    /// </summary>
    public ParticleSystem effect;

    /// <summary>
    /// The radius where the grenade will affect entities.
    /// </summary>
    public float explodeRadius = 50f;

    /// <summary>
    /// The damage dealt by the grenade explosion.
    /// </summary>
    public float explodeDamage = 70f;

    /// <summary>
    /// Indicates whether the grenade has been thrown.
    /// </summary>
    [HideInInspector]
    public bool thrown = false;

    /// <summary>
    /// Indicates whether the grenade has been set up for throwing.
    /// </summary>
    bool set = true;

    /// <summary>
    /// Indicates whether the grenade has exploded.
    /// </summary>
    bool exploded = false;

    /// <summary>
    /// The sound played when the grenade explodes.
    /// </summary>
    [SerializeField]
    AudioClip grenadeSound;

    /// <summary>
    /// The duration of the camera shake effect.
    /// </summary>
    [SerializeField]
    float shakeTimer;

    /// <summary>
    /// The starting value of the shake timer.
    /// </summary>
    float shakeTimerStart;

    /// <summary>
    /// The intensity of the camera shake effect.
    /// </summary>
    [SerializeField]
    float shakeIntensity;

    /// <summary>
    /// The frequency of the camera shake effect.
    /// </summary>
    [SerializeField]
    float shakeFrequency;

    [SerializeField]
    bool realGrenade;

    [SerializeField]
    GameObject realGrenadeObject;

    GameObject grenadeObject = null;

    Player currentPlayer;

    float currentTimer = 0;

    bool throwing = false;

    // Start is called before the first frame update
    void Start()
    {
        if (realGrenade)
        {
            StartCoroutine(GrenadeTick());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(throwing)
        {
            GameManager.instance.thePlayer.FaceForward();
        }
        if (thrown)
        {
            GameManager.instance.thePlayer.FaceForward();
            currentTimer += Time.deltaTime;
            GameManager.instance.animator.SetLayerWeight(6, Mathf.Lerp(1, 1-currentTimer, 1f));
            if(currentTimer > 1f)
            {
                GameManager.instance.animator.SetLayerWeight(6, 0);
                currentTimer = 0;
                thrown = false;
                GameManager.instance.readySwap = true;
                GameManager.instance.readyShoot = true;
            }
        }
    }

    private void GrenadeExplode()
    {
        //effect.GetComponent<ParticleSystem>().Play();

        //AudioSource.PlayClipAtPoint(grenadeSound, transform.position);

        Collider[] entities = Physics.OverlapSphere(transform.position, explodeRadius);

        foreach (Collider collider in entities)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                //UpdateEnemyHealth(collider.gameObject, explodeDamage);
            }
            else if (collider.gameObject.tag == "Boss")
            {
                //UpdateEnemyHealth(collider.gameObject, explodeDamage);
            }
            else if (collider.gameObject.tag == "Player") //&& !GameManager.instance.isImmune)
            {
                //GameManager.instance.playerHealth -= explodeDamage;
            }
        }
        Destroy(gameObject);
    }

    public IEnumerator GrenadeThrow(Player thePlayer)
    {
        throwing = true;
        GameManager.instance.readySwap = false;
        thePlayer.animator.SetLayerWeight(6, 1);
        thePlayer.animator.SetTrigger("Throw");
        yield return new WaitForSeconds(.35f / .6f);
        throwing = false;
        grenadeObject = Instantiate(realGrenadeObject, gameObject.transform.position, gameObject.transform.rotation);
        grenadeObject.GetComponent<Grenade>().thrown = true;
        gameObject.SetActive(false);

    }

    IEnumerator GrenadeTick()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(grenadeDistance * GameManager.instance.playerCamera.transform.forward);
        yield return new WaitForSeconds(delay);
        GrenadeExplode();
    }

    


}
