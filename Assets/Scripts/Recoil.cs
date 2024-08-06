using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Recoil : MonoBehaviour
{
    Vector3 currentRotation;

    Vector3 targetRotation;

    Vector3 targetPosition;

    Vector3 currentPosition;

    Vector3 initialGunPosition;

    public Transform cam;

    [SerializeField]
    float recoilX;

    [SerializeField]
    float recoilY;

    [SerializeField]
    float recoilZ;

    [SerializeField]
    float kickbackZ;

    public float snappiness;

    public float returnAmount;

    // Start is called before the first frame update
    void Start()
    {
        initialGunPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);
        cam.rotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFunc()
    {
        //targetPosition -= new Vector3(0, 0, kickbackZ);
        targetRotation += new Vector3( recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    void back()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);
        transform.localPosition = currentPosition;
    }

}
