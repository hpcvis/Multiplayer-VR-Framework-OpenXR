using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnHitOwnershipTransfer : MonoBehaviourPun
{
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Paddle collision");
        var view = collision.gameObject.GetComponent<PhotonView>();
        if (!view)
        {
            Debug.Log("Collided object has no PhotonView.");
            return;
        }

        var owner = view.Owner;
        Debug.Log(owner);
        if (owner != null) // for some reason, photon Player objects don't automatically use null as a false value
        {
            Debug.Log("Current owner: " + owner.UserId);
            Debug.Log("Next owner: " + owner.GetNext().UserId);
            view.TransferOwnership(owner.GetNext());
        }
        else
        {
            Debug.Log("Collided object has no owner.");
        }
    }
}
