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
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// Server owner. Communication between client and server.
/// </summary>
public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

    private Dictionary<string, List<SkillName>> skillDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;

            if (skillDict != null) return;

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sReader = new StreamReader(Application.streamingAssetsPath + "/CreatureSkill.txt"))
            using (JsonReader jReader = new JsonTextReader(sReader))
            {
                Dictionary<string, List<string>> data;
                data = serializer.Deserialize<Dictionary<string, List<string>>>(jReader);
                if (data is null) return;

                skillDict = new Dictionary<string, List<SkillName>>();
                foreach (var item in data)
                {
                    var listSkill = new List<SkillName>();
                    try
                    {
                        item.Value.ForEach(s => listSkill.Add((SkillName)Enum.Parse(typeof(SkillName), s)));
                        skillDict.Add(item.Key, listSkill);
                    }
                    catch
                    {
                        //...
                    }
                }
            }
        }
    }

    public GameObject CreatureInstantiate(string name)
    {
        if (!IsServer) return null;

        CreatureBuilder builder = new OtherCreatureBuilder();
        CreatureDirector.Instance.Builder = builder;
        CreatureDirector.Instance.OtherBuild(name);

        var rs = builder.Release();

        return (rs as NetworkBehaviour).gameObject;
    }

    public void BossSpawn(BossName boss)
    {
        if (!IsServer) return;

        CreatureBuilder builder = new BossBuilder();
        CreatureDirector.Instance.Builder = builder;
        CreatureDirector.Instance.BossBuild(boss);

        var rs = builder.Release();

        SpawnCreature(rs as Creature, "Boss", true);
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
        SpawnAsPlayerObject(control, clientId);
        SpawnCreature(rs as Creature, "Character");

        //Listen in server
        var scriptCtrl = control.GetComponent<PlayerControl>();
        playerTransform.gameObject.GetComponent<PlayerController>().AddControl(scriptCtrl);

        //Setup camera
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };
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
    private void CreatureSpawnClientRpc(CreatureForm form, string cName, NetworkObjectReference creature, ClientRpcParams clientRpcParams = default)
    {
        if (!IsClient || IsHost) return;

        //Load scriptable object
        var script = GetCreatureModel(form, cName.Replace(" ", ""));
        creature.TryGet(out NetworkObject creatureObj);
        ICreatureBuild builder = creatureObj.GetComponent<Creature>();
        /*var a = builder is null;
        var b = script is null;
        Debug.Log(a + " " + b + " " + cName.Replace(" ", ""));*/
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
        var skillName = GetCreatureSkill(cName.Replace(" ", ""));
        var skills = new List<Skill>();

        skillName.ForEach(s => skills.Add(new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/" + s.ToString()))));

        attackable.Skills = skills;
        builder.InitAttack(attackable);

        CreatureSetup(builder as Creature, creatureObj.tag);
    }

    /// <summary>
    /// Setup creature after spawn
    /// </summary>
    private void CreatureSetup(Creature obj, string tag)
    {
        if (obj is null) return;

        switch (tag)
        {
            case "Character":
                {
                    obj.AddComponent<PlayerController>();
                    break;
                }
            case "Boss":
                {
                    obj.AddComponent<BossController>();
                    break;
                }
            case "Enemy":
                {
                    obj.AddComponent<EnemyController>();
                    break;
                }
            case "Ally":
                {

                    break;
                }
            case "Mobs":
                {

                    break;
                }
        }

        //Setup skill
        var skills = obj.GetSkills();
        var skillActives = obj.GetComponentsInChildren<SkillActive>();
        for (int i = 0; i < skills.Count; i++)
        {
            skillActives[i].name = skills[i].SkillName.ToString();
            skillActives[i].SetupSkill();
        }

        var netS = obj.GetComponentInChildren<NetworkStats>();
        if (IsServer)
        {
            //Setup NetworkVariable (Status + Healthbar)
            netS.Health.Value = obj.GetStats(StatsType.Health).GetValue();
            netS.Strength.Value = obj.GetStats(StatsType.Strength).GetValue();
            netS.Defense.Value = obj.GetStats(StatsType.Defense).GetValue();
            netS.Speed.Value = obj.GetStats(StatsType.Speed).GetValue();
            netS.CriticalHit.Value = obj.GetStats(StatsType.CriticalHit).GetValue();

            netS.MaxHealth.Value = obj.GetStats(StatsType.Health).GetValue(false);
            netS.MaxStrength.Value = obj.GetStats(StatsType.Strength).GetValue(false);
            netS.MaxDefense.Value = obj.GetStats(StatsType.Defense).GetValue(false);
            netS.MaxSpeed.Value = obj.GetStats(StatsType.Speed).GetValue(false);
            netS.MaxCriticalHit.Value = obj.GetStats(StatsType.CriticalHit).GetValue(false);
        }

        netS.Setup();
    }

    private CreatureModel GetCreatureModel(CreatureForm form, string cName)
    {
        CreatureModel model = null;

        switch (form)
        {
            case CreatureForm.Character:
                {
                    model = Resources.Load<CharacterModel>("AssetObjects/Creatures/" + "Player/" + cName + "_model");

                    break;
                }
            case CreatureForm.Boss:
                {
                    model = Resources.Load<BossModel>("AssetObjects/Creatures/" + "Boss/" + cName);

                    break;
                }
            case CreatureForm.Other:
                {
                    model = Resources.Load<OtherCreatureModel>("AssetObjects/Creatures/" + "OtherCreature/" + cName);

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

    public List<SkillName> GetCreatureSkill(string name) => skillDict.ContainsKey(name) ? skillDict[name] : null;

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

    public void SpawnCreature(Creature creatureObj, string tag, bool destroyWithScene = false)
    {
        if (!IsServer && creatureObj is null) return;
        
        if (creatureObj.TryGetComponent(typeof(NetworkObject), out var netObj))
        {
            (netObj as NetworkObject).Spawn(destroyWithScene);
            //Debug.Log(netObj.name);

            CreatureSetup(creatureObj, tag);

            CreatureSpawnClientRpc(creatureObj.Form, creatureObj.Name, creatureObj.NetworkObject);
        }
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
}
