using Unity.Netcode;

namespace Assets.Scripts.Both.Creature.Status
{
    using Scriptable;
    using UnityEngine;
    using UnityEngine.UI;

    public class NetworkStats : NetworkBehaviour
    {
        public NetworkVariable<int> Health = new NetworkVariable<int>();
        public NetworkVariable<int> Strength = new NetworkVariable<int>();
        public NetworkVariable<int> Defense = new NetworkVariable<int>();
        public NetworkVariable<int> Speed = new NetworkVariable<int>();
        public NetworkVariable<int> CriticalHit = new NetworkVariable<int>();

        private int maxHp;
        /*private int maxStr;
        private int maxDef;
        private int maxSpd;
        private int maxCrH;*/
        private Slider healthSlider;

        public void Setup()
        {
            healthSlider = GetComponentInChildren<Slider>();

            var parent = gameObject.transform.parent.GetComponent<Creature>();
            if (parent is null) return;

            if (IsServer)
            {
                /*Health = new NetworkVariable<int>(parent.GetStats(StatsType.Health).GetValue());
                Strength = new NetworkVariable<int>(parent.GetStats(StatsType.Strength).GetValue());
                Defense = new NetworkVariable<int>(parent.GetStats(StatsType.Defense).GetValue());
                Speed = new NetworkVariable<int>(parent.GetStats(StatsType.Speed).GetValue());
                CriticalHit = new NetworkVariable<int>(parent.GetStats(StatsType.CriticalHit).GetValue());*/

                parent.StatsChange += Parent_StatsChange;
            }

            maxHp = parent.GetStats(StatsType.Health).GetValue(false);
            HealthSliderChange(0, Health.Value);
            Health.OnValueChanged += HealthSliderChange;
        }

        private void HealthSliderChange(int oV, int nV)
        {
            //Debug.Log(oV + " " + maxHp);
            healthSlider.value = 1.0f * nV / maxHp;
        }

        private void Parent_StatsChange(object sender, StatsChangeEventArgs args)
        {
            var type = args.Type.Name;

            switch (type)
            {
                case "Health":
                    {
                        Health.Value = args.NewValue;                       

                        break;
                    }
                case "Strength":
                    {
                        Strength.Value = args.NewValue;

                        break;
                    }
                case "Defense":
                    {
                        Defense.Value = args.NewValue;

                        break;
                    }
                case "Speed":
                    {
                        Speed.Value = args.NewValue;

                        break;
                    }
                case "CriticalHit":
                    {
                        CriticalHit.Value = args.NewValue;

                        break;
                    }
            }
        }
    }
}