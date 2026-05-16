using UnityEngine;

namespace JebbyJump.Player
{
    [CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "JebbyJump/PlayerMovementConfig")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Horizontal")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _groundAcceleration = 80f;
        [SerializeField] private float _groundDeceleration = 80f;
        [SerializeField] private float _airAcceleration = 50f;
        [SerializeField] private float _airDeceleration = 50f;

        [Header("Jump")]
        [SerializeField] private float _jumpForce = 18f;
        [SerializeField] private float _coyoteTime = 0.15f;
        [SerializeField] private float _jumpBufferTime = 0.15f;

        [Header("Gravity")]
        [SerializeField] private float _fallGravityMultiplier = 2.5f;
        [SerializeField] private float _lowJumpGravityMultiplier = 2.0f;
        [SerializeField] private float _maxFallSpeed = 20f;

        [Header("Ground Detection")]
        [SerializeField] private float _groundCheckRadius = 0.1f;

        public float MoveSpeed => _moveSpeed;
        public float GroundAcceleration => _groundAcceleration;
        public float GroundDeceleration => _groundDeceleration;
        public float AirAcceleration => _airAcceleration;
        public float AirDeceleration => _airDeceleration;
        public float JumpForce => _jumpForce;
        public float CoyoteTime => _coyoteTime;
        public float JumpBufferTime => _jumpBufferTime;
        public float FallGravityMultiplier => _fallGravityMultiplier;
        public float LowJumpGravityMultiplier => _lowJumpGravityMultiplier;
        public float MaxFallSpeed => _maxFallSpeed;
        public float GroundCheckRadius => _groundCheckRadius;
    }
}
