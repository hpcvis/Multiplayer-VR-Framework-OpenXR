using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ViveSR.anipal.Lip;

[RequireComponent(typeof(LipTrackingBehaviour))]
[DisallowMultipleComponent]

public class NetworkLipTracking : NetworkBehaviour
{
    [Networked] 
    [Capacity((int)LipShape_v2.Max)]
    private NetworkDictionary<LipShape_v2, float> NetDictTest => default;
    
    [Networked]
    private bool IsLipTrackingWorking { get; set; }

    private Dictionary<LipShape_v2, float> _currentLipWeightings;
    private LipTrackingBehaviour LipV2 { get; set; }

    private void Awake()
    {
        CacheLipBehavior();
    }

    public override void Spawned()
    {
        base.Spawned();
        CacheLipBehavior();
    }

    private void CacheLipBehavior()
    {
        if (LipV2 == null)
        {
            LipV2 = GetComponent<LipTrackingBehaviour>();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Want to check if object belongs to local player, because lip framework is a scene object, not an individual
        // component
        IsLipTrackingWorking = ((SRanipal_Lip_Framework.Status == SRanipal_Lip_Framework.FrameworkStatus.WORKING) &&
                                Object.HasInputAuthority);

        if (!IsLipTrackingWorking) return;
        
        ComputeUnInterpolatedLipWeights(NetDictTest);
        ApplyUnInterpolatedLipWeights(NetDictTest);
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
        
        //
        LipV2.UpdateLipShapes(lipWeights);
    }
}
