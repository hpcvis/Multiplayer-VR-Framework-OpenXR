using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//This script creates a laser pointer from the hand that was specified and from the laser that is specified on the given hand
//This script should go onto a player pointer prefab that is on the hand that is going to be used.
//The camera component should also be added and should be disabled. This is because raycasts can be made from the camera
//Without the need to implement our own raycast, as it's already done by the camera class
//Also note that the dot at the end is just any gameObject that is used to indicate what the line is pointing to and is purely visual

[RequireComponent(typeof(LineRenderer))]
public class Pointer : MonoBehaviour
{
    public float defaultLength = 5.0f;
    public GameObject dot;

    //This is using the custom VRInputModule that is made and not the InputModule that was given
    public VRInputModule inputModule;

    private LineRenderer lineRenderer = null;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        PointerEventData data = inputModule.GetPointerData();
        //Following line allows it to change in length, however, it doesn't work and will result in an error
        //float targetLength = data.pointerCurrentRaycast.distance == 0 ? defaultLength : data.pointerCurrentRaycast.distance;
        float targetLength = 4.0f;

        RaycastHit hit = CreateRaycast(targetLength);

        Vector3 endPos = transform.position + (transform.forward * targetLength);

        if (hit.collider != null)
            endPos = hit.point;

        dot.transform.position = endPos;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPos);
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }
}