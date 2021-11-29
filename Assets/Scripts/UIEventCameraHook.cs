using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Hook script that gives a reference to an XR HMD camera to any Canvases that require one.
/// Canvas names are used to find the Canvases themselves.
/// </summary>
public class UIEventCameraHook : HookBase
{
    [Tooltip("List of names of canvas objects that require an event camera")]
    public string[] canvasNames;
    public Camera XRCamera;

    protected override void Hook()
    {
        foreach (string c in canvasNames)
        {
            GameObject canvasObj = GameObject.Find(c);
            if (canvasObj)
            {
                Canvas canvas = canvasObj.GetComponent<Canvas>();
                canvas.worldCamera = XRCamera;
            }
        }
    }
}
