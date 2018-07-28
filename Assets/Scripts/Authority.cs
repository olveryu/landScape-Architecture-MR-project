using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Authority : NetworkBehaviour, IInputClickHandler {
    public GameObject player;

    public void OnInputClicked(InputClickedEventData eventData) {
        CmdSetAuthority(GetComponent<NetworkIdentity>(), player.GetComponent<NetworkIdentity>());
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }

    [Command]
    void CmdSetAuthority(NetworkIdentity grabID, NetworkIdentity playerID) {
        grabID.AssignClientAuthority(playerID.connectionToClient);
    }

    [Command]
    void CmdRemoveAuthority(NetworkIdentity grabID, NetworkIdentity playerID) {
        grabID.RemoveClientAuthority(playerID.connectionToClient);
    }
}
