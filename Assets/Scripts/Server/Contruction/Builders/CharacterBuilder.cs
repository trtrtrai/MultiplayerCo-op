using Assets.Scripts.Both.Creature;

namespace Assets.Scripts.Server.Contruction.Builders
{
    public class CharacterBuilder : CreatureBuilder
    {
        public override void InstantiateGameObject(string name)
        {
            creature = CreatureFactory.Instance.CreateCreature(CreatureForm.Character, name);
        }
    }
}