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
    public GameObject[] controllers;
    public uint selectedController = 0;
    [SerializeField]
    InputActionProperty cycleInteractor;

    private void Awake()
    {
        foreach (GameObject ctrl in controllers)
        {
            ctrl.SetActive(false);
        }

        if (controllers.Length > 0)
        {
            controllers[selectedController].SetActive(true);
        }

        cycleInteractor.action.started += IncrementController;
    }

    private void IncrementController(InputAction.CallbackContext obj)
    {
        Debug.Log("Interaction incremented.");
        controllers[selectedController].SetActive(false);
        selectedController = (selectedController + 1) % (uint) controllers.Length;
        controllers[selectedController].SetActive(true);
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
