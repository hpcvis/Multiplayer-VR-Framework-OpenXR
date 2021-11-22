using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

/// <summary>
/// Transfers the ownership of an object that this script is placed on to the player that most recently interacted with it.
/// Ownership will transfer back to the server if nobody has interacted with it [is this true? I don't think so, we need to update the behavior so that it is true]
/// Ownership transfer allows for lower latency for the player that is currently interacting with the object.
/// This is important: the owner should see minimum latency for objects they interact with to increase immersion.
/// While other users will experience a short delay, this tradeoff is worth it, as opposed to a larger, universal delay for all players.
/// </summary>
public class TransferOwnership : MonoBehaviourPun
{
    /// <summary>
    /// Transfer ownership of this object to the player that invoked this callback.
    /// This callback should be registered in the XRBaseInteractable callbacks of the object.
    /// </summary>
    public void transferOwnership()
    {
        base.photonView.RequestOwnership();
        Debug.Log("Ownership of object " + this.name + " transferred to controller " + base.photonView.OwnerActorNr);
    }
}

