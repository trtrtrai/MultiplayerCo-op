using Assets.Scripts.Both.Creature.Attackable;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Controllers
{
    public class EnemyController : NetworkBehaviour, ICreatureController
    {
        private ICreature creature;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D player;
        [SerializeField] private Transform target;
        [SerializeField] private List<ISkill> skills;
        [SerializeField] private float timer;

        public bool IsUpdateAnimation { get; set; }
        public Animator Animator => animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<Rigidbody2D>();
            creature = GetComponent<Creature>();
        }

        void Start()
        {
            skills = creature.GetSkills();
            timer = 0;
            //init animation
            IsUpdateAnimation = true;
        }

        public void MoveNonAffect()
        {
            //throw new System.NotImplementedException();
        }

        public void Root()
        {
            //throw new System.NotImplementedException();
        }

        private void FixedUpdate()
        {      
            if (NetworkManager.Singleton.LocalClientId != 0) return;

            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                Creature targetCreature;

                if (target) targetCreature = target.GetComponent<Creature>();
                else targetCreature = GetComponent<Creature>();
                creature.ActivateSkill(0, () => { }, targetCreature);

                timer = skills[0].Cooldown;
            }
        }
    }
}