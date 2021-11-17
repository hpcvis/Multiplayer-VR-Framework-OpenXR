using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventCameraHook : MonoBehaviour
{
    [Tooltip("List of names of canvas objects that require an event camera")]
    public string[] canvasNames;
    public Camera XRCamera;

    private void Awake()
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
