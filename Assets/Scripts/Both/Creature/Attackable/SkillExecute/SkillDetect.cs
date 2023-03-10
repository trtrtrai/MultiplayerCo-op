using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class SkillDetect : NetworkBehaviour
    {
        [SerializeField] private IActiveDetect parent;

        public void Setup(IActiveDetect skill)
        {
            parent = skill;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject is null) return;

            //Debug.Log(collision.gameObject.name);

            parent.SkillTagExecuteCollider2d(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject is null) return;

            //Debug.Log(collision.gameObject.name);

            parent.SkillTagExecuteTrigger2d(collision.gameObject);
        }
    }
}