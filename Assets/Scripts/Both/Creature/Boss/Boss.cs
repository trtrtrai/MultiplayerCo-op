using System.Collections.Generic;

namespace Assets.Scripts.Both.Creature.Boss
{
    public class Boss : Creature
    {
        private List<float> changeLimit;

        private void Awake()
        {
            form = CreatureForm.Boss;
        }

        public void Setup(List<float> changeLimit)
        {
            if (this.changeLimit != null) return;

            this.changeLimit = changeLimit;
        }

        public List<float> GetLimitChange()
        {
            var rt = new List<float>(changeLimit);

            return rt;
        }
    }
}