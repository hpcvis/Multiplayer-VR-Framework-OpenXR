using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPhotonLogLevel : MonoBehaviour
{
    public Photon.Pun.PunLogLevel logLevel;

    private void Start()
    {
        Photon.Pun.PhotonNetwork.LogLevel = logLevel;
    }
}
