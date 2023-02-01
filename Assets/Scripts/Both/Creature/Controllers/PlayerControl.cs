using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Both.Creature.Controllers
{

    /// <summary>
    /// Client owner. Control its character.
    /// </summary>
    public class PlayerControl : NetworkBehaviour
    {
        public NetworkVariable<Vector2> VectorAxis = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Vector2> VectorState = new NetworkVariable<Vector2>(Vector2.one, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<bool> AttackTrigger = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> SpAttackTrigger = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> SpAttackTrigger2 = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [SerializeField] private PlayerInput script;

        private void OnEnable()
        {
            if (!IsOwner)
            {
                return;
            }

            ListenMovement();
            ListenAttack();
            ListenSpAttack();
            ListenSpAttack2();
        }

        private void OnDisable()
        {
            if (!IsOwner) return;

            UnlistenMovement();
            UnlistenAttack();
            UnlistenSpAttack();
            UnlistenSpAttack2();
        }

        #region Input listener setup
        private void ListenMovement()
        {
            script.actions["Movement"].started += Movement;
            script.actions["Movement"].performed += Movement;
            script.actions["Movement"].canceled += Movement;
        }

        private void UnlistenMovement()
        {
            script.actions["Movement"].started -= Movement;
        }

        private void ListenAttack()
        {
            script.actions["Attack"].started += Attack;
        }

        private void UnlistenAttack()
        {
            script.actions["Attack"].started -= Attack;
        }

        private void ListenSpAttack()
        {
            script.actions["SpecialAttack"].started += SpAttack;
        }

        private void UnlistenSpAttack()
        {
            script.actions["SpecialAttack"].started -= SpAttack;
        }

        private void ListenSpAttack2()
        {
            script.actions["SpecialAttack2"].started += SpAttack2;
        }

        private void UnlistenSpAttack2()
        {
            script.actions["SpecialAttack2"].started -= SpAttack2;
        }
        #endregion

        #region Action listener
        private void Movement(InputAction.CallbackContext ctx)
        {
            VectorAxis.Value = ctx.ReadValue<Vector2>();
        }

        private void Attack(InputAction.CallbackContext ctx)
        {
            if (AttackTrigger.Value) return;
            
            AttackTrigger.Value = true;
        }

        private void SpAttack(InputAction.CallbackContext obj)
        {
            if (SpAttackTrigger.Value) return;
                
            SpAttackTrigger.Value = true;
        }

        private void SpAttack2(InputAction.CallbackContext obj)
        {
            if (SpAttackTrigger2.Value) return;
            
            SpAttackTrigger2.Value = true;
        }
        #endregion
    }
}