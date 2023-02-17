using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature;
using System.Collections.Generic;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Server.Creature.Attackable;

namespace Assets.Scripts.Server.Contruction.Builders
{
    public abstract class CreatureBuilder
    {
        protected ICreatureBuild creature = null;

        public abstract void InstantiateGameObject(string name);

        public virtual void GiveName(string name)
        {
            creature.InitName(name);
        }

        public virtual void GiveStatus(List<Stats> status)
        {
            creature.InitStatus(status);
        }

        public virtual void GiveAttackable(Attackable attackable)
        {
            creature.InitAttack(attackable);
            if (attackable.TouchDamage)
            {
                var detect = (creature as Both.Creature.Creature).gameObject.AddComponent<TouchDamageDetect>();
                detect.Owner = creature as ICreature;
            }
        }

        public virtual void AttachGameObject(string path, string name = "")
        {
            if (creature is null) return;

            var obj = GameController.Instance.InstantiateGameObject(path, (creature as Both.Creature.Creature).transform);

            if (name.Equals("")) return;

            obj.name = name;
        }

        public virtual ICreature Release()
        {
            var rs = creature;

            creature = null;

            return (ICreature)rs;
        }
    }
}