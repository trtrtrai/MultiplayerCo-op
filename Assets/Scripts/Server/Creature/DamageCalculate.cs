using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.DynamicObject;
using Assets.Scripts.Both.Scriptable;
using System.Collections.Generic;
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

        public void DamageTo(ICreature creature, NetworkObject attacker, int damage) // if attacker is destroyed -> cannot calc????
        {
            if (creature is null || attacker is null) return;

            var immuns = (creature as Both.Creature.Creature).GetComponents<ImmunStats>().ToList();
            foreach (var item in immuns)
            {
                if (item.Type == StatsType.Health) return;
            }

            if (attacker.tag.Equals("Bullet"))
            {
                creature.GetStats(StatsType.Health).SetValue(-damage + (creature.GetStats(StatsType.Defense).GetValue() / 2));
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

                creature.GetStats(StatsType.Health).SetValue(-damage);
                (creature as Both.Creature.Creature).AddComponent<Knockback>().Setup((atker as Both.Creature.Creature).transform.localPosition, atkerRigid.mass*2, .75f);
            }         
        }

        public void TouchTo(ICreature creature, NetworkObject attacker, int damage)
        {
            if (creature is null || attacker is null) return;

            var immuns = (creature as Both.Creature.Creature).GetComponents<ImmunStats>().ToList();
            foreach (var item in immuns)
            {
                if (item.Type == StatsType.Health) return;
            }

            /*//Defense reduce
            damage -= creature.GetStats(StatsType.Defense).GetValue() / 2;*/

            creature.GetStats(StatsType.Health).SetValue(-damage);
            (creature as Both.Creature.Creature).AddComponent<ImmunStats>().Setup(StatsType.Health, 0.5f);
        }
    }
}