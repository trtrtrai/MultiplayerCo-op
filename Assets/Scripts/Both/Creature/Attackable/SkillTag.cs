using Assets.Scripts.Both.Scriptable;

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
        public float Duration; // time to end of skill's effect
        public float EffectNumber; // number effect to calculation

        // Attack tag
        public bool AddOrMultiple;

        // Effect tag
        public StatsType StatsType;

        // Special tag
    }
}
