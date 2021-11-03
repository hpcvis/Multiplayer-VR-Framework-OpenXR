using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

//Streams what other players are saying if their given local Speech to text is on
public class STTData : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //this is called by "CustomStreamingRecognizer.cs"
    public void sendText(string transcript)
    {
        GetComponent<PhotonView>().RPC("RPC_SendAudio", RpcTarget.Others, transcript);
        Debug.Log("Transcript: " + transcript);
    }

    [PunRPC]
    private void RPC_SendAudio(string transcript)
    {
        Debug.Log("Other player said: " + transcript);
    }
}
