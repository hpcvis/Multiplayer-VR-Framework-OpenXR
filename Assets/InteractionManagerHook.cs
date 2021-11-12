using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractionManagerHook : MonoBehaviour
{
    public string interactionManagerName;
    public XRBaseInteractable interactable = null;
    public XRBaseInteractor interactor = null;

    void Awake()
    {
        GameObject obj = GameObject.Find(interactionManagerName);
        XRInteractionManager manager = obj.GetComponent<XRInteractionManager>();
        if (interactable)
        {
            interactable.interactionManager = manager;
        }
        if (interactor)
        {
            interactor.interactionManager = manager;
        }
    }
}
