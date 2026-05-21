using JebbyJump.Flow;
using JebbyJump.Level;
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
        [SerializeField] private TextMeshProUGUI _liveTimerText;   // optional top-right live timer

        private static readonly Color RankColorS = new Color(1.00f, 0.84f, 0.10f); // gold
        private static readonly Color RankColorA = new Color(0.85f, 0.85f, 0.90f); // silver
        private static readonly Color RankColorB = new Color(0.80f, 0.50f, 0.20f); // bronze
        private static readonly Color RankColorC = new Color(0.60f, 0.60f, 0.60f); // gray

        private void Awake()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(false);
            if (_gameOverRetryButton != null) _gameOverRetryButton.onClick.AddListener(OnRetryClicked);
            if (_levelCompleteRetryButton != null) _levelCompleteRetryButton.onClick.AddListener(OnRetryClicked);
            if (_gameOverMenuButton != null) _gameOverMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteMenuButton != null) _levelCompleteMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteNextButton != null) _levelCompleteNextButton.onClick.AddListener(OnNextLevelClicked);
        }

        private void OnDestroy()
        {
            if (_gameOverRetryButton != null) _gameOverRetryButton.onClick.RemoveListener(OnRetryClicked);
            if (_levelCompleteRetryButton != null) _levelCompleteRetryButton.onClick.RemoveListener(OnRetryClicked);
            if (_gameOverMenuButton != null) _gameOverMenuButton.onClick.RemoveListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteMenuButton != null) _levelCompleteMenuButton.onClick.RemoveListener(SceneLoader.LoadMainMenu);
            if (_levelCompleteNextButton != null) _levelCompleteNextButton.onClick.RemoveListener(OnNextLevelClicked);
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
            if (_liveTimerText != null) _liveTimerText.text = FormatTime(0f);
        }

        private void Update()
        {
            // Live HUD timer (top-right). Updates only while the run is active.
            if (_liveTimerText != null && _levelTimer != null && _levelTimer.IsRunning)
                _liveTimerText.text = FormatTime(_levelTimer.Elapsed);
        }

        // MM:SS.SS — e.g. "00:12.34".
        private static string FormatTime(float seconds)
        {
            if (float.IsNaN(seconds) || seconds < 0f) seconds = 0f;
            int  minutes = (int)(seconds / 60f);
            float rest   = seconds - minutes * 60f;
            return $"{minutes:00}:{rest:00.00}";
        }

        private void RefreshLevelText()
        {
            if (_levelText != null && _levelSession != null)
                _levelText.text = "Level " + (_levelSession.CurrentLevelIndex + 1);
        }

        private void OnLivesChanged(int lives) => RefreshLives(lives);

        private void RefreshLives(int lives)
        {
            if (_livesIconContainer != null && _lifeIconSprite != null)
            {
                for (int i = _livesIconContainer.childCount - 1; i >= 0; i--)
                    Destroy(_livesIconContainer.GetChild(i).gameObject);
                for (int i = 0; i < lives; i++)
                {
                    var go = new GameObject($"Heart_{i}", typeof(RectTransform), typeof(Image));
                    go.transform.SetParent(_livesIconContainer, false);
                    go.GetComponent<RectTransform>().sizeDelta = new Vector2(52f, 52f);
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
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(false);
            if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
        }

        private void OnLevelCompleted()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(true);

            bool isFinal = _levelSession != null && _levelSession.IsFinalLevel;
            if (_levelCompleteNextButton != null) _levelCompleteNextButton.gameObject.SetActive(!isFinal);
            if (_levelCompleteTitleText != null)
                _levelCompleteTitleText.text = isFinal ? "MVP Complete!" : "Level Complete!";

            PopulateTimeRank();
        }

        private void PopulateTimeRank()
        {
            if (_levelTimer == null) return;

            float elapsed = _levelTimer.Elapsed;
            string levelKey = _levelSession != null && _levelSession.CurrentConfig != null
                ? _levelSession.CurrentConfig.name
                : null;

            bool isNewBest = BestTimeStore.TrySetBest(levelKey, elapsed);
            float best = BestTimeStore.GetBest(levelKey);

            if (_levelCompleteTimeText != null)
                _levelCompleteTimeText.text = $"Time: {FormatTime(elapsed)}";

            if (_levelCompleteBestTimeText != null)
                _levelCompleteBestTimeText.text = isNewBest
                    ? $"Best: {FormatTime(best)}  (New!)"
                    : (float.IsNaN(best) ? "Best: --" : $"Best: {FormatTime(best)}");

            if (_levelCompleteRankText != null && _rankConfig != null)
            {
                var rank = _rankConfig.GetRank(elapsed);
                _levelCompleteRankText.text = $"Rank: {rank}";
                _levelCompleteRankText.color = rank switch
                {
                    TimeRank.S => RankColorS,
                    TimeRank.A => RankColorA,
                    TimeRank.B => RankColorB,
                    _          => RankColorC
                };
            }
        }

        private void OnRetryClicked()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(false);
            _phaseController?.RestartLevel();
        }

        private void OnNextLevelClicked()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(false);
            _phaseController?.StartNextLevel();
        }
    }
}
