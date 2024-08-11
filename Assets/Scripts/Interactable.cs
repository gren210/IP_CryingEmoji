using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public AudioClip interactSound;

    public string interactText;

    public virtual void Interact(Player thePlayer)
    {
        AudioSource.PlayClipAtPoint(interactSound,transform.position);
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

    protected void CursorLock(bool isLock)
    {
        if (isLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    protected void ChangeSound(AudioClip soundToChange, bool play)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = soundToChange;
        if (play)
        {
            GetComponent<AudioSource>().Play();
        }
    }


}
