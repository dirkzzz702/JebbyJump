using JebbyJump.Analytics;
using JebbyJump.Inputs;
using JebbyJump.Session;
using JebbyJump.UI;
using UnityEngine;

namespace JebbyJump.Items
{
    // One controller per equipped skill. Slot determines which InputReader
    // event the controller listens to.
    public class ActiveSkillController : MonoBehaviour
    {
        public enum SkillSlot { Slot1 = 0, Slot2 = 1, Slot3 = 2 }

        [SerializeField] private InputReader _input;
        [SerializeField] private SkillSlot _slot = SkillSlot.Slot1;
        [SerializeField] private ActiveSkillEffect _effect;
        [SerializeField] private GameFeedbackUI _feedbackUI;
        [SerializeField] private float _cooldownSeconds = 10f;
        [SerializeField] private bool _resetCooldownOnLevelRestart = true;
        [SerializeField] private string _displayName = "Skill";

        private float _cooldownTimer;
        private bool _canUseSkill;

        public bool  IsCooldownReady    => _cooldownTimer <= 0f;
        public bool  CanUseSkill        => _canUseSkill;
        public float CooldownRemaining  => Mathf.Max(0f, _cooldownTimer);
        public float CooldownSeconds    => _cooldownSeconds;
        public bool  IsSkillActive      => _effect != null && _effect.IsActive;
        public SkillSlot Slot           => _slot;

        private void OnEnable()
        {
            if (_input == null) return;
            switch (_slot)
            {
                case SkillSlot.Slot1: _input.UseItemStartedEvent   += TryUseSkill; break;
                case SkillSlot.Slot2: _input.UseSkill2StartedEvent += TryUseSkill; break;
                case SkillSlot.Slot3: _input.UseSkill3StartedEvent += TryUseSkill; break;
            }
        }

        private void OnDisable()
        {
            if (_input == null) return;
            switch (_slot)
            {
                case SkillSlot.Slot1: _input.UseItemStartedEvent   -= TryUseSkill; break;
                case SkillSlot.Slot2: _input.UseSkill2StartedEvent -= TryUseSkill; break;
                case SkillSlot.Slot3: _input.UseSkill3StartedEvent -= TryUseSkill; break;
            }
        }

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
        }

        public void SetCanUseSkill(bool canUse) => _canUseSkill = canUse;

        public void TryUseSkill()
        {
            // Block activation while paused. timeScale 0 freezes the
            // cooldown/duration, but Activate() applies its stat changes
            // immediately, so it must not run during pause.
            if (PauseState.IsPaused) return;

            if (!_canUseSkill)
            {
                _feedbackUI?.ShowMessage("Wait for Go!", 0.8f);
                return;
            }
            if (_cooldownTimer > 0f)
            {
                _feedbackUI?.ShowMessage($"{_displayName} cooling down!", 0.8f);
                return;
            }
            _effect?.Activate();
            _cooldownTimer = _cooldownSeconds;
            AnalyticsService.Track("skill_used",
                AnalyticsParam.Of("skill_type", _displayName),
                AnalyticsParam.Of("level_index", LevelContext.CurrentIndex),
                AnalyticsParam.Of("level_number", LevelContext.CurrentNumber));
            Debug.Log($"[ActiveSkill:{_slot}] {_displayName} used. Cooldown {_cooldownSeconds}s.");
        }

        public void CancelActiveSkill() => _effect?.CancelEffect();

        public void ResetForLevel()
        {
            CancelActiveSkill();
            if (_resetCooldownOnLevelRestart)
                _cooldownTimer = 0f;
        }
    }
}
