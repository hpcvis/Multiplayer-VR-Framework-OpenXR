using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

//Instantiates the interactables and the players
//This script needs to be put onto any object, preferably a common spawn point object
//
//To use: Drag the player onto the player prefab into its respective locations in the editor
//Drag the spawn points for the players and the spectators into their respective locations in the editor
//
//Note (Important): Ensure that both spawn points and spectator spawn points are within a parent prefab (as in, the actual spawnpoint locations are subsets of a spawnpoints prefab)
//This is to allow for multiple players to spawn in different locations
//For implementation, it is better to have it this way anyways, because this helps with being able to distinguish where the players should be spawning in
//Refer to the Template scene and look at the "PUN Spawns" in the heiarchy to see this script in use
//

public class Instantiation : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //Reference to the main player prefab
    public GameObject playerPrefab;
    public GameObject networkedPlayerPrefab;

    //spawnPoints is a container that contains spawn points
    //You can uncomment this code out if you don't want to waste runtime finding the spawnpoints yourself and would rather do it in the editor
    //public GameObject[] spawnPoints;

    [Tooltip("Select this to be true if there is exactly 1 spawnpoint in your world")]
    public bool oneSpawnPoint = false;

    //This list will instantiate all interactables defined in the editor here
    public List<string> interactablePrefabNames;

    private void Start()
    {
        //Checking master client avoid race conditions, as the scene is not loaded in, masterclient returns false when the program is first started
        //This is intended behavior, since we can only spawn in interactable objects when the photon server registers our game
        //And since OnJoinedRoom only gets called once the player joins the photon server, the interactables still will need to be spawned in if there is a scene transition
        //Therefore, the following line is only called when a new scene is loaded in, as when the game first starts we can't make the objects immediately
        if (PhotonNetwork.IsMasterClient)
        {
            MakeSceneInteractables();
        }
    }

    //OnJoinedRoom gets called locally when that player joins the given room
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called");
        base.OnJoinedRoom();

        createPlayer();

        //This only gets called once when a new player joins the room. This also only happens locally
        MakeSceneInteractables();
    }


    //This script makes the interactables inside the room that were in the scene be instantiated
    //This works by getting the interactable's information, deleting the object inside the scene, and then spawning in a room object via photon network
    //This is done because objects that are interactables, constantly updating, etc. need to be added as room objects, however, you can not add a room object that already exists within the scene
    //Therefore the object is destroyed, deleting the information before making a copy that has been reuploaded
    private void MakeSceneInteractables()
    {
        foreach (string interactableName in interactablePrefabNames)
        {
            foreach (GameObject newInteractable in GameObject.FindGameObjectsWithTag("Interactable"))
            {
                if (PhotonNetwork.IsMasterClient) //We only want to spawn one set of interactables
                {
                    Vector3 newPosition = newInteractable.transform.position;
                    Quaternion newRotation = newInteractable.transform.rotation;
                    Destroy(newInteractable);

                    PhotonNetwork.InstantiateRoomObject(interactableName, newPosition, newRotation);
                }
                else
                {
                    Destroy(newInteractable);
                }
            }
        }
    }


    // createPlayer() instantiates a player for in the network
    // It depends off of whether or not you marked or include several spawn points
    // If there is only one spawnPoint, it just instantiates it at the first spawn point it finds (it just assumes that there is one and spawns it)
    // If there are multiple, the current player count is gotten from the server, and then based on that, it finds the index of the corresponding spawnPoint
    private void createPlayer()
    {
        Debug.Log("Instantiation::createPlayer() called");
        if (oneSpawnPoint)
        {
            //If you don't mark oneSpawnPoint while there only exists one spawnpoint, it'll be fine, it'll just have a little more overhead
            GameObject spawnLocation;
            spawnLocation = GameObject.FindGameObjectWithTag("SpawnPoint");
            GameObject localPlayer = Instantiate(networkedPlayerPrefab);
            PlayerManager.inst.LocalPlayerInstance = localPlayer;
        }
        else
        {
            //We want to find all spawnpoints, so here's how we do it without doing extra work in the editor
            GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");

            //We get the index we want to spawn by looking at how many players are - 1 (since it counts yourself), we only want to count other players
            int spawnPointIndex;
            spawnPointIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
            Debug.LogError("Current amount of players in network is: " + spawnPointIndex + " , spawning you in index " + spawnPointIndex);
            
            //this loop ensures that the amount of spawnpoints available doesn't exceed the amount of players
            //As a result, if there are more players than spawnpoints, we just loop back to the beginning spawnpoint
            while (spawnPointIndex > spawnLocations.Length)
                spawnPointIndex -= spawnLocations.Length;

            //This for loop iterates through each spawnpoint until the correct one is found
            foreach (GameObject spawnLocation in spawnLocations)
            {
                if (spawnLocation.GetComponent<SpawnPointHelper>().spawnPointIndex == spawnPointIndex)
                {
                    GameObject localPlayer = Instantiate(networkedPlayerPrefab);
                    PlayerManager.inst.LocalPlayerInstance = localPlayer;
                    this.photonView.RPC("RPC_SpawnpointUsed", RpcTarget.AllBuffered, spawnPointIndex);
                    break; //We have found the correct spawnpoint index
                }
            }
        }
    }


    //Indicates to other players that the given spawnpoint was used
    [PunRPC]
    private void RPC_SpawnpointUsed(int indexUsed)
    {
        GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject spawnLocation in spawnLocations)
        {
            if (spawnLocation.GetComponent<SpawnPointHelper>().spawnPointIndex == indexUsed)
                spawnLocation.GetComponent<SpawnPointHelper>().spawnPointUsed = true;
        }
    }


    //The following 2 methods are called by the canvas class, the first one is for the "Example Scene" and the following is for the "Template"
    //It was done this way to keep instantiation of objects/people in one central location
    public void RefreshBalls()
    {
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-1.94f, 5f, 1f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-1.94f, 5f, 3.5f), Quaternion.identity);
    }
    public void SpawnBowling()
    {
        PhotonNetwork.InstantiateRoomObject("Pins", new Vector3(-20f, 5f, 3f), Quaternion.identity);
        //In this case, pins are instantiated all together. This is to show that multiple objects can be instantiated at once with a parent container

        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-8.7f, 5f, 0f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-8.7f, 5f, 3f), Quaternion.identity);
        PhotonNetwork.InstantiateRoomObject("InteractableBall", new Vector3(-8.7f, 5f, 6f), Quaternion.identity);
    }
}