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
        [SerializeField] private Vector2 VectorSpeed;

        public bool IsUpdateAnimation { get; set; }
        public Animator Animator => animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<Rigidbody2D>();
            creature = GetComponent<Creature>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //init animation
            IsUpdateAnimation = true;
            animator.SetInteger("orientation", 2);
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;

            if (control is null) return;

            //Movement
            VectorSpeed = new Vector2(control.VectorAxis.Value.x * 250 * Time.fixedDeltaTime, control.VectorAxis.Value.y * 250 * Time.fixedDeltaTime);
            player.velocity = VectorSpeed * control.VectorState.Value;

            if (!IsUpdateAnimation) return;

            UpdateAnimation();
        }

        #region Config movement direct
        public void MoveNonAffect()
        {
            if (IsServer)
                control.VectorState.Value = Vector2.one; //only server can change value
        }

        public void Root()
        {
            if (IsServer)
                control.VectorState.Value = Vector2.zero; //only server can change value
        }
        #endregion

        private void Attack()
        {
            creature.ActivateSkill(0, ResetAttack);
        }

        private void ResetAttack()
        {
            control.AttackTrigger.Value = false;
        }

        private void SpAttack()
        {
            creature.ActivateSkill(1, ResetSpAttack);
        }

        private void ResetSpAttack()
        {
            control.SpAttackTrigger.Value = false;
        }

        private void SpAttack2()
        {
            creature.ActivateSkill(2, ResetSpAttack2);
        }

        private void ResetSpAttack2()
        {
            control.SpAttackTrigger2.Value = false;
        }

        /// <summary>
        /// Control animation state
        /// </summary>
        private void UpdateAnimation()
        {
            //handle movement animation (depend on animator architect)
            animator.SetFloat("speed", Mathf.Abs(VectorSpeed.x) + Mathf.Abs(VectorSpeed.y));

            if (VectorSpeed.x < 0) animator.SetInteger("orientation", 3);//left animate
            else if (VectorSpeed.x > 0) animator.SetInteger("orientation", 4); //right animate
            if (VectorSpeed.y > 0) animator.SetInteger("orientation", 1); //up animate
            else if (VectorSpeed.y < 0) animator.SetInteger("orientation", 2); //down animate
        }

        #region Add PlayerControl
        /// <summary>
        /// Inject PLayerControl
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