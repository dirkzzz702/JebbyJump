using JebbyJump.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Displays the equipped active skill as a slot icon.
    // Icon dims when blocked or on cooldown.
    // A small cooldown countdown appears inside the slot while cooling down.
    public class ActiveSkillHUD : MonoBehaviour
    {
        [SerializeField] private ActiveSkillController _skillController;
        [SerializeField] private Image  _icon;
        [SerializeField] private Image  _cooldownOverlay;   // dark fill that drains as cooldown expires
        [SerializeField] private TMP_Text _cooldownLabel;   // shows "3.2s" while cooling; hidden otherwise

        private void Update()
        {
            if (_skillController == null) return;

            bool  blocked  = !_skillController.CanUseSkill && !_skillController.IsSkillActive;
            bool  cooling  = _skillController.CooldownRemaining > 0f;
            bool  active   = _skillController.IsSkillActive;
            float fraction = _skillController.CooldownSeconds > 0f
                ? _skillController.CooldownRemaining / _skillController.CooldownSeconds
                : 0f;

            // Icon brightness
            if (_icon != null)
            {
                float a = (blocked || cooling) ? 0.35f : 1f;
                _icon.color = new Color(1f, 1f, 1f, a);
            }

            // Radial cooldown fill overlay
            if (_cooldownOverlay != null)
            {
                _cooldownOverlay.gameObject.SetActive(cooling);
                _cooldownOverlay.fillAmount = fraction;
            }

            // Cooldown text
            if (_cooldownLabel != null)
            {
                if (cooling)
                {
                    _cooldownLabel.gameObject.SetActive(true);
                    _cooldownLabel.text = $"{_skillController.CooldownRemaining:F1}";
                }
                else
                {
                    _cooldownLabel.gameObject.SetActive(false);
                }
            }
        }
    }
}
