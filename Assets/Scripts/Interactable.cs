using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact(Player thePlayer)
    {

    }

    protected void ShakeCamera(float shakeIntensity, float shakeFrequency)
    {
        CinemachineBasicMultiChannelPerlin cinemachineComponent = GameManager.instance.currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineComponent.m_AmplitudeGain = shakeIntensity;
        cinemachineComponent.m_FrequencyGain = shakeFrequency;
    }

    protected IEnumerator ShakeCameraOverTime(float shakeIntensity, float shakeFrequency, float shakeTime)
    {
        ShakeCamera(shakeIntensity, shakeFrequency);
        yield return new WaitForSeconds(shakeTime);
        ShakeCamera(0, 0);
    }

}
