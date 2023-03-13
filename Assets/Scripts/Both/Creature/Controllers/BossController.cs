using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Controllers
{
    public class BossController : NetworkBehaviour, ICreatureController
    {
        private ICreature creature;
        private int lookAtRad;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private Transform target;
        [SerializeField] private NetworkStats stats;
        [SerializeField] private int speed;
        [SerializeField] private float nextWayPointDistance = .5f;
        Path path;
        [SerializeField] int currentWayPoint;
        [SerializeField] bool reachedEndOfPath; // is in range of target
        Seeker seeker;
        [SerializeField] private List<ISkill> skills;
        [SerializeField] private float timer;

        private Queue<int> skillQueue;
        private List<bool> skillActivable;

        private int currentHealth;
        private int limitChange; // list???

        public bool IsUpdateAnimation { get; set; }
        public Animator Animator => animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            creature = GetComponent<Creature>();

            rigid = GetComponent<Rigidbody2D>();
            stats = GetComponentInChildren<NetworkStats>();

            if (NetworkManager.Singleton.LocalClientId != 0) return;
            seeker = GetComponent<Seeker>();
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
            stats.TriggerRotation.OnValueChanged += OnRotationTriggered;

            if (NetworkManager.Singleton.LocalClientId != 0) return;

            stats.VectorState.Value = Vector2.one;
            stats.VectorScale.Value = transform.localScale;

            target = FindCharacter();

            // get and listen speed
            speed = creature.GetStats(StatsType.Speed).GetValue();
            (creature as Creature).StatsChange += (o, a) => { if (a.Type.Name.Equals(StatsType.Speed.ToString())) speed = a.NewValue; };

            //listen health to find other target (30-45%)
            limitChange = (int)((creature as Boss.Boss).GetLimitChange()[0] * creature.GetStats(StatsType.Health).GetValue(false));
            currentHealth = creature.GetStats(StatsType.Health).GetValue();
            (creature as Creature).StatsChange += (o, a) => { 
                if (a.Type.Name.Equals(StatsType.Health.ToString())) currentHealth = a.NewValue; 

                if (a.OldValue > a.NewValue)
                {
                    var rand = Random.Range(0f, 1f);
                    if (rand <= 0.3f)
                    {
                        Debug.Log("Follow other target");
                        target = FindCharacter();
                    }
                }

                if (a.NewValue < limitChange)
                {
                    Transformer();
                }
            };

            //Setup skill choice
            skills = creature.GetSkills();

            skillActivable = new List<bool>();
            for (int i = 0; i < skills.Count; i++)
            {
                skillActivable.Add(true);
            }

            skillQueue = new Queue<int>();
            FillSkillQueue();
            timer = 0f;

            //init animation
            IsUpdateAnimation = true;

            //Pathfinding
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!IsHost && IsClient)
            {
                rigid.velocity = stats.VectorSpeed.Value * stats.VectorState.Value; //only Client
            }

            transform.localScale = stats.VectorScale.Value; //Client + server

            if (/*control.GetComponent<NetworkObject>().OwnerClientId != */NetworkManager.Singleton.LocalClientId != 0) return;

            if (!target)
            {
                target = FindCharacter();
                if (!target) return;
            }

            //Move
            MoveTarget();

            //Skill active timer
            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }

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

        #region Skill choice
        private void FillSkillQueue()
        {
            if (skillQueue.Count <= 5)
            {
                var addAmount = Random.Range(15, 21);

                for (int i = 0; i < addAmount; i++)
                {
                    var skillNum = Random.Range(0, 1000);

                    if (currentHealth <= limitChange) skillQueue.Enqueue(skills.Count - 1);
                    else skillQueue.Enqueue(skillNum % (skills.Count - 1)); // last skill is after transformer
                }
            }
        }

        private void NextSkill()
        {
            skillQueue.TryDequeue(out int t);

            FillSkillQueue(); // <=5 count --> fill

            int limit = 5;
            while (limit != 0 && !skillActivable[skillQueue.Peek()])  //if Peek() activable is false -> turn it to last queue
            {
                var i = skillQueue.Dequeue();
                skillQueue.Enqueue(i);
                limit--;
            }

            timer = Random.Range(1.5f, 3f); //new timer
        }

        private void ActiveSkillQueue()
        {
            switch (skillQueue.Peek())
            {
                case 0:
                    {
                        Attack();
                        break;
                    }
                case 1:
                    {
                        Summon();
                        break;
                    }
                case 2:
                    {
                        CastSpell();
                        break;
                    }
                case 3:
                    {
                        SpecialAttack();
                        break;
                    }
            }
        }

        void Attack() ////Treant: TreeAttack
        {
            skillActivable[0] = false;
            creature.ActivateSkill(0, () => { skillActivable[0] = true; }, target);

            NextSkill();
        }

        void Summon() //Treant: BatSummon
        {
            skillActivable[1] = false;
            creature.ActivateSkill(1, () => { skillActivable[1] = true; }, target);

            NextSkill();
        }

        void CastSpell() //Treant: TriFireBall
        {
            skillActivable[2] = false;
            creature.ActivateSkill(2, () => { skillActivable[2] = true; }, target);

            NextSkill();
        }

        void Transformer() //Treant: Transformerrrrrrrr
        {
            //animation parameter "isTransformer"
            skillQueue.Clear();
            FillSkillQueue();
            animator.SetBool("isTransformer", true);
            StartCoroutine(Wait());
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.25f);

            animator.SetBool("isTransformer", false);
        }

        void SpecialAttack() //Treant: SunBoost
        {
            skillActivable[3] = false;
            creature.ActivateSkill(3, () => { skillActivable[3] = true; }, transform);

            NextSkill();
        }
        #endregion

        #region Animation
        private void UpdateAnimation()
        {
            LookAtTarget();
        }

        //Flipped and rotate boss follow target distance
        private void LookAtTarget()
        {
            Vector3 flipped = transform.localScale;
            var distanceVector = target.localPosition - (transform.localPosition + new Vector3(0, 2.5f));
            var distance = distanceVector.normalized;

            if (distance.x > 0 && distance.y > 0) //top-right = boss's face is back-right (zFlipped = -1 and yRotate = 0)
            {
                if (lookAtRad == 1) return;

                if (lookAtRad != 4)
                {
                    flipped.z *= -1f;
                    stats.VectorScale.Value = flipped;
                }

                if (lookAtRad != 2)
                {
                    stats.TriggerRotation.Value = true;
                }
                //Debug.Log(distance);
                lookAtRad = 1;
            }
            else if (distance.x > 0 && distance.y <= 0) //bottom-right = boss's face is front-right (zFlipped = 1 and yRotate = 0)
            {
                if (lookAtRad == 2) return;

                if (lookAtRad != 3)
                {
                    flipped.z *= -1f;
                    stats.VectorScale.Value = flipped;
                }

                if (lookAtRad != 1)
                {
                    stats.TriggerRotation.Value = true;
                }
                //Debug.Log(distance);
                lookAtRad = 2;
            }
            else if (distance.x <= 0 && distance.y > 0) //top-left = boss's face is back-left (zFlipped = 1 and yRotate = 180)
            {
                if (lookAtRad == 3) return;

                if (lookAtRad != 2)
                {
                    flipped.z *= -1f;
                    stats.VectorScale.Value = flipped;
                }

                if (lookAtRad != 4)
                {
                    stats.TriggerRotation.Value = true;
                }
                //Debug.Log(distance);
                lookAtRad = 3;
            }
            else if (distance.x <= 0 && distance.y <= 0) //bottom-left = boss's face is back-left (zFlipped = -1 and yRotate = 180)
            {
                if (lookAtRad == 4) return;

                if (lookAtRad != 1)
                {
                    flipped.z *= -1f;
                    stats.VectorScale.Value = flipped;
                }

                if (lookAtRad != 3)
                {
                    stats.TriggerRotation.Value = true;
                }
                //Debug.Log(distance);
                lookAtRad = 4;
            }
        }

        private void OnRotationTriggered(bool oldV, bool newV)
        {
            if (newV)
            {
                transform.Rotate(0f, 180f, 0f);

                if (NetworkManager.Singleton.LocalClientId == 0) stats.TriggerRotation.Value = false;
            }
        }
        #endregion

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

            if (/*skillQueue.Count != 0 &&*/ (skills[skillQueue.Peek()].Range == -1f || Vector2.Distance((Vector2)target.localPosition, rigid.position) <= skills[skillQueue.Peek()].Range))
            {
                if (timer <= 0 && skillActivable[skillQueue.Peek()]) ActiveSkillQueue();
                animator.SetFloat("speed", 0f);
                return;
            }

            var direction = ((Vector2)path.vectorPath[currentWayPoint] - rigid.position).normalized;
            stats.VectorSpeed.Value = direction * speed * Time.fixedDeltaTime;
            rigid.velocity = stats.VectorSpeed.Value * stats.VectorState.Value;
            animator.SetFloat("speed", Mathf.Abs(stats.VectorSpeed.Value.x) + Mathf.Abs(stats.VectorSpeed.Value.y));

            float distance = Vector2.Distance(rigid.position, path.vectorPath[currentWayPoint]);

            if (distance < nextWayPointDistance)
            {
                currentWayPoint++;
            }
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

        public override void OnDestroy()
        {
            base.OnDestroy();
            stats.TriggerRotation.OnValueChanged -= OnRotationTriggered;
            StopAllCoroutines();
        }
    }
}