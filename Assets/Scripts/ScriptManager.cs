/*
 * Author: Thaqif Adly Bin Mazalan
 * Date: 17/7/24
 * Description: Script manager base class for non-interactable objects
 */

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptManager : MonoBehaviour
{
    /// <summary>
    /// Locks or unlocks the cursor.
    /// </summary>
    /// <param name="isLock">True to lock the cursor, false to unlock it.</param>
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

    /// <summary>
    /// Shakes the camera.
    /// </summary>
    /// <param name="shakeIntensity">The intensity of the shake.</param>
    /// <param name="shakeFrequency">The frequency of the shake.</param>
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
        ShakeCamera(0,0);
    }

    protected IEnumerator SceneLoad(int sceneIndex)
    {
        GameManager.instance.immune = true;
        GameManager.instance.transition.GetComponent<Animator>().SetBool("Transition", true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneIndex);
    }

    protected virtual void ChangeMusic(int musicIndex)
    {
        GameManager.instance.musicSource.Stop();
        GameManager.instance.musicSource.clip = GameManager.instance.music[musicIndex];
        GameManager.instance.musicSource.Play();
    }
}
