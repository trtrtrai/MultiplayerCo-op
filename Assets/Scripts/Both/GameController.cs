using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Controllers;
using Assets.Scripts.Both.Creature.Player;
using Assets.Scripts.Both.Scriptable;
using Assets.Scripts.Server.Contruction;
using Assets.Scripts.Server.Contruction.Builders;
using System.IO;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Server owner. Communication between client and server.
/// </summary>
public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

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

    //[ServerRpc(RequireOwnership = false)]
    public void SpawnPlayer(ulong clientId)
    {
        CreatureBuilder builder = new CharacterBuilder();
        CreatureDirector.Instance.Builder = builder;
        CreatureDirector.Instance.CharacterBuild(CharacterClass.TankerSlash_model);

        var rs = builder.Release();
        var playerTransform = (rs as NetworkBehaviour).transform;

        var control = InstantiateGameObject("Player/PlayerControl", null); //PLayer control (real owned by client)

        playerTransform.AddComponent<PlayerController>().AddControl(control.GetComponent<PlayerControl>()); //Add control into controller

        //Spawn accross network
        SpawnAsPlayerObject(control, clientId, true);
        control.GetComponent<PlayerControl>().enabled = false;
        control.GetComponent<PlayerControl>().enabled = true;
        SpawnGameObject(playerTransform.gameObject);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        //Setup camera
        SpawnCameraClientRpc((rs as NetworkBehaviour).NetworkObject, clientRpcParams);
    }

    /*public void CastSpell(SkillName name, NetworkObject netRef)
    {
        Debug.Log("Cast " + name);
    }*/

    /// <summary>
    /// ClientRpc params only send to client ID targeted
    /// </summary>
    /// <param name="clientRpcParams"></param>
    [ClientRpc]
    private void SpawnCameraClientRpc(NetworkObjectReference player, ClientRpcParams clientRpcParams = default)
    {
        if (IsClient)
        {
            var cmr = GameObject.FindGameObjectWithTag("MainCamera");

            if (cmr is null)
            {
                cmr = Instantiate(Resources.Load<GameObject>("CameraFollow"));
                cmr.GetComponent<NetworkObject>().Spawn();
            }

            //var player = GameObject.FindGameObjectsWithTag("Player").ToList().First((p) => p.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId).transform;
            var cmrFolow = cmr.GetComponent<CameraFollower>();
            player.TryGet(out NetworkObject playerObj);
            cmrFolow.Target = playerObj.transform;
            cmrFolow.StartFocus();
        }
    }

    public GameObject InstantiateGameObject(string path, Transform parent)
    {
        if (parent)
        {
            return Instantiate(Resources.Load<GameObject>(path), parent);
        }
        else
        {
            return Instantiate(Resources.Load<GameObject>(path));
        }
    }

    public GameObject InstantiateGameObject(string path, Transform parent, Vector3 pos)
    {
        GameObject obj;

        if (parent)
        {
            obj = Instantiate(Resources.Load<GameObject>(path), parent);
        }
        else
        {
            obj = Instantiate(Resources.Load<GameObject>(path));
        }

        obj.transform.localPosition = pos;

        return obj;
    }

    public void SpawnGameObject(GameObject gameObj, bool destroyWithScene = false)
    {
        if (gameObj.TryGetComponent(typeof(NetworkObject), out var netObj))
        {
            (netObj as NetworkObject).Spawn(destroyWithScene);
        }
    }

    public void SpawnAsPlayerObject(GameObject gameObj, ulong clientId, bool destroyWithScene = false)
    {
        if (gameObj.TryGetComponent(typeof(NetworkObject), out var netObj))
        {
            (netObj as NetworkObject).SpawnAsPlayerObject(clientId, destroyWithScene);
        }
    }

    public void SpawnWithOwnerShip()
    {

    }

    public GameObject FindGameObject(string name)
    {
        return GameObject.Find(name);
    }
}
