using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Base class for all hook scripts.
/// Used to set up callbacks that (re)execute the hook scripts.
/// </summary>
public abstract class HookBase : MonoBehaviour
{
    /// <summary>
    /// Fires Hook() on awake and registers callbacks.
    /// </summary>
    protected void Awake()
    {
        Hook();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Fires Hook().
    /// </summary>
    /// <param name="scene">The scene being loaded</param>
    /// <param name="mode">Mode that scene is being loaded in</param>
    protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Hook();
    }

    protected abstract void Hook();
}
