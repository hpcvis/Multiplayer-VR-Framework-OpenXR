using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Valve.VR.InteractionSystem;
using System;

//Transfers ownership on the server to the player interacting with it, or back to the server if nobody is interacting with it
public class TransferOwnership : MonoBehaviourPun
{
    //Transfers ownership of the interactable objects between players
    //This method gets invoked in the modified Interactable.cs when a player either touches or let's go of the given object
    //Transfer ownership also allows for lower latency for the person that is currently interacting with the object, since they are the owner they should see everything live
    //Other users will experience a short delay, however, it's worth the trade off in comparison to a universal delay
    public void transferOwnership()
    {
        base.photonView.RequestOwnership();
    }
}

