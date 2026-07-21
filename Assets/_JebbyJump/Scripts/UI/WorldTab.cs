using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // One world selector tab in the Level Select world strip
    // (WorldExpansion100, phase P34D). Like LevelSelectCard, it is a dumb cell:
    // it reports clicks with its world number and shows a selected/locked mark;
    // LevelSelectController owns what selecting a world means.
    public class WorldTab : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private GameObject _selectedIndicator;
        [SerializeField] private GameObject _lockedIndicator;

        public event Action<int> Clicked;
        public int WorldNumber { get; private set; }

        // Focusable for keyboard/gamepad nav. Locked worlds stay focusable so
        // they can be previewed (mirrors locked level cards).
        public Selectable Selectable => _button;

        private void Awake()
        {
            if (_button != null) _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(OnClicked);
        }

        public void Bind(int worldNumber, string label, bool locked)
        {
            WorldNumber = worldNumber;
            if (_label != null) _label.text = label;
            if (_lockedIndicator != null) _lockedIndicator.SetActive(locked);
            if (_button != null) _button.interactable = true;
        }

        public void SetSelected(bool selected)
        {
            if (_selectedIndicator != null) _selectedIndicator.SetActive(selected);
        }

        private void OnClicked() => Clicked?.Invoke(WorldNumber);
    }
}
