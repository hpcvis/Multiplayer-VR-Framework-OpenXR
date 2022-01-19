using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

// READ THIS FIRST:
// There have been a few substantial changes to this file:
//  * Interactable instantiation was moved to a NetworkInstantiation script that is placed on the interactables themselves.
//  * Spawnpoint selection was simplified.
//  If any issues occur with spawning players or room objects, see commit 6d39ce5ece863a1e7df680c8329cbf9804e65a17.
//  This commit has the old code commented out.


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
    /// Photon callback. Called locally when the player joins a room.
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        createPlayer();
    }

    /// <summary>
    /// Instantiates a player in the network.
    /// The player is spawned at a spawnpoint with the index of player count - 1,
    /// wrapping back around to 0 if it exceeds the player count.
    /// </summary>
    private void createPlayer()
    {
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
                break;
            }
        }
    }

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