using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FallbackCharacterController : MonoBehaviour
{
    [SerializeField] private Transform[] hands = new Transform[2];
    
    [SerializeField] private InputActionProperty moveForward;
    [SerializeField] private InputActionProperty grab;
    
    [SerializeField] private float movementSpeed = 2.0f;

    private void Update()
    {
        if (moveForward.action.inProgress)
        {
            var value = moveForward.action.ReadValue<Vector2>();
            Vector2 normalizedInput = movementSpeed * Time.deltaTime * value.normalized;
            Vector3 displacement = (transform.forward * normalizedInput.y) + (transform.right * normalizedInput.x);
            transform.position += displacement;
        }

        if (grab.action.triggered)
        {
            ArticulateArms();
        }
    }

    public void ArticulateArms()
    {
        foreach (var hand in hands)
        {
            hand.position += new Vector3(0, 1, 0);
        }
    }
}