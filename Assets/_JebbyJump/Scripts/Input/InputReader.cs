using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JebbyJump.Inputs
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "JebbyJump/InputReader")]
    public class InputReader : ScriptableObject, JebbyInputActions.IPlayerActions
    {
        public event Action JumpStartedEvent;
        public event Action JumpCanceledEvent;
        public event Action UseItemStartedEvent;
        public event Action PauseEvent;

        public Vector2 Move { get; private set; }
        public bool IsJumpHeld { get; private set; }

        private JebbyInputActions _actions;

        private void OnEnable()
        {
            _actions?.Dispose();
            _actions = new JebbyInputActions();
            _actions.Player.AddCallbacks(this);
            _actions.Player.Enable();
        }

        private void OnDisable()
        {
            _actions.Player.RemoveCallbacks(this);
            _actions.Player.Disable();
            _actions.Dispose();
        }

        void JebbyInputActions.IPlayerActions.OnMove(InputAction.CallbackContext ctx)
        {
            Move = ctx.ReadValue<Vector2>();
        }

        void JebbyInputActions.IPlayerActions.OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                IsJumpHeld = true;
                JumpStartedEvent?.Invoke();
            }
            else if (ctx.canceled)
            {
                IsJumpHeld = false;
                JumpCanceledEvent?.Invoke();
            }
        }

        void JebbyInputActions.IPlayerActions.OnUseItem(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                UseItemStartedEvent?.Invoke();
        }

        void JebbyInputActions.IPlayerActions.OnPause(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                PauseEvent?.Invoke();
        }
    }
}
