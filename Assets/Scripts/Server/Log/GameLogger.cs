using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Server.Log
{
    public class GameLogger : MonoBehaviour
    {
        public static GameLogger Instance { get; private set; }

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

        [SerializeField] private Dictionary<ulong, int> damageDealt;
        [SerializeField] private Dictionary<ulong, int> healthHeal;

        //Boss also list in future
        [SerializeField] private int bossDamageDealth = 0; //use after
        [SerializeField] private int bossHealthHeal = 0; //use after

        [SerializeField] private float timer = 0f;

        public void AddPlayer(ulong clientId)
        {
            try
            {
                damageDealt.Add(clientId, 0);

            }
            catch
            {
                damageDealt = new Dictionary<ulong, int>();
                damageDealt.Add(clientId, 0);
            }

            try
            {
                healthHeal.Add(clientId, 0);

            }
            catch
            {
                healthHeal = new Dictionary<ulong, int>();
                healthHeal.Add(clientId, 0);
            }
        }

        public void PlayerLog(ulong clientId, int amount, bool isDamage = true)
        {
            if (!damageDealt.ContainsKey(clientId)) return; //dameDealt same with healthHeal in this case

            if (isDamage)
            {
                damageDealt[clientId] += amount;
            }
            else
            {
                healthHeal[clientId] += amount;
            }
        }

        public void BossLog(int amount, bool isDamage = true)
        {
            if (isDamage)
            {
                bossDamageDealth += amount;
            }
            else
            {
                bossHealthHeal += amount;
            }
        }

        public void GetPlayerLog(out Dictionary<ulong, int> damageDealt, out Dictionary<ulong, int> healthHeal)
        {
            damageDealt = this.damageDealt;
            healthHeal = this.healthHeal;
        }

        //public void GetBossLog(out int damage, out int healing)

        public float GetTime() => timer;

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime / 2;
        }
    }
}