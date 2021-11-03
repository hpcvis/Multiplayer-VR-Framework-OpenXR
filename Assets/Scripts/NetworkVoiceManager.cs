using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using System;

//Creates the network necessary for voice communication
//Enables the speakers
[RequireComponent(typeof(VoiceConnection))]
public class NetworkVoiceManager : MonoBehaviour
{
    public Transform remoteVoiceParent;

    private VoiceConnection voiceConnection;

    void Awake()
    {
        voiceConnection = GetComponent<VoiceConnection>();
    }

    // Adds a speaker currently on the list of speakers connected to the scene
    private void OnEnable()
    {
        voiceConnection.SpeakerLinked += this.OnSpeakerCreated;
    }

    // Removes a speaker currently on the list of speakers connected to the scene
    private void OnDisable()
    {
        voiceConnection.SpeakerLinked -= this.OnSpeakerCreated;
    }

    // When a speaker is created (either you or someone else joining the room) add it to the speaker
    private void OnSpeakerCreated(Speaker speaker)
    {
        speaker.transform.SetParent(this.remoteVoiceParent);
        speaker.OnRemoteVoiceRemoveAction += OnRemoteVoiceRemove;
    }

    private void OnRemoteVoiceRemove(Speaker speaker)
    {
        if (speaker != null)
        {
            Destroy(speaker.gameObject);
        }
    }
}