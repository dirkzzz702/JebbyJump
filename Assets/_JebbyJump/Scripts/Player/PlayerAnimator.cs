using UnityEngine;

namespace JebbyJump.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private Animator _animator;

        private static readonly int SpeedParam           = Animator.StringToHash("Speed");
        private static readonly int IsGroundedParam      = Animator.StringToHash("IsGrounded");
        private static readonly int VerticalVelocityParam = Animator.StringToHash("VerticalVelocity");
        private static readonly int LandTriggerParam     = Animator.StringToHash("LandTrigger");
        private static readonly int HurtTriggerParam     = Animator.StringToHash("HurtTrigger");
        private static readonly int VictoryTriggerParam  = Animator.StringToHash("VictoryTrigger");

        private void Update()
        {
            if (_animator == null || _motor == null) return;

            _animator.SetFloat(SpeedParam, Mathf.Abs(_motor.Velocity.x));
            _animator.SetBool(IsGroundedParam, _motor.IsGrounded);
            _animator.SetFloat(VerticalVelocityParam, _motor.Velocity.y);
        }

        public void TriggerLand()    => _animator?.SetTrigger(LandTriggerParam);
        public void TriggerHurt()    => _animator?.SetTrigger(HurtTriggerParam);
        public void TriggerVictory() => _animator?.SetTrigger(VictoryTriggerParam);
    }
}
