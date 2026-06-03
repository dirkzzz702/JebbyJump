using System.Collections.Generic;
using JebbyJump.Analytics;
using JebbyJump.Flow;
using JebbyJump.Level;
using JebbyJump.Progression;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Populates the Level Select grid from a LevelCatalog. Best time
    // comes from BestTimeStore. Rank is computed dynamically at display
    // time from best time + LevelConfig.RankConfig and is never stored,
    // so future TimeRankConfig tuning cannot strand a stale stored rank.
    public class LevelSelectController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private LevelCatalog _catalog;

        [Header("Layout")]
        [SerializeField] private RectTransform _cardContainer;
        [SerializeField] private LevelSelectCard _cardPrefab;

        [Header("Navigation")]
        [SerializeField] private Button _backButton;

        [SerializeField] private GameObject _panelRoot;

        private readonly List<LevelSelectCard> _spawned =
            new List<LevelSelectCard>();

        private void Awake()
        {
            if (_backButton != null) _backButton.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            if (_backButton != null) _backButton.onClick.RemoveListener(Close);
            ClearCards();
        }

        public void Open()
        {
            AnalyticsService.Track("level_select_opened");
            if (_panelRoot != null) _panelRoot.SetActive(true);
            Rebuild();
        }

        public void Close()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);
        }

        public void Rebuild()
        {
            ClearCards();

            if (_catalog == null)
            {
                Debug.LogError(
                    "[LevelSelect] No LevelCatalog assigned. "
                    + "Run 'Jebby Jump/Progression/Create Or Sync Level Catalog' "
                    + "and drag the asset into the Catalog slot.",
                    this);
                return;
            }
            if (_catalog.Count == 0)
            {
                Debug.LogWarning(
                    "[LevelSelect] LevelCatalog has zero entries.", this);
                return;
            }
            if (_cardPrefab == null || _cardContainer == null)
            {
                Debug.LogWarning(
                    "[LevelSelect] Missing card prefab or container.", this);
                return;
            }

            for (int i = 0; i < _catalog.Count; i++)
            {
                var card = Instantiate(_cardPrefab, _cardContainer);
                card.gameObject.name = $"LevelCard_{i + 1}";

                bool unlocked = LevelProgressStore.IsUnlocked(i);
                string levelKey = _catalog.GetLevelKey(i);

                float best = !string.IsNullOrEmpty(levelKey)
                    ? BestTimeStore.GetBest(levelKey)
                    : float.NaN;
                bool hasBest = !float.IsNaN(best);
                string bestText = hasBest
                    ? $"Best {FormatTime(best)}"
                    : "Best --";

                // Rank is recomputed every time the panel opens.
                // No persistent rank store; tuning the TimeRankConfig
                // immediately reflects in the displayed rank.
                var cfg = _catalog.Get(i);
                TimeRank? rank = null;
                if (hasBest && cfg != null && cfg.RankConfig != null)
                {
                    rank = cfg.RankConfig.GetRank(best);
                }
                string rankText = rank.HasValue
                    ? $"Rank {rank.Value}"
                    : "Rank --";

                var state = LevelCardClassifier.Classify(unlocked, hasBest);
                card.Bind(i, state, bestText, rankText);
                card.Clicked += OnCardClicked;
                _spawned.Add(card);
            }
        }

        private void OnCardClicked(int levelIndex)
        {
            if (_catalog == null) return;
            if (levelIndex < 0 || levelIndex >= _catalog.Count) return;
            if (!LevelProgressStore.IsUnlocked(levelIndex)) return;

            string levelKey = _catalog.GetLevelKey(levelIndex);
            bool hasBest = !string.IsNullOrEmpty(levelKey)
                && !float.IsNaN(BestTimeStore.GetBest(levelKey));
            AnalyticsService.Track("level_selected",
                AnalyticsParam.Of("level_index", levelIndex),
                AnalyticsParam.Of("level_number", levelIndex + 1),
                AnalyticsParam.Of("is_replay", hasBest),
                AnalyticsParam.Of("has_best_time", hasBest));

            PendingLevelSelection.Index = levelIndex;
            PendingLevelSelection.Source = "level_select";
            SceneLoader.LoadGame();
        }

        private void ClearCards()
        {
            for (int i = 0; i < _spawned.Count; i++)
            {
                if (_spawned[i] == null) continue;
                _spawned[i].Clicked -= OnCardClicked;
                Destroy(_spawned[i].gameObject);
            }
            _spawned.Clear();
        }

        // Mirrors HUDController.FormatTime so the two screens display
        // best times in the same MM:SS.SS format.
        private static string FormatTime(float seconds)
        {
            if (float.IsNaN(seconds) || seconds < 0f) seconds = 0f;
            int   minutes = (int)(seconds / 60f);
            float rest    = seconds - minutes * 60f;
            return $"{minutes:00}:{rest:00.00}";
        }
    }
}
