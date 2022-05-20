using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FallbackCharacterController : MonoBehaviour
{
    public float movespeed = 2.0f;

    public InputActionProperty moveForward;
    
    void Update()
    {
        if (moveForward.action.inProgress)
        {
            transform.position += new Vector3(movespeed * Time.deltaTime, 0, 0);
        }
    }
}
