using Assets.Scripts.Both.Creature.Status;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

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

            var spdStats = creature.GetStats(Scriptable.StatsType.Speed);
            speed = spdStats.GetValue();
            spdStats.OnStatsChange += SpdStats_OnStatsChange;
        }

        private void SpdStats_OnStatsChange(object sender, Status.StatsChangeEventArgs e)
        {
            speed = e.NewValue;
        }

        // Start is called before the first frame update
        void Start()
        {
            //init animation
            MoveNonAffect();
            IsUpdateAnimation = true;
            animator.SetInteger("orientation", 2);
        }

        private void FixedUpdate()
        {
            if (!IsHost && IsClient)
            {
                player.velocity = stats.VectorSpeed.Value * stats.VectorState.Value;
            }

            if (control is null) return;

            if (NetworkManager.Singleton.LocalClientId != 0) return;

            //Movement
            stats.VectorSpeed.Value = new Vector2(control.VectorAxis.Value.x * speed * Time.fixedDeltaTime, control.VectorAxis.Value.y * speed * Time.fixedDeltaTime);
            player.velocity = stats.VectorSpeed.Value * stats.VectorState.Value;

            if (!IsUpdateAnimation) return;

            UpdateAnimation();
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
        private void UpdateAnimation()
        {
            //handle movement animation (depend on animator architect)
            animator.SetFloat("speed", Mathf.Abs(stats.VectorSpeed.Value.x) + Mathf.Abs(stats.VectorSpeed.Value.y));

            if (stats.VectorSpeed.Value.x < 0) animator.SetInteger("orientation", 3);//left animate
            else if (stats.VectorSpeed.Value.x > 0) animator.SetInteger("orientation", 4); //right animate
            if (stats.VectorSpeed.Value.y > 0) animator.SetInteger("orientation", 1); //up animate
            else if (stats.VectorSpeed.Value.y < 0) animator.SetInteger("orientation", 2); //down animate
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

        public override void OnDestroy()
        {
            if (!IsServer) return;

            base.OnDestroy();
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