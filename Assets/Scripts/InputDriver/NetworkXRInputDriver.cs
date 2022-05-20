using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkXRInputDriver : MonoBehaviour
{
    [SerializeField] private NetworkXRInputContainer inputTransforms;
    //[SerializeField] private float inputUpdatePeriod = 0.25f; 
    
    private NetworkXRInputReceiver receiver;

    //private float updateTimer = 0.0f;

    public void RegisterPlayerRig(NetworkXRInputReceiver receiver)
    {
        this.receiver = receiver;
    }

    public void UpdateTransforms()
    {
        Debug.Log("Updating network transforms");
        if (receiver)
            receiver.UpdateRigTransforms(inputTransforms);
    }

    private void Update()
    {
        // not sure why i thought a timer was necessary
        //updateTimer += Time.deltaTime;
        
        //if (updateTimer < inputUpdatePeriod) return;
        
        UpdateTransforms();
        //updateTimer = 0.0f;
    }
}
