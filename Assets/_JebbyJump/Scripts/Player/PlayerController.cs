using JebbyJump.Inputs;
using JebbyJump.Session;
using UnityEngine;

namespace JebbyJump.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private PlayerMotor _motor;

        private void Awake()
        {
            if (_input == null)
            {
                Debug.LogError("[PlayerController] InputReader is not assigned.", this);
            }

            if (_motor == null)
            {
                Debug.LogError("[PlayerController] PlayerMotor is not assigned.", this);
            }
        }

        private void OnEnable()
        {
            if (_input == null)
            {
                return;
            }

            _input.JumpStartedEvent += OnJumpStarted;
            _input.JumpCanceledEvent += OnJumpCanceled;
        }

        private void OnDisable()
        {
            if (_input == null)
            {
                return;
            }

            _input.JumpStartedEvent -= OnJumpStarted;
            _input.JumpCanceledEvent -= OnJumpCanceled;
        }

        private void Update()
        {
            if (_input == null || _motor == null)
            {
                return;
            }

            // Ignore horizontal input while paused; movement is applied in
            // FixedUpdate (frozen at timeScale 0) but zeroing the intent
            // prevents a held button from lurching on resume.
            _motor.SetMoveInput(
                PauseState.IsPaused ? 0f : _input.Move.x);
        }

        private bool _jumpEnabled = true;

        public void SetJumpEnabled(bool enabled)
        {
            _jumpEnabled = enabled;
            if (!enabled && _motor != null)
                _motor.ResetJump();
        }

        public void SetJumpMultiplier(float multiplier)
        {
            if (_motor != null)
                _motor.SetJumpMultiplier(multiplier);
        }

        public void Respawn(Vector3 position)
        {
            if (_motor != null)
                _motor.Respawn(position);
        }

        private void OnJumpStarted()
        {
            if (PauseState.IsPaused) return;
            if (_motor != null && _jumpEnabled)
            {
                _motor.RequestJump();
            }
        }

        private void OnJumpCanceled()
        {
            if (_motor != null)
            {
                _motor.CancelJump();
            }
        }
    }
}
