using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GameObject.Instantiate(ballPrefab, this.transform.position, this.transform.rotation);
    }
}
