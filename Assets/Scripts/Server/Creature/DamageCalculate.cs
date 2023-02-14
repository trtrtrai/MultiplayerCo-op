using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.DynamicObject;
using Assets.Scripts.Both.Scriptable;
using System.Collections.Generic;
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

        private List<Both.Creature.Creature> immunList;

        public void DamageTo(ICreature creature, NetworkObject attacker, int damage) // if attacker is destroyed -> cannot calc????
        {
            if (attacker.tag.Equals("Bullet"))
            {
                creature.GetStats(StatsType.Health).SetValue(-damage + (creature.GetStats(StatsType.Defense).GetValue() / 2));
            }
            else // attaker is Creature
            {
                //Debug.Log(creature.GetStats(Assets.Scripts.Both.Scriptable.StatsType.Health).GetValue() + " " + damage);
                var atker = attacker.GetComponent<ICreature>();

                //Crit change?
                var rand = Random.Range(1, 101);
                if (rand <= atker.GetStats(StatsType.CriticalHit).GetValue()) // It is a critical hit
                {
                    damage += damage / 2; //boost 50% damage
                }

                //Defense reduce
                damage -= creature.GetStats(StatsType.Defense).GetValue() / 2;

                creature.GetStats(StatsType.Health).SetValue(-damage);
                (creature as Both.Creature.Creature).AddComponent<Knockback>().Setup((atker as Both.Creature.Creature).transform.localPosition, 10f, 0.5f);
            }         
        }

        // Attack this code to creature (AddComponent)
        /*public void PlayFeedback(GameObject sender)
        {
            StopAllCoroutines();
            OnBegin?.Invoke();
            Vector2 direction = (transform.position - sender.transform.position).normalized;
            var script = sender.GetComponent<ObjectDetectHit>();
            if (script is null) rigid.AddForce(direction * strength, ForceMode2D.Impulse);
            else rigid.AddForce(direction * (sender.transform.position.y < gameObject.transform.position.y ? script.Strength + 0.5f : script.Strength), ForceMode2D.Impulse);
            StartCoroutine(Reset());
        }

        private IEnumerator Reset()
        {
            yield return new WaitForSeconds(delay);
            rigid.velocity = Vector2.zero;
            OnDone?.Invoke();
        }*/
    }
}