using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Hook script that provides a reference to a controller manager to the menu system.
/// This is used to facilitate the menu button that disables pointer lines.
/// The hook fires on player instantiation and on each scene change.
/// </summary>
public class MenuSystemHook : MonoBehaviour
{
    public string menuSystemName;
    public ControllerManager ctrlMgr = null;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Hook();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Hook();
    }

    private void Hook()
    {
        GameObject obj = GameObject.Find(menuSystemName);
        MenuSystem menu = obj.GetComponent<MenuSystem>();
        if (menu && ctrlMgr)
        {
            menu.ctrlMgr = ctrlMgr;
        }
    }
}
