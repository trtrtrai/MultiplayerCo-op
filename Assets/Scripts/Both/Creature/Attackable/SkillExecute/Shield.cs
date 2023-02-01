using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class Shield : SkillActive
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
            owner.Root();

            base.SpecializedBehaviour();

            StartCoroutine(ShieldDuration());
            var orien = owner.Animator.GetInteger("orientation");
            owner.Animator.SetInteger("orientation", orien < 0 ? orien : -orien);
            owner.Animator.SetBool("isShield", true);
        }

        private IEnumerator ShieldDuration() //Delay animation
        {
            yield return new WaitForSeconds(skillTags[0].Duration);

            owner.IsUpdateAnimation = true;
            owner.Animator.SetBool("isShield", false);
            owner.Animator.SetInteger("orientation", Mathf.Abs(owner.Animator.GetInteger("orientation")));
            owner.MoveNonAffect();
        }

        public override void SkillTagExecuteCollider2d(GameObject obj)
        {
            if (obj is null) return;

            if (obj.tag.Equals("Bullet"))
            {
                Destroy(obj);
            }
        }

        public override void SkillTagExecuteTrigger2d(GameObject obj)
        {
            if (obj is null) return;

            if (obj.tag.Equals("Bullet"))
            {
                Destroy(obj);
            }
        }
    }
}