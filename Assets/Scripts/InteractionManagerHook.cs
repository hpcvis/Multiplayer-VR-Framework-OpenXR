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
public class InteractionManagerHook : MonoBehaviour
{
    public string interactionManagerName;
    public XRBaseInteractable interactable = null;
    public XRBaseInteractor interactor = null;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Hook();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Hook();
    }

    private void Hook()
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
