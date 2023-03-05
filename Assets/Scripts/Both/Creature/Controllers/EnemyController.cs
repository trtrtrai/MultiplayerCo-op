using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Scriptable;
using Pathfinding;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Controllers
{
    public class EnemyController : NetworkBehaviour, ICreatureController
    {
        private ICreature creature;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private Transform target;
        [SerializeField] private int speed;
        [SerializeField] private float nextWayPointDistance = .5f;
        Path path;
        [SerializeField]  int currentWayPoint;
        [SerializeField]  bool reachedEndOfPath; // is in range of target
        Seeker seeker;
        [SerializeField] private List<ISkill> skills;
        [SerializeField] private float timer; //list skills -> list timer

        public bool IsUpdateAnimation { get; set; }
        public Animator Animator => animator;

        private void Awake()
        {
            if (NetworkManager.Singleton.LocalClientId != 0) return;

            animator = GetComponent<Animator>();
            creature = GetComponent<Creature>();   

            rigid = GetComponent<Rigidbody2D>();
            seeker = GetComponent<Seeker>();

            target = FindCharacter();
        }

        void Start()
        {
            if (NetworkManager.Singleton.LocalClientId != 0) return;

            speed = creature.GetStats(StatsType.Speed).GetValue();
            (creature as Creature).StatsChange += (o, a) => { if (a.Type.Name.Equals(StatsType.Speed.ToString())) speed = a.NewValue; };

            skills = creature.GetSkills();
            timer = Time.realtimeSinceStartup;

            //init animation
            IsUpdateAnimation = true;

            InvokeRepeating("UpdatePath", 0f, 0.5f);
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

            if (!target)
            {
                target = FindCharacter();
                if (!target) return;
            }

            //Move
            MoveTarget();

            //Skill choice
            /*if (Time.realtimeSinceStartup >= timer)
            {
                timer = Time.realtimeSinceStartup + skills[0].Cooldown;
            }*/

            //Animation
            if (!IsUpdateAnimation) return;

            UpdateAnimation();
        }

        private Transform FindCharacter()
        {
            var objs = GameObject.FindGameObjectsWithTag("Character");

            if (objs.Length == 0) return null;
            if (objs.Length == 1) return objs[0].transform;

            var rand = Random.Range(0, objs.Length);

            return objs[rand].transform;
        }

        private void UpdateAnimation()
        {
            var scale = transform.localScale;

            if (rigid.velocity.x > 0.01f)
            {
                transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
            }
            else if (rigid.velocity.x < -0.01f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
            }
        }

        #region Pathfinding
        private void MoveTarget()
        {
            if (path is null) return;

            if (currentWayPoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            var direction = ((Vector2)path.vectorPath[currentWayPoint] - rigid.position).normalized;

            if (Time.realtimeSinceStartup >= timer && Vector2.Distance((Vector2)target.localPosition, rigid.position) <= skills[0].Range)
            {
                Attack();
                animator.SetFloat("speed", 0);
                return;
            }

            var vectorSpeed = direction * speed * Time.fixedDeltaTime;
            rigid.velocity = new Vector2(vectorSpeed.x, vectorSpeed.y);
            animator.SetFloat("speed", Mathf.Abs(vectorSpeed.x) + Mathf.Abs(vectorSpeed.y));

            float distance = Vector2.Distance(rigid.position, path.vectorPath[currentWayPoint]);

            if (distance < nextWayPointDistance)
            {
                currentWayPoint++;
            }
        }

        void Attack()
        {
            creature.ActivateSkill(0, () => { }, target);

            timer = Time.realtimeSinceStartup + skills[0].Cooldown;
        }

        private void UpdatePath()
        {
            if (target && seeker.IsDone())
            {
                seeker.StartPath(rigid.position, target.localPosition, OnPathComplete);
            }
        }

        private void OnPathComplete(Path p)
        {
            if (p.error) return;

            path = p;
            currentWayPoint = 0;
        }
        #endregion
    }
}