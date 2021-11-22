using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

/// <summary>
/// Object that mananages sets of interactors. One interactor may be active at a time.
/// Necessary, since only one XRBaseINteractor is allowed to be on an object with an ActionBasedController component.
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

    [SerializeField]
    public InputActionProperty cycleInteractor;

    [SerializeField]
    public InputActionProperty triggerAnimation;

    private void Awake()
    {
        foreach (GameObject ctrl in controllers)
        {
            ctrl.SetActive(false);
        }

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
        if (pointers.Length > 0)
        {
            pointers[selectedController].enabled = !disablePointers;
        }
    }

    private void IncrementController(InputAction.CallbackContext obj)
    {
        Debug.Log("Interaction incremented.");
        controllers[selectedController].SetActive(false);
        selectedController = (selectedController + 1) % (uint) controllers.Length;
        controllers[selectedController].SetActive(true);

        networkedPlayer.handAnimators[isRightHand ? 1 : 0] = animators[selectedController];
    }

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
