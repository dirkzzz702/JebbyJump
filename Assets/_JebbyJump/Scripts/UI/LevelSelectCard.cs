using System;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // A single Level Select cell. Spawned and bound by
    // LevelSelectController. The card itself does not know what "open
    // this level" means; it just reports clicks with its index.
    public class LevelSelectCard : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private TextMeshProUGUI _bestTimeText;
        [SerializeField] private TextMeshProUGUI _bestRankText;
        [SerializeField] private TextMeshProUGUI _starsText;
        [SerializeField] private GameObject _lockedOverlay;
        [SerializeField] private Image _background;

        [SerializeField] private Color _unlockedColor =
            new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color _lockedColor =
            new Color(0.55f, 0.55f, 0.55f, 1f);
        // Soft green; defaulted in code so the existing prefab needs no
        // edit to show the completed tint.
        [SerializeField] private Color _completedColor =
            new Color(0.78f, 0.93f, 0.78f, 1f);

        public event Action<int> Clicked;
        public int LevelIndex { get; private set; }

        // P21: the card's Selectable, for explicit grid navigation. Locked cards
        // stay focusable (interactable) so players can read their info; the
        // controller blocks activation (no scene load, no analytics).
        public Selectable Selectable => _button;
        public bool IsUnlocked { get; private set; }

        private void Awake()
        {
            if (_button != null) _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(OnClicked);
        }

        public void Bind(
            int levelIndex,
            LevelCardState state,
            string bestTimeDisplay,
            string bestRankDisplay,
            int stars)
        {
            LevelIndex = levelIndex;
            bool isUnlocked = state != LevelCardState.Locked;
            IsUnlocked = isUnlocked;

            if (_levelNumberText != null)
                _levelNumberText.text = (levelIndex + 1).ToString();

            if (_bestTimeText != null)
                _bestTimeText.text = bestTimeDisplay;

            if (_bestRankText != null)
                _bestRankText.text = bestRankDisplay;

            if (_starsText != null)
                _starsText.text = StarRewardFormatter.Label(stars);

            if (_lockedOverlay != null)
                _lockedOverlay.SetActive(!isUnlocked);

            // P21: keep locked cards interactable so they remain focusable for
            // keyboard/gamepad info; the controller no-ops their activation.
            // Lock state is shown by the overlay + tint (non-color-only).
            if (_button != null)
                _button.interactable = true;

            if (_background != null)
            {
                _background.color = state switch
                {
                    LevelCardState.Completed => _completedColor,
                    LevelCardState.Unlocked  => _unlockedColor,
                    _                        => _lockedColor,
                };
            }
        }

        private void OnClicked() => Clicked?.Invoke(LevelIndex);
    }
}
