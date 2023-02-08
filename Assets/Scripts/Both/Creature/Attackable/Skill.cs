using Assets.Scripts.Both.Scriptable;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Both.Creature.Attackable
{
    [Serializable]
    public class Skill : ISkill, ISkillActivate
    {
        [SerializeField] private SkillName skillName;
        [SerializeField] private float castDelay;
        [SerializeField] private float cooldown;

        public SkillName SkillName => skillName;
        public float CastDelay => castDelay;
        public float Cooldown => cooldown;
        public string Description { get; set; }

        public Skill(SkillModel model)
        {
            skillName = model.SkillName;
            Description = model.Description;
            castDelay = model.CastDelay;
            cooldown = model.Cooldown;
        }

        public void Activate(Action callback, Creature owner, Creature target)
        {
            OnActivate?.Invoke(callback, new SkillPackageEventArg(owner.GetComponent<NetworkObject>(), target.GetComponent<NetworkObject>(), new Vector3(0.2f, 0.2f)));
        }

        public void AddListener(Action<Action, SkillPackageEventArg> subscriber)
        {
            OnActivate += subscriber;
        }

        public void RemoveListener(Action<Action, SkillPackageEventArg> subscriber)
        {
            OnActivate += subscriber;
        }

        public Action<Action, SkillPackageEventArg> OnActivate;
    }

    public interface ISkill
    {
        SkillName SkillName { get; }
        float CastDelay { get; }
        float Cooldown { get; }
        string Description { get; }
        void AddListener(Action<Action, SkillPackageEventArg> subscriber);
        void RemoveListener(Action<Action, SkillPackageEventArg> subscriber);
    }

    public interface ISkillActivate
    {
        void Activate(Action callback, Creature owner, Creature target);
    }

    public class SkillPackageEventArg : EventArgs
    {
        public readonly NetworkObject Caster;
        public readonly NetworkObject Target;
        public readonly Vector3 CastPlace;

        public SkillPackageEventArg(NetworkObject caster, NetworkObject target, Vector3 castPlace)
        {
            Caster = caster;
            Target = target;
            CastPlace = castPlace;
        }
    }
}