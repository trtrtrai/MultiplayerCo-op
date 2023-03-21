using Assets.Scripts.Server.Creature;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class TreeLifeSteal : SkillActive, IActiveDetect
    {
        public override void SetupSkill()
        {
            base.SetupSkill();

            var obj = transform.parent.GetComponent<Collider2D>();
            if (obj is null) return;

            obj.AddComponent<SkillDetect>().Setup(this);
        }

        public override void SkillTagExecuteCollider2d(GameObject obj)
        {
            if (!IsOwner) return;

            if (GameController.Instance.CreatureTagDetect(transform.parent.tag, obj.tag) == true) // other option lifesteal: detect include enemy (boss steal enemy health to healing :v)
            {
                //send to execute skill tag place
                Creature attacker = GetComponentInParent<Creature>();
                //Debug.Log("Slash dame" + attacker.GetStats(Scriptable.StatsType.Strength).GetValue() + " " + obj.tag);
                var damage = GameController.Instance.Damage(obj.GetComponent<Creature>(), attacker.NetworkObject, attacker.GetStats(Scriptable.StatsType.Strength).GetValue());
                GameController.Instance.Log((owner as NetworkBehaviour).GetComponent<ICreature>(), damage);

                DamageCalculate.Instance.BuffTo((owner as NetworkBehaviour).GetComponent<ICreature>(), damage, Scriptable.StatsType.Health);
                GameController.Instance.Log((owner as NetworkBehaviour).GetComponent<ICreature>(), damage, false);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}