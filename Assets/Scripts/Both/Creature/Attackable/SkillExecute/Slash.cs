using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class Slash : SkillActive
    {
        public override void SetupSkill()
        {
            base.SetupSkill();
            //Debug.Log("Sword Skill Start");

            var objs = GetComponentsInChildren<Collider2D>();
            if (objs.Length == 0) return;
        }

        protected override void SpecializedBehaviour()
        {
            owner.IsUpdateAnimation = false;

            //base.SpecializedBehaviour();

            StartCoroutine(SlashDuration());
            var orien = owner.Animator.GetInteger("orientation");
            owner.Animator.SetInteger("orientation", orien < 0 ? orien : -orien);
            owner.Animator.SetBool("isAttack", true);
        }

        private IEnumerator SlashDuration() //Delay animation
        {
            yield return new WaitForSeconds(skillTags[0].Duration);

            owner.IsUpdateAnimation = true;
            owner.Animator.SetBool("isAttack", false);
            owner.Animator.SetInteger("orientation", Mathf.Abs(owner.Animator.GetInteger("orientation")));
        }

        public override void SkillTagExecuteCollider2d(GameObject obj)
        {
            if (!IsOwner) return;
            if (obj.tag.Equals("Boss") || obj.tag.Equals("Enemy"))
            {

                //send to execute skill tag place
                ICreature attacker = GetComponentInParent<Creature>();
                //Debug.Log("Slash dame" + attacker.GetStats(Scriptable.StatsType.Strength).GetValue() + " " + obj.tag);
                GameController.Instance.Damage(obj.GetComponent<Creature>(), attacker, attacker.GetStats(Scriptable.StatsType.Strength).GetValue());
            }
        }
    }
}