using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkXRInputReceiver : MonoBehaviour
{
    [SerializeField] private NetworkXRInputContainer rigTransforms;

    public void UpdateRigTransforms(NetworkXRInputContainer inputTransforms)
    {
        rigTransforms.CopyFrom(inputTransforms);
    }
}
