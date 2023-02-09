using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Player;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Server.Contruction
{
    /// <summary>
    /// Server-side execute
    /// </summary>
    public class CreatureFactory : NetworkBehaviour
    {
        public static CreatureFactory Instance { get; private set; }

        [SerializeField] private List<ICreature> creatures;

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

        public ICreatureBuild CreateCreature(CreatureForm form, string name)
        {
            Both.Creature.Creature creature = null;
            Transform obj;

            switch (form)
            {
                case CreatureForm.Character:
                    {
                        obj = Instantiate(Resources.Load<GameObject>("Player/" + name)).transform;

                        creature = obj.gameObject.GetComponent<Both.Creature.Creature>();   
                        break;
                    }
                case CreatureForm.Boss:
                    {
                        obj = Instantiate(Resources.Load<GameObject>("Boss/" + name + "/" + name)).transform;

                        creature = obj.gameObject.GetComponent<Both.Creature.Creature>();

                        break;
                    }
                case CreatureForm.Other:
                    {
                        obj = Instantiate(Resources.Load<GameObject>("OtherCreature/" + name + "/" + name)).transform;

                        creature = obj.gameObject.GetComponent<Both.Creature.Creature>();

                        break;
                    }
            }

            if (creature != null) IdentifyCreature(creature);

            return creature;
        }

        private void IdentifyCreature(ICreature creature)
        {
            try
            {
                creatures.Add(creature);
            }
            catch
            {
                creatures = new List<ICreature>();
                creatures.Add(creature);
            }

            (creature as Both.Creature.Creature).StatsChange += CreatureFactory_StatsChange;
        }

        private void CreatureFactory_StatsChange(object arg1, StatsChangeEventArgs arg2)
        {
            Debug.Log(arg1 is null);
            if (arg1 is null) return;
            Debug.Log(arg2.Type.Name + "=Health? " + arg2.Type.Name.Equals(StatsType.Health.ToString()) + " " + arg2.NewValue);
            if (arg2.Type.Name.Equals(StatsType.Health.ToString()) && arg2.NewValue <= 0)
            {
                Debug.Log((arg1 as ICreature).Name + " dead");

                (arg1 as Both.Creature.Creature).NetworkObject.Despawn();
                Destroy((arg1 as Both.Creature.Creature).gameObject);
            }
        }

    }
}