using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Periodically generates ping pong balls.
/// </summary>
public class BallGenerator : MonoBehaviour
{
    public GameObject ballPrefab;
    public float interval = 2.0f;
    private float timer = 0.0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            SpawnBall();
            timer = 0.0f;
        }
    }

    public void SpawnBall()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
            Photon.Pun.PhotonNetwork.InstantiateRoomObject(
                ballPrefab.name, 
                this.transform.position,
                this.transform.rotation);
    }
}
