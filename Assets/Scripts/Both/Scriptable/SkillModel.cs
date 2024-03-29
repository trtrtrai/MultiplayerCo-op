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
        [TextArea(3, 5)] public string Description;
        public float Range;
        public float CastDelay; // time delay before skill actully cast
        public float Cooldown; // time to wait for the next activate
        public Sprite SkillIcon;
        public List<SkillTag> SkillTags;
    }
}