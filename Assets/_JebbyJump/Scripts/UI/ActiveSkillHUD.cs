using JebbyJump.Items;
using TMPro;
using UnityEngine;

namespace JebbyJump.UI
{
    public class ActiveSkillHUD : MonoBehaviour
    {
        [SerializeField] private ActiveSkillController _skillController;
        [SerializeField] private TMP_Text _label;

        private void Update()
        {
            if (_skillController == null || _label == null) return;

            if (_skillController.IsSkillActive)
                _label.text = "Boots: Active";
            else if (_skillController.CooldownRemaining > 0f)
                _label.text = $"Boots: {_skillController.CooldownRemaining:F1}s";
            else if (_skillController.CanUseSkill)
                _label.text = "Boots: Ready";
            else
                _label.text = "Boots: Wait";
        }
    }
}
