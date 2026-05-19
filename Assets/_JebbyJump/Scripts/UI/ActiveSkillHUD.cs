using JebbyJump.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    public class ActiveSkillHUD : MonoBehaviour
    {
        [SerializeField] private ActiveSkillController _skillController;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;

        private void Update()
        {
            if (_skillController == null || _label == null) return;

            string status;
            bool active;

            if (_skillController.IsSkillActive)
            {
                status = "Active";
                active = true;
            }
            else if (_skillController.CooldownRemaining > 0f)
            {
                status = $"{_skillController.CooldownRemaining:F1}s";
                active = false;
            }
            else if (_skillController.CanUseSkill)
            {
                status = "Ready";
                active = true;
            }
            else
            {
                status = "Wait";
                active = false;
            }

            _label.text = status;

            if (_icon != null)
            {
                var c = _icon.color;
                c.a = active ? 1f : 0.4f;
                _icon.color = c;
            }
        }
    }
}
