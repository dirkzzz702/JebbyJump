// GENERATED AUTOMATICALLY FROM 'Assets/_JebbyJump/Settings/Input/JebbyInputActions.inputactions'
// To regenerate: select the asset in the Unity Editor Inspector and enable 'Generate C# Class'.
// Unity will overwrite this file when it regenerates. Do not edit by hand.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @JebbyInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }

    public @JebbyInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""JebbyInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a74b7a85-ce69-4b21-bb3b-ff1636e4c0d1"",
            ""actions"": [
                { ""name"": ""Move"",    ""type"": ""Value"",  ""id"": ""b3c14d2e-7a8f-4c23-9e1d-0f5a8b2c3d4e"", ""expectedControlType"": ""Vector2"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": true  },
                { ""name"": ""Jump"",    ""type"": ""Button"", ""id"": ""c4d25e3f-8b9a-5d34-af2e-1a6b9c3d4e5f"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""UseItem"",""type"": ""Button"", ""id"": ""d5e36f4a-9cab-6e45-b03f-2b7cad4e5f6a"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Pause"",  ""type"": ""Button"", ""id"": ""e6f47a5b-adbc-7f56-c140-3c8dbe5f6a7b"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false }
            ],
            ""bindings"": [
                { ""name"": ""2D Vector"",         ""id"": ""f7a58b6c-becd-8067-d251-4d9ecf6a7b8c"", ""path"": ""2DVector"",              ""groups"": """", ""action"": ""Move"",    ""isComposite"": true,  ""isPartOfComposite"": false },
                { ""name"": ""up"",                ""id"": ""08b69c7d-cfde-9178-e362-5eafd07b8c9d"", ""path"": """",                       ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": ""down"",              ""id"": ""19c7ad8e-d0ef-a289-f473-6fb0e18c9dae"", ""path"": """",                       ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": ""left"",              ""id"": ""2ad8be9f-e1fa-b39a-0584-7ac1f29daebf"", ""path"": ""<Keyboard>/a"",           ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": ""right"",             ""id"": ""3be9cfa0-f20b-c4ab-1695-8bd203aebfc0"", ""path"": ""<Keyboard>/d"",           ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": ""2D Vector (Arrows)"",""id"": ""4cfad0b1-034c-d5bc-27a6-9ce314bfc0d1"", ""path"": ""2DVector"",              ""groups"": """", ""action"": ""Move"",    ""isComposite"": true,  ""isPartOfComposite"": false },
                { ""name"": ""up"",                ""id"": ""5d0be1c2-145d-e6cd-38b7-ade425c0d1e2"", ""path"": """",                       ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": ""down"",              ""id"": ""6e1cf2d3-256e-f7de-49c8-bef536d1e2f3"", ""path"": """",                       ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": ""left"",              ""id"": ""7f2da3e4-367f-08ef-5ad9-cfa647e2f3a4"", ""path"": ""<Keyboard>/leftArrow"",  ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": ""right"",             ""id"": ""8a3eb4f5-478a-19fa-6bea-d0b758f3a4b5"", ""path"": ""<Keyboard>/rightArrow"", ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": true  },
                { ""name"": """",                  ""id"": ""9b4fc5a6-589b-2a0b-7cfb-e1c869a4b5c6"", ""path"": ""<Gamepad>/leftStick"",   ""groups"": """", ""action"": ""Move"",    ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""ac50d6b7-69ac-3b1c-8d0c-f2d97ab5c6d7"", ""path"": ""<Keyboard>/space"",      ""groups"": """", ""action"": ""Jump"",    ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""bd61e7c8-7abd-4c2d-9e1d-03ea8bc6d7e8"", ""path"": ""<Gamepad>/buttonSouth"", ""groups"": """", ""action"": ""Jump"",    ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""ce72f8d9-8bce-5d3e-af2e-14fb9cd7e8f9"", ""path"": ""<Keyboard>/leftShift"",  ""groups"": """", ""action"": ""UseItem"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""df83a9ea-9cdf-6e4f-b03f-25ac0de8f9aa"", ""path"": ""<Keyboard>/j"",          ""groups"": """", ""action"": ""UseItem"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""ea94bafb-adea-7f50-c140-36bd1ef9aabb"", ""path"": ""<Gamepad>/rightShoulder"",""groups"": """",""action"": ""UseItem"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""fb05cb0c-bef1-8061-d251-47ce2fa9bbcc"", ""path"": ""<Gamepad>/buttonWest"",  ""groups"": """", ""action"": ""UseItem"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""0c16dc1d-cf02-9172-e362-58df30baccdd"", ""path"": ""<Keyboard>/escape"",     ""groups"": """", ""action"": ""Pause"",   ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",                  ""id"": ""1d27ed2e-d013-a283-f473-69e041cbddee"", ""path"": ""<Gamepad>/start"",       ""groups"": """", ""action"": ""Pause"",   ""isComposite"": false, ""isPartOfComposite"": false }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        var map = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move    = map.FindAction("Move",    throwIfNotFound: true);
        m_Player_Jump    = map.FindAction("Jump",    throwIfNotFound: true);
        m_Player_UseItem = map.FindAction("UseItem", throwIfNotFound: true);
        m_Player_Pause   = map.FindAction("Pause",   throwIfNotFound: true);
    }

    ~@JebbyInputActions()
    {
        asset.Dispose();
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

    public bool Contains(InputAction action) => asset.Contains(action);

    public IEnumerator<InputAction> GetEnumerator() => asset.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Enable()  => asset.Enable();
    public void Disable() => asset.Disable();

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        => asset.FindAction(actionNameOrId, throwIfNotFound);

    public int FindBinding(InputBinding bindingMask, out InputAction action)
        => asset.FindBinding(bindingMask, out action);

    // ── Player action map ──────────────────────────────────────────────────

    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_UseItem;
    private readonly InputAction m_Player_Pause;

    public struct @PlayerActions
    {
        private @JebbyInputActions m_Wrapper;
        public @PlayerActions(@JebbyInputActions wrapper) { m_Wrapper = wrapper; }

        public InputAction @Move    => m_Wrapper.m_Player_Move;
        public InputAction @Jump    => m_Wrapper.m_Player_Jump;
        public InputAction @UseItem => m_Wrapper.m_Player_UseItem;
        public InputAction @Pause   => m_Wrapper.m_Player_Pause;

        public InputActionMap Get() => m_Wrapper.asset.FindActionMap("Player", throwIfNotFound: true);
        public void Enable()  => Get().Enable();
        public void Disable() => Get().Disable();
        public bool enabled   => Get().enabled;

        public static implicit operator InputActionMap(@PlayerActions set) => set.Get();

        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started    -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed  -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled   -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Jump.started    -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed  -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled   -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @UseItem.started    -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.performed  -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.canceled   -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @Pause.started   -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                @Pause.canceled  -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
            }

            m_Wrapper.m_PlayerActionsCallbackInterface = instance;

            if (instance != null)
            {
                @Move.started    += instance.OnMove;
                @Move.performed  += instance.OnMove;
                @Move.canceled   += instance.OnMove;
                @Jump.started    += instance.OnJump;
                @Jump.performed  += instance.OnJump;
                @Jump.canceled   += instance.OnJump;
                @UseItem.started    += instance.OnUseItem;
                @UseItem.performed  += instance.OnUseItem;
                @UseItem.canceled   += instance.OnUseItem;
                @Pause.started   += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled  += instance.OnPause;
            }
        }
    }

    public @PlayerActions @Player => new @PlayerActions(this);

    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
    }
}
