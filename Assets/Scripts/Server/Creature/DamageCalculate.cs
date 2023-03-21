using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.DynamicObject;
using Assets.Scripts.Both.Scriptable;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Server.Creature
{
    public class DamageCalculate : MonoBehaviour
    {
        public static DamageCalculate Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public int DamageTo(ICreature creature, NetworkObject attacker, int damage) // if attacker is destroyed -> cannot calc????
        {
            if (creature is null || attacker is null) return 0;

            var immuns = (creature as Both.Creature.Creature).GetComponents<ImmunStats>().ToList();
            foreach (var item in immuns)
            {
                if (item.Type == StatsType.Health) return 0;
            }

            if (attacker.tag.Equals("Bullet"))
            {
                damage -= creature.GetStats(StatsType.Defense).GetValue() / 2;
                
                SetDamage(creature, attacker.transform.localPosition, damage);
                //knockback??
            }
            else // attaker is Creature
            {
                //Debug.Log(creature.GetStats(Assets.Scripts.Both.Scriptable.StatsType.Health).GetValue() + " " + damage);
                var atker = attacker.GetComponent<ICreature>();
                var atkerRigid = attacker.GetComponent<Rigidbody2D>();

                //Crit change?
                var rand = Random.Range(1, 101);
                if (rand <= atker.GetStats(StatsType.CriticalHit).GetValue()) // It is a critical hit
                {
                    damage += damage / 2; //boost 50% damage
                }

                //Defense reduce
                damage -= creature.GetStats(StatsType.Defense).GetValue() / 2;

                SetDamage(creature, attacker.transform.localPosition, damage);
                if (creature.Form == CreatureForm.Boss) return damage;
                (creature as Both.Creature.Creature).AddComponent<Knockback>().Setup((atker as Both.Creature.Creature).transform.localPosition, atkerRigid.mass*2, .75f);
            }

            return damage;
        }

        public int TouchTo(ICreature creature, NetworkObject attacker, int damage)
        {
            if (creature is null || attacker is null) return 0;

            var immuns = (creature as Both.Creature.Creature).GetComponents<ImmunStats>().ToList();
            foreach (var item in immuns)
            {
                if (item.Type == StatsType.Health) return 0;
            }

            /*//Defense reduce
            damage -= creature.GetStats(StatsType.Defense).GetValue() / 3;*/

            SetDamage(creature, attacker.transform.localPosition, damage);
            (creature as Both.Creature.Creature).AddComponent<ImmunStats>().Setup(StatsType.Health, 0.5f);
            return damage;
        }

        public void BuffTo(ICreature creature, int amount, StatsType type)
        {
            if (creature is null) return;

            creature.GetStats(type).SetValue(amount);
            DamageUI((creature as NetworkBehaviour).transform.localPosition, (creature as NetworkBehaviour).transform.localPosition + Vector3.down, amount);
        }

        private void SetDamage(ICreature creature, Vector3 atkerPos, int damage)
        {
            creature.GetStats(StatsType.Health).SetValue(-damage);
            DamageUI((creature as NetworkBehaviour).transform.localPosition, atkerPos, damage);
        }

        private void DamageUI(Vector3 worldPosition, Vector3 atkerPos, int damage)
        {
            var obj = GameController.Instance.InstantiateGameObject("DynamicObject/DamageText", null);

            obj.transform.localPosition = worldPosition;

            //obj.GetComponentInChildren<TMP_Text>().text = "-" + damage.ToString();

            GameController.Instance.SpawnGameObject(obj, true);

            obj.AddComponent<DamageText>().Setup(worldPosition - atkerPos, damage);
        }
    }
}