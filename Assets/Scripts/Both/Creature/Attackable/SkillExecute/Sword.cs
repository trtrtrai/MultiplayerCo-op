using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class Sword : SkillActive, IActiveDetect
    {
        public override void SetupSkill()
        {
            base.SetupSkill();
            //Debug.Log("Sword Skill Start");

            var objs = GetComponentsInChildren<Collider2D>();
            if (objs.Length == 0) return;

            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].AddComponent<SkillDetect>().Setup(this);
            }
        }

        protected override void SpecializedBehaviour()
        {
            if (owner is null && !GetOwner()) return;
            owner.IsUpdateAnimation = false;

            //base.SpecializedBehaviour();

            StartCoroutine(SwordDuration());
            var orien = owner.Animator.GetInteger("orientation");
            owner.Animator.SetInteger("orientation", orien < 0 ? orien : -orien);
            owner.Animator.SetBool("isAttack", true);
        }

        private IEnumerator SwordDuration() //Delay animation
        {
            yield return new WaitForSeconds(skillTags[0].Duration);

            if (owner != null || GetOwner())
            {
                owner.IsUpdateAnimation = true;
                owner.Animator.SetBool("isAttack", false);
                owner.Animator.SetInteger("orientation", Mathf.Abs(owner.Animator.GetInteger("orientation")));
            }               
        }

        public override void SkillTagExecuteCollider2d(GameObject obj)
        {
            if (!IsOwner) return;
            if (obj.tag.Equals("Boss") || obj.tag.Equals("Enemy") || obj.tag.Equals("Mobs"))
            {

                //send to execute skill tag place
                Creature attacker = GetComponentInParent<Creature>();
                //Debug.Log("Slash dame" + attacker.GetStats(Scriptable.StatsType.Strength).GetValue() + " " + obj.tag);
                GameController.Instance.Damage(obj.GetComponent<Creature>(), attacker.NetworkObject, attacker.GetStats(Scriptable.StatsType.Strength).GetValue());
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}