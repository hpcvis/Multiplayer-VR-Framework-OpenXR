using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Photon.Voice.PUN;

public class EnableVoiceTransmission : MonoBehaviour
{
    public GameObject networkedPlayer;
    private PhotonVoiceView networkedVoice;

    [Tooltip("Input action to toggle audio transmission.")]
    public InputActionProperty toggleAudio;

    // Start is called before the first frame update
    private void Awake()
    {
        toggleAudio.action.started += ToggleAudioTransmission;
    }
    void Start()
    {
        networkedVoice = networkedPlayer.GetComponentInChildren(typeof(PhotonVoiceView)) as PhotonVoiceView;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ToggleAudioTransmission (InputAction.CallbackContext obj)
    {
        networkedVoice.RecorderInUse.TransmitEnabled = !networkedVoice.RecorderInUse.TransmitEnabled;
        Debug.Log("EnableVoiceTransmission::ToggleAudioTransmission(): Toggled Transmit Enabled: " + networkedVoice.RecorderInUse.TransmitEnabled);
    }
}
