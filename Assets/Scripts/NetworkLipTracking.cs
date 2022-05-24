using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ViveSR.anipal.Lip;

[RequireComponent(typeof(LipTrackingBehaviour))]
[DisallowMultipleComponent]

public class NetworkLipTracking : NetworkBehaviour, IBeforeUpdate
{
    public LipTrackingBehaviour LipV2;

    [Networked]
    [Capacity((int)LipShape_v2.Max)]
    private NetworkDictionary<LipShape_v2, float> PredictedLipShapeFrom => default;

    [Networked]
    [Capacity((int)LipShape_v2.Max)]
    private NetworkDictionary<LipShape_v2, float> PredictedLipShapeTo => default;

    private Dictionary<LipShape_v2, float> CurrentLipWeightings;

    private bool _wasRenderedThisUpdate;

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
        if (LipV2 == null)
        {
            LipV2 = GetComponent<LipTrackingBehaviour>();
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
            SRanipal_Lip_v2.GetLipWeightings(out CurrentLipWeightings);
        }
    }
}
