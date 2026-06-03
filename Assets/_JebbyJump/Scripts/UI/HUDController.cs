using JebbyJump.Analytics;
using JebbyJump.Flow;
using JebbyJump.Level;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Sequence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private LevelProgressTracker _tracker;
        [SerializeField] private MemoryPhaseController _phaseController;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private Transform _livesIconContainer;
        [SerializeField] private Sprite _lifeIconSprite;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private GameObject _levelCompletePanel;
        [SerializeField] private Button _gameOverRetryButton;
        [SerializeField] private Button _levelCompleteRetryButton;
        [SerializeField] private Button _gameOverMenuButton;
        [SerializeField] private Button _levelCompleteMenuButton;
        [SerializeField] private Button _levelCompleteNextButton;
        [SerializeField] private LevelSessionController _levelSession;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _levelCompleteTitleText;

        [Header("Time / Rank")]
        [SerializeField] private LevelTimer _levelTimer;
        [SerializeField] private TimeRankConfig _rankConfig;
        [SerializeField] private TextMeshProUGUI _levelCompleteTimeText;
        [SerializeField] private TextMeshProUGUI _levelCompleteBestTimeText;
        [SerializeField] private TextMeshProUGUI _levelCompleteRankText;
        // Optional per-level mastery stars on the result panel.
        // Null until scaffolded; star logic/store/analytics run regardless.
        [SerializeField] private TextMeshProUGUI _levelCompleteStarsText;
        // Optional top-right live timer.
        [SerializeField] private TextMeshProUGUI _liveTimerText;

        // Gold.
        private static readonly Color RankColorS =
            new Color(1.00f, 0.84f, 0.10f);
        // Silver.
        private static readonly Color RankColorA =
            new Color(0.85f, 0.85f, 0.90f);
        // Bronze.
        private static readonly Color RankColorB =
            new Color(0.80f, 0.50f, 0.20f);
        // Gray.
        private static readonly Color RankColorC =
            new Color(0.60f, 0.60f, 0.60f);

        private void Awake()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(false);

            if (_gameOverRetryButton != null)
                _gameOverRetryButton.onClick.AddListener(OnRetryClicked);
            if (_levelCompleteRetryButton != null)
                _levelCompleteRetryButton.onClick.AddListener(OnRetryClicked);
            if (_gameOverMenuButton != null)
                _gameOverMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteMenuButton != null)
                _levelCompleteMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteNextButton != null)
                _levelCompleteNextButton.onClick.AddListener(OnNextLevelClicked);
        }

        private void OnDestroy()
        {
            if (_gameOverRetryButton != null)
                _gameOverRetryButton.onClick.RemoveListener(OnRetryClicked);
            if (_levelCompleteRetryButton != null)
                _levelCompleteRetryButton.onClick.RemoveListener(OnRetryClicked);
            if (_gameOverMenuButton != null)
                _gameOverMenuButton.onClick.RemoveListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteMenuButton != null)
                _levelCompleteMenuButton.onClick.RemoveListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteNextButton != null)
                _levelCompleteNextButton.onClick.RemoveListener(OnNextLevelClicked);
        }

        private void OnEnable()
        {
            if (_tracker != null)
            {
                _tracker.LivesChanged += OnLivesChanged;
                _tracker.GameOver += OnGameOver;
            }
            if (_phaseController != null)
                _phaseController.LevelCompleted += OnLevelCompleted;
            if (_levelSession != null)
                _levelSession.LevelChanged += RefreshLevelText;
        }

        private void OnDisable()
        {
            if (_tracker != null)
            {
                _tracker.LivesChanged -= OnLivesChanged;
                _tracker.GameOver -= OnGameOver;
            }
            if (_phaseController != null)
                _phaseController.LevelCompleted -= OnLevelCompleted;
            if (_levelSession != null)
                _levelSession.LevelChanged -= RefreshLevelText;
        }

        private void Start()
        {
            if (_tracker != null)
                RefreshLives(_tracker.Lives);
            RefreshLevelText();
            if (_liveTimerText != null)
                _liveTimerText.text = FormatTime(0f);
        }

        private void Update()
        {
            // Live HUD timer (top-right).
            // Updates only while the run is active.
            if (_liveTimerText != null
                && _levelTimer != null
                && _levelTimer.IsRunning)
            {
                _liveTimerText.text = FormatTime(_levelTimer.Elapsed);
            }
        }

        // Format seconds as MM:SS.SS.
        // Example: 12.34 -> "00:12.34".
        private static string FormatTime(float seconds)
        {
            if (float.IsNaN(seconds) || seconds < 0f) seconds = 0f;
            int   minutes = (int)(seconds / 60f);
            float rest    = seconds - minutes * 60f;
            return $"{minutes:00}:{rest:00.00}";
        }

        private void RefreshLevelText()
        {
            if (_levelText != null && _levelSession != null)
            {
                _levelText.text =
                    "Level " + (_levelSession.CurrentLevelIndex + 1);
            }
        }

        private void OnLivesChanged(int lives) => RefreshLives(lives);

        private void RefreshLives(int lives)
        {
            if (_livesIconContainer != null && _lifeIconSprite != null)
            {
                for (int i = _livesIconContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(_livesIconContainer.GetChild(i).gameObject);
                }
                for (int i = 0; i < lives; i++)
                {
                    var go = new GameObject(
                        $"Heart_{i}", typeof(RectTransform), typeof(Image));
                    go.transform.SetParent(_livesIconContainer, false);
                    go.GetComponent<RectTransform>().sizeDelta =
                        new Vector2(52f, 52f);
                    go.GetComponent<Image>().sprite = _lifeIconSprite;
                }
            }
            else if (_livesText != null)
            {
                _livesText.text = "♥ " + lives;
            }
        }

        private void OnGameOver()
        {
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(false);
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(true);
        }

        private void OnLevelCompleted()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(true);

            bool isFinal =
                _levelSession != null && _levelSession.IsFinalLevel;
            if (_levelCompleteNextButton != null)
            {
                _levelCompleteNextButton.gameObject.SetActive(!isFinal);
            }
            if (_levelCompleteTitleText != null)
            {
                _levelCompleteTitleText.text =
                    isFinal ? "MVP Complete!" : "Level Complete!";
            }

            // Progression side effect lives here, not inside the display
            // formatter below. Game Over and Retry never reach this method,
            // so unlock cannot leak into failure paths.
            RecordLevelCompletion();

            PopulateTimeRank();
        }

        private void RecordLevelCompletion()
        {
            if (_levelSession == null) return;
            LevelProgressStore.UnlockNext(_levelSession.CurrentLevelIndex);
        }

        private void PopulateTimeRank()
        {
            if (_levelTimer == null) return;

            float elapsed = _levelTimer.Elapsed;
            string levelKey =
                _levelSession != null
                && _levelSession.CurrentConfig != null
                    ? _levelSession.CurrentConfig.name
                    : null;

            // Capture the prior best BEFORE TrySetBest overwrites it, so
            // best_time_improved can report old/new. Read-only capture;
            // TrySetBest behaviour and save semantics are unchanged.
            float oldBest = BestTimeStore.GetBest(levelKey);
            bool isNewBest = BestTimeStore.TrySetBest(levelKey, elapsed);
            float best = BestTimeStore.GetBest(levelKey);

            if (_levelCompleteTimeText != null)
            {
                _levelCompleteTimeText.text =
                    $"Time: {FormatTime(elapsed)}";
            }

            if (_levelCompleteBestTimeText != null)
            {
                if (isNewBest)
                {
                    _levelCompleteBestTimeText.text =
                        $"Best: {FormatTime(best)}  (New!)";
                }
                else if (float.IsNaN(best))
                {
                    _levelCompleteBestTimeText.text = "Best: --";
                }
                else
                {
                    _levelCompleteBestTimeText.text =
                        $"Best: {FormatTime(best)}";
                }
            }

            // Per-level rank config takes priority.
            // Fall back to the shared default if the level does not set one.
            var rankCfg =
                _levelSession != null
                && _levelSession.CurrentConfig != null
                && _levelSession.CurrentConfig.RankConfig != null
                    ? _levelSession.CurrentConfig.RankConfig
                    : _rankConfig;

            TimeRank? computedRank = rankCfg != null
                ? rankCfg.GetRank(elapsed)
                : (TimeRank?)null;

            if (_levelCompleteRankText != null && computedRank.HasValue)
            {
                var rank = computedRank.Value;
                _levelCompleteRankText.text = $"Rank: {rank}";
                _levelCompleteRankText.color = rank switch
                {
                    TimeRank.S => RankColorS,
                    TimeRank.A => RankColorA,
                    TimeRank.B => RankColorB,
                    _          => RankColorC
                };
            }

            EmitCompletionAnalytics(elapsed, oldBest, best, isNewBest, computedRank);
            GrantStars(computedRank);
        }

        // Awards mastery stars for this clear (S/A=3, B=2, C=1, completed
        // with no rank config=1). Best-only via StarRewardStore (never
        // decreases). Updates the result-panel stars text if wired, and
        // emits reward analytics only when the stored best increases.
        private void GrantStars(TimeRank? computedRank)
        {
            int levelIndex = _levelSession != null
                ? _levelSession.CurrentLevelIndex : 0;
            int levelCount = _levelSession != null
                ? _levelSession.TotalLevels : 0;

            string rankStr = computedRank.HasValue
                ? computedRank.Value.ToString() : null;
            int starsThisClear =
                StarRewardCalculator.StarsForRank(rankStr, completed: true);

            int prevStars = StarRewardStore.GetStars(levelIndex);
            int oldTotal  = StarRewardStore.GetTotalStars(levelCount);
            int storedStars =
                StarRewardStore.SetStarsIfHigher(levelIndex, starsThisClear);
            bool improved = storedStars > prevStars;

            if (_levelCompleteStarsText != null)
            {
                _levelCompleteStarsText.text =
                    $"Stars: {starsThisClear}/{StarRewardCalculator.MaxStars}"
                    + (improved ? "  (New Star Best!)" : string.Empty);
            }

            // Emit only when the stored best actually increased - a replay
            // with an equal/lower rank grants nothing and emits nothing.
            if (!improved) return;

            int newTotal = StarRewardStore.GetTotalStars(levelCount);
            AnalyticsService.Track(AnalyticsEvents.RewardGranted,
                AnalyticsParam.Of(AnalyticsParams.RewardType, "stars"),
                AnalyticsParam.Of(AnalyticsParams.LevelIndex, levelIndex),
                AnalyticsParam.Of(AnalyticsParams.LevelNumber, levelIndex + 1),
                AnalyticsParam.Of(AnalyticsParams.Amount, storedStars - prevStars),
                AnalyticsParam.Of(AnalyticsParams.TotalForLevel, storedStars),
                AnalyticsParam.Of(AnalyticsParams.PreviousForLevel, prevStars),
                AnalyticsParam.Of(AnalyticsParams.Reason, "level_clear"));
            AnalyticsService.Track(AnalyticsEvents.StarTotalChanged,
                AnalyticsParam.Of(AnalyticsParams.OldTotal, oldTotal),
                AnalyticsParam.Of(AnalyticsParams.NewTotal, newTotal),
                AnalyticsParam.Of(AnalyticsParams.Delta, newTotal - oldTotal));
        }

        // Single source of truth for completion analytics. rank is included
        // in level_completed (so no separate rank_earned event - that would
        // duplicate rank data). best_time_improved fires only when there
        // was a prior best and the new time beat it.
        private void EmitCompletionAnalytics(
            float elapsed, float oldBest, float newBest,
            bool isNewBest, TimeRank? rank)
        {
            int levelIndex = _levelSession != null
                ? _levelSession.CurrentLevelIndex : 0;
            int levelNumber = levelIndex + 1;

            AnalyticsService.Track(AnalyticsEvents.LevelCompleted,
                AnalyticsParam.Of(AnalyticsParams.LevelIndex, levelIndex),
                AnalyticsParam.Of(AnalyticsParams.LevelNumber, levelNumber),
                AnalyticsParam.Of(AnalyticsParams.ElapsedTime, elapsed),
                AnalyticsParam.Of(AnalyticsParams.Rank, rank.HasValue ? rank.Value.ToString() : "none"),
                AnalyticsParam.Of(AnalyticsParams.IsNewBest, isNewBest));

            if (isNewBest && !float.IsNaN(oldBest))
            {
                AnalyticsService.Track(AnalyticsEvents.BestTimeImproved,
                    AnalyticsParam.Of(AnalyticsParams.LevelIndex, levelIndex),
                    AnalyticsParam.Of(AnalyticsParams.LevelNumber, levelNumber),
                    AnalyticsParam.Of(AnalyticsParams.OldBestTime, oldBest),
                    AnalyticsParam.Of(AnalyticsParams.NewBestTime, newBest),
                    AnalyticsParam.Of(AnalyticsParams.ImprovementSeconds, oldBest - newBest));
            }
        }

        private void OnRetryClicked()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(false);
            _phaseController?.RestartLevel();
        }

        private void OnNextLevelClicked()
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(false);
            _phaseController?.StartNextLevel();
        }
    }
}
