using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TempPingPongBallLogger : MonoBehaviourPun
{
    private void Update()
    {
        Debug.Log("Object: " + base.photonView.ViewID + " - Owner : " + base.photonView.Owner.UserId);
        Debug.Log("Object: " + base.photonView.ViewID + " - Owner : " + base.photonView.Owner.GetNext().UserId);
    }
}
