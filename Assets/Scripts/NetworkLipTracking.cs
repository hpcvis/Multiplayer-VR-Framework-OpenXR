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
    
    [Networked]
    public bool IsLipTrackingWorking { get; set; }

    private Dictionary<LipShape_v2, float> _currentLipWeightings;
    public LipTrackingBehaviour LipV2 { get; private set; }

    private void Awake()
    {
        IsLipTrackingWorking = ((SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) &&
                                Object.HasInputAuthority);
        
        CacheLipBehavior();
    }

    public override void Spawned()
    {
        base.Spawned();
        IsLipTrackingWorking = ((SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) &&
                                Object.HasInputAuthority);

        CacheLipBehavior();
    }

    private void CacheLipBehavior()
    {
        if (LipV2 == null)
        {
            LipV2 = GetComponent<LipTrackingBehaviour>();
        }

        _currentLipWeightings = LipV2.GetLipWeightsDict();
    }

    public void BeforeUpdate()
    {
        IsLipTrackingWorking = ((SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING) &&
                                Object.HasInputAuthority);
    }

    public override void FixedUpdateNetwork()
    {
        if (IsLipTrackingWorking)
        {        
            ComputeUnInterpolatedLipWeights(NetDictTest);
            ApplyUnInterpolatedLipWeights(NetDictTest);
        }
    }

    private void ComputeUnInterpolatedLipWeights(NetworkDictionary<LipShape_v2, float> networkedLipWeights)
    {
        if (!Object.HasInputAuthority) return;
        
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
