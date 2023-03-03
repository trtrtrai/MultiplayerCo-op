using Assets.Scripts.Both.Scriptable;
using System.Numerics;

namespace Assets.Scripts.Both.Creature.Attackable 
{
    /// <summary>
    /// List at SkillModel to setup skill behaviour
    /// </summary>
    [System.Serializable]
    public class SkillTag
    {
        public TagType Tag; // Choice tag type

        // 1 in 3 option will show for choice
        public AttackTag Attack;
        public EffectTag Effect;
        public SpecialTag Special;

        // Always show
        public float Duration; //time to end of skill's effect
        public float EffectNumber; //number effect to calculation

        // Attack tag
        public bool AddOrMultiple; //true is multiple
        public bool IsNormal; //Bullet
        public int BulletAmount; //Bullet
        public int BulletRadian; //Bullet
        public CastDirection Direction; //Bullet
        public AreaType AreaType; //Area tags
        public string ObjName; //Area tags + Bullet
        public float PerSeconds; //Self Area
        //public bool isFollow //Self Area

        // Effect tag
        public StatsType StatsType; // Effect tag + Self Area

        // Special tag
        public SummonPlace SPlace; //Summon
        public string SummonCreature; //Summon
        public int SummonAmount; //Summon
        //Vector2 Offset Summon??
        public TeleportPlace TPlace; //Teleport. true -> caster always teleport on this point instead of touch obstacles
        public float Distance; //Teleport
    }
}
