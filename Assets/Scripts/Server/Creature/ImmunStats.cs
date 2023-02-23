using Assets.Scripts.Both.Scriptable;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Server.Creature
{
    /// <summary>
    /// Server only
    /// </summary>
    public class ImmunStats : MonoBehaviour
    {
        [SerializeField] private StatsType type; //future is List???
        [SerializeField] private float time;
        //[SerializeField] private bool isSetup = false;

        public StatsType Type => type;

        public void Setup(StatsType type, float time)
        {
            this.type = type;
            this.time = time;

            //isSetup = true;

            StartCoroutine(Wait());
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);

            Destroy(this);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}