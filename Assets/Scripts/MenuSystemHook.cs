using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Hook script that provides a reference to a controller manager to the menu system.
/// This is used to facilitate the menu button that disables pointer lines.
/// The hook fires on player instantiation and on each scene change.
/// </summary>
public class MenuSystemHook : HookBase
{
    public string menuSystemName;
    public ControllerManager ctrlMgr = null;

    protected override void Hook()
    {
        GameObject obj = GameObject.Find(menuSystemName);
        if (!obj)
        {
            Debug.LogError("MenuSystemHook::Hook(): Could not find an object in the scene with name " + menuSystemName);
            return;
        }

        MenuSystem menu = obj.GetComponent<MenuSystem>();
        if (!obj)
        {
            Debug.LogError("MenuSystemHook::Hook(): Object " + menuSystemName + " has no component of type MenuSystem");
        }

        if (menu && ctrlMgr)
        {
            menu.ctrlMgr = ctrlMgr;
        }
    }
}
