using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Attackable.SkillExecute;
using Assets.Scripts.Both.DynamicObject;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
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

            if (target is null)
            {
                var p = GameObject.FindGameObjectWithTag("Character");
                target = p is null? null : p.transform;
            }

            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                Creature targetCreature;

                if (target) targetCreature = target.GetComponent<Creature>();
                else targetCreature = GetComponent<Creature>();
                
                var place = FindSpawningPlace(targetCreature.transform);
                var skillObj = GameController.Instance.InstantiateGameObject("SkillEffect/" + creature.GetSkills()[0].SkillName, null);
                skillObj.transform.localPosition = place.localPosition;
                GameController.Instance.SpawnGameObject(skillObj, true);
                skillObj.GetComponent<AutoDestroy>().Setup(3.5f);

                creature.ActivateSkill(0, () => { }, place);
                timer = skills[0].Cooldown;
            }
        }

        private Transform FindSpawningPlace(Transform targetCreature)
        {
            var objs = GameObject.FindGameObjectsWithTag("SpecialPoint").Select(o => o.transform);

            List<float> distance = new List<float>();


            for (int i = 0; i < objs.Count(); i++)
            {
                distance.Add((objs.ElementAt(i).localPosition - targetCreature.localPosition).magnitude);
            }

            if (distance.Count == 0) return objs.ElementAt(0);

            return objs.ElementAt(distance.IndexOf(distance.Min()));
        }
    }
}