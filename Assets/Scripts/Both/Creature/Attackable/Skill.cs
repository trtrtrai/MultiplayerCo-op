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

        public void Activate(Action callback, Creature owner)
        {
            OnActivate?.Invoke(callback, owner.GetComponent<NetworkObject>());
        }

        public void AddListener(Action<Action, NetworkObject> subscriber)
        {
            OnActivate += subscriber;
        }

        public void RemoveListener(Action<Action, NetworkObject> subscriber)
        {
            OnActivate += subscriber;
        }

        public Action<Action, NetworkObject> OnActivate;
    }

    public interface ISkill
    {
        SkillName SkillName { get; }
        float CastDelay { get; }
        float Cooldown { get; }
        string Description { get; }
        void AddListener(Action<Action, NetworkObject> subscriber);
        void RemoveListener(Action<Action, NetworkObject> subscriber);
    }

    public interface ISkillActivate
    {
        void Activate(Action callback, Creature owner);
    }
}