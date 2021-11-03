using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using Photon.Pun;

//Allows movement of the vr player using the trackpad and head rotation
public class TrackpadMovement : MonoBehaviour
{
    /*The hand that well press the button (left or right), set the value in  the editor*/
    public Hand hand;
    public Transform mainCamera;

    /*The buttton to be pressed, set the value in  the editor, may be changed in future to track finger placement on button, from steamVR plugin*/
    public SteamVR_Action_Vector2 trackpadPos;

    //How fast you move
    public float speed = 1.5f;

    void Start()
    {
        trackpadPos.AddOnChangeListener(OnTrackpadMovement, hand.handType); // create the action listener
    }

    //Moves this transform in relation to the where you are facing in vr and based off of where you are touching on the trackpad
    private void OnTrackpadMovement(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        Vector2 touchPos = trackpadPos.GetAxis(hand.handType);
        float yvalue = transform.localPosition.y;
        transform.Translate(Vector3.Normalize(new Vector3(touchPos.x, 0, touchPos.y)) * Time.deltaTime * speed, mainCamera);
        transform.localPosition = new Vector3(transform.localPosition.x, yvalue, transform.localPosition.z);
    }
}
