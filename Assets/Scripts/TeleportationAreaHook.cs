using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

/// <summary>
/// Hook script that gives the TeleportationArea a reference to the TeleportationProvider.
/// The TeleportationArea is intended to find the TeleportationProvider when the scene loads,
/// but since we load the player in late, we need to do this ourselves.
/// </summary>
public class TeleportationAreaHook : HookBase
{
    [Tooltip("The name of the object in the scene that contains the TeleportationArea script.")]
    public string teleportationAreaName;
    public TeleportationProvider teleportationProvider;

    protected override void Hook()
    {
        GameObject obj = GameObject.Find(teleportationAreaName);
        if (!obj)
        {
            Debug.LogError("TeleportationAreaHook::Hook(): Could not find an object in the scene with name " + teleportationAreaName);
            return;
        }

        TeleportationArea area = obj.GetComponent<TeleportationArea>();
        if (!area)
        {
            Debug.LogError("TeleportationAreaHook::Hook(): Object " + teleportationAreaName + " has not component of type TeleportationArea");
            return;
        }

        area.teleportationProvider = teleportationProvider;
    }
}
