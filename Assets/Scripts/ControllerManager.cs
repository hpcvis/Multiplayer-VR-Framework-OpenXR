using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

/// <summary>
/// Object that mananages sets of interactors. Only one interactor may be active at a time.
/// Necessary, since only one XRBaseInteractor is allowed to be on an object with an ActionBasedController component.
/// </summary>
public class ControllerManager : MonoBehaviour
{
    public NetworkedPlayer networkedPlayer;
    public bool isRightHand;
    public uint selectedController = 0;

    public GameObject[] controllers;
    public Animator[] animators;
    public bool disablePointers;
    public XRInteractorLineVisual[] pointers;

    [Tooltip("Input action to cycle to the next controller.")]
    public InputActionProperty cycleInteractor;
    [Tooltip("Input action that will trigger an animation.")]
    public InputActionProperty triggerAnimation;


    private void Awake()
    {
        foreach (GameObject ctrl in controllers)
        {
            ctrl.SetActive(false);
        }

        // set selectedController to active
        if (controllers.Length > 0)
        {
            controllers[selectedController].SetActive(true);
            networkedPlayer.handAnimators[isRightHand ? 1 : 0] = animators[selectedController];
        }

        cycleInteractor.action.started += IncrementController;
    }

    private void Update()
    {
        animators[selectedController].SetBool("IsGrabbing", triggerAnimation.action.ReadValue<float>() > 0.0f);
        if (pointers.Length > 0 && pointers[selectedController])
        {
            pointers[selectedController].enabled = !disablePointers;
        }
    }

    /// <summary>
    /// Increments the controller index by one, looping back to 0 when necessary.
    /// </summary>
    /// <param name="obj">Unity input system callback context.</param>
    private void IncrementController(InputAction.CallbackContext obj)
    {
        Debug.Log("Controller incremented for " + (isRightHand ? "right" : "left") + " hand.");
        controllers[selectedController].SetActive(false);
        selectedController = (selectedController + 1) % (uint) controllers.Length;
        controllers[selectedController].SetActive(true);

        networkedPlayer.handAnimators[isRightHand ? 1 : 0] = animators[selectedController];
    }

    /// <summary>
    /// Sets the currently active controller.
    /// </summary>
    /// <param name="controllerIndex">Index of controller to switch to.</param>
    public void SetActiveController(uint controllerIndex)
    {
        if (controllerIndex >= controllers.Length)
        {
            Debug.LogError("ControllerManager::SetActiveController(): controllerIndex out of range.");
            return;
        }

        controllers[selectedController].SetActive(false);
        selectedController = controllerIndex;
        controllers[selectedController].SetActive(true);
    }
}
