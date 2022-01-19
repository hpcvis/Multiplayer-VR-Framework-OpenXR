using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Spawns an arbitrary prefab as a room object.
/// It's not used in the ping pong demo, but it might be useful to someone.
/// </summary>
public class SpawnNetworkedObject : MonoBehaviour
{
    [Tooltip("Prefab of the GameObject to spawn")]
    public GameObject prefab;

    /// <summary>
    /// Instantiates the object given by objectName as a room object
    /// with the position and rotation of this object.
    /// </summary>
    public void Spawn()
    {
        // photon instantiate
        PhotonNetwork.InstantiateRoomObject(prefab.name, this.transform.position, this.transform.rotation);

        // kill the spawner; it's useless now
        GameObject.Destroy(this.gameObject);
    }
}
