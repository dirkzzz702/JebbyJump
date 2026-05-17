using JebbyJump.Inputs;
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

            _motor.SetMoveInput(_input.Move.x);
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
