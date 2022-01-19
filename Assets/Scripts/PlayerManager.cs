using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity;
using System;

/// <summary>
/// Provides a reference to the player object that can be accessed anywhere.
/// Player object must be registered with SetLocalPlayerInstance() first.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager inst;
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene. Do not set manually")]
    public GameObject LocalPlayerInstance;
    public List<GameObject> players;

    void Awake()
    {
        if (!inst)
        {
            inst = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Sets the local player instance. For use in any instantiation scripts only.
    /// </summary>
    /// <param name="localPlayer">Instance of a Networked player that is the local player</param>
    public void SetLocalPlayerInstance(GameObject localPlayer)
    {
        LocalPlayerInstance = localPlayer;
    }
}
