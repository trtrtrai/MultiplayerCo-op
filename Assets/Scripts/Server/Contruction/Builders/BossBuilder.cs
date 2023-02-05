namespace Assets.Scripts.Server.Contruction.Builders
{
    public class BossBuilder : CreatureBuilder
    {
        public override void InstantiateGameObject(string name)
        {
            creature = CreatureFactory.Instance.CreateCreature(Both.Creature.CreatureForm.Boss, name);
        }
    }
}