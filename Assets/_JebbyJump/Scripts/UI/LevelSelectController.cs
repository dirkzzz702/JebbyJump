using System.Collections.Generic;
using JebbyJump.Analytics;
using JebbyJump.Flow;
using JebbyJump.Level;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Shell;
using JebbyJump.Wardrobe.Visual; // ScrollIntoViewCalculator (reused)
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private ScrollRect _scrollRect; // optional; scroll-to-focus

        private readonly List<LevelSelectCard> _spawned =
            new List<LevelSelectCard>();
        private GameObject _opener;          // restore focus here on Close
        private int _lastFocusedCardIndex = -1;

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
            _opener = EventSystem.current != null
                ? EventSystem.current.currentSelectedGameObject : null;
            AnalyticsService.Track(AnalyticsEvents.LevelSelectOpened);
            if (_panelRoot != null) _panelRoot.SetActive(true);
            Rebuild();
            BuildNavigationAndFocus();
        }

        public void Close()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);
            ShellFocusUtil.Select(_opener); // restore Main Menu opener (P21)
        }

        // P21: explicit grid navigation over the live cards (locked cards stay
        // focusable; bottom row -> Back) + deterministic initial focus
        // (current/continue level if valid, else first card, else Back).
        private void BuildNavigationAndFocus()
        {
            var cards = new List<Selectable>(_spawned.Count);
            foreach (var c in _spawned)
                cards.Add(c != null ? c.Selectable : null);
            ShellFocusUtil.BuildGridNavigation(
                cards, ShellLayoutMetrics.LevelSelectColumns, _backButton);

            int count = _spawned.Count;
            int preferred = _catalog != null
                ? LevelProgressStore.GetContinueIndex(_catalog.Count) : 0;
            int idx = ShellFocusResolver.ResolvePreferredOrFirst(count, preferred);
            _lastFocusedCardIndex = -1;
            if (idx >= 0 && _spawned[idx] != null)
                ShellFocusUtil.Select(_spawned[idx].Selectable);
            else
                ShellFocusUtil.Select(_backButton);
        }

        private void Update()
        {
            if (_panelRoot == null || !_panelRoot.activeSelf) return;
            ScrollFocusedCardIntoView();
        }

        private void ScrollFocusedCardIntoView()
        {
            if (_scrollRect == null || _scrollRect.content == null
                || _scrollRect.viewport == null || EventSystem.current == null)
                return;
            var sel = EventSystem.current.currentSelectedGameObject;
            if (sel == null) return;

            int idx = -1;
            for (int i = 0; i < _spawned.Count; i++)
                if (_spawned[i] != null && _spawned[i].Selectable != null
                    && _spawned[i].Selectable.gameObject == sel) { idx = i; break; }
            if (idx < 0 || idx == _lastFocusedCardIndex) return;
            _lastFocusedCardIndex = idx;

            int columns = ShellLayoutMetrics.LevelSelectColumns;
            int row = idx / columns;
            float itemTop = ShellLayoutMetrics.GridPadding
                + row * (ShellLayoutMetrics.LevelSelectCellHeight
                    + ShellLayoutMetrics.GridSpacing);
            _scrollRect.verticalNormalizedPosition =
                ScrollIntoViewCalculator.ComputeVerticalNormalized(
                    _scrollRect.content.rect.height,
                    _scrollRect.viewport.rect.height, itemTop,
                    ShellLayoutMetrics.LevelSelectCellHeight,
                    _scrollRect.verticalNormalizedPosition);
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

                // Read-only display of stored best stars (P7A store).
                // Level Select never writes stars or emits analytics.
                int stars = StarRewardStore.GetStars(i);

                var state = LevelCardClassifier.Classify(unlocked, hasBest);
                card.Bind(i, state, bestText, rankText, stars);
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
            AnalyticsService.Track(AnalyticsEvents.LevelSelected,
                AnalyticsParam.Of(AnalyticsParams.LevelIndex, levelIndex),
                AnalyticsParam.Of(AnalyticsParams.LevelNumber, levelIndex + 1),
                AnalyticsParam.Of(AnalyticsParams.IsReplay, hasBest),
                AnalyticsParam.Of(AnalyticsParams.HasBestTime, hasBest));

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
