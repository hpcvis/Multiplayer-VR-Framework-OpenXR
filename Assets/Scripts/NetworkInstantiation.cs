using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class NetworkInstantiation : MonoBehaviourPunCallbacks
{
    [Tooltip("Should be set to the name of the current prefab, unless you explicitly need to instantate a different prefab.")]
    public string prefabName;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // We check for the master client here to avoid a race condition.
        // PhotonNetwork.InstantiateRoomObject() can only be called once the player is in a room.
        // Hence, we do so in OnJoinedRoom. However, that callback is only called once during the program lifecycle.
        // If we move to another scene, we will still need to instantiate room objects, which is why we do it here as well.
        if (PhotonNetwork.IsMasterClient)
        {
            InstantiateNetworkObject();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        InstantiateNetworkObject();
    }

    /// <summary>
    /// Replaces the current object with a room object specified by prefabName.
    /// </summary>
    private void InstantiateNetworkObject()
    {
        Destroy(this.gameObject);
        // prevents the newly-instantated room object from interacting with the about-to-be-deleted regular GameObject
        var collider = GetComponent<Collider>();
        if (collider)
        {
            collider.enabled = false;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            PhotonNetwork.InstantiateRoomObject(prefabName, pos, rot);
        }
    }
}
