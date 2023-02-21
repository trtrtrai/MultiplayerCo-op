using Assets.Scripts.Both.Creature.Boss;
using System.Collections.Generic;

namespace Assets.Scripts.Server.Contruction.Builders
{
    public class BossBuilder : CreatureBuilder
    {
        public override void InstantiateGameObject(string name)
        {
            creature = CreatureFactory.Instance.CreateCreature(Both.Creature.CreatureForm.Boss, name);
        }

        public void Setup(List<float> changeLimit)
        {
            (creature as Boss).Setup(changeLimit);
        }
    }
}