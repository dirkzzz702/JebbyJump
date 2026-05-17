using JebbyJump.Level;
using JebbyJump.Sequence;
using TMPro;
using UnityEngine;

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

        private void Awake()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
            if (_levelCompletePanel != null) _levelCompletePanel.SetActive(false);
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
        }

        private void Start()
        {
            if (_tracker != null)
            {
                RefreshLives(_tracker.Lives);
                RefreshScore(_tracker.Score);
            }
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
        }
    }
}
