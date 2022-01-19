using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// I should mention that this is not a very good idea in the case of ping pong.
/// This implementation means that you can only hit the ball once, and then it falls out of your control.
/// Sure, that would work in an ideal situation, but in any other case it results in a weird user experience.
/// For example, you can't repeatedly bounce the ball on your paddle, which is a fun thing that people will probably try to do!
/// (At the very least, it's how I'm testing physics)
/// What if the player misses and has to re-serve?
/// There will be some time where the ball is not owned by that player despite it being on their side of the table.
/// 
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
