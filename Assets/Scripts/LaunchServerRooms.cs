using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//Launches the server
//Made sendRate and serializationRate higher to allow smoother updating of the interactable transform while it is being moved

public class LaunchServerRooms : MonoBehaviourPunCallbacks
{
    public GameObject gameManager;

    #region MonoBehaviour CallBacks
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
        
        Connect();
    }
    #endregion

    #region Public Methods

    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.

            //May want to mess with these settings depending on your input
            PhotonNetwork.SendRate = 40; //Default send rate is 20, per second
            PhotonNetwork.SerializationRate = 20; //Default Serialization rate is 10
            
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        //Create room options, if you want to add your own do it here. In this case we allow so cache of the given player is maintained
        //This is because sometimes room objects belong to certain players that we want to keep (such as interactable objects)
        RoomOptions roomOptions;
        roomOptions = new RoomOptions();
        // Disabling the CleanupCacheOnLeave flag results in orphan objects (player head, hands) remaining when a player is disconnected
        // Whether or not keeping this flag enabled breaks other things is yet to be determined
        //roomOptions.CleanupCacheOnLeave = false;

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    private void Update()
    {
        //Escape is how to exit the program, if escape is pressed, it will close the application down
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F4))
        {
            Debug.LogError("Escape was pressed, closing application. ");
            Quit();
        }
    }

    // Exits the application
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

    #endregion
}