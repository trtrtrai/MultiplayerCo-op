using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Attackable.SkillExecute;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Controllers
{
    public class BossController : NetworkBehaviour, ICreatureController
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

        public void MoveNonAffect()
        {
            //throw new System.NotImplementedException();
        }

        public void Root()
        {
            //throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start()
        {
            skills = creature.GetSkills();
            timer = 0;
            //init animation
            IsUpdateAnimation = true;
            //animator.SetInteger("orientation", 2);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (/*control.GetComponent<NetworkObject>().OwnerClientId != */NetworkManager.Singleton.LocalClientId != 0) return;

            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                creature.ActivateSkill(0, () => { });

                timer = skills[0].Cooldown;
            }
        }
    }
}