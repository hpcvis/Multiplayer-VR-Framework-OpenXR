using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 12pt tracking lol
/// </summary>
[System.Serializable]
public class NetworkXRInputContainer
{
    // TODO: turn into a serializable dict?
    // issue would be ensuring the keys are correct
    public Transform head;
    public Transform pelvis;
    public Transform leftHand;
    public Transform leftElbow;
    public Transform leftShoulder;
    public Transform rightHand;
    public Transform rightElbow;
    public Transform rightShoulder;
    public Transform leftFoot;
    public Transform leftKnee;
    public Transform rightFoot;
    public Transform rightKnee;

    public void CopyFrom(NetworkXRInputContainer other)
    {
        head.position = other.head.position;
        head.rotation = other.head.rotation;
        leftHand.position = other.leftHand.position;
        leftHand.rotation = other.leftHand.rotation;
        rightHand.position = other.rightHand.position;
        rightHand.rotation = other.rightHand.rotation;
    }
}
