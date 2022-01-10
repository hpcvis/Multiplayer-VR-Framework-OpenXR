using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Changes the PhotonView of the current object in response to some event.
/// </summary>
public class SwapPhotonView : MonoBehaviour
{
    public PhotonRigidbodyView view1;
    public PhotonTransformView view2;

    public void Swap()
    {
        view1.enabled = !view1.enabled;
        view2.enabled = !view2.enabled;
    }
}
