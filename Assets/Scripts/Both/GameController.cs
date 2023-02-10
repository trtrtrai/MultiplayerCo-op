using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Attackable.SkillExecute;
using Assets.Scripts.Both.Creature.Controllers;
using Assets.Scripts.Both.Creature.Player;
using Assets.Scripts.Both.Scriptable;
using Assets.Scripts.Server.Contruction;
using Assets.Scripts.Server.Contruction.Builders;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Server.Creature.Attackable;
using Unity.VisualScripting;

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

    public GameObject CreatureInstantiate(string name, string behaviour)
    {
        if (!IsServer) return null;

        CreatureBuilder builder = new OtherCreatureBuilder();
        CreatureDirector.Instance.Builder = builder;
        CreatureDirector.Instance.OtherBuild(name);

        var rs = builder.Release();

        (rs as NetworkBehaviour).AddComponent<EnemyController>(); //...

        var skills = rs.GetSkills();
        var skillActives = (rs as NetworkBehaviour).GetComponentsInChildren<SkillActive>();
        for (int i = 0; i < skills.Count; i++)
        {
            skillActives[i].name = skills[i].SkillName.ToString();
            skillActives[i].SetupSkill();
        }

        return (rs as NetworkBehaviour).gameObject;
    }

    public void BossSpawn(int index)
    {
        if (!IsServer) return;

        CreatureBuilder builder = new BossBuilder();
        CreatureDirector.Instance.Builder = builder;
        CreatureDirector.Instance.BossBuild(index);

        var rs = builder.Release();

        SpawnGameObject((rs as NetworkBehaviour).gameObject, true);

        (rs as NetworkBehaviour).AddComponent<BossController>();

        var skills = rs.GetSkills();
        var skillActives = (rs as NetworkBehaviour).GetComponentsInChildren<SkillActive>();
        for (int i = 0; i < skills.Count; i++)
        {
            skillActives[i].name = skills[i].SkillName.ToString();
            skillActives[i].SetupSkill();
        }

        CreatureSpawnClientRpc(CreatureForm.Boss, index, (rs as NetworkBehaviour).NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId)
    {
        if (!IsServer) return;

        CreatureBuilder builder = new CharacterBuilder();
        CreatureDirector.Instance.Builder = builder;
        CreatureDirector.Instance.CharacterBuild(CharacterClass.TankerSlash_model);

        var rs = builder.Release();
        var playerTransform = (rs as NetworkBehaviour).transform;

        var control = InstantiateGameObject("Player/PlayerControl", null); //PLayer control (real owned by client)

        //Spawn accross network
        SpawnAsPlayerObject(control, clientId, true);
        SpawnGameObject(playerTransform.gameObject);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        //Listen in server
        var scriptCtrl = control.GetComponent<PlayerControl>();
        playerTransform.gameObject.AddComponent<PlayerController>().AddControl(scriptCtrl);
        //scriptCtrl.StartListen();

        if (clientId != NetworkManager.Singleton.LocalClientId) CreatureSpawnClientRpc(CreatureForm.Character, 0, (rs as NetworkBehaviour).NetworkObject);
        //GameObject skil setup
        var skills = rs.GetSkills();
        var skillActives = playerTransform.GetComponentsInChildren<SkillActive>();
        for (int i = 0; i < skills.Count; i++)
        {
            skillActives[i].name = skills[i].SkillName.ToString();
            skillActives[i].SetupSkill();
        }

        //Setup camera
        SpawnCameraClientRpc((rs as NetworkBehaviour).NetworkObject, clientRpcParams);
    }

    public void Cast(List<SkillTag> skillTags, SkillPackageEventArg args)
    {
        if (!IsServer) return;

        SkillBehaviour.Instance.Cast(skillTags, args);
    }

    public void Damage(ICreature target, ICreature attacker, int damage)
    {
        DamageCalculate.Instance.DamageTo(target, damage);
    }

    [ClientRpc]
    private void CreatureSpawnClientRpc(CreatureForm form, int indexName, NetworkObjectReference creature, ClientRpcParams clientRpcParams = default)
    {
        if (!IsClient || IsHost) return;

        //Load scriptable object
        var script = GetCreatureModel(form, indexName);
        creature.TryGet(out NetworkObject creatureObj);
        ICreatureBuild builder = creatureObj.GetComponent<Creature>();

        //Init property
        builder.InitName(script.CreatureName);

        var status = new List<Stats>();
        script.Status.ForEach(i =>
        {
            var statsT = Type.GetType("Assets.Scripts.Both.Creature.Status." + i.Type.ToString());
            if (statsT is null) return;

            status.Add((Stats)Activator.CreateInstance(statsT, i.Amount));
        });
        builder.InitStatus(status);

        var attackable = new Attackable();
        attackable.TouchDamage = script.TouchDamage;
        attackable.SkillSlot = script.SkillSlot;

        //Instantiate skills
        var skills = new List<Skill>()
            {
                new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/BatSummon")),
                /*new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/Shield")),
                new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/FireBall")),*/
            };

        attackable.Skills = skills;
        builder.InitAttack(attackable);

        var skillActives = creatureObj.GetComponentsInChildren<SkillActive>();
        for (int i = 0; i < skills.Count; i++)
        {
            skillActives[i].name = skills[i].SkillName.ToString();
            skillActives[i].SetupSkill();
        }
    }

    private CreatureModel GetCreatureModel(CreatureForm form, int index)
    {
        CreatureModel model = null;

        switch (form)
        {
            case CreatureForm.Character:
                {
                    model = Resources.Load<CharacterModel>("AssetObjects/Creatures/" + "Player/" + CharacterClass.TankerSlash_model.ToString());

                    break;
                }
            case CreatureForm.Boss:
                {
                    model = Resources.Load<BossModel>("AssetObjects/Creatures/" + "Boss/" + "Treant");

                    break;
                }
            case CreatureForm.Other:
                {
                    model = Resources.Load<OtherCreatureModel>("AssetObjects/Creatures/" + "Bat/Bat");

                    break;
                }
        }

        return model;
    }

    /// <summary>
    /// ClientRpc params only send to client ID targeted
    /// </summary>
    /// <param name="clientRpcParams"></param>
    [ClientRpc]
    private void SpawnCameraClientRpc(NetworkObjectReference player, ClientRpcParams clientRpcParams = default)
    {
        if (IsClient)
        {
            //Camera Follow
            var cmr = GameObject.FindGameObjectWithTag("MainCamera");

            if (cmr is null)
            {
                cmr = Instantiate(Resources.Load<GameObject>("CameraFollow"));
                cmr.GetComponent<NetworkObject>().Spawn();
            }

            var cmrFolow = cmr.GetComponent<CameraFollower>();
            player.TryGet(out NetworkObject playerObj);
            cmrFolow.Target = playerObj.transform;
            cmrFolow.StartFocus();

            //Add control into controller - client
            var control = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault(c => c.GetComponent<NetworkObject>().IsOwner);
            if (control is null) return;
            var scriptCtrl = control.GetComponent<PlayerControl>();
            //playerObj.GetComponent<PlayerController>().AddControl(scriptCtrl);
            scriptCtrl.StartListen();
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

            if (netObj.GetComponent<Creature>() != null)
            {
                var netS = netObj.GetComponentInChildren<NetworkStats>();
                netS.Health.Value = netObj.GetComponent<Creature>().GetStats(StatsType.Health).GetValue();
                netS.Setup();
            }
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
}
