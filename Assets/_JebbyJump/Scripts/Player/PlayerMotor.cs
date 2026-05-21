using System;
using UnityEngine;

namespace JebbyJump.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private PlayerMovementConfig _config;
        [SerializeField] private PlayerStats _stats;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundMask;

        public event Action<Collider2D> Landed;
        public event Action Jumped;

        public bool IsGrounded { get; private set; }
        public Collider2D CurrentGroundCollider { get; private set; }
        public Vector2 Velocity => _rb.linearVelocity;

        private Rigidbody2D _rb;
        private float _moveInput;
        private bool _jumpButtonHeld;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private float _jumpMultiplier = 1f;
        private bool _wasGrounded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (_config == null)
            {
                Debug.LogError("[PlayerMotor] PlayerMovementConfig is not assigned.", this);
            }

            if (_groundCheck == null)
            {
                Debug.LogError("[PlayerMotor] GroundCheck transform is not assigned.", this);
            }
        }

        private void FixedUpdate()
        {
            if (_config == null || _groundCheck == null)
            {
                return;
            }

            UpdateGrounded();
            UpdateCoyoteTime();
            UpdateJumpBuffer();
            UpdateGravity();
            UpdateHorizontal();
            ClampFallSpeed();
        }

        public void SetMoveInput(float x)
        {
            _moveInput = x;
        }

        public void RequestJump()
        {
            _jumpButtonHeld = true;
            _jumpBufferTimer = _config.JumpBufferTime;
        }

        public void CancelJump()
        {
            _jumpButtonHeld = false;
        }

        public void ResetJump()
        {
            _jumpButtonHeld = false;
            _jumpBufferTimer = 0f;
        }

        public void Respawn(Vector3 position)
        {
            transform.position = position;
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 1f;
            _coyoteTimer = 0f;
            _wasGrounded = false;
            ResetJump();
        }

        public void SetJumpMultiplier(float multiplier)
        {
            _jumpMultiplier = multiplier;
        }

        private void UpdateGrounded()
        {
            _wasGrounded = IsGrounded;
            var hit = Physics2D.OverlapCircle(
                _groundCheck.position, _config.GroundCheckRadius, _groundMask);
            // Reject upward contact to prevent false grounded state when passing through
            // a one-way platform from below. Without this, _wasGrounded=true from the
            // pass-through detection consumes the rising edge needed for the real landing.
            bool grounded = hit != null && _rb.linearVelocity.y <= 0.1f;
            IsGrounded = grounded;
            CurrentGroundCollider = grounded ? hit : null;
            if (!_wasGrounded && IsGrounded)
                Landed?.Invoke(hit);
        }

        private void UpdateCoyoteTime()
        {
            if (IsGrounded)
            {
                _coyoteTimer = _config.CoyoteTime;
            }
            else
            {
                _coyoteTimer -= Time.fixedDeltaTime;
            }
        }

        private void UpdateJumpBuffer()
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpBufferTimer -= Time.fixedDeltaTime;
            }

            if (_coyoteTimer > 0f && _jumpBufferTimer > 0f)
            {
                PerformJump();
            }
        }

        private void PerformJump()
        {
            float jumpForce = _stats != null ? _stats.JumpForce : _config.JumpForce;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce * _jumpMultiplier);
            _coyoteTimer = 0f;
            _jumpBufferTimer = 0f;
            Jumped?.Invoke();
        }

        private void UpdateGravity()
        {
            if (_rb.linearVelocity.y < 0f)
            {
                _rb.gravityScale = _config.FallGravityMultiplier;
            }
            else if (_rb.linearVelocity.y > 0f && !_jumpButtonHeld)
            {
                _rb.gravityScale = _config.LowJumpGravityMultiplier;
            }
            else
            {
                _rb.gravityScale = 1f;
            }
        }

        private void UpdateHorizontal()
        {
            float targetSpeed = _moveInput * (_stats != null ? _stats.MoveSpeed : _config.MoveSpeed);
            float currentSpeed = _rb.linearVelocity.x;

            float acceleration;
            if (IsGrounded)
            {
                acceleration = Mathf.Abs(_moveInput) > 0.01f
                    ? _config.GroundAcceleration
                    : _config.GroundDeceleration;
            }
            else
            {
                acceleration = Mathf.Abs(_moveInput) > 0.01f
                    ? _config.AirAcceleration
                    : _config.AirDeceleration;
            }

            float newX = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(newX, _rb.linearVelocity.y);
        }

        private void ClampFallSpeed()
        {
            if (_rb.linearVelocity.y < -_config.MaxFallSpeed)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -_config.MaxFallSpeed);
            }
        }
    }
}
