using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class Slash : SkillActive
    {
        protected override void Start()
        {
            base.Start();
            //Debug.Log("Sword Skill Start");

            var objs = GetComponentsInChildren<Collider2D>();
            if (objs.Length == 0) return;

            objs.ToList().ForEach(o => o.gameObject.AddComponent<SkillDetect>());
        }

        protected override void SpecializedBehaviour()
        {
            owner.IsUpdateAnimation = false;

            base.SpecializedBehaviour();

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
            if (obj.tag.Equals("Boss") || obj.tag.Equals("Enemy"))
            {
                Debug.Log("Slash dame " + obj.tag);

                //send to execute skill tag place
            }
        }
    }
}