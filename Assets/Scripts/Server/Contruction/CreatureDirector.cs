using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using Assets.Scripts.Server.Contruction.Builders;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Server.Contruction
{
    public class CreatureDirector : MonoBehaviour
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

        public void CharacterBuild(string charClass)
        {
            //Game Object
            builder.InstantiateGameObject(charClass);

            //Load scriptable object
            var script = Resources.Load<CharacterModel>(path + "Player/" + charClass);

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
            var skillName = GameController.Instance.GetCreatureSkill(charClass.Replace("_model", ""));
            var skills = new List<Skill>();

            skillName.ForEach(s => skills.Add(new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/" + s.ToString()))));

            attackable.Skills = skills;
            builder.GiveAttackable(attackable);
        }

        public void BossBuild(BossName boss)
        {
            //Game Object
            builder.InstantiateGameObject(boss.ToString());

            //Load scriptable object
            var script = Resources.Load<BossModel>(path + "Boss/" + boss.ToString());

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
            var skillName = GameController.Instance.GetCreatureSkill(boss.ToString());
            var skills = new List<Skill>();

            skillName.ForEach(s => skills.Add(new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/" + s.ToString()))));

            attackable.Skills = skills;
            builder.GiveAttackable(attackable);

            // Limit change only boss
            (builder as BossBuilder).Setup(script.ChangeLimit);
        }

        public void OtherBuild(string cName)
        {
            //Game Object
            builder.InstantiateGameObject(cName);

            //Load scriptable object
            var script = Resources.Load<OtherCreatureModel>(path + "OtherCreature/" + cName);

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
            var skillName = GameController.Instance.GetCreatureSkill(cName);
            var skills = new List<Skill>();

            skillName.ForEach(s => skills.Add(new Skill(Resources.Load<SkillModel>("AssetObjects/Skills/" + s.ToString()))));

            attackable.Skills = skills;
            builder.GiveAttackable(attackable);
        }
    }
}