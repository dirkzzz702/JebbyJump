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

        // P22: gameplay input layer gate. When false, movement / jump / skill
        // inputs are ignored and any held state is cleared (Pause + UI nav are
        // unaffected). Defaults true so normal play is unchanged; only the
        // GameplayModalInputGate flips it while a shell modal is active.
        public bool GameplayInputEnabled { get; private set; } = true;

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
            Move = GameplayInputEnabled ? ctx.ReadValue<Vector2>() : Vector2.zero;
        }

        void JebbyInputActions.IPlayerActions.OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                if (!GameplayInputEnabled) return;
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
            if (ctx.started && GameplayInputEnabled) UseItemStartedEvent?.Invoke();
        }

        void JebbyInputActions.IPlayerActions.OnUseSkill2(InputAction.CallbackContext ctx)
        {
            if (ctx.started && GameplayInputEnabled) UseSkill2StartedEvent?.Invoke();
        }

        void JebbyInputActions.IPlayerActions.OnUseSkill3(InputAction.CallbackContext ctx)
        {
            if (ctx.started && GameplayInputEnabled) UseSkill3StartedEvent?.Invoke();
        }

        // P22 correction #1: block AND clear the gameplay input layer. Disabling
        // ignores move/jump/skill (keyboard, gamepad, and on-screen touch all
        // route through these callbacks) and clears any held state so a control
        // held when a modal opens cannot stick "on" or lurch on resume. The
        // Pause action and the UI input module are deliberately untouched.
        public void SetGameplayInputEnabled(bool enabled)
        {
            if (GameplayInputEnabled == enabled) return;
            GameplayInputEnabled = enabled;
            if (!enabled)
            {
                Move = Vector2.zero;
                if (IsJumpHeld)
                {
                    IsJumpHeld = false;
                    JumpCanceledEvent?.Invoke();
                }
            }
        }

        void JebbyInputActions.IPlayerActions.OnPause(InputAction.CallbackContext ctx)
        {
            if (ctx.started) PauseEvent?.Invoke();
        }
    }
}
