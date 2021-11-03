using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


//This script controls the hand animation, if a trigger is pressed, the hand is contracted, if it is not pressed, the hand is open
//This script is attached at each hand
public class HandAnimation : MonoBehaviour {

    //Reference to the animator of the hand
    private Animator _anim;

    [Tooltip("Insert the given hand that the script is attacehd to, such as Left Hand or Right Hand")]
    public SteamVR_Input_Sources inputSource;

    void Start ()
    {
        _anim = GetComponentInChildren<Animator>();
	}
	
	void Update ()
    {        
        //Checks to see whether or not you or squeezing the trigger on controller.
        if(SteamVR_Actions._default.Squeeze.GetAxis(inputSource) == 1f)
        {
            if (!_anim.GetBool("IsGrabbing"))
            {
                Debug.Log("Is grabbing");
                _anim.SetBool("IsGrabbing", true);
            }
        }
        else
        {
            //if we let go of grab, set IsGrabbing to false
            if(_anim.GetBool("IsGrabbing"))
            {
                Debug.Log("Is not grabbing");
                _anim.Play("IdleAnimation");
                _anim.SetBool("IsGrabbing", false);
            }
        }
    }
}
