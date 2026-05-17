using JebbyJump.Level;
using JebbyJump.Platforms;
using JebbyJump.Player;
using JebbyJump.Sequence;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.Audio
{
    public class AudioFeedbackController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [Header("Player")]
        [SerializeField] private AudioClip _jumpClip;
        [SerializeField] private AudioClip _landClip;

        [Header("Gameplay")]
        [SerializeField] private AudioClip _correctClip;
        [SerializeField] private AudioClip _wrongClip;
        [SerializeField] private AudioClip _cactusHitClip;
        [SerializeField] private AudioClip _levelCompleteClip;
        [SerializeField] private AudioClip _gameOverClip;

        [Header("UI")]
        [SerializeField] private AudioClip _buttonClickClip;

        [Header("Event Sources")]
        [SerializeField] private PlayerMotor _playerMotor;
        [SerializeField] private MemoryPhaseController _phaseController;
        [SerializeField] private LevelProgressTracker _progressTracker;
        [SerializeField] private PlatformSpawner _platformSpawner;

        [Header("UI Buttons")]
        [SerializeField] private Button _gameOverRetryButton;
        [SerializeField] private Button _gameOverMenuButton;
        [SerializeField] private Button _levelCompleteRetryButton;
        [SerializeField] private Button _levelCompleteMenuButton;

        private void OnEnable()
        {
            if (_playerMotor != null)
            {
                _playerMotor.Jumped += OnJumped;
                _playerMotor.Landed += OnLanded;
            }
            if (_phaseController != null)
            {
                _phaseController.CorrectLanding += OnCorrect;
                _phaseController.WrongLanding   += OnWrong;
                _phaseController.LevelCompleted += OnLevelComplete;
            }
            if (_progressTracker != null)
                _progressTracker.GameOver += OnGameOver;
            if (_platformSpawner != null)
                _platformSpawner.CactusHit += OnCactusHit;

            if (_gameOverRetryButton != null)      _gameOverRetryButton.onClick.AddListener(OnButtonClick);
            if (_gameOverMenuButton != null)       _gameOverMenuButton.onClick.AddListener(OnButtonClick);
            if (_levelCompleteRetryButton != null) _levelCompleteRetryButton.onClick.AddListener(OnButtonClick);
            if (_levelCompleteMenuButton != null)  _levelCompleteMenuButton.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            if (_playerMotor != null)
            {
                _playerMotor.Jumped -= OnJumped;
                _playerMotor.Landed -= OnLanded;
            }
            if (_phaseController != null)
            {
                _phaseController.CorrectLanding -= OnCorrect;
                _phaseController.WrongLanding   -= OnWrong;
                _phaseController.LevelCompleted -= OnLevelComplete;
            }
            if (_progressTracker != null)
                _progressTracker.GameOver -= OnGameOver;
            if (_platformSpawner != null)
                _platformSpawner.CactusHit -= OnCactusHit;

            if (_gameOverRetryButton != null)      _gameOverRetryButton.onClick.RemoveListener(OnButtonClick);
            if (_gameOverMenuButton != null)       _gameOverMenuButton.onClick.RemoveListener(OnButtonClick);
            if (_levelCompleteRetryButton != null) _levelCompleteRetryButton.onClick.RemoveListener(OnButtonClick);
            if (_levelCompleteMenuButton != null)  _levelCompleteMenuButton.onClick.RemoveListener(OnButtonClick);
        }

        private void Play(AudioClip clip)
        {
            if (_audioSource == null || clip == null) return;
            _audioSource.PlayOneShot(clip);
        }

        private void OnJumped()                     => Play(_jumpClip);
        private void OnLanded(Collider2D _)         => Play(_landClip);
        private void OnCorrect()                    => Play(_correctClip);
        private void OnWrong()                      => Play(_wrongClip);
        private void OnCactusHit()                  => Play(_cactusHitClip);
        private void OnLevelComplete()              => Play(_levelCompleteClip);
        private void OnGameOver()                   => Play(_gameOverClip);
        private void OnButtonClick()                => Play(_buttonClickClip);
    }
}
