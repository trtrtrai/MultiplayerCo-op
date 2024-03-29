using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.Creature.Attackable.SkillExecute;
using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using Assets.Scripts.Server.Creature.Attackable;
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
        [SerializeField] private int lookAtRad;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private Transform target;
        [SerializeField] private NetworkStats stats;
        [SerializeField] private TouchDamageDetect touchDetect;
        [SerializeField] private SkillDetect skillDetect;
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
            touchDetect = GetComponent<TouchDamageDetect>();
            StartCoroutine(SkillDetectCatcher());
        }

        IEnumerator SkillDetectCatcher()
        {
            while (skillDetect is null)
            {
                skillDetect = GetComponent<SkillDetect>();

                yield return null;
            }

            skillDetect.enabled = false;
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
            stats.TriggerScale.OnValueChanged += OnScaleTriggered;

            if (NetworkManager.Singleton.LocalClientId != 0) return;

            stats.VectorState.Value = Vector2.one;

            target = FindCharacter();

            // get and listen speed
            speed = creature.GetStats(StatsType.Speed).GetValue();
            (creature as Creature).StatsChange += (o, a) => { if (a.Type.Name.Equals(StatsType.Speed.ToString())) speed = a.NewValue; };

            //listen health to find other target (30-45%)
            limitChange = (int)((creature as Boss.Boss).GetLimitChange()[0] * creature.GetStats(StatsType.Health).GetValue(false));
            currentHealth = creature.GetStats(StatsType.Health).GetValue();
            (creature as Creature).StatsChange += (o, a) => { 
                if (a.Type.Name.Equals(StatsType.Health.ToString())) currentHealth = a.NewValue; 

                if (a.Type.Name.Equals(StatsType.Health.ToString()) && a.OldValue > a.NewValue)
                {
                    var rand = Random.Range(0f, 1f);
                    if (rand <= 0.3f) // 30% change target if it be attacked
                    {
                        Debug.Log("Follow other target");
                        target = FindCharacter();
                    }
                }
            };

            (creature as Creature).StatsChange += BossController_HealthChange;

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

        private void BossController_HealthChange(object sender, StatsChangeEventArgs args)
        {
            if (!args.Type.Name.Equals(StatsType.Health.ToString())) return;

            if (args.NewValue < limitChange)
            {
                Transformer();
            }
            else if (animator.GetBool("isTransformer") && args.NewValue >= limitChange + 200)
            {
                DeTransformer();
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (NetworkManager.Singleton.LocalClientId != 0) return;

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
                timer -= Time.fixedDeltaTime / 2;
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
                var addAmount = Random.Range(15, 21); // add 15-20 skill into queue
                var isT = animator.GetBool("isTransformer");
                for (int i = 0; i < addAmount; i++)
                {
                    var skillNum = Random.Range(0, 1000);

                    if (isT) skillQueue.Enqueue(skills.Count - 2); // check transformer
                    else skillQueue.Enqueue(skillNum % (skills.Count - 2)); // except last skill is after transformer
                }
            }
        }

        private void NextSkill()
        {
            skillQueue.TryDequeue(out int t);

            FillSkillQueue(); // <=5 count --> fill

            // Check current skill can active? if not --> pass
            int limit = 5;
            while (limit != 0 && !skillActivable[skillQueue.Peek()])  //if Peek() activable is false -> turn it to last queue
            {
                var i = skillQueue.Dequeue();
                skillQueue.Enqueue(i);
                limit--;
            }

            timer = Random.Range(0.75f, 1.25f); //new timer
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
            animator.SetBool("isTransformer", true);
            touchDetect.enabled = false;
            skillDetect.enabled = true;
            skillQueue.Clear();
            FillSkillQueue();
            //StartCoroutine(Wait());
        }

        void DeTransformer()
        {
            //animation parameter "isTransformer"
            animator.SetBool("isTransformer", false);
            skillDetect.enabled = false;
            touchDetect.enabled = true;
            skillQueue.Clear();
            FillSkillQueue();
        }

        /*IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.25f);

            animator.SetBool("isTransformer", false);
        }*/

        void SpecialAttack() //Treant: TreeLifeSteal
        {
            skillActivable[3] = false;
            creature.ActivateSkill(3, () => { skillActivable[3] = true; }, target);

            NextSkill();
        }

        void Boost() //Treant: SunBoost - active special, don't in queue
        {
            skillActivable[4] = false;
            creature.ActivateSkill(4, () => { skillActivable[4] = true; }, transform);

            NextSkill();
        }
        #endregion

        #region Animation
        private void UpdateAnimation()
        {
            if (lookAtRad == 0)
            {
                var distanceVector = target.localPosition - (transform.localPosition + new Vector3(0, 2.5f));
                var distance = distanceVector.normalized;

                if (distance.x > 0 && distance.y > 0) //top-right = boss's face is back-right (zFlipped = -1 and yRotate = 0)
                {

                    stats.TriggerScale.Value = false;
                    stats.TriggerRotation.Value = false;
                    lookAtRad = 1;
                }
                else if (distance.x > 0 && distance.y <= 0) //bottom-right = boss's face is front-right (zFlipped = 1 and yRotate = 0)
                {
                    stats.TriggerScale.Value = true;
                    stats.TriggerRotation.Value = false;
                    lookAtRad = 2;
                }
                else if (distance.x <= 0 && distance.y > 0) //top-left = boss's face is back-left (zFlipped = 1 and yRotate = 180)
                {
                    stats.TriggerScale.Value = true;
                    stats.TriggerRotation.Value = true;
                    lookAtRad = 3;
                }
                else if (distance.x <= 0 && distance.y <= 0) //bottom-left = boss's face is back-left (zFlipped = -1 and yRotate = 180)
                {
                    stats.TriggerScale.Value = false;
                    stats.TriggerRotation.Value = true;
                    lookAtRad = 4;
                }
            }
            else LookAtTarget();
        }

        //Flipped and rotate boss follow target distance
        private void LookAtTarget()
        {
            var distanceVector = target.localPosition - (transform.localPosition + new Vector3(0, 2.5f));
            var distance = distanceVector.normalized;

            if (distance.x > 0 && distance.y > 0) //top-right = boss's face is back-right (zFlipped = -1 and yRotate = 0)
            {
                if (lookAtRad == 1) return;

                if (lookAtRad != 4)
                {
                    stats.TriggerScale.Value = false;
                }

                if (lookAtRad != 2)
                {
                    stats.TriggerRotation.Value = false;
                }
                //Debug.Log(distance);
                lookAtRad = 1;
            }
            else if (distance.x > 0 && distance.y <= 0) //bottom-right = boss's face is front-right (zFlipped = 1 and yRotate = 0)
            {
                if (lookAtRad == 2) return;

                if (lookAtRad != 3)
                {
                    stats.TriggerScale.Value = true;
                }

                if (lookAtRad != 1)
                {
                    stats.TriggerRotation.Value = false;
                }
                //Debug.Log(distance);
                lookAtRad = 2;
            }
            else if (distance.x <= 0 && distance.y > 0) //top-left = boss's face is back-left (zFlipped = 1 and yRotate = 180)
            {
                if (lookAtRad == 3) return;

                if (lookAtRad != 2)
                {
                    stats.TriggerScale.Value = true;
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
                    stats.TriggerScale.Value = false;
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
            //Debug.Log("ROTATION Old: " + oldV + " New: " + newV);
            if (newV)
            {
                transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        private void OnScaleTriggered(bool oldV, bool newV)
        {
            //Debug.Log("SCALE Old: " + oldV + " New: " + newV);
            if (newV)
            {
                var scale = transform.localScale;
                scale.z = Mathf.Abs(scale.z);
                transform.localScale = scale;
            }
            else
            {
                var scale = transform.localScale;
                scale.z = -Mathf.Abs(scale.z);
                transform.localScale = scale;
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

            if (animator.GetBool("isTransformer") && skillActivable[4])
            {
                Boost();
            }

            if (/*skillQueue.Count != 0 &&*/ (skills[skillQueue.Peek()].Range == -1f || Vector2.Distance((Vector2)target.localPosition, rigid.position) <= skills[skillQueue.Peek()].Range))
            {
                if (timer <= 0 && skillActivable[skillQueue.Peek()]) ActiveSkillQueue();
                animator.SetFloat("speed", 0f);
                return;
            }

            var direction = ((Vector2)path.vectorPath[currentWayPoint] - rigid.position).normalized;
            var vectorSpeed = direction * speed * Time.fixedDeltaTime;
            rigid.velocity = vectorSpeed * stats.VectorState.Value;
            animator.SetFloat("speed", Mathf.Abs(vectorSpeed.x) + Mathf.Abs(vectorSpeed.y));

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
            stats.TriggerScale.OnValueChanged -= OnScaleTriggered;
            StopAllCoroutines();
        }
    }
}