using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddVRCamera : MonoBehaviour
{
    // Since the player is instantiated at runtime, we need to add the camera to the canvas at runtime
    // As a result, this script needs to be added to the canvas in order to properly reference the camera
    // This means that you can now ignore the warning for the event camera
    private Camera cameraReference;

    // You need to manually add the canvas reference, so essentially this can be added anywhere
    // For the purposes of this script I just put it on the canvas itself
    public Canvas canvasReference;

    // We should only need to Update this once
    private bool foundCameraReference = false;


    void Update()
    {
        if (!foundCameraReference)
        {
            try
            { 
                // The following line of code will produce an error until the player is instantiated
                cameraReference = GameObject.FindWithTag("EventCamera").GetComponent<Camera>(); 
            }
            catch 
            {
            }
            if (cameraReference != null)
            {
                canvasReference.worldCamera = cameraReference;
                foundCameraReference = true;
            }
        }
    }
}
