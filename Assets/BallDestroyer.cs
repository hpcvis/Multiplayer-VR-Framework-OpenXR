using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        Destroy(collision.gameObject);
    }
}
