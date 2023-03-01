using Assets.Scripts.Both.Scriptable;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
namespace Assets.Scripts.Both
{
    public class NetworkListener : MonoBehaviour
    {
        public bool StartMyServer(bool isHost)
        {
            var success = false;
            if (isHost)
            {
                success = NetworkManager.Singleton.StartHost();
            }
            else
            {
                success = NetworkManager.Singleton.StartServer();
            }

            if (success)
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            }

            return success;
        }

        public bool StartMyClient()
        {
            var success = NetworkManager.Singleton.StartClient();//Debug.Log("Client connect status " + success);
            if (success)
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            }
            return success;
        }

        private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            /*if (sceneEvent.SceneName.Equals("PlayGame")) Debug.Log("PG");
            if (sceneEvent.SceneName.Equals("Room"))
            {
                Debug.Log("R");
                //if (GameObject.Find("StartGame") is null) { Debug.Log("null"); return; }
                //GameObject.FindGameObjectWithTag("Stand").GetComponent<Button>().onClick.AddListener(() => NetworkManager.Singleton.SceneManager.LoadScene("PlayGame", LoadSceneMode.Single));
            }*/

            // Both client and server receive these notifications
            switch (sceneEvent.SceneEventType)
            {
                // Handle server to client Load Notifications
                case SceneEventType.Load:
                    {
                        // This event provides you with the associated AsyncOperation
                        // AsyncOperation.progress can be used to determine scene loading progression
                        var asyncOperation = sceneEvent.AsyncOperation;
                        // Since the server "initiates" the event we can simply just check if we are the server here
                        if (NetworkManager.Singleton.IsServer)
                        {
                            Debug.Log("Load Server");
                            if (NetworkManager.Singleton.IsHost)
                            {
                                Debug.Log("Host in server");
                            }
                        }
                        else
                        {
                            Debug.Log("Load Client");
                        }
                        break;
                    }
                // Handle server to client unload notifications
                case SceneEventType.Unload:
                    {
                        // You can use the same pattern above under SceneEventType.Load here
                        break;
                    }
                // Handle client to server LoadComplete notifications
                case SceneEventType.LoadComplete:
                    {
                        // This will let you know when a load is completed
                        // Server Side: receives this notification for both itself and all clients
                        if (NetworkManager.Singleton.IsServer)
                        {
                            if (sceneEvent.ClientId == NetworkManager.Singleton.LocalClientId)
                            {
                                // Handle server side LoadComplete related tasks here
                                Debug.Log("Server load completed");

                                if (sceneEvent.SceneName.Equals("PlayGame"))
                                {
                                    //Spawn GameManager
                                    var gameCtroller = Instantiate(Resources.Load<GameObject>("Manager/GameController"));
                                    GameController.Instance.SpawnGameObject(gameCtroller, true);

                                    var creatureContruct = GameController.Instance.InstantiateGameObject("Manager/CreatureConstruction", null);
                                    GameController.Instance.SpawnGameObject(creatureContruct, true);

                                    GameController.Instance.InstantiateGameObject("Manager/SkillBehaviour", null);
                                    GameController.Instance.InstantiateGameObject("Manager/DamageCalc", null);

                                    var cmr = GameController.Instance.InstantiateGameObject("CameraFollow", null);
                                    GameController.Instance.SpawnGameObject(cmr);

                                    GameController.Instance.BossSpawn(BossName.Treant);
                                }

                                if (sceneEvent.SceneName.Equals("Room"))
                                {
                                    var roomCtroller = Instantiate(Resources.Load<GameObject>("Manager/RoomController"));
                                    roomCtroller.GetComponent<RoomController>().NetworkObject.Spawn(true);
                                    //Debug.Log(roomCtroller.GetComponent<NetworkObject>().IsSpawned);
                                    RoomController.Instance.SetupRoom();
                                }

                                if (NetworkManager.Singleton.IsHost)
                                {
                                    Debug.Log("Server is also host");

                                    if (sceneEvent.SceneName.Equals("PlayGame")) GameController.Instance.SpawnPlayerServerRpc(sceneEvent.ClientId);
                                    if (sceneEvent.SceneName.Equals("Room")) RoomController.Instance.PlayerLoadCompletedServerRpc(sceneEvent.ClientId);
                                }
                            }
                            else
                            {
                                // Handle client LoadComplete **server-side** notifications here
                                Debug.Log("Server side client load " + sceneEvent.ClientId + " completed");
                            }                    
                        }
                        else // Clients generate this notification locally
                        {
                            // Handle client side LoadComplete related tasks here
                            Debug.Log("Client load " + sceneEvent.ClientId + " completed");

                            StartCoroutine(Wait(sceneEvent.ClientId, sceneEvent.SceneName));
                        }

                        // So you can use sceneEvent.ClientId to also track when clients are finished loading a scene
                        break;
                    }
                // Handle Client to Server Unload Complete Notification(s)
                case SceneEventType.UnloadComplete:
                    {
                        // This will let you know when an unload is completed
                        // You can follow the same pattern above as SceneEventType.LoadComplete here

                        // Server Side: receives this notification for both itself and all clients
                        // Client Side: receives this notification for itself

                        // So you can use sceneEvent.ClientId to also track when clients are finished unloading a scene
                        break;
                    }
                // Handle Server to Client Load Complete (all clients finished loading notification)
                case SceneEventType.LoadEventCompleted:
                    {
                        // This will let you know when all clients have finished loading a scene
                        // Received on both server and clients
                        foreach (var clientId in sceneEvent.ClientsThatCompleted)
                        {
                            // Example of parsing through the clients that completed list
                            if (NetworkManager.Singleton.IsServer)
                            {
                                // Handle any server-side tasks here
                            }
                            else
                            {
                                // Handle any client-side tasks here
                            }
                        }
                        break;
                    }
                // Handle Server to Client unload Complete (all clients finished unloading notification)
                case SceneEventType.UnloadEventCompleted:
                    {
                        // This will let you know when all clients have finished unloading a scene
                        // Received on both server and clients
                        foreach (var clientId in sceneEvent.ClientsThatCompleted)
                        {
                            // Example of parsing through the clients that completed list
                            if (NetworkManager.Singleton.IsServer)
                            {
                                // Handle any server-side tasks here
                            }
                            else
                            {
                                // Handle any client-side tasks here
                            }
                        }
                        break;
                    }
            }
        }

        private IEnumerator Wait(ulong clientId, string sceneName)
        {
            switch (sceneName)
            {
                case "PlayGame":
                    {
                        while (GameController.Instance is null)
                        {
                            yield return null;
                        }

                        if (sceneName.Equals("PlayGame")) GameController.Instance.SpawnPlayerServerRpc(clientId);
                        break;
                    }
                case "Room":
                    {
                        while (RoomController.Instance is null)
                        {
                            yield return null;
                        }

                        if (sceneName.Equals("Room")) RoomController.Instance.PlayerLoadCompletedServerRpc(clientId);
                        break;
                    }
            }   
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}