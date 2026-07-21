using System.Collections.Generic;
using JebbyJump.Analytics;
using JebbyJump.Core;
using JebbyJump.Flow;
using JebbyJump.Level;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Shell;
using JebbyJump.Wardrobe.Visual; // ScrollIntoViewCalculator (reused)
using JebbyJump.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Populates the Level Select grid from a LevelCatalog. Best time comes
    // from BestTimeStore. Rank is computed dynamically at display time from
    // best time + LevelConfig.RankConfig and is never stored.
    //
    // WorldExpansion100 P34D: the grid is now WORLD-SCOPED. A world tab strip
    // selects one of the ten worlds, and only that world's ten levels render -
    // replacing the old flat grid of all levels, which does not scale to 100.
    // Cards still carry the GLOBAL level index, so save keys (index-based
    // unlock/stars, name-based best time) are unchanged.
    public class LevelSelectController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private LevelCatalog _catalog;
        [SerializeField] private WorldCatalog _worldCatalog;

        [Header("Layout")]
        [SerializeField] private RectTransform _cardContainer;
        [SerializeField] private LevelSelectCard _cardPrefab;

        [Header("World strip")]
        [SerializeField] private Transform _worldTabContainer; // holds WorldTab children
        [SerializeField] private TMPro.TextMeshProUGUI _worldTitle;

        [Header("Navigation")]
        [SerializeField] private Button _backButton;

        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private ScrollRect _scrollRect; // optional; scroll-to-focus

        private readonly List<LevelSelectCard> _spawned =
            new List<LevelSelectCard>();
        private readonly List<WorldTab> _worldTabs = new List<WorldTab>();
        private GameObject _opener;          // restore focus here on Close
        private int _lastFocusedCardIndex = -1;
        private int _currentWorld = 1;       // 1-based

        private void Awake()
        {
            if (_backButton != null) _backButton.onClick.AddListener(Close);
            CollectWorldTabs();
        }

        private void OnDestroy()
        {
            if (_backButton != null) _backButton.onClick.RemoveListener(Close);
            foreach (var t in _worldTabs)
                if (t != null) t.Clicked -= OnWorldTabClicked;
            ClearCards();
        }

        private void CollectWorldTabs()
        {
            _worldTabs.Clear();
            if (_worldTabContainer == null) return;
            for (int i = 0; i < _worldTabContainer.childCount; i++)
            {
                var tab = _worldTabContainer.GetChild(i).GetComponent<WorldTab>();
                if (tab == null) continue;
                tab.Clicked -= OnWorldTabClicked;
                tab.Clicked += OnWorldTabClicked;
                _worldTabs.Add(tab);
            }
        }

        public void Open()
        {
            _opener = EventSystem.current != null
                ? EventSystem.current.currentSelectedGameObject : null;
            AnalyticsService.Track(AnalyticsEvents.LevelSelectOpened);
            if (_panelRoot != null) _panelRoot.SetActive(true);

            // Open on the world containing the player's continue level.
            int continueIndex = _catalog != null
                ? LevelProgressStore.GetContinueIndex(_catalog.Count) : 0;
            _currentWorld = WorldMapping.WorldNumberForLevelIndex(continueIndex);
            if (_currentWorld <= 0) _currentWorld = 1;

            BindWorldTabs();
            SelectWorld(_currentWorld, focusContinueLevel: true);
        }

        public void Close()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);
            ShellFocusUtil.Select(_opener); // restore Main Menu opener (P21)
        }

        private void OnWorldTabClicked(int worldNumber)
            => SelectWorld(worldNumber, focusContinueLevel: false);

        private void SelectWorld(int worldNumber, bool focusContinueLevel)
        {
            _currentWorld = Mathf.Clamp(worldNumber, 1, WorldMapping.WorldCount);
            for (int i = 0; i < _worldTabs.Count; i++)
                if (_worldTabs[i] != null)
                    _worldTabs[i].SetSelected(_worldTabs[i].WorldNumber == _currentWorld);

            if (_worldTitle != null)
                _worldTitle.text = WorldTitle(_currentWorld);

            AnalyticsService.Track(AnalyticsEvents.LevelSelectOpened,
                AnalyticsParam.Of(AnalyticsParams.WorldNumber, _currentWorld));

            Rebuild();
            BuildNavigationAndFocus(focusContinueLevel);
        }

        private string WorldTitle(int worldNumber)
        {
            var def = _worldCatalog != null ? _worldCatalog.GetByNumber(worldNumber) : null;
            string name = def != null && !string.IsNullOrEmpty(def.DisplayName)
                ? def.DisplayName : "World " + worldNumber;
            return "World " + worldNumber + " - " + name;
        }

        private void BindWorldTabs()
        {
            for (int i = 0; i < _worldTabs.Count; i++)
            {
                var tab = _worldTabs[i];
                if (tab == null) continue;
                int worldNumber = i + 1;
                int firstIndex = WorldMapping.FirstLevelIndexOfWorld(worldNumber);
                bool locked = firstIndex < 0 || !LevelProgressStore.IsUnlocked(firstIndex);
                tab.Bind(worldNumber, worldNumber.ToString(), locked);
            }
        }

        // Grid navigation over the live cards + the world tab row above them.
        private void BuildNavigationAndFocus(bool focusContinueLevel)
        {
            var cards = new List<Selectable>(_spawned.Count);
            foreach (var c in _spawned)
                cards.Add(c != null ? c.Selectable : null);
            ShellFocusUtil.BuildGridNavigation(
                cards, ShellLayoutMetrics.LevelSelectColumns, _backButton);

            WireWorldTabNavigation(cards);

            int count = _spawned.Count;
            int idx = -1;
            if (focusContinueLevel)
            {
                int continueIndex = _catalog != null
                    ? LevelProgressStore.GetContinueIndex(_catalog.Count) : 0;
                int localSlot = continueIndex - WorldFirstIndex();
                if (localSlot >= 0 && localSlot < count) idx = localSlot;
            }
            _lastFocusedCardIndex = -1;
            if (idx >= 0 && _spawned[idx] != null)
                ShellFocusUtil.Select(_spawned[idx].Selectable);
            else if (_worldTabs.Count >= _currentWorld && _worldTabs[_currentWorld - 1] != null)
                ShellFocusUtil.Select(_worldTabs[_currentWorld - 1].Selectable);
            else
                ShellFocusUtil.Select(_backButton);
        }

        // World tabs: horizontal row; Down -> first card. Top-row cards Up ->
        // the current world's tab. Keeps the strip reachable by keyboard/gamepad.
        private void WireWorldTabNavigation(IList<Selectable> cards)
        {
            var tabSel = new List<Selectable>(_worldTabs.Count);
            foreach (var t in _worldTabs) tabSel.Add(t != null ? t.Selectable : null);

            Selectable firstCard = cards.Count > 0 ? cards[0] : _backButton;
            for (int i = 0; i < tabSel.Count; i++)
            {
                if (tabSel[i] == null) continue;
                var nav = new Navigation { mode = Navigation.Mode.Explicit };
                nav.selectOnLeft = PrevNonNull(tabSel, i);
                nav.selectOnRight = NextNonNull(tabSel, i);
                nav.selectOnDown = firstCard;
                tabSel[i].navigation = nav;
            }

            Selectable currentTab = (_currentWorld - 1) < tabSel.Count
                ? tabSel[_currentWorld - 1] : null;
            if (currentTab != null)
            {
                int columns = ShellLayoutMetrics.LevelSelectColumns;
                for (int i = 0; i < cards.Count && i < columns; i++)
                {
                    if (cards[i] == null) continue;
                    var nav = cards[i].navigation;
                    nav.selectOnUp = currentTab;
                    cards[i].navigation = nav;
                }
            }
        }

        private static Selectable PrevNonNull(IList<Selectable> list, int from)
        {
            for (int i = from - 1; i >= 0; i--) if (list[i] != null) return list[i];
            return null;
        }

        private static Selectable NextNonNull(IList<Selectable> list, int from)
        {
            for (int i = from + 1; i < list.Count; i++) if (list[i] != null) return list[i];
            return null;
        }

        private int WorldFirstIndex()
            => Mathf.Max(0, WorldMapping.FirstLevelIndexOfWorld(_currentWorld));

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

        // Rebuilds the grid for the CURRENT world's ten levels only.
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

            int first = WorldFirstIndex();
            int last = WorldMapping.LastLevelIndexOfWorld(_currentWorld);
            if (last < 0) last = _catalog.Count - 1;

            for (int i = first; i <= last && i < _catalog.Count; i++)
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

                var cfg = _catalog.Get(i);
                TimeRank? rank = null;
                if (hasBest && cfg != null && cfg.RankConfig != null)
                    rank = cfg.RankConfig.GetRank(best);
                string rankText = rank.HasValue
                    ? $"Rank {rank.Value}"
                    : "Rank --";

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
                AnalyticsParam.Of(AnalyticsParams.WorldNumber,
                    WorldMapping.WorldNumberForLevelIndex(levelIndex)),
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

        // Mirrors HUDController.FormatTime so the two screens display best
        // times in the same MM:SS.SS format.
        private static string FormatTime(float seconds)
        {
            if (float.IsNaN(seconds) || seconds < 0f) seconds = 0f;
            int   minutes = (int)(seconds / 60f);
            float rest    = seconds - minutes * 60f;
            return $"{minutes:00}:{rest:00.00}";
        }
    }
}
