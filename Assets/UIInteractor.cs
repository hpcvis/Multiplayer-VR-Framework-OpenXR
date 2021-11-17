using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteractor : MonoBehaviour
{
    public Transform handTransform;

    private void Update()
    {
        RaycastHit result;
        Physics.Raycast(handTransform.position, Vector3.Normalize(handTransform.position), out result);
        Debug.Log("Raycast Origin: " + handTransform.position);
        Debug.Log("Raycast Direction: " + Vector3.Normalize(handTransform.position));
        Debug.Log("RaycastHit: " + result.point);
    }
}
