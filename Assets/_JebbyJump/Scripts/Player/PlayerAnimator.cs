using UnityEngine;

namespace JebbyJump.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _sr;

        private static readonly int SpeedParam           = Animator.StringToHash("Speed");
        private static readonly int IsGroundedParam      = Animator.StringToHash("IsGrounded");
        private static readonly int VerticalVelocityParam = Animator.StringToHash("VerticalVelocity");
        private static readonly int LandTriggerParam     = Animator.StringToHash("LandTrigger");
        private static readonly int HurtTriggerParam     = Animator.StringToHash("HurtTrigger");
        private static readonly int VictoryTriggerParam  = Animator.StringToHash("VictoryTrigger");

        private bool _facingLeft;

        private void Update()
        {
            if (_animator == null || _motor == null) return;

            _animator.SetFloat(SpeedParam, Mathf.Abs(_motor.Velocity.x));
            _animator.SetBool(IsGroundedParam, _motor.IsGrounded);
            _animator.SetFloat(VerticalVelocityParam, _motor.Velocity.y);

            if (Mathf.Abs(_motor.Velocity.x) > 0.01f)
                _facingLeft = _motor.Velocity.x < 0;

            if (_sr != null)
                _sr.flipX = _facingLeft;
        }

        public void TriggerLand()    => _animator?.SetTrigger(LandTriggerParam);
        public void TriggerHurt()    => _animator?.SetTrigger(HurtTriggerParam);
        public void TriggerVictory() => _animator?.SetTrigger(VictoryTriggerParam);
        public void ResetToIdle()    => _animator?.Play("Idle", 0);
    }
}
