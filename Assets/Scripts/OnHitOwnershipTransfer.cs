using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Transfers the ownership of a colliding PhotonView object to the next player.
/// This is intended to be used in a situation with two players.
/// The idea is to transfer the ownership of a ping pong ball to the player that the ball is approaching
/// (i.e. after the other player hits it back with their paddle).
/// </summary>
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
        if (owner != null)
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
