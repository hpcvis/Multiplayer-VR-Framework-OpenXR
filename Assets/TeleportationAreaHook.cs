using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Class that gives the TeleportationArea a reference to the TeleportationProvider.
/// The TeleportationArea is intended to find the TeleportationProvider when the scene loads,
/// but since we load the player in late, we need to do this ourselves.
/// </summary>
public class TeleportationAreaHook : MonoBehaviour
{
    [Tooltip("The name of the object in the scene that contains the TeleportationArea script.")]
    public string teleportationAreaName;
    public TeleportationProvider teleportationProvider;

    /// <summary>
    /// Gives the TeleportationArea a reference to the TeleportationProvider in the XR Rig.
    /// </summary>
    private void Awake()
    {
        GameObject obj = GameObject.Find(teleportationAreaName);
        if (obj)
        {
            TeleportationArea area = obj.GetComponent<TeleportationArea>();
            area.teleportationProvider = teleportationProvider;
        }
    }
}
