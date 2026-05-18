using JebbyJump.Inputs;
using JebbyJump.UI;
using UnityEngine;

namespace JebbyJump.Items
{
    public class ActiveSkillController : MonoBehaviour
    {
        [SerializeField] private InputReader _input;
        [SerializeField] private RocketBootsEffect _rocketBoots;
        [SerializeField] private GameFeedbackUI _feedbackUI;
        [SerializeField] private float _cooldownSeconds = 10f;
        [SerializeField] private bool _resetCooldownOnLevelRestart = true;

        private float _cooldownTimer;
        private bool _canUseSkill;

        public bool IsCooldownReady => _cooldownTimer <= 0f;

        private void OnEnable()
        {
            if (_input != null) _input.UseItemStartedEvent += TryUseSkill;
        }

        private void OnDisable()
        {
            if (_input != null) _input.UseItemStartedEvent -= TryUseSkill;
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
        }

        public void SetCanUseSkill(bool canUse)
        {
            _canUseSkill = canUse;
        }

        public void TryUseSkill()
        {
            if (!_canUseSkill)
            {
                Debug.Log("[ActiveSkill] Cannot use skill outside Playing phase.");
                _feedbackUI?.ShowMessage("Wait for Go!", 0.8f);
                return;
            }
            if (_cooldownTimer > 0f)
            {
                Debug.Log("[ActiveSkill] Rocket Boots cooling down.");
                _feedbackUI?.ShowMessage("Rocket Boots cooling down!", 0.8f);
                return;
            }
            Debug.Log($"[ActiveSkill] Rocket Boots used. Cooldown: {_cooldownSeconds}s.");
            _rocketBoots?.Activate();
            _cooldownTimer = _cooldownSeconds;
        }

        public void CancelActiveSkill()
        {
            _rocketBoots?.CancelEffect();
        }

        public void ResetForLevel()
        {
            CancelActiveSkill();
            if (_resetCooldownOnLevelRestart)
                _cooldownTimer = 0f;
            Debug.Log("[ActiveSkill] Reset for level.");
        }
    }
}
