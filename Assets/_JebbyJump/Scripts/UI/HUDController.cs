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
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverScoreText;
        [SerializeField] private GameObject _levelCompletePanel;
        [SerializeField] private TextMeshProUGUI _levelCompleteScoreText;
        [SerializeField] private Button _gameOverRetryButton;
        [SerializeField] private Button _levelCompleteRetryButton;
        [SerializeField] private Button _gameOverMenuButton;
        [SerializeField] private Button _levelCompleteMenuButton;
        [SerializeField] private Button _levelCompleteNextButton;
        [SerializeField] private LevelSessionController _levelSession;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _levelCompleteTitleText;

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
                _tracker.ScoreChanged += OnScoreChanged;
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
                _tracker.ScoreChanged -= OnScoreChanged;
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
            {
                RefreshLives(_tracker.Lives);
                RefreshScore(_tracker.Score);
            }
            RefreshLevelText();
        }

        private void RefreshLevelText()
        {
            if (_levelText != null && _levelSession != null)
                _levelText.text = "Level " + (_levelSession.CurrentLevelIndex + 1);
        }

        private void OnLivesChanged(int lives) => RefreshLives(lives);
        private void OnScoreChanged(int score) => RefreshScore(score);

        private void RefreshLives(int lives)
        {
            if (_livesText != null) _livesText.text = "♥ " + lives;
        }

        private void RefreshScore(int score)
        {
            if (_scoreText != null) _scoreText.text = "Score: " + score;
        }

        private void OnGameOver()
        {
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(false);
            if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
            if (_gameOverScoreText != null && _tracker != null)
                _gameOverScoreText.text = "Score: " + _tracker.Score;
        }

        private void OnLevelCompleted()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(true);
            if (_levelCompleteScoreText != null && _tracker != null)
                _levelCompleteScoreText.text = "Score: " + _tracker.Score;

            bool isFinal = _levelSession != null && _levelSession.IsFinalLevel;
            if (_levelCompleteNextButton != null) _levelCompleteNextButton.gameObject.SetActive(!isFinal);
            if (_levelCompleteTitleText != null)
                _levelCompleteTitleText.text = isFinal ? "MVP Complete!" : "Level Complete!";
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
