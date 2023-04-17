using Assets.Scripts.Both;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using Assets.Scripts.Both.UIHolder;
using Assets.Scripts.Both.Scriptable;

public class RoomController : NetworkBehaviour
{
    public static RoomController Instance { get; private set; }
    
    // Lobby[LocalClientId][CharacterIndex]
    private Dictionary<ulong, int> lobby = new Dictionary<ulong, int>();
    public Scene Room;
    public bool isSecond;

    [SerializeField] private GameObject loading;
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
            Room = SceneManager.GetActiveScene();
        }
    }

    public void SetupRoom()
    {
        loading = GameObject.FindGameObjectWithTag("Loading");
        loading.transform.GetChild(0).gameObject.SetActive(false);
        script = GameObject.Find("Canvas").GetComponent<RoomButton>();
        script.IpAddrTxt.text = GetLocalIPAddress() ?? "0.0.0.0";

        if (!IsServer) 
        {
            script.DisableBossChoice();
            return;
        }

        if (lobby is null) lobby = new Dictionary<ulong, int>();

        script.StartGameBtn.gameObject.SetActive(true);

        if (!IsHost)
        {
            script.StartGameBtn.interactable = false;
        }
    }

    #region Setup new player connection
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

        if (isSecond && lobby.ContainsKey(clientId))
        {
            if (clientId == NetworkManager.ServerClientId) return;

            GameLoadDeactiveClientRpc(clientRpcParams);

            int i = 0;
            foreach (var item in lobby)
            {
                DrawPlayerGridClientRpc(i, item.Value, item.Key == clientId, clientRpcParams);
                i++;
            }
            return;
        }

        //Setup before players to sender (maybe NetworkVariable)
        for (int i = 0; i < lobby.Count; i++)
        {
            ChangedCharacterIndexClientRpc(i, lobby.Values.ElementAtOrDefault(i), true, clientRpcParams); // don't need to know clientId
        }

        lobby.Add(clientId, 0);

        if (lobby.Count > 4) return;
        //Debug.Log(lobby.Count);

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
        player.OnUse = true;
        player.IsOwner = isActiveButton;

        if (player.IsOwner) player.GetComponent<Image>().color = player.OwnerColor;
    }
    #endregion

    public Dictionary<ulong, int> GetLobby() => lobby;

    #region Out room
    public void OutRoom()
    {
        //Debug.Log("Out " + IsServer + " " + IsClient);
        if (IsServer)
        {
            OutRoomAllClientRpc();
            StartCoroutine(WaitToDisconnectServer());
        }
        else if (IsClient) 
        {
            Debug.Log("adfs");
            OutRoomServerRpc(NetworkManager.Singleton.LocalClientId);
        } 
    }

    [ClientRpc]
    private void OutRoomAllClientRpc()
    {
        if (IsHost) return;
        Debug.Log("Out room all");

        DisconnectServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisconnectServerRpc(ulong clientId)
    {
        if (!IsServer) return;

        if (lobby.ContainsKey(clientId))
        {
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            DisconnectClientRpc(clientRpcParams);
        }
    }

    [ClientRpc]
    private void OutRoomClientRpc(int index, ClientRpcParams clientRpcParams = default)
    {
        if (IsHost) return;

        UpdatePlayerGrid(index);
    }

    private void UpdatePlayerGrid(int index)
    {
        Debug.Log("UpdateGrid");
        int onUseCount = 0; // >=1
        for (int i = 0; i < script.Players.Count; i++)
        {
            if (script.Players[i].OnUse)
            {
                onUseCount++;
            }
        }

        var player = script.Players[index];
        if (index == onUseCount - 1)
        {
            player.UIContainer.SetActive(false);
            player.ButtonContainer.SetActive(false);
            player.OnUse = false;
        }
        else if (index < onUseCount - 1)
        {
            var offset = index + 1;
            while (offset < onUseCount) // e.g: index 1 out, onuse = 3 (index 2) --> index 2 become index 1, remove index 2
            {
                var nextPlayer = script.Players[offset];

                player.PlayerName.text = nextPlayer.PlayerName.text;
                player.CharacterIndex = nextPlayer.CharacterIndex;
                player.UpdateCharacter();

                if (nextPlayer.IsOwner)
                {
                    player.IsOwner = true;
                    nextPlayer.IsOwner = false;

                    player.ButtonContainer.SetActive(true);
                    nextPlayer.ButtonContainer.SetActive(false);
                }

                player = nextPlayer;
                offset++;
            }

            player.UIContainer.SetActive(false);
            player.ButtonContainer.SetActive(false);
            player.OnUse = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OutRoomServerRpc(ulong clientId)
    {
        if (!IsServer) return;
        if (!lobby.ContainsKey(clientId)) return;
        Debug.Log("Out room " + clientId);
        var index = lobby.Keys.ToList().IndexOf(clientId);
        lobby.Remove(clientId);

        UpdatePlayerGrid(index);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = lobby.Keys.ToArray()
            }
        };

        OutRoomClientRpc(index, clientRpcParams);

        clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId}
            }
        };

        DisconnectClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void DisconnectClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("DisconnectClientRpc");

        Instance = null;
        NetworkManager.Singleton.Shutdown();

        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        SceneManager.LoadScene("GameMenu");
    }
    #endregion

    #region Reroom
    public void DrawPlayerGrid()
    {
        int i = 0;
        foreach (var item in lobby)
        {
            SetupPlayer(i, item.Key == NetworkManager.Singleton.LocalClientId);

            script.Players[i].CharacterIndex = item.Value;
            script.Players[i].UpdateCharacter();
            i++;
        }
    }

    [ClientRpc]
    public void DrawPlayerGridClientRpc(int index, int cIndex, bool activeButton, ClientRpcParams clientRpcParams = default)
    {
        SetupPlayer(index, activeButton);

        script.Players[index].CharacterIndex = cIndex;
        script.Players[index].UpdateCharacter();
    }

    [ClientRpc]
    private void GameLoadDeactiveClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (IsServer) return;

        SetupRoom();
    }
    #endregion

    [ClientRpc]
    private void GameLoadClientRpc()
    {
        if (IsServer) return;

        loading.transform.GetChild(0).gameObject.SetActive(true);
    }

    [ServerRpc]
    public void BossChoiceServerRpc(int index, ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer) return;

        BossChoiceClientRpc(index);
    }

    [ClientRpc]
    private void BossChoiceClientRpc(int index, ClientRpcParams clientRpcParams = default)
    {
        if (IsServer) return;

        script.Script.Select(script.Script.ListBossBtn[index]);
    }

    public void StartGame()
    {
        if (!IsServer) return;

        loading.transform.GetChild(0).gameObject.SetActive(true);
        GameLoadClientRpc();
        NetworkListener.Lobby = lobby;
        NetworkManager.Singleton.SceneManager.LoadScene("PlayGame", LoadSceneMode.Single);
    }

    public void SetLobby(Dictionary<ulong, int> lobby)
    {
        isSecond = true;
        this.lobby = lobby;
    }

    IEnumerator WaitToDisconnectServer()
    {
        while (NetworkManager.ConnectedClients.Count > 1)
        {
            yield return null;
        }
        Debug.Log("WaitToDisconnectServer");
        NetworkListener.Lobby = null;
        Instance = null;
        NetworkManager.Singleton.Shutdown();

        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        SceneManager.LoadScene("GameMenu");
    }

    private string GetLocalIPAddress()
    {
        if (IsServer)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        else if (IsClient)
        {
            Debug.Log("Get IP Client");

            return NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
        }

        return null;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }
}
