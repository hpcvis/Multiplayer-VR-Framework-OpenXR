using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;
using ViveSR.anipal.Lip;

[RequireComponent(typeof(LipTrackingBehaviour))]
[DisallowMultipleComponent]

public class NetworkLipTracking : NetworkBehaviour, IBeforeUpdate
{
    [Networked] 
    [Capacity((int)LipShape_v2.Max)]
    private NetworkDictionary<LipShape_v2, float> NetDictTest => default;

    private Dictionary<LipShape_v2, float> _currentLipWeightings;
    public LipTrackingBehaviour LipV2 { get; private set; }

    private void Awake()
    {
        if ((SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) && Object.HasInputAuthority)
        {
            Debug.LogWarning((object) "NetworkLipTracking::Awake(): Lip Framework is not working. Removing component.");
            //UnityEngine.Object.Destroy((UnityEngine.Object) this);
        }

        CacheLipBehavior();
    }

    public override void Spawned()
    {
        base.Spawned();
        if ((SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) && Object.HasInputAuthority)
        {
            Debug.LogWarning((object) "NetworkLipTracking::Awake(): Lip Framework is not working. Removing component.");
            //UnityEngine.Object.Destroy((UnityEngine.Object) this);
        }

        CacheLipBehavior();
    }

    private void CacheLipBehavior()
    {
        if (LipV2 == null)
        {
            LipV2 = GetComponent<LipTrackingBehaviour>();
        }
    }

    public void BeforeUpdate()
    {

    }

    public override void FixedUpdateNetwork()
    {
        ComputeUnInterpolatedLipWeights(NetDictTest);
        ApplyUnInterpolatedLipWeights(NetDictTest);
    }

    private void ComputeUnInterpolatedLipWeights(NetworkDictionary<LipShape_v2, float> networkedLipWeights)
    {
        var lipWeights = LipV2.GetLipWeightsDict();
        networkedLipWeights.Clear();
        foreach (var keyPair in lipWeights)
        {
            networkedLipWeights.Add(keyPair.Key, keyPair.Value);
        }
    }

    private void ApplyUnInterpolatedLipWeights(NetworkDictionary<LipShape_v2, float> networkedLipWeights)
    {
        var lipWeights = new Dictionary<LipShape_v2, float>();
        foreach (var keyPair in networkedLipWeights)
        {
            lipWeights.Add(keyPair.Key, keyPair.Value);
        }
        LipV2.UpdateLipShapes(lipWeights);
    }
}
