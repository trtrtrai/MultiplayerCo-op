using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using Assets.Scripts.Server.Contruction.Builders;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Server.Contruction
{
    public class CreatureDirector : NetworkBehaviour
    {
        public static CreatureDirector Instance { get; private set; }

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

        private CreatureBuilder builder;
        private readonly string path = "AssetObjects/Creatures/";

        public CreatureBuilder Builder { set => builder = value; }

        public void CharacterBuild(CharacterClass charClass)
        {
            //Game Object
            builder.InstantiateGameObject(charClass.ToString());

            //Load scriptable object
            var script = Resources.Load<CharacterModel>(path + "Player/" + charClass.ToString());

            //Init property
            builder.GiveName(script.CreatureName);

            var status = new List<Stats>();
            script.Status.ForEach(i =>
            {
                var statsT = Type.GetType("Assets.Scripts.Both.Creature.Status." + i.Type.ToString());
                if (statsT is null) return;

                status.Add((Stats)Activator.CreateInstance(statsT, i.Amount));
            });
            builder.GiveStatus(status);

            var attackable = new Attackable();
            attackable.TouchDamage = script.TouchDamage;
            attackable.SkillSlot = script.SkillSlot;

            //Instantiate skills
            var skills = new List<Skill>()
            {
                new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/Slash")),
                new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/Shield")),
                new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/Ragnarok")),
            };

            attackable.Skills = skills;
            builder.GiveAttackable(attackable);
        }

        public void BossBuild(int num)
        {
            //Game Object
            builder.InstantiateGameObject("Treant");

            //Load scriptable object
            var script = Resources.Load<BossModel>(path + "Boss/Treant");

            //Init property
            builder.GiveName(script.CreatureName);

            var status = new List<Stats>();
            script.Status.ForEach(i =>
            {
                //Debug.Log(Type.GetType("Assets.Scripts.Both.Creature.Status." + i.Type.ToString()));
                var statsT = Type.GetType("Assets.Scripts.Both.Creature.Status." + i.Type.ToString());
                if (statsT is null) return;

                status.Add((Stats)Activator.CreateInstance(statsT, i.Amount));
            });
            builder.GiveStatus(status);

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
            builder.GiveAttackable(attackable);
        }

        public void OtherBuild(string name)
        {
            //Game Object
            builder.InstantiateGameObject(name);

            //Load scriptable object
            var script = Resources.Load<OtherCreatureModel>(path + "OtherCreature/" + name);

            //Init property
            builder.GiveName(script.CreatureName);

            var status = new List<Stats>();
            script.Status.ForEach(i =>
            {
                var statsT = Type.GetType("Assets.Scripts.Both.Creature.Status." + i.Type.ToString());
                if (statsT is null) return;

                status.Add((Stats)Activator.CreateInstance(statsT, i.Amount));
            });
            builder.GiveStatus(status);

            var attackable = new Attackable();
            attackable.TouchDamage = script.TouchDamage;
            attackable.SkillSlot = script.SkillSlot;

            //Instantiate skills
            var skills = new List<Skill>()
            {
                new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/BatBite")),
                /*new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/Shield")),
                new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/FireBall")),*/
            };

            attackable.Skills = skills;
            builder.GiveAttackable(attackable);
        }
    }
}