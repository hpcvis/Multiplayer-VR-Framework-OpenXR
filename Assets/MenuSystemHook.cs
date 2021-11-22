using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
