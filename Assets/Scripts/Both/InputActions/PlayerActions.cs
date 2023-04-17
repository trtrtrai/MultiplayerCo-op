//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Both/InputActions/PlayerActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Assets.Scripts.Both.InputActions
{
    public partial class @PlayerActions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
    ""maps"": [
        {
            ""name"": ""Ground"",
            ""id"": ""0c877833-c138-44cc-a6aa-0e2528e25114"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""b2be0e69-3433-4c5d-bd6c-9256e8e7ce52"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""c259a436-557b-42c0-9700-dba70a8e3a87"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SpecialAttack"",
                    ""type"": ""Button"",
                    ""id"": ""9541bada-4953-43ad-8d65-bb7affb2b753"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SpecialAttack2"",
                    ""type"": ""Button"",
                    ""id"": ""8757a9b0-26b2-4596-80ba-a28b9af51ac4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PlayMenu"",
                    ""type"": ""Button"",
                    ""id"": ""48fe58e5-6059-46bc-bfd9-da84181920ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""592b7793-8059-4034-a234-d0ad181fa315"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d203ccef-1122-4cb4-9367-0c43823c4c29"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""47ea1c91-dc79-4d51-921a-15702d5fbb82"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c03eb18a-0889-4f06-a48a-c55206c43ddd"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9d057b83-edd0-4f54-b179-631e7ae1792a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftStick"",
                    ""id"": ""865bc784-b204-40b9-8271-600bc68a1023"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c26e9a0d-1fde-4680-ba0f-2412f4e22cfd"",
                    ""path"": ""<XInputController>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c0e526b5-b13f-4581-bd18-5a7db7562986"",
                    ""path"": ""<XInputController>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""48919604-4e82-4e38-8f03-02d7b11b7ad9"",
                    ""path"": ""<XInputController>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""255d0376-5de5-4202-a1cf-0e9b4bbaac2a"",
                    ""path"": ""<XInputController>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow"",
                    ""id"": ""55bdb9b0-0def-4165-bfa0-939f494919cf"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""bcecadac-dff2-4e8a-ab56-cc52498dde90"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5a123321-7cab-4ca0-86d9-7ba9c99d25cd"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""693ac93e-74d1-4e2c-8acf-4e1c37fb20ab"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c70d3303-cdb6-42ef-99c8-dbc678bc4722"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""88bd5045-5f87-416f-b49d-02cff7718bc0"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5dd20e19-893f-4099-a1eb-6987dc3d5f9b"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06bcce97-1e60-4bce-b16f-4e128f8d2a69"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SpecialAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2a15adf8-b0f0-48a1-bdf7-4e174aa1f056"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""SpecialAttack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""737d5255-7147-4eec-938d-083b735e2bbf"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""SpecialAttack2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3847333b-cb6b-458b-8f6f-a09fb00d5821"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""SpecialAttack2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48e6ef62-61e9-4bda-8c32-cffe61f68280"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""PlayMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""10bf2f48-5cc3-4542-befe-781af2652518"",
                    ""path"": ""<XInputController>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Xbox Controller"",
                    ""action"": ""PlayMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Xbox Controller"",
            ""bindingGroup"": ""Xbox Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Ground
            m_Ground = asset.FindActionMap("Ground", throwIfNotFound: true);
            m_Ground_Movement = m_Ground.FindAction("Movement", throwIfNotFound: true);
            m_Ground_Attack = m_Ground.FindAction("Attack", throwIfNotFound: true);
            m_Ground_SpecialAttack = m_Ground.FindAction("SpecialAttack", throwIfNotFound: true);
            m_Ground_SpecialAttack2 = m_Ground.FindAction("SpecialAttack2", throwIfNotFound: true);
            m_Ground_PlayMenu = m_Ground.FindAction("PlayMenu", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Ground
        private readonly InputActionMap m_Ground;
        private IGroundActions m_GroundActionsCallbackInterface;
        private readonly InputAction m_Ground_Movement;
        private readonly InputAction m_Ground_Attack;
        private readonly InputAction m_Ground_SpecialAttack;
        private readonly InputAction m_Ground_SpecialAttack2;
        private readonly InputAction m_Ground_PlayMenu;
        public struct GroundActions
        {
            private @PlayerActions m_Wrapper;
            public GroundActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Ground_Movement;
            public InputAction @Attack => m_Wrapper.m_Ground_Attack;
            public InputAction @SpecialAttack => m_Wrapper.m_Ground_SpecialAttack;
            public InputAction @SpecialAttack2 => m_Wrapper.m_Ground_SpecialAttack2;
            public InputAction @PlayMenu => m_Wrapper.m_Ground_PlayMenu;
            public InputActionMap Get() { return m_Wrapper.m_Ground; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GroundActions set) { return set.Get(); }
            public void SetCallbacks(IGroundActions instance)
            {
                if (m_Wrapper.m_GroundActionsCallbackInterface != null)
                {
                    @Movement.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnMovement;
                    @Movement.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnMovement;
                    @Movement.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnMovement;
                    @Attack.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnAttack;
                    @Attack.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnAttack;
                    @Attack.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnAttack;
                    @SpecialAttack.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnSpecialAttack;
                    @SpecialAttack.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnSpecialAttack;
                    @SpecialAttack.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnSpecialAttack;
                    @SpecialAttack2.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnSpecialAttack2;
                    @SpecialAttack2.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnSpecialAttack2;
                    @SpecialAttack2.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnSpecialAttack2;
                    @PlayMenu.started -= m_Wrapper.m_GroundActionsCallbackInterface.OnPlayMenu;
                    @PlayMenu.performed -= m_Wrapper.m_GroundActionsCallbackInterface.OnPlayMenu;
                    @PlayMenu.canceled -= m_Wrapper.m_GroundActionsCallbackInterface.OnPlayMenu;
                }
                m_Wrapper.m_GroundActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Movement.started += instance.OnMovement;
                    @Movement.performed += instance.OnMovement;
                    @Movement.canceled += instance.OnMovement;
                    @Attack.started += instance.OnAttack;
                    @Attack.performed += instance.OnAttack;
                    @Attack.canceled += instance.OnAttack;
                    @SpecialAttack.started += instance.OnSpecialAttack;
                    @SpecialAttack.performed += instance.OnSpecialAttack;
                    @SpecialAttack.canceled += instance.OnSpecialAttack;
                    @SpecialAttack2.started += instance.OnSpecialAttack2;
                    @SpecialAttack2.performed += instance.OnSpecialAttack2;
                    @SpecialAttack2.canceled += instance.OnSpecialAttack2;
                    @PlayMenu.started += instance.OnPlayMenu;
                    @PlayMenu.performed += instance.OnPlayMenu;
                    @PlayMenu.canceled += instance.OnPlayMenu;
                }
            }
        }
        public GroundActions @Ground => new GroundActions(this);
        private int m_KeyboardSchemeIndex = -1;
        public InputControlScheme KeyboardScheme
        {
            get
            {
                if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
                return asset.controlSchemes[m_KeyboardSchemeIndex];
            }
        }
        private int m_XboxControllerSchemeIndex = -1;
        public InputControlScheme XboxControllerScheme
        {
            get
            {
                if (m_XboxControllerSchemeIndex == -1) m_XboxControllerSchemeIndex = asset.FindControlSchemeIndex("Xbox Controller");
                return asset.controlSchemes[m_XboxControllerSchemeIndex];
            }
        }
        public interface IGroundActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
            void OnSpecialAttack(InputAction.CallbackContext context);
            void OnSpecialAttack2(InputAction.CallbackContext context);
            void OnPlayMenu(InputAction.CallbackContext context);
        }
    }
}
