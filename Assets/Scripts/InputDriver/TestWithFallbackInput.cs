using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWithFallbackInput : MonoBehaviour
{
    public GameObject inputPrefab;

    private void Awake()
    {
        var obj = Instantiate(inputPrefab, transform.position, transform.rotation);
        var driver = obj.GetComponent<NetworkXRInputDriver>();
        var receiver = GetComponent<NetworkXRInputReceiver>();
        driver.RegisterPlayerRig(receiver);
    }
}
