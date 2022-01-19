using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that implements an instance of a player over the network.
/// NetworkedPlayer represents a player, as well as components of the player that should be seen over the network,
/// such as the player's head and hands, and synchronizes the positions and andimations of these components.
/// </summary>
public class NetworkedPlayer : MonoBehaviour
{
    public GameObject voiceConnectionPrefab;
    public GameObject remotePlayerHeadPrefab;
    public GameObject remotePlayerHandPrefab;
    public Transform rootTransform;
    public Transform cameraTransform;
    public Transform networkedRepresentationParent;

    public Transform[] handTransforms;
    public Animator[] handAnimators;

    private GameObject voiceConnection;
    private GameObject networkedPlayerHead;
    private GameObject[] networkedHands;
    private Animator[] networkedHandAnimators;

    #region Unity Callbacks
    /// <summary>
    /// Instantates network representations of the player (head, hands) 
    /// </summary>
    protected void Awake()
    {
        // allow for scene transitions
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("autocleanup: " + PhotonNetwork.CurrentRoom.AutoCleanUp);
        CreateNetworkedRepresentation();
        CreateVoiceConnection();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(devices);
        foreach (var dev in devices)
        {
            Debug.Log("Device: " + dev.name);
        }
        UnityEngine.XR.InputDevices.deviceConnected += OnDeviceConnect;
    }

    private void OnDeviceConnect(UnityEngine.XR.InputDevice dev)
    {
        Debug.Log("Device: " + dev.name);
    }

    /// <summary>
    /// Synchronizes the positions of the network object representations.
    /// </summary>
    protected void Update()
    {
        if (networkedPlayerHead)
        {
            SyncNetworkTransform(networkedPlayerHead, cameraTransform);
        }
        for (int i = 0; i < networkedHands.Length; i++)
        {
            if (networkedHands[i])
            {
                SyncNetworkTransform(networkedHands[i], handTransforms[i]);
                SyncNetworkHandAnimations(networkedHandAnimators[i], handAnimators[i]);
            }
        }
    }

    /// <summary>
    /// Destroys the networked representations of each object.
    /// This is *not* called when the application exits, because Photon shuts down in OnApplicationQuit().
    /// I have tried many things to get these networked objects manually cleaned up, but never got them working.
    /// The solution to this is to let Photon clean up orphaned objects.
    /// </summary>
    protected void OnDestroy()
    {
        DestroyNetworkedRepresentation();
    }

    /// <summary>
    /// Initializes the networked representation of the plate on scene load.
    /// Necessary, since the player object is a DontDestroyOnLoad object.
    /// Note: This is not called on game initialization, presumably because the delegates have not been assigned yet.
    /// </summary>
    /// <param name="scene">Loaded scene</param>
    /// <param name="mode">Loaded scene mode</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateNetworkedRepresentation();
    }

    /// <summary>
    /// Destroys the networked representation of the player object on scene unload.
    /// Necessary, since the player object is a DontDestroyOnLoad object.
    /// </summary>
    /// <param name="current">Current scene name</param>
    void OnSceneUnloaded(Scene current)
    {
        DestroyNetworkedRepresentation();
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Initializes a prefab that manages the player's connection to the voice server.
    /// </summary>
    public void CreateVoiceConnection()
    {
        voiceConnection = PhotonNetwork.Instantiate(
            voiceConnectionPrefab.name,
            cameraTransform.position,
            cameraTransform.rotation
        );
        voiceConnection.transform.SetParent(cameraTransform);
    }

    /// <summary>
    /// Initializes the networked representations of each object.
    /// </summary>
    public void CreateNetworkedRepresentation()
    {
        networkedPlayerHead = PhotonNetwork.Instantiate(
            remotePlayerHeadPrefab.name,
            cameraTransform.position,
            cameraTransform.rotation);
        networkedPlayerHead.transform.SetParent(networkedRepresentationParent);

        // disable local rendering of the player head to avoid visual issues with shadows
        if (networkedPlayerHead.GetComponent<PhotonView>().IsMine)
        {
            networkedPlayerHead.GetComponent<MeshRenderer>().enabled = false;
        }

        // 0 => left hand
        // 1 => right hand
        networkedHands = new GameObject[2];
        networkedHandAnimators = new Animator[2];
        for (int i = 0; i < networkedHands.Length; i++)
        {
            networkedHands[i] = PhotonNetwork.Instantiate(
                remotePlayerHandPrefab.name,
                handTransforms[i].position,
                handTransforms[i].rotation);
            networkedHands[i].transform.SetParent(networkedRepresentationParent);
            networkedHandAnimators[i] = networkedHands[i].GetComponentInChildren<Animator>();

            // disable the mesh of the networked player instance locally, since there are SteamVR hands to render
            if (networkedHands[i].GetComponent<PhotonView>().IsMine)
            {
                networkedHands[i].GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
        }

        // flip the model of the right hand so it looks like a right hand over the network
        Vector3 rightHandScale = networkedHands[1].transform.localScale;
        rightHandScale.x *= -1.0f;
        networkedHands[1].transform.localScale = rightHandScale;
    }

    /// <summary>
    /// Destroys the networked representations of each object.
    /// </summary>
    public void DestroyNetworkedRepresentation()
    {
        PhotonNetwork.Destroy(networkedPlayerHead);
        for (int i = 0; i < networkedHands.Length; i++)
        {
            PhotonNetwork.Destroy(networkedHands[i]);
        }
    }

    /// <summary>
    /// Copies transforms of the local player to the network representations.
    /// </summary>
    /// <param name="networkRepresentation"></param>
    /// <param name="sourceTransform"></param>
    private void SyncNetworkTransform(GameObject networkRepresentation, Transform sourceTransform)
    {
        // corrects for movement of the base NetworkedPlayer position
        // the position of the networked representation will be incorrect on the client side,
        // but that doesn't matter, since it should be invisible anyway
        networkRepresentation.transform.position = sourceTransform.position + rootTransform.position;
        networkRepresentation.transform.rotation = sourceTransform.rotation;
    }

    /// <summary>
    /// Copies animation state of the local player's hands to their network representation.
    /// </summary>
    /// <param name="networkedHand"></param>
    /// <param name="sourceHand"></param>
    private void SyncNetworkHandAnimations(Animator networkedHand, Animator sourceHand)
    {
        networkedHand.SetBool("IsGrabbing", sourceHand.GetBool("IsGrabbing"));
    }
    #endregion
}
