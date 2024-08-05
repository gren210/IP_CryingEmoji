using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact(Player thePlayer)
    {

    }

    protected void ShakeCamera(float shakeIntensity, float shakeFrequency, CinemachineVirtualCamera currentVirtualCamera)
    {
        CinemachineBasicMultiChannelPerlin cinemachineComponent = currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineComponent.m_AmplitudeGain = shakeIntensity;
        cinemachineComponent.m_FrequencyGain = shakeFrequency;
    }

    protected IEnumerator ShakeCameraOverTime(float shakeIntensity, float shakeFrequency, float shakeTime, CinemachineVirtualCamera currentVirtualCamera)
    {
        ShakeCamera(shakeIntensity, shakeFrequency, currentVirtualCamera);
        yield return new WaitForSeconds(shakeTime);
        ShakeCamera(0, 0, currentVirtualCamera);
    }

}
