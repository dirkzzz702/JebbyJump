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
        public event Action UseItemStartedEvent;   // Skill slot 0 (J / LeftShift / RightShoulder)
        public event Action UseSkill2StartedEvent; // Skill slot 1 (K)
        public event Action UseSkill3StartedEvent; // Skill slot 2 (L)
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
            if (Application.isPlaying)
                _actions.Dispose();
            else
                UnityEngine.Object.DestroyImmediate(_actions.asset);
            _actions = null;
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
            if (ctx.started) UseItemStartedEvent?.Invoke();
        }

        void JebbyInputActions.IPlayerActions.OnUseSkill2(InputAction.CallbackContext ctx)
        {
            if (ctx.started) UseSkill2StartedEvent?.Invoke();
        }

        void JebbyInputActions.IPlayerActions.OnUseSkill3(InputAction.CallbackContext ctx)
        {
            if (ctx.started) UseSkill3StartedEvent?.Invoke();
        }

        void JebbyInputActions.IPlayerActions.OnPause(InputAction.CallbackContext ctx)
        {
            if (ctx.started) PauseEvent?.Invoke();
        }
    }
}
