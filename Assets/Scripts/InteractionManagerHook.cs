using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

/// <summary>
/// Hook script that gives an interactable or an interactor a reference to its corresponding interaction manager.
/// The interaction manager is searched for by name.
/// The hook fires on player instantiation and on each scene change.
/// </summary>
public class InteractionManagerHook : HookBase
{
    public string interactionManagerName;
    public XRBaseInteractable interactable = null;
    public XRBaseInteractor interactor = null;

    protected override void Hook()
    {
        GameObject obj = GameObject.Find(interactionManagerName);
        if (!obj)
        {
            Debug.LogError("InteractionManagerHook::Hook(): Could not find an object in the scene with name " + interactionManagerName);
            return;
        }

        XRInteractionManager manager = obj.GetComponent<XRInteractionManager>();
        if (!manager)
        {
            Debug.LogError("InteractionManagerHook::Hook(): Object " + interactionManagerName + " has no component of type XRInteractionManager");
        }

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
