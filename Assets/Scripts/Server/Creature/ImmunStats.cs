using Assets.Scripts.Both.Scriptable;
using UnityEngine;

namespace Assets.Scripts.Server.Creature
{
    /// <summary>
    /// Server only
    /// </summary>
    public class ImmunStats : MonoBehaviour
    {
        [SerializeField] private StatsType type; //future is List???
        [SerializeField] private float timer;
        [SerializeField] private bool isSetup = false;

        public StatsType Type => type;

        public void Setup(StatsType type, float time)
        {
            this.type = type;
            timer = time;

            isSetup = true;
        }

        private void FixedUpdate()
        {
            if (!isSetup) return;

            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}