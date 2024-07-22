using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [HideInInspector]
    public bool reloading = false;

    [SerializeField]
    float reloadTime;

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
}
