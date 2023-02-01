using Assets.Scripts.Both.Creature.Attackable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Both.Scriptable
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Creature/Skill")]
    public class SkillModel : ScriptableObject
    {
        public SkillName SkillName;
        [TextArea] public string Description;
        public float CastDelay; // time delay before skill actully cast
        public float Cooldown; // time to wait for the next activate
        public List<SkillTag> SkillTags;
    }
}