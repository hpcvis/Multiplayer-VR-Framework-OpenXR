using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys network objects that enter this object's collider.
/// </summary>
public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        Photon.Pun.PhotonNetwork.Destroy(collision.gameObject);
    }
}
