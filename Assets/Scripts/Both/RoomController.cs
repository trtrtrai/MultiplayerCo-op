using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : NetworkBehaviour
{
    public static RoomController Instance { get; private set; }
    
    // Lobby[LocalClientId][CharacterIndex]
    private Dictionary<ulong, int> lobby = new Dictionary<ulong, int>();

    [SerializeField] private RoomButton script;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SetupRoom()
    {
        script = GameObject.Find("Canvas").GetComponent<RoomButton>();

        if (!IsServer) return;

        lobby = new Dictionary<ulong, int>();

        script.StartGameBtn.gameObject.SetActive(true);

        if (!IsHost)
        {
            script.StartGameBtn.interactable = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangedCharacterIndexServerRpc(int index, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;
        //Server cung phai update
        var changer = serverRpcParams.Receive.SenderClientId;
        Debug.Log("ChangedCharacterIndexServerRpc " + changer);
        lobby[changer] = index;
        var idSend = new List<ulong>();
        
        foreach (var item in lobby)
        {
            if (item.Key != changer)
            {
                idSend.Add(item.Key);
            }
        }

        if (idSend.Count == 0) return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = idSend.ToArray()
            }
        };
        ChangedCharacterIndexClientRpc(lobby.Keys.ToList().IndexOf(changer), index, false, clientRpcParams);
    }

    /// <summary>
    /// Update for other player card
    /// </summary>
    /// <param name="changer">Index of player card</param>
    /// <param name="index">Character index</param>
    /// <param name="clientRpcParams"></param>
    [ClientRpc]
    public void ChangedCharacterIndexClientRpc(int changer, int index, bool isSetup, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("ChangedCharacterIndexClientRpc " + changer + " " + index + " " + isSetup);

        if (isSetup)
        {
            SetupRoom();
            SetupPlayer(changer);
        }

        var player = script.Players[changer];

        player.CharacterIndex = index;
        player.UpdateCharacter();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangedPlayerNameServerRpc(string pName, ServerRpcParams serverRpcParams = default)
    {
        /*if (!IsServer) return;
        //Server cung phai update
        var changer = serverRpcParams.Receive.SenderClientId;
        lobby[changer] = index;
        var idSend = new List<ulong>();

        foreach (var item in lobby)
        {
            if (item.Key != changer)
            {
                idSend.Add(item.Key);
            }
        }

        if (idSend.Count == 0) return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = idSend.ToArray()
            }
        };
        ChangedCharacterIndexClientRpc(changer, index, clientRpcParams);*/
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerLoadCompletedServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };
        //Setup before players to sender (maybe NetworkVariable)
        for (int i = 0; i < lobby.Count; i++)
        {
            ChangedCharacterIndexClientRpc(i, lobby.Values.ElementAtOrDefault(i), true, clientRpcParams); // don't need to know clientId
        }

        lobby.Add(clientId, 0);

        if (lobby.Count > 4) return;
        Debug.Log(lobby.Count);

        SetupPlayerClientRpc(clientId, lobby.Count - 1);
    }

    [ClientRpc]
    private void SetupPlayerClientRpc(ulong clientId, int index, ClientRpcParams clientRpcParams = default)
    {
        if (clientId == NetworkManager.LocalClientId) SetupPlayer(index, true);
        else SetupPlayer(index);
    }

    private void SetupPlayer(int index, bool isActiveButton = false)
    {
        var player = script.Players[index];

        player.PlayerName.text = "Player" + index;
        player.Initial();

        player.UIContainer.SetActive(true);
        player.ButtonContainer.SetActive(isActiveButton);
    }

    public void OutRoom()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("GameMenu");
    }

    public void StartGame()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.LoadScene("PlayGame", LoadSceneMode.Single);
    }
}
