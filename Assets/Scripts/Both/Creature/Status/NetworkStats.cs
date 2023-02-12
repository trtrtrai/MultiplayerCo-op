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

        public NetworkVariable<int> MaxHealth = new NetworkVariable<int>();
        public NetworkVariable<int> MaxStrength = new NetworkVariable<int>();
        public NetworkVariable<int> MaxDefense = new NetworkVariable<int>();
        public NetworkVariable<int> MaxSpeed = new NetworkVariable<int>();
        public NetworkVariable<int> MaxCriticalHit = new NetworkVariable<int>();
        private HealthColorManage healthSlider;

        public void Setup()
        {
            healthSlider = GetComponentInChildren<HealthColorManage>();

            var parent = gameObject.transform.parent.GetComponent<Creature>();
            if (parent is null) return;

            if (IsServer)
            {
                /*Health.Value = parent.GetStats(StatsType.Health).GetValue();
                Strength = new NetworkVariable<int>(parent.GetStats(StatsType.Strength).GetValue());
                Defense = new NetworkVariable<int>(parent.GetStats(StatsType.Defense).GetValue());
                Speed = new NetworkVariable<int>(parent.GetStats(StatsType.Speed).GetValue());
                CriticalHit = new NetworkVariable<int>(parent.GetStats(StatsType.CriticalHit).GetValue());

                MaxHealth.Value = parent.GetStats(StatsType.Health).GetValue(false);
                MaxStrength.Value = parent.GetStats(StatsType.Strength).GetValue(false);
                MaxDefense.Value = parent.GetStats(StatsType.Defense).GetValue(false);
                MaxSpeed.Value = parent.GetStats(StatsType.Speed).GetValue(false);
                MaxCriticalHit.Value = parent.GetStats(StatsType.CriticalHit).GetValue(false);*/
                parent.StatsChange += Parent_StatsChange;
            }

            HealthSliderChange(0, Health.Value);
            Health.OnValueChanged += HealthSliderChange;
        }

        private void HealthSliderChange(int oV, int nV)
        {
            //Debug.Log(oV + " " + maxHp);
            var ratio = 1.0f * nV / MaxHealth.Value;
            healthSlider.Fill.fillAmount = ratio;
            healthSlider.SliderValueChange();
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