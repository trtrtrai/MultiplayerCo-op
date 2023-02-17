using Assets.Scripts.Both.Creature;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Server.Creature.Attackable
{
    /// <summary>
    /// Server only
    /// </summary>
    public class TouchDamageDetect : MonoBehaviour
    {
        public ICreature Owner;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject is null || Owner is null) return;

            var creature = collision.gameObject.GetComponent<ICreature>();
            if (creature is null) return;
            
            var rs = GameController.Instance.CreatureTagDetect(tag, collision.gameObject.tag); // true-touch/false-noTouch/null-return

            if (rs == true)
            {
                DamageCalculate.Instance.TouchTo(creature, GetComponent<NetworkObject>(), Owner.GetStats(Both.Scriptable.StatsType.Strength).GetValue() / 3);
            }
        }
    }
}