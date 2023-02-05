using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Player;
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
        }
    }
}