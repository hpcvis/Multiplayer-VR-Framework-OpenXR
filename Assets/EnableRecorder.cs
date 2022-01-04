using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRecorder : MonoBehaviour
{
    public Photon.Voice.Unity.Recorder recorder;
    public Photon.Voice.Unity.VoiceConnection connection;

    void Start()
    {
        recorder.Init(connection);
    }
}
