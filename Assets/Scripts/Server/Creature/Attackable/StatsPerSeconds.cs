using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Scriptable;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Server.Creature.Attackable
{
    public class StatsPerSeconds : NetworkBehaviour
    {
        [SerializeField] protected int numberEffect;
        [SerializeField] protected float perSecs;
        [SerializeField] protected StatsType type;
        [SerializeField] protected string targetTag; // It's decision what gameObject.tag will take effect
        [SerializeField] protected bool isSetup = false;
        //[SerializeField] protected new Collider2D collider2D;
        [SerializeField] private bool triggered;

        protected Both.Creature.Creature owner;

        public void Setup(int numberEffect, float perSecs, StatsType type, ICreature caster, string targetTag)
        {
            this.numberEffect = numberEffect;
            this.perSecs = perSecs;
            this.type = type;
            this.targetTag = targetTag;
            triggered = true;

            owner = caster as Both.Creature.Creature;
            StartCoroutine(Gap());

            isSetup = true;
        }

        private void Active(GameObject creature)
        {
            //Debug.Log("Active " + targetTag + " " + creature.tag);
            if (!triggered) return;
            switch (targetTag)
            {
                case "Ally":
                    {
                        if (creature.tag.Equals("Character") || creature.tag.Equals("Ally"))
                        {
                            DamageCalculate.Instance.BuffTo(creature.GetComponent<ICreature>(), numberEffect, type);
                        }

                        break;
                    }
                case "Enemy":
                    {
                        if (creature.tag.Equals("Enemy") || creature.tag.Equals("Boss"))
                        {
                            DamageCalculate.Instance.BuffTo(creature.GetComponent<ICreature>(), numberEffect, type);
                        }        

                        break;
                    }
            }
        }

        private void FixedUpdate()
        {
        }

        IEnumerator Gap()
        {
            yield return new WaitForSeconds(.02f);

            triggered = false;

            yield return new WaitForSeconds(perSecs);

            triggered = true;

            StartCoroutine(Gap());
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!IsOwner) return;
            if (!IsSpawned) return;
            if (collision.gameObject is null || collision.gameObject.GetComponent<ICreature>() is null) return;

            //Debug.Log(collision.gameObject.name);
            Active(collision.gameObject);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopAllCoroutines();
        }
    }
}