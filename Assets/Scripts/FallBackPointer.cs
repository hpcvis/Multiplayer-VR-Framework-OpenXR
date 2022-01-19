using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script is obsolete, but may be handy for reference in case fallback players need to be implemented again.
/// </summary>
[RequireComponent(typeof(Camera))]
public class FallBackPointer : MonoBehaviour
{
    Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                IPointerClickHandler click = hit.collider.gameObject.GetComponent<IPointerClickHandler>();
                if (click != null)
                {
                    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                    
                    click.OnPointerClick(pointerEventData);
                }

            }
        }
    }
}
