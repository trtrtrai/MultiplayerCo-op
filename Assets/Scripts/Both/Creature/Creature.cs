using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature
{
    [Serializable]
    public abstract class Creature : NetworkBehaviour, ICreatureBuild, ICreature
    {
        [SerializeField] protected string creatureName;
        [SerializeField] protected List<Stats> status;
        [SerializeField] protected Attackable.Attackable attackable;
        [SerializeField] protected CreatureForm form;

        public string Name => creatureName;
        public int SkillSlot => attackable.SkillSlot;
        public bool TouchDamage => attackable.TouchDamage;
        public float AttackRange => attackable.AttackRange;
        public CreatureForm Form => form;

        public virtual IStats GetStats(StatsType type)
        {
            for (int i = 0; i < status.Count; i++)
            {
                if (status[i].GetType().Name == type.ToString())
                {
                    return status[i];
                }
            }

            return null;
        }

        public List<ISkill> GetSkills()
        {
            return attackable.Skills.Select(s => (ISkill)s).ToList();
        }

        public bool ActivateSkill(int index, Action callback, Transform target)
        {
            ISkillActivate skill = attackable.Skills[index];

            skill.Activate(callback, this, target); //this func will return bool after
            return true;
        }

        public virtual void InitName(string name)
        {
            creatureName = name;
            this.name = name; // GameObject name
        }

        public virtual void InitStatus(List<Stats> status)
        {
            this.status = status;

            this.status.ForEach(s =>
            {
                s.OnStatsChange += Status_OnStatsChange;
            });
        }

        private void Status_OnStatsChange(object sender, StatsChangeEventArgs args)
        {
            StatsChange?.Invoke(this, args);
        }

        public virtual void InitAttack(Attackable.Attackable attackable)
        {
            this.attackable = attackable;
        }

        public event Action<object, StatsChangeEventArgs> StatsChange;
    }

    public enum CreatureForm
    {
        Character,
        Boss,
        Other,
        /*Enemy,
        Mobs,
        Ally*/
    }

    public interface ICreatureBuild
    {
        void InitName(string name);
        void InitStatus(List<Stats> status);
        void InitAttack(Attackable.Attackable attackable);
    }

    public interface ICreature
    {
        public string Name { get; }
        public int SkillSlot { get; }
        public bool TouchDamage { get; }
        public float AttackRange { get; }
        public CreatureForm Form { get; }

        IStats GetStats(StatsType type);
        public List<ISkill> GetSkills();
        public bool ActivateSkill(int index, Action callback, Transform target);
    }
}