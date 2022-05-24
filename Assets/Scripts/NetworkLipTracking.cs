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
    public LipTrackingBehaviour lipV2;

    [Networked]
    [Capacity((int)LipShape_v2.Max)]
    private NetworkDictionary<LipShape_v2, float> PredictedLipShapeFrom => default;

    [Networked]
    [Capacity((int)LipShape_v2.Max)]
    private NetworkDictionary<LipShape_v2, float> PredictedLipShapeTo => default;

    [Networked] 
    [Capacity((int)LipShape_v2.Max)]
    private NetworkDictionary<LipShape_v2, float> NetDictTest => default;

    private Dictionary<LipShape_v2, float> _currentLipWeightings;

    private bool _wasRenderedThisUpdate;

    private int count = 0;

    private void Awake()
    {
        if (SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING)
        {
            Debug.LogWarning((object) "NetworkLipTracking::Awake(): Lip Framework is not working. Removing component.");
            UnityEngine.Object.Destroy((UnityEngine.Object) this);
        }
        else
        {
            CacheLipBehavior();
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        if (SRanipal_Lip_Framework.Status != SRanipal_Lip_Framework.FrameworkStatus.WORKING)
        {
            Debug.LogWarning((object) "NetworkLipTracking::Awake(): Lip Framework is not working. Removing component.");
            UnityEngine.Object.Destroy((UnityEngine.Object) this);
        }
        else
        {
            CacheLipBehavior();
        }
    }

    private void CacheLipBehavior()
    {
        if (lipV2 == null)
        {
            lipV2 = GetComponent<LipTrackingBehaviour>();
        }
    }

    public void BeforeUpdate()
    {
        this._wasRenderedThisUpdate = false;
    }

    public override void Render()
    {
        if (this.InterpolationDataSource == NetworkBehaviour.InterpolationDataSources.NoInterpolation ||
            this._wasRenderedThisUpdate)
        {
            //this._wasRenderedThisUpdate = true;
        }
        else
        {
            //this._wasRenderedThisUpdate = true;
            ComputeUnInterpolatedLipWeights(NetDictTest);
            ApplyUnInterpolatedLipWeights(NetDictTest);
        }
    }

    private void ComputeUnInterpolatedLipWeights(NetworkDictionary<LipShape_v2, float> networkedLipWeights)
    {
        SRanipal_Lip_v2.GetLipWeightings(out var lipWeights);
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
        lipV2.UpdateLipShapes(lipWeights);
        Debug.Log(++count + ": {" + string.Join(",", lipWeights.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}");
    }
}
