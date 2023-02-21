using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Both.Scriptable
{
    public abstract class CreatureModel : ScriptableObject
    {
        public string CreatureName;
        public List<StatsInfo> Status;
        public int SkillSlot;
        public bool TouchDamage;
    }

    [Serializable]
    public class StatsInfo
    {
        public StatsType Type;
        public int Amount;
    }

    public enum StatsType
    {
        Health,
        Strength,
        Defense,
        Speed,
        CriticalHit,
    }
}