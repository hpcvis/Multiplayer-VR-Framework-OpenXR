using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Instantiates the interactables and the players.
/// This script needs to be put onto any object, preferably a common spawn point object.
/// 
/// To use: Drag the player onto the player prefab into its respective locations in the editor
/// Drag the spawn points for the players and the spectators into their respective locations in the editor
/// </summary>
/// <remarks>
/// Note (Important): Ensure that both spawn points and spectator spawn points are within a parent prefab (as in, the actual spawnpoint locations are subsets of a spawnpoints prefab)
/// This is to allow for multiple players to spawn in different locations
/// For implementation, it is better to have it this way anyways, because this helps with being able to distinguish where the players should be spawning in
/// Refer to the Template scene and look at the "PUN Spawns" in the heiarchy to see this script in use
/// </remarks>
public class Instantiation : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    /// <summary>
    /// Reference to the player prefab for network instantiation.
    /// </summary>
    public GameObject networkedPlayerPrefab;

    /// <summary>
    /// List of spawn point objects in the scene. Can be uncommented if you would rather not let Unity find the spawnpoints for you for performance reasons.
    /// </summary>
    //public GameObject[] spawnPoints;

    [Tooltip("Select this to be true if there is exactly 1 spawnpoint in your world")]
    public bool oneSpawnPoint = false;

    /// <summary>
    /// List of all interactables that need to be instantated as room objects.
    /// </summary>
    //public List<string> interactablePrefabNames;

    /// <summary>
    /// Spawns in the scene interactables if the current client is the master client.
    /// </summary>
    private void Start()
    {
        // Interactable instantiation was moved to a NetworkInstantiation script that is placed on the interactables themselves.

        ////Checking master client avoid race conditions, as the scene is not loaded in, masterclient returns false when the program is first started
        ////This is intended behavior, since we can only spawn in interactable objects when the photon server registers our game
        ////And since OnJoinedRoom only gets called once the player joins the photon server, the interactables still will need to be spawned in if there is a scene transition
        ////Therefore, the following line is only called when a new scene is loaded in, as when the game first starts we can't make the objects immediately
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    MakeSceneInteractables();
        //}
    }

    /// <summary>
    /// Photon callback. Called locally when the player joins a room.
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        createPlayer();

        // Interactable instantiation was moved to a NetworkInstantiation script that is placed on the interactables themselves.

        ////This only gets called once when a new player joins the room. This also only happens locally
        //MakeSceneInteractables();
    }

    // Interactable instantiation was moved to a NetworkInstantiation script that is placed on the interactables themselves.

    ///// <summary>
    ///// Instantiates the required interactables in the scene.
    ///// This is done by finding interactables in the scene, deleting them, and spawning them back in as a Photon Room Object.
    ///// We do it this way because Room Objects cannot be added to the scene directly.
    ///// </summary>
    //private void MakeSceneInteractables()
    //{
    //    // TODO: this needs to be refactored
    //    foreach (string interactableName in interactablePrefabNames)
    //    {
    //        foreach (GameObject newInteractable in GameObject.FindGameObjectsWithTag("Interactable"))
    //        {
    //            if (PhotonNetwork.IsMasterClient) //We only want to spawn one set of interactables
    //            {
    //                Vector3 newPosition = newInteractable.transform.position;
    //                Quaternion newRotation = newInteractable.transform.rotation;
    //                Destroy(newInteractable);

    //                PhotonNetwork.InstantiateRoomObject(interactableName, newPosition, newRotation);
    //            }
    //            else
    //            {
    //                Destroy(newInteractable);
    //            }
    //        }
    //    }

    //    foreach (GameObject newObject in GameObject.FindGameObjectsWithTag("SpawnLocation"))
    //    {
    //        newObject.GetComponent<SpawnNetworkedObject>().Spawn();
    //    }
    //}

    /// <summary>
    /// Instantiates a player in the network.
    /// If there is only one spawnpoint, the player will be instantiated at the first spawm point it finds.
    /// If there are multiple spawnpoints, the current player count is retrieved from the server and the player is spawned at the index (playerCount - 1).
    /// </summary>
    private void createPlayer()
    {
        //if (oneSpawnPoint)
        //{
        //    //If you don't mark oneSpawnPoint while there only exists one spawnpoint, it'll be fine, it'll just have a little more overhead
        //    GameObject spawnLocation;
        //    spawnLocation = GameObject.FindGameObjectWithTag("SpawnPoint");
        //    GameObject localPlayer = Instantiate(networkedPlayerPrefab);
        //    PlayerManager.inst.LocalPlayerInstance = localPlayer;
        //}
        //else
        //{
        //    //We want to find all spawnpoints, so here's how we do it without doing extra work in the editor
        //    GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //    //We get the index we want to spawn by looking at how many players are - 1 (since it counts yourself), we only want to count other players
        //    int spawnPointIndex;
        //    spawnPointIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        //    Debug.Log("Current amount of players in network is: " + spawnPointIndex + " , spawning you in index " + spawnPointIndex);

        //    //this loop ensures that the amount of spawnpoints available doesn't exceed the amount of players
        //    //As a result, if there are more players than spawnpoints, we just loop back to the beginning spawnpoint
        //    while (spawnPointIndex > spawnLocations.Length)
        //        spawnPointIndex -= spawnLocations.Length;

        //    //This for loop iterates through each spawnpoint until the correct one is found
        //    foreach (GameObject spawnLocation in spawnLocations)
        //    {
        //        if (spawnLocation.GetComponent<SpawnPointHelper>().spawnPointIndex == spawnPointIndex)
        //        {
        //            Vector3 spawnPos = spawnLocation.transform.position;
        //            spawnPos.y = 0.0f;
        //            Quaternion spawnRot = spawnLocation.transform.rotation;
        //            GameObject localPlayer = Instantiate(networkedPlayerPrefab, spawnPos, spawnRot);
        //            PlayerManager.inst.LocalPlayerInstance = localPlayer;
        //            this.photonView.RPC("RPC_SpawnpointUsed", RpcTarget.AllBuffered, spawnPointIndex);
        //            break; //We have found the correct spawnpoint index
        //        }
        //    }
        //}

        GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int spawnPointIndex = (PhotonNetwork.CurrentRoom.PlayerCount - 1) % spawnLocations.Length;
        Debug.Log("Current amount of players in network (including yourself) is " + PhotonNetwork.CurrentRoom.PlayerCount + ", spawning you in index " + spawnPointIndex);

        foreach (GameObject spawn in spawnLocations)
        {
            if (spawn.GetComponent<SpawnPointHelper>().spawnPointIndex == spawnPointIndex)
            {
                Vector3 spawnPos = spawn.transform.position;
                spawnPos.y = 0.0f;
                Quaternion spawnRot = spawn.transform.rotation;
                // this object is instantiated locally because it handles it's network capabilities itself
                GameObject localPlayer = Instantiate(networkedPlayerPrefab, spawnPos, spawnRot);
                PlayerManager.inst.LocalPlayerInstance = localPlayer;
                // this function was used in the previous code, but had no effect on anything
                //this.photonView.RPC("RPC_SpawnpointUsed", RpcTarget.AllBuffered, spawnPointIndex);
                break;
            }
        }

    }

    ///// <summary>
    ///// Photon Remote Procedure Call used to broadcast to other players that a spawnpoint index has been used.
    ///// </summary>
    ///// <param name="indexUsed">The spawnpoint index that has been used by some other client.</param>
    //[PunRPC]
    //private void RPC_SpawnpointUsed(int indexUsed)
    //{
    //    GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");

    //    foreach (GameObject spawnLocation in spawnLocations)
    //    {
    //        if (spawnLocation.GetComponent<SpawnPointHelper>().spawnPointIndex == indexUsed)
    //            spawnLocation.GetComponent<SpawnPointHelper>().spawnPointUsed = true;
    //    }
    //}

    /// <summary>
    /// Instantiates two InteractableBalls as room objects. Intended to be called from a menu system (in Example Scene).
    /// </summary>
    public void RefreshBalls()
    {
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-1.94f, 5f, 1f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-1.94f, 5f, 3.5f), Quaternion.identity);
    }

    /// <summary>
    /// Instantiates a set of bowling pins and three balls used to knock them down.
    /// Intended to be called from a menu system (in the Template scene).
    /// </summary>
    public void SpawnBowling()
    {
        PhotonNetwork.InstantiateRoomObject("Pins", new Vector3(-20f, 5f, 3f), Quaternion.identity);
        //In this case, pins are instantiated all together. This is to show that multiple objects can be instantiated at once with a parent container

        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-8.7f, 5f, 0f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-8.7f, 5f, 3f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-8.7f, 5f, 6f), Quaternion.identity);
    }
}