using System.Collections;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class Shield : SkillActive, IActiveDetect
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

            if (owner != null || GetOwner())
            {
                owner.IsUpdateAnimation = true;
                owner.Animator.SetBool("isShield", false);
                owner.Animator.SetInteger("orientation", Mathf.Abs(owner.Animator.GetInteger("orientation")));
                owner.MoveNonAffect();
            }         
        }

        public override void SkillTagExecuteTrigger2d(GameObject obj)
        {
            if (!IsOwner) return;
            if (obj is null) return;

            if (obj.tag.Equals("Bullet"))
            {
                if (!obj.GetComponent<NetworkObject>().IsSpawned) return;
                obj.GetComponent<NetworkObject>().Despawn();
                Destroy(obj);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}