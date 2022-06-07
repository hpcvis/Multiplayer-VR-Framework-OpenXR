using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using GoogleCloudStreamingSpeechToText;
using UnityEngine;

[RequireComponent(typeof(StreamingRecognizer))]
public class NetworkSTT : EnchancedNetworkBehaviour
{
    [SerializeField] private bool enableDebug;

    private StreamingRecognizer recognizer;

    private void Awake()
    {
        recognizer = GetComponent<StreamingRecognizer>();
    }

    public void OnFinalResult(string sttOutput)
    {
        RPC_OnFinalResult(sttOutput);
    }

    public void OnInterimResult(string sttInterim)
    {
        if (!enableDebug) return;
        
        Debug.Log($"Interim result: {sttInterim}");
    }

    public void OnRecognizerInitialized()
    {
        Debug.Log("Speech to text service initialized and listening.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        recognizer.StartListening();
    }

    [ObserversRpc]
    private void RPC_OnFinalResult(string sttOutput)
    {
        Debug.Log($"Received sttOutput as RPC: {sttOutput}");
    }
}
