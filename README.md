# Multiplayer VR Template

This is a proof-of-concept for a multiplayer VR game, using ping pong as a demonstration. This project makes use of the Unity XR Interaction Toolkit, Photon Unity Networking (PUN), Photon Voice, and Unity's OpenXR backend.

## Table of Contents

1. [Installation](#Installation)
2. [General Information](#General-Information)
   1. [The player prefab](#the-player-prefab)
   2. [Hook scripts](#hook-scripts)
   3. [Animations](#animations)
   4. [Menu Systems](#menu-systems)
   5. [Photon Voice](#photon-voice)
   6. [Other Remarks](#other-remarks)
3. [Project Setup](#project-setup)
   1. [XR specific](#xr-specific)
   2. [Photon specific](#photon-specific)
   3. [General components](#general-components)
   4. [Example of an interactable](#example-of-an-interactable)

## Installation

1. Enable preview packages in Unity
2. Import the *OpenXR Plugin* and *XR Interaction Toolkit* packages
3. Under the *XR Interaction Toolkit* package, expand *Samples* and import the *Default Input Actions*
4. Go to Edit > Project Settings
5. Under *XR Plug-in Management*, check the box next to OpenXR
6. There should be a warning symbol that appears next to the OpenXR line. Click that and follow the instructions.
7. The networking libraries (PUN, Photon Voice) should already be included in this project

## General Information

### The player prefab

There are two main components to the NetworkedPlayer prefab. The first is the parts that are present locally. These are typically the components that track the VR headset and controllers, manage input, etc. The second component is the networked representation of the player. These are components that are presented to the other players in the scene, such as the player’s head and hands.Information that needs to be networked (namely the transforms of the HMD and controllers and hand animation state) is copied from their source (the local components) to an object with appropriate PhotonViews (the networked components).

A voice connection is instantiated when the player connects.

The Unity XR Interaction Toolkit only allows one controller component on an object. If you wish to have multiple interactors on one hand, you will have to use the ControllerManager component, and make all of your controllers for that hand child objects of the ControllerManager. You can then decide on an input that will cycle through the controllers.

### Hook scripts

A caveat of PUN is that objects that need to be synchronized over the network need to be instantiated at runtime using `PhotonNetwork.Instantiate()` or `PhotonNetwork.InstantiateRoomObject()`. This means you lose the advantage of linking together objects in the scene in the editor. The solution I came up with is to create several "hook scripts" to assign references to scene objects at runtime by searching for names of game objects in the scene. This isn't the most elegant solution (it seems to scale poorly), so if you have a better idea, feel free to use it.

### Animations

The implementation of animations in this template is very minimal: only hand animations are propogated over the network. Due to the way that the controller manager works, you have to give each control scheme its own animator.

### Menu systems

`Assets/Scenes/Template` is an older example that contains a few other notable features, namely menu interactions. Setting up a menu like this shiuld proceed just like setting up a normal Unity menu. Since this is in VR, you will probably want to place the menu in world space.

There is a hook script (UIEventCameraHook) present on the NetworkedPlayer prefab (NetworkedPlayer > XR Rig > CameraOffset > HeadCamera) that sets the event camera of the menu system to the VR camera. It tries to find a Canvas object in the scene with the name MenuInterface by default. This can be edited, and can search for multiple canvases as well.

Another hook script (MenuSystemHook) lets the MenuSystem know about the ControllerManager in order to enable/disable the pointer line visual. This is less important overall, but it's an example of a possible way to share information between statically-instantiated and runtime-instantiated objects.

By default, the NetworkedPlayer prefab will have a pointer that can interact with menus on the left hand.

### Photon Voice

The VoiceConnection prefab should have working settings. The most important part of this prefab is the Transmit Enabled flag in the Recorder component of this prefab. When this flag is enabled, the user’s voice will be transmitted across the network. I have this flag unset by default. You can enable it straight up if you want always-on voice transmission, or you can write a monobehaviour that gives you push-to-talk functionality.

Note: during my testing, the voice connection would throw the following error at least once:

`OnJoinRandomFailed errorCode=32760 errorMessage=No match found`

I believe that you can safely ignore this and voice will work fine. It seems to be attempting to join a room before it has fully connected to the voice server, and will eventually join a room properly.

### Other Remarks

I would recommend staying away from physics-heavy games/simulations. Photon does not use an authoritative physics server; the authority is the client who has ownership of the object. This means you have to do a lot of ownership transfer to ensure things look right to the right players. Even so, this can still be troublesome. The higher the speed of the objects being simulated, the harder it will be to work with the physics. The ping pong demo should be a good example of how things can get out of control very quickly.

This project uses OpenXR as its backend. This was chosen to maximize compatibility with VR devices. However, OpenXR is still a very new standard, and is missing some useful features. For example, support for Vive trackers was only added [a few months ago](https://github.com/KhronosGroup/OpenXR-SDK/releases/tag/release-1.0.20), and these changes have not yet propagated to Unity’s OpenXR environment yet.

This may have been fixed as of January 11, 2022, <!-- [which was three days ago as of writing this!] -->
but I have yet to test it. See these links:
* [SteamVR beta 1.21.5 patch notes](https://store.steampowered.com/news/app/250820?emclan=103582791435040972&emgid=3126061077819506317)
* [This thread on the Unity forums, toward the end](https://forum.unity.com/threads/openxr-and-openvr-together.1113136/)

## Project Setup

The ping pong scene should have all the necessary components for basic multiplayer VR. These components are:

### XR specific
* InputActionManager
  * Set *Action assets* to *XRI Default Input Actions* (this is the thing you imported before)
* XRInteractionManager
  * You may have multiple interaction managers in your scene to allow for you to semantically differentiate different actions that the player may use. For example, you could disable teleportation by disabling the teleportation interaction manager.
  * Pay attention to the names that you give these managers: there is an interaction manager hook script that requires you to provide a name for the interaction manager you want the interactor/interactable to link with.
    * This is required due to Photon requiring that room objects be instantiated at runtime. Ordinarily, you could just directly reference the manager in the scene, but this requires the interactable to be instantiated statically.
* TeleportationArea (if you plan to use teleportation)
  * These can be made child objects of your floor; I think they will automatically stretch to encompass the extent of your floor. Be sure to set the teleportation area to sit a little bit higher than the floor (+0.001 units should work) so that it is visible to the teleportation provider.
* An InteractionManagerHook script must be placed on each interactor/interactable that you wish to place in the scene. This allows the interactable/interactor to assign themselves to an interaction manager at runtime.

### Photon specific
* Prefabs that you wish to use over the network must be placed in `Assets/Resources`
* PhotonHandler
* All entities that must be visible over the network must have a PhotonView component. These entities must be instantiated at runtime with `PhotonNetwork.Instantiate()` or `PhotonNetwork.InstantiateRoomObject()`
  * Specializations of the PhotonView component may be added as necessary. For example, a networked rigidbody would require a PhotonRigidbodyView component in addition to the PhotonView component.
* NetworkInstantiation
  * This is a script that will automatically instantiate room objects (objects that have the same lifetime as the room). Place this on any objects/prefabs you wish to instantiate in the scene, and supply the script with the name of the prefab you wish to instantiate.
* TransferOwnership
  * This is a script that provides a function that transfers ownership of the object to the client who called the function.
  * This function is intended to be used with events. For example, add this function to the SelectEntered event of an XRGrabInteractable to ensure that the client that is holding the object also owns it. This makes physics smoother for the client that grabbed the object.

### General components
* Launcher (LaunchServerRoom)
  * Connects to and initializes the Photon server.
* PlayerManager
  * Utility object that provides a global reference to the player.
* RoomManager (Prefab)
  * Manages instantiation of the player.
* Spawnpoints (Prefab)
  * Make sure to set the indices in the order you want the spawnpoints to be used.

### Example of an Interactable

* Rigidbody
* XRBaseInteractable
  * Such as an XRGrabInteractable
  * When the player interacts with this object, we want one of the interactable events to call `TransferOwnership.transferOwnership()`
    * Usually the select event
* TransferOwnership
* InteractionManagerHook
  * Attaches the object to the correct interaction manager (not required if you only use one interaction manager)
* PhotonView
* PhotonRigidbodyView
* PhotonTransformView
* NetworkInstantiation
  * Automatically handles instantiation of a room object when the server starts