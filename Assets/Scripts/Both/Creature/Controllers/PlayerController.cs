using Assets.Scripts.Both.Creature.Status;
using Assets.Scripts.Both.Scriptable;
using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Both.Creature.Controllers
{
    /// <summary>
    /// Server owner
    /// </summary>
    public class PlayerController : NetworkBehaviour, ICreatureController
    {
        private ICreature creature;
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D player;
        [SerializeField] private PlayerControl control;
        [SerializeField] private float speed;
        [SerializeField] private NetworkStats stats;

        public bool IsUpdateAnimation { get; set; }
        public Animator Animator => animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<Rigidbody2D>();
            creature = GetComponent<Creature>();
            stats = GetComponentInChildren<NetworkStats>();         
        }

        private void Speed_StatsChange(object sender, StatsChangeEventArgs e)
        {
            if (e.Type.Name.Equals(StatsType.Speed.ToString())) speed = e.NewValue;
        }

        private void Speed_ValueChange(int oldV, int newV)
        {
            speed = newV;
        }

        // Start is called before the first frame update
        void Start()
        {
            // get and listen speed
            if (NetworkManager.Singleton.LocalClientId != 0)
            {
                GameController.Instance.RequestPlayerControlIdServerRpc(NetworkObject); //find player control on client

                speed = stats.Speed.Value;
                stats.Speed.OnValueChanged += Speed_ValueChange;
            }
            else
            {
                speed = creature.GetStats(StatsType.Speed).GetValue();
                (creature as Creature).StatsChange += Speed_StatsChange;
            }
            MoveNonAffect();

            //init animation
            IsUpdateAnimation = true;
            animator.SetInteger("orientation", 2);
        }

        private void FixedUpdate()
        {
            if (control is null) return;

            if (NetworkManager.Singleton.LocalClientId != 0) //Client
            {
                var clientSpeed = new Vector2(control.VectorAxis.Value.x * speed * Time.fixedDeltaTime, control.VectorAxis.Value.y * speed * Time.fixedDeltaTime);
                player.velocity = clientSpeed * stats.VectorState.Value;
            }
            else //Server/Host
            {
                //Movement
                var vectorSpeed = new Vector2(control.VectorAxis.Value.x * speed * Time.fixedDeltaTime, control.VectorAxis.Value.y * speed * Time.fixedDeltaTime);
                player.velocity = vectorSpeed * stats.VectorState.Value;

                if (!IsUpdateAnimation) return;

                UpdateAnimation(player.velocity);
            }          
        }

        #region Config movement direct
        public void MoveNonAffect()
        {
            if (NetworkManager.LocalClientId == 0)
                stats.VectorState.Value = Vector2.one; //only server can change value
        }

        public void Root()
        {
            if (NetworkManager.LocalClientId == 0)
                stats.VectorState.Value = Vector2.zero; //only server can change value
        }
        #endregion

        private void Attack()
        {
            if (this is null) return;
            creature.ActivateSkill(0, ResetAttack, transform);
        }

        private void SpAttack()
        {
            if (this is null) return;
            creature.ActivateSkill(1, ResetSpAttack, transform);
        }

        private void SpAttack2()
        {
            if (this is null) return;
            creature.ActivateSkill(2, ResetSpAttack2, transform);
        }

        private void ResetAttack()
        {
            control.ResetAttackClientRpc(new ClientRpcParams()
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { control.OwnerClientId }
                }
            });
        }

        private void ResetSpAttack()
        {
            control.ResetSpAttackClientRpc(new ClientRpcParams()
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { control.OwnerClientId }
                }
            });
        }

        private void ResetSpAttack2()
        {
            control.ResetSpAttack2ClientRpc(new ClientRpcParams()
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { control.OwnerClientId }
                }
            });
        }

        /// <summary>
        /// Control animation state
        /// </summary>
        private void UpdateAnimation(Vector2 velocity)
        {
            //handle movement animation (depend on animator architect)
            animator.SetFloat("speed", Mathf.Abs(velocity.x) + Mathf.Abs(velocity.y));

            if (velocity.x < 0) animator.SetInteger("orientation", 3);//left animate
            else if (velocity.x > 0) animator.SetInteger("orientation", 4); //right animate
            if (velocity.y > 0) animator.SetInteger("orientation", 1); //up animate
            else if (velocity.y < 0) animator.SetInteger("orientation", 2); //down animate
        }

        #region Add PlayerControl
        /// <summary>
        /// Inject PLayerControl (Server call)
        /// </summary>
        /// <param name="ctrl"></param>
        public void AddControl(PlayerControl ctrl) 
        { 
            control = ctrl;

            // Listen PlayerControl trigger
            ctrl.AttackTrigger.OnValueChanged += AttackTrigger_OnValueChange;
            ctrl.SpAttackTrigger.OnValueChanged += SpAttackTrigger_OnValueChange;
            ctrl.SpAttackTrigger2.OnValueChanged += SpAttackTrigger2_OnValueChange;
        } 

        private void AttackTrigger_OnValueChange(bool oldV, bool newV)
        {
            if (oldV != newV && !oldV) // detect change from false => true
            {
                Attack();
            }
        }

        private void SpAttackTrigger_OnValueChange(bool oldV, bool newV)
        {
            if (oldV != newV && !oldV) // detect change from false => true
            {
                SpAttack();
            }
        }

        private void SpAttackTrigger2_OnValueChange(bool oldV, bool newV)
        {
            if (oldV != newV && !oldV) // detect change from false => true
            {
                SpAttack2();
            }
        }
        #endregion

        IEnumerator FindPlayerControl(ulong clientId)
        {
            while (control is null)
            {
                var pCs = GameObject.FindGameObjectsWithTag("Player");

                pCs.ToList().ForEach((p) =>
                {
                if (p.GetComponent<NetworkObject>().OwnerClientId == clientId)
                    {
                        control = p.GetComponent<PlayerControl>();
                    }
                });

                yield return null;
            }
        }

        public void ResponsePlayerControlId(ulong clientId)
        {
            Debug.Log("ResponsePlayerControlId " + clientId + "-" + NetworkManager.Singleton.LocalClientId);

            StartCoroutine(FindPlayerControl(clientId));
        }

        public override void OnDestroy()
        {
            if (!IsServer) return;

            base.OnDestroy();
            StopAllCoroutines();
            stats.Speed.OnValueChanged -= Speed_ValueChange;
            (creature as Creature).StatsChange -= Speed_StatsChange;
            control.AttackTrigger.OnValueChanged -= AttackTrigger_OnValueChange;
            control.SpAttackTrigger.OnValueChanged -= SpAttackTrigger_OnValueChange;
            control.SpAttackTrigger2.OnValueChanged -= SpAttackTrigger2_OnValueChange;
        }
    }

    public interface ICreatureController
    {
        bool IsUpdateAnimation { get; set; }
        Animator Animator { get; }
        void MoveNonAffect();
        void Root();
        //void AddControl(PlayerControl ctrl);
    }
}