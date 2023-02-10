using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Server.Creature
{
    public class TemporaryStats : MonoBehaviour
    {
        [SerializeField] private ICreature owner;
        [SerializeField] private IStats stats;
        [SerializeField] private int temp;

        public void Setup(ICreature owner, StatsType type, int value, float secs)
        {
            if (owner is null) return;

            temp = value;
            stats = owner.GetStats(type);
            stats.SetTemporary(temp);
            StartCoroutine(ResetTemporary(secs));
        }

        private IEnumerator ResetTemporary(float secs)
        {
            yield return new WaitForSecondsRealtime(secs);

            stats.SetTemporary(-temp);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}