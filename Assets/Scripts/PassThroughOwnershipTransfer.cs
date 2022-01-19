using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PassThroughOwnershipTransfer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered ownership transfer zone.");
        var view = other.gameObject.GetComponent<PhotonView>();
        if (!view)
        {
            Debug.Log("Collided object has no PhotonView.");
            return;
        }

        var owner = view.Owner;
        var nextOwner = view.Owner.GetNext();
        if (owner != null && nextOwner != null)
        {
            Debug.Log("Current owner: " + owner.UserId);
            Debug.Log("Next owner: " + nextOwner.UserId);
            view.TransferOwnership(nextOwner);
        }
        else
        {
            Debug.Log("Collided object has no owner.");
        }
    }
}
