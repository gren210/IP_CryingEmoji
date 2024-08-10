/*
 * Author: Thaqif Adly Bin Mazalan
 * Date: 30/6/24
 * Description: Script that manages the scenes and transitions to other scenes.
 */

using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : ScriptManager
{
    /// <summary>
    /// Index of the scene to switch to.
    /// </summary>
    public int sceneIndex;

    /// <summary>
    /// GameObject which stores the transition for scene changes.
    /// </summary>
    GameObject transition;

    /// <summary>
    /// Animator for handling transition animations.
    /// </summary>
    Animator transitionAnimator;

    /// <summary>
    /// Time duration for the transition animation.
    /// </summary>
    [SerializeField]
    float transitionTime;

    /// <summary>
    /// Timer for tracking transition time.
    /// </summary>
    float currentTimer;

    /// <summary>
    /// Bool to check if a scene change is occurring.
    /// </summary>
    bool changeScene;

    /// <summary>
    /// Bool to check if this is the first scene.
    /// </summary>
    [SerializeField]
    bool firstScene;

    /// <summary>
    /// This start function starts each scene with the correct settings
    /// </summary>
    void Start()
    {
        CursorLock(true);
        currentTimer = 0;
        changeScene = false;

        //transition = GameManager.instance.transition;
        //transitionAnimator = GameManager.instance.transitionAnimator;
        //if (!firstScene || GameManager.instance.hasRestarted)
        //{
        //transitionAnimator.SetTrigger("Start");
        //}

        // Runs when the first level (spaceship) is loaded.
        //if (SceneManager.GetActiveScene().buildIndex == 6)
        //{
        //GameManager.instance.objectiveText.text = GameManager.instance.objectiveStrings[0];
        //GameManager.instance.UI.SetActive(true);
        //ChangeMusic(5);
        //}

        GameManager.instance.playerUI.SetActive(true);

    }

    /// <summary>
    /// This update function just facilitates the transition animation.
    /// </summary>
    void Update()
    {
        if (changeScene)
        {
            //GameManager.instance.isImmune = true;
        }
    }

    /// <summary>
    /// Triggers a scene change when the player enters the collider.
    /// </summary>
    /// <param name="other">Collider that the player enters.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
