using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Associates a Photon Voice Recorder with a VoiceConnection and enables the Recorder.
/// </summary>
public class EnableRecorder : MonoBehaviour
{
    public Photon.Voice.Unity.Recorder recorder;
    public Photon.Voice.Unity.VoiceConnection connection;

    void Start()
    {
        recorder.Init(connection);
    }
}
