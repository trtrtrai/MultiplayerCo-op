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
using Assets.Scripts.Server.Creature;
using Assets.Scripts.Client;
using Assets.Scripts.Both;
using UnityEngine.SceneManagement;
using Assets.Scripts.Both.UIHolder;
using System.Collections;
using Assets.Scripts.Server.Log;

/// <summary>
/// Server owner. Communication between client and server.
/// </summary>
public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

    private Dictionary<string, List<SkillName>> skillDict;
    private Dictionary<ICreature, ulong> characters;
    public NetworkVariable<float> Timer = new NetworkVariable<float>(0);

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

        //Find BossSpawn point
        var bossSpawn = GameObject.FindGameObjectWithTag("BossSpawn");
        var rigid = (rs as NetworkBehaviour).GetComponent<Rigidbody2D>();
        //rigid.collisionDetectionMode = CollisionDetectionMode2D.Discrete; // should be, but it is default
        rigid.MovePosition(bossSpawn.transform.localPosition);

        SpawnCreature(rs as Creature, "Boss", true);

        (rs.GetStats(StatsType.Health) as Health).OnDeadEvent += OnBossDeath;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId)
    {
        if (!IsServer) return;

        CreatureBuilder builder = new CharacterBuilder();
        CreatureDirector.Instance.Builder = builder;
        var cIndex = NetworkListener.Lobby[clientId];
        
        CreatureDirector.Instance.CharacterBuild(Enum.GetName(typeof(CharacterClass), cIndex));

        var rs = builder.Release();
        var playerTransform = (rs as NetworkBehaviour).transform;

        //Find BossSpawn point
        var playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        var rigid = (rs as NetworkBehaviour).GetComponent<Rigidbody2D>();
        //rigid.collisionDetectionMode = CollisionDetectionMode2D.Discrete; // should be, but it is default
        rigid.MovePosition(playerSpawn.transform.localPosition);

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
        SpawnCameraClientRpc((rs as NetworkBehaviour).NetworkObject, scriptCtrl.NetworkObject, clientRpcParams);

        try
        {
            characters.Add(rs, clientId);
            GameLogger.Instance.AddPlayer(clientId);
        }
        catch
        {
            characters = new Dictionary<ICreature, ulong>();
            characters.Add(rs, clientId);
            GameLogger.Instance.AddPlayer(clientId);
        }

        if (characters.Count == NetworkListener.Lobby.Count)
        {
            GameLoadCompletedClientRpc();
        }
    }

    public void Cast(List<SkillTag> skillTags, SkillPackageEventArg args)
    {
        if (!IsServer) return;

        SkillBehaviour.Instance.Cast(skillTags, args);
    }

    public int Damage(ICreature target, NetworkObject attacker, int damage)
    {
        return DamageCalculate.Instance.DamageTo(target, attacker, damage);
    }

    public bool? CreatureTagDetect(string ownerTag, string collisionTag)
    {
        if (collisionTag.Equals("Mobs")) return true; //always take damage
        if (collisionTag.Equals(ownerTag) || collisionTag.Equals("Spell")) return false;

        switch (collisionTag)
        {
            case "Character":
                {
                    if (ownerTag.Equals("Ally")) return false;
                    else return true;
                }
            case "Ally":
                {
                    if (ownerTag.Equals("Character")) return false;
                    else return true;
                }
            case "Boss":
                {
                    if (ownerTag.Equals("Enemy")) return false;
                    else return true;
                }
            case "Enemy":
                {
                    if (ownerTag.Equals("Boss")) return false;
                    else return true;
                }
        }

        return null;
    }

    [ClientRpc]
    private void CreatureSpawnClientRpc(CreatureForm form, string cName, string tag, NetworkObjectReference creature, ClientRpcParams clientRpcParams = default)
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

        Debug.Log(cName + " " + tag);
        
        CreatureSetup(builder as Creature, tag);
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
    private void SpawnCameraClientRpc(NetworkObjectReference player, NetworkObjectReference control, ClientRpcParams clientRpcParams = default)
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

            FindObjectsOfType(typeof(Creature)).ToList().ForEach(c => {
                if (c is null) return;

                var script = (c as Creature).GetComponentInChildren<NetworkStats>();
                if (script != null && !script.IsSetup)
                {
                    script.Setup();
                    script.IsSetup = true;
                }
            });
            //Debug.Log(creatures.Length);

            var skillUI = FindObjectsOfType(typeof(SkillUI));
            var skills = playerObj.GetComponentsInChildren<SkillActive>();
            control.TryGet(out NetworkObject controlObj);
            
            for (int i = 0; i < skillUI.Length; i++)
            {
                //Debug.Log(skillUI[i].name);
                (skillUI[i] as SkillUI).Setup(skills[int.Parse(skillUI[i].name)], controlObj.GetComponent<PlayerControl>(), int.Parse(skillUI[i].name)); //FindObjectsOfType are not sequential -> int.Parse(skillUI[i].name)
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestPlayerControlIdServerRpc(NetworkObjectReference player, ServerRpcParams serverRpcParams = default) //All PlayerController call this, get control to get Axis vector
    {
        if (!NetworkManager.Singleton.IsServer) return;

        player.TryGet(out NetworkObject playerObj);
        var clientId = characters[playerObj.GetComponent<ICreature>()];

        Debug.Log("RequestPlayerControlIdServerRpc " + serverRpcParams.Receive.SenderClientId + "->" + clientId);
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
            }
        };

        ResponsePlayerControlIdClientRpc(playerObj, clientId, clientRpcParams);
    }

    [ClientRpc]
    private void ResponsePlayerControlIdClientRpc(NetworkObjectReference player, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.IsServer) return;

        player.TryGet(out NetworkObject playerObj);
        playerObj.GetComponent<PlayerController>().ResponsePlayerControlId(clientId);
    }

    [ClientRpc]
    private void GameLoadCompletedClientRpc()
    {
        if (!IsClient) return;

        GameObject.Find("Loading").SetActive(false);
        var script = GameObject.Find("Canvas").GetComponentInChildren<WaitToPlayHolder>();
        script.Setup();
    }

    private void OnBossDeath()
    {
        if (!IsServer) return;

        Time.timeScale = 0f;
        Timer.Value = 5f;
        StartCoroutine(WaitToReroom());

        ShowResultClientRpc(true);
    }

    public void IsCharacterDeath(ICreature creature)
    {
        if (!IsServer) return;

        if (characters.ContainsKey(creature))
        {
            characters.Remove(creature);

            if (characters.Count == 0)
            {
                Time.timeScale = 0f;
                Timer.Value = 5f;
                StartCoroutine(WaitToReroom());

                ShowResultClientRpc(false);
            }
        }
    }

    IEnumerator WaitToReroom()
    {
        while (Timer.Value > 0)
        {
            Timer.Value -= Time.fixedUnscaledDeltaTime / 2; // (1/50) / 2: two frame per 0.1s
            yield return null;
        }

        Time.timeScale = 1f;
        ToRoomScene();
    }

    public void Log(ICreature attacker, int amount, bool isDamage = true)
    {
        var creatureTag = (attacker as NetworkBehaviour).tag;

        switch (creatureTag)
        {
            case "Player":
                {
                    if (!characters.ContainsKey(attacker)) return;

                    if (isDamage)
                    {
                        GameLogger.Instance.PlayerLog(characters[attacker], amount);
                    }
                    else
                    {
                        GameLogger.Instance.PlayerLog(characters[attacker], amount, false);
                    }

                    break;
                }
            case "Boss":
                {
                    if (isDamage)
                    {
                        GameLogger.Instance.BossLog(amount);
                    }
                    else
                    {
                        GameLogger.Instance.BossLog(amount, false);
                    }

                    break;
                }
        }
    }

    [ClientRpc]
    private void ShowResultClientRpc(bool isWin, ClientRpcParams clientRpcParams = default)
    {
        if (!IsClient) return;

        //Stop something on Client: control,...
        GameObject.Find("Canvas").GetComponent<GameButton>().ResetPlayerInput();

        var script = GameObject.Find("Canvas").GetComponentInChildren<ResultPanelHolder>();

        if (isWin)
        {
            script.Label.text = "congratulation";
            script.Content.text = "The Boss is destroyed, Hero team win.";
        }
        else
        {
            script.Label.text = "mission failed";
            script.Content.text = "All Heroes are destroyed, The Boss win.";
        }

        script.StartTiming();
        script.Container.SetActive(true);
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

            //Debug.Log(netObj.name + " " + netObj.transform.localPosition);
            CreatureSpawnClientRpc(creatureObj.Form, creatureObj.Name, tag, creatureObj.NetworkObject);
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

    public void ToRoomScene()
    {
        if (IsServer)
        {
            GameObject.FindGameObjectsWithTag("Character").ToList().ForEach(c => { c.GetComponent<NetworkObject>().Despawn(); Destroy(c); });
            GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(c => { c.GetComponent<NetworkObject>().RemoveOwnership(); c.GetComponent<NetworkObject>().Despawn(); Destroy(c); });
            NetworkManager.Singleton.SceneManager.LoadScene("Room", LoadSceneMode.Single);
        }
        else //Client, not include host
        {
            Instance = null;
            NetworkManager.Singleton.Shutdown();

            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            SceneManager.LoadScene("GameMenu");
        }
    }
}
