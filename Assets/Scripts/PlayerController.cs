// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/PlayerController.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerController : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerController"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f7fb4a72-3cde-422c-897f-cdf2b42caa97"",
            ""actions"": [
                {
                    ""name"": ""Push"",
                    ""type"": ""Button"",
                    ""id"": ""5bac4541-d327-47a1-823e-1da0ad2b6b98"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press,Hold""
                },
                {
                    ""name"": ""Turning"",
                    ""type"": ""PassThrough"",
                    ""id"": ""b8e63f18-44be-4d58-a3b4-a9279ea1615e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Break"",
                    ""type"": ""Button"",
                    ""id"": ""6ecc23e9-4840-4398-8f43-27928fe80568"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Switch"",
                    ""type"": ""Button"",
                    ""id"": ""f11d43ca-f6ed-452f-8b13-29ab03cfa508"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0b904b7a-7d20-4499-9b0d-ada09e5e07b3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GoLeft"",
                    ""type"": ""Button"",
                    ""id"": ""f66cbc83-84e6-40d9-9896-bb7eb7423ca3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""GoRight"",
                    ""type"": ""Button"",
                    ""id"": ""f738fd90-1623-477e-ae2a-43684651973a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Ollie"",
                    ""type"": ""Button"",
                    ""id"": ""e1e52606-7750-4b11-abc7-25c4b191f2ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""3469b4fc-64e4-4200-a0e2-23e9ac2fcb84"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""Manual"",
                    ""type"": ""PassThrough"",
                    ""id"": ""878598a2-6d49-4ce0-b310-945dc47087b1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Respawn"",
                    ""type"": ""Button"",
                    ""id"": ""cf814185-b925-4a3c-b475-f5e207cc1dc2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""329a28fe-9387-4bfd-a193-4a542aed3cba"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Push"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""219c17a4-97a3-4f28-ab45-559d33f8a79d"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Turning"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f57cfe3-27a9-4961-8249-5c1673d686fc"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Break"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93cb75c8-f079-442b-9e3a-19567fb684b8"",
                    ""path"": ""<XInputController>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Switch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""afea5841-0a22-41bb-a30d-d01b0dff5605"",
                    ""path"": ""<XInputController>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60ff87dc-b46b-4ebd-8e1f-5984ca1f46a4"",
                    ""path"": ""<XInputController>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GoLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d15e3549-9192-4af4-9750-31c0c9cc6e0f"",
                    ""path"": ""<XInputController>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GoRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2d2338df-37a1-4de2-9698-5d6eaf5b49d9"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ollie"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""36fdf1f8-fa64-4e31-90ef-6c6be9c14199"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c7a9b1a7-2166-4096-ad76-352b44252ff5"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Manual"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1f2ad54-c839-4968-bf07-7fccc3adccb5"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Respawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Push = m_Player.FindAction("Push", throwIfNotFound: true);
        m_Player_Turning = m_Player.FindAction("Turning", throwIfNotFound: true);
        m_Player_Break = m_Player.FindAction("Break", throwIfNotFound: true);
        m_Player_Switch = m_Player.FindAction("Switch", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_GoLeft = m_Player.FindAction("GoLeft", throwIfNotFound: true);
        m_Player_GoRight = m_Player.FindAction("GoRight", throwIfNotFound: true);
        m_Player_Ollie = m_Player.FindAction("Ollie", throwIfNotFound: true);
        m_Player_Pause = m_Player.FindAction("Pause", throwIfNotFound: true);
        m_Player_Manual = m_Player.FindAction("Manual", throwIfNotFound: true);
        m_Player_Respawn = m_Player.FindAction("Respawn", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Push;
    private readonly InputAction m_Player_Turning;
    private readonly InputAction m_Player_Break;
    private readonly InputAction m_Player_Switch;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_GoLeft;
    private readonly InputAction m_Player_GoRight;
    private readonly InputAction m_Player_Ollie;
    private readonly InputAction m_Player_Pause;
    private readonly InputAction m_Player_Manual;
    private readonly InputAction m_Player_Respawn;
    public struct PlayerActions
    {
        private @PlayerController m_Wrapper;
        public PlayerActions(@PlayerController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Push => m_Wrapper.m_Player_Push;
        public InputAction @Turning => m_Wrapper.m_Player_Turning;
        public InputAction @Break => m_Wrapper.m_Player_Break;
        public InputAction @Switch => m_Wrapper.m_Player_Switch;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @GoLeft => m_Wrapper.m_Player_GoLeft;
        public InputAction @GoRight => m_Wrapper.m_Player_GoRight;
        public InputAction @Ollie => m_Wrapper.m_Player_Ollie;
        public InputAction @Pause => m_Wrapper.m_Player_Pause;
        public InputAction @Manual => m_Wrapper.m_Player_Manual;
        public InputAction @Respawn => m_Wrapper.m_Player_Respawn;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Push.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPush;
                @Push.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPush;
                @Push.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPush;
                @Turning.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurning;
                @Turning.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurning;
                @Turning.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurning;
                @Break.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBreak;
                @Break.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBreak;
                @Break.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBreak;
                @Switch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitch;
                @Switch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitch;
                @Switch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSwitch;
                @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                @GoLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGoLeft;
                @GoLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGoLeft;
                @GoLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGoLeft;
                @GoRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGoRight;
                @GoRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGoRight;
                @GoRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGoRight;
                @Ollie.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOllie;
                @Ollie.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOllie;
                @Ollie.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOllie;
                @Pause.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Manual.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnManual;
                @Manual.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnManual;
                @Manual.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnManual;
                @Respawn.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRespawn;
                @Respawn.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRespawn;
                @Respawn.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRespawn;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Push.started += instance.OnPush;
                @Push.performed += instance.OnPush;
                @Push.canceled += instance.OnPush;
                @Turning.started += instance.OnTurning;
                @Turning.performed += instance.OnTurning;
                @Turning.canceled += instance.OnTurning;
                @Break.started += instance.OnBreak;
                @Break.performed += instance.OnBreak;
                @Break.canceled += instance.OnBreak;
                @Switch.started += instance.OnSwitch;
                @Switch.performed += instance.OnSwitch;
                @Switch.canceled += instance.OnSwitch;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @GoLeft.started += instance.OnGoLeft;
                @GoLeft.performed += instance.OnGoLeft;
                @GoLeft.canceled += instance.OnGoLeft;
                @GoRight.started += instance.OnGoRight;
                @GoRight.performed += instance.OnGoRight;
                @GoRight.canceled += instance.OnGoRight;
                @Ollie.started += instance.OnOllie;
                @Ollie.performed += instance.OnOllie;
                @Ollie.canceled += instance.OnOllie;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Manual.started += instance.OnManual;
                @Manual.performed += instance.OnManual;
                @Manual.canceled += instance.OnManual;
                @Respawn.started += instance.OnRespawn;
                @Respawn.performed += instance.OnRespawn;
                @Respawn.canceled += instance.OnRespawn;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnPush(InputAction.CallbackContext context);
        void OnTurning(InputAction.CallbackContext context);
        void OnBreak(InputAction.CallbackContext context);
        void OnSwitch(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnGoLeft(InputAction.CallbackContext context);
        void OnGoRight(InputAction.CallbackContext context);
        void OnOllie(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnManual(InputAction.CallbackContext context);
        void OnRespawn(InputAction.CallbackContext context);
    }
}
