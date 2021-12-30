using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnHitOwnershipTransfer : MonoBehaviourPun
{
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Paddle collision");
        var owner = base.photonView.Owner;
        Debug.Log(owner);
        if (owner != null)
        {
            Debug.Log("Next owner: " + base.photonView.Owner.GetNext());
            base.photonView.TransferOwnership(base.photonView.Owner.GetNext());
        }
        else
        {
            Debug.Log("Collided object has no owner.");
        }
    }
}
