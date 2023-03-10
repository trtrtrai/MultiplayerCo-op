using UnityEngine;

namespace Assets.Scripts.Both.Scriptable
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Creature/Character")]
    public class CharacterModel : CreatureModel
    {
        public CharacterClass Classify;
    }

    public enum CharacterClass
    {
        TankerSlash_model,
        FighterSword_model,
        /*FighterSpear_model,*/
        MagicianAttack_model,
        /*MagicianSummon_model,*/
        PriestHealing_model,
    }

    public enum BossName
    {
        Treant,
        Yeti,
        Golem,
    }
}