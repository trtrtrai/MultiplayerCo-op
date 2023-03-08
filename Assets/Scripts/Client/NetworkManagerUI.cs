using Assets.Scripts.Both;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Client
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button btnServer;
        [SerializeField] private Button btnHost;
        [SerializeField] private Button btnClient;
        [SerializeField] private TMP_InputField ipAddrInput;

        [SerializeField] UnityTransport transport;

        private void Awake()
        {
            if (GameObject.FindGameObjectWithTag("DDOL") is null)
            {
                Instantiate(Resources.Load<GameObject>("Manager/NetworkManager"));
                NetworkManager.Singleton.SetSingleton();
            }

            btnServer.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.GetComponent<NetworkListener>().StartMyServer(false);
                NetworkManager.Singleton.SceneManager.LoadScene("Room", LoadSceneMode.Single);
            });

            btnHost.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.GetComponent<NetworkListener>().StartMyServer(true);
                NetworkManager.Singleton.SceneManager.LoadScene("Room", LoadSceneMode.Single);
            });

            btnClient.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.GetComponent<NetworkListener>().StartMyClient();
                /*if (SetIpAddress())
                {
                    NetworkManager.Singleton.GetComponent<NetworkListener>().StartMyClient();
                }*/
            });
        }

        /*private bool SetIpAddress()
        {
            transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = ipAddrInput.text;

            return true;
        }*/

        private void OnDestroy()
        {
            btnServer.onClick.RemoveAllListeners();
            btnHost.onClick.RemoveAllListeners();
            btnClient.onClick.RemoveAllListeners();
        }
    }
}