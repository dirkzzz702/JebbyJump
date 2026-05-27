using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // A single Level Select cell. Spawned and bound by
    // LevelSelectController. The card itself does not know what "open
    // this level" means — it just reports clicks with its index.
    public class LevelSelectCard : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _levelNumberText;
        [SerializeField] private TextMeshProUGUI _bestTimeText;
        [SerializeField] private TextMeshProUGUI _bestRankText;
        [SerializeField] private GameObject _lockedOverlay;
        [SerializeField] private Image _background;

        [SerializeField] private Color _unlockedColor =
            new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color _lockedColor =
            new Color(0.55f, 0.55f, 0.55f, 1f);

        public event Action<int> Clicked;
        public int LevelIndex { get; private set; }

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
            bool isUnlocked,
            string bestTimeDisplay,
            string bestRankDisplay)
        {
            LevelIndex = levelIndex;

            if (_levelNumberText != null)
                _levelNumberText.text = (levelIndex + 1).ToString();

            if (_bestTimeText != null)
                _bestTimeText.text = bestTimeDisplay;

            if (_bestRankText != null)
                _bestRankText.text = bestRankDisplay;

            if (_lockedOverlay != null)
                _lockedOverlay.SetActive(!isUnlocked);

            if (_button != null)
                _button.interactable = isUnlocked;

            if (_background != null)
                _background.color = isUnlocked ? _unlockedColor : _lockedColor;
        }

        private void OnClicked() => Clicked?.Invoke(LevelIndex);
    }
}
