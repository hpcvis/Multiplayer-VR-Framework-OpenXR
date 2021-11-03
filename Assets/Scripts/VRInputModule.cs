using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.EventSystems;

public class VRInputModule : BaseInputModule
{
    // This script is made to get a raycast from the player's hand and to then cast
    // It to whatever object it is hitting
    // This script also handles the VR input for the option menu, it allows it to act similar to a click for more overall usability

    public SteamVR_Input_Sources targetSource;
    public SteamVR_Action_Boolean clickAction;

    private GameObject currentObject = null;
    private PointerEventData pointerData = null;

    protected override void Awake()
    {
        base.Awake();

        pointerData = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        if (Camera.main != null)
        {
            pointerData.Reset();
            pointerData.position = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            currentObject = pointerData.pointerCurrentRaycast.gameObject;

            m_RaycastResultCache.Clear();

            HandlePointerExitAndEnter(pointerData, currentObject);

            if (clickAction.GetStateDown(targetSource))
                ProcessPress(pointerData);

            if (clickAction.GetStateUp(targetSource))
                ProcessRelease(pointerData);
        }
    }

    public PointerEventData GetPointerData()
    {
        return pointerData;
    }
    public void ProcessPress(PointerEventData dataPress)
    {
        dataPress.pointerPressRaycast = dataPress.pointerCurrentRaycast;

        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(currentObject, dataPress, ExecuteEvents.pointerDownHandler);

        if (newPointerPress == null)
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

        dataPress.pressPosition = dataPress.position;
        dataPress.pointerPress = newPointerPress;
        dataPress.rawPointerPress = currentObject;
    }
    public void ProcessRelease(PointerEventData dataPress)
    {
        ExecuteEvents.Execute(dataPress.pointerPress, dataPress, ExecuteEvents.pointerUpHandler);

        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

        if(dataPress.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(dataPress.pointerPress, dataPress, ExecuteEvents.pointerClickHandler);
        }


        eventSystem.SetSelectedGameObject(null);

        dataPress.pressPosition = Vector2.zero;
        dataPress.pointerPress = null;
        dataPress.rawPointerPress = null;
    }
}
