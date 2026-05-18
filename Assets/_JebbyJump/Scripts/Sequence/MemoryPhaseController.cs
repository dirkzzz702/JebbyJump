using System;
using System.Collections;
using JebbyJump.Items;
using JebbyJump.Level;
using JebbyJump.Obstacles;
using JebbyJump.Platforms;
using JebbyJump.Player;
using JebbyJump.UI;
using UnityEngine;

namespace JebbyJump.Sequence
{
    public class MemoryPhaseController : MonoBehaviour
    {
        [SerializeField] private ColorSequenceManager _sequenceManager;
        [SerializeField] private PlayerLandingDetector _landingDetector;
        [SerializeField] private SequenceDisplayUI _displayUI;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlatformSpawner _spawner;
        [SerializeField] private LevelProgressTracker _progressTracker;
        [SerializeField] private LevelSessionController _levelSession;
        [SerializeField] private GameFeedbackUI _feedbackUI;
        [SerializeField] private RocketBootsEffect _rocketBoots;
        [SerializeField] private RocketBootsPickup _rocketBootsPickup;

        public event Action LevelCompleted;
        public event Action CorrectLanding;
        public event Action WrongLanding;
        public event Action MemoryPhaseStarted;

        private enum Phase { ShowingSequence, Playing, Completed }
        private Phase _phase;
        private Vector3 _spawnPosition;

        private void Awake()
        {
            if (_sequenceManager == null) Debug.LogError("[MemoryPhaseController] SequenceManager not assigned.", this);
            if (_landingDetector == null) Debug.LogError("[MemoryPhaseController] LandingDetector not assigned.", this);
            if (_displayUI == null) Debug.LogError("[MemoryPhaseController] DisplayUI not assigned.", this);
            if (_playerController == null) Debug.LogError("[MemoryPhaseController] PlayerController not assigned.", this);
            if (_spawner == null) Debug.LogError("[MemoryPhaseController] PlatformSpawner not assigned.", this);
            if (_progressTracker == null) Debug.LogError("[MemoryPhaseController] LevelProgressTracker not assigned.", this);
        }

        private void OnEnable()
        {
            if (_landingDetector != null) _landingDetector.LandedOnPlatform += OnLanded;
            if (_sequenceManager != null) _sequenceManager.SequenceComplete += OnSequenceComplete;
            if (_progressTracker != null)
            {
                _progressTracker.LifeLost += OnLifeLost;
                _progressTracker.GameOver += OnGameOver;
            }
            if (_spawner != null) _spawner.CactusHit += OnCactusHit;
        }

        private void OnDisable()
        {
            if (_landingDetector != null) _landingDetector.LandedOnPlatform -= OnLanded;
            if (_sequenceManager != null) _sequenceManager.SequenceComplete -= OnSequenceComplete;
            if (_progressTracker != null)
            {
                _progressTracker.LifeLost -= OnLifeLost;
                _progressTracker.GameOver -= OnGameOver;
            }
            if (_spawner != null) _spawner.CactusHit -= OnCactusHit;
        }

        private void Start()
        {
            if (_sequenceManager == null || _spawner == null || _displayUI == null) return;

            _spawnPosition = _playerController != null ? _playerController.transform.position : Vector3.zero;
            ApplySessionConfig();
            _progressTracker?.Initialize(_sequenceManager.Config.StartingLives);

            _sequenceManager.GenerateSequence();

            if (_sequenceManager.Sequence == null || _sequenceManager.Sequence.Count == 0) return;

            _spawner.SpawnPlatforms(_sequenceManager.Sequence);
            StartCoroutine(RunMemoryPhase());
        }

        private IEnumerator RunMemoryPhase()
        {
            _phase = Phase.ShowingSequence;
            _playerController?.SetJumpMultiplier(_sequenceManager.Config.MemoryPhaseJumpMultiplier);
            _displayUI.Show(_sequenceManager.Sequence);
            MemoryPhaseStarted?.Invoke();
            _feedbackUI?.ShowMessage("Remember the colors!", _sequenceManager.Config.MemoryTimeSeconds);
            yield return new WaitForSeconds(_sequenceManager.Config.MemoryTimeSeconds);
            _displayUI.Hide();
            _feedbackUI?.ShowMessage("Go!", 1f);
            _phase = Phase.Playing;
            _playerController?.SetJumpMultiplier(1f);
            Debug.Log("[MemoryPhaseController] Memory phase ended. Playing.");
        }

        private void OnLanded(Platform platform)
        {
            if (_phase != Phase.Playing) return;
            if (_sequenceManager.IsComplete) return;

            if (platform.RowIndex < _sequenceManager.CurrentStepIndex)
            {
                // Already-completed row — player still standing on it, ignore.
                return;
            }

            if (platform.RowIndex > _sequenceManager.CurrentStepIndex)
            {
                // Jumped ahead of the expected row — treat as wrong landing.
                Debug.Log("[Sequence] Skipped to Row " + platform.RowIndex + " — expected Row " + _sequenceManager.CurrentStepIndex + ". Wrong.");
                _feedbackUI?.ShowMessage("Wrong color!", 0.9f);
                WrongLanding?.Invoke();
                _progressTracker?.LoseLife();
                return;
            }

            bool correct = platform.Color == _sequenceManager.ExpectedColor;
            if (correct)
            {
                Debug.Log("[Sequence] Step " + (_sequenceManager.CurrentStepIndex + 1) + "/" + _sequenceManager.Sequence.Count + " — Correct: " + platform.Color);
                _feedbackUI?.ShowMessage("Correct!", 0.7f);
                _progressTracker?.AddScore(10);
                CorrectLanding?.Invoke();
                _sequenceManager.AdvanceStep();
            }
            else
            {
                Debug.Log("[Sequence] Step " + (_sequenceManager.CurrentStepIndex + 1) + "/" + _sequenceManager.Sequence.Count + " — Wrong: got " + platform.Color + ", expected " + _sequenceManager.ExpectedColor);
                _feedbackUI?.ShowMessage("Wrong color!", 0.9f);
                WrongLanding?.Invoke();
                _progressTracker?.LoseLife();
            }
        }

        private void OnCactusHit()
        {
            if (_phase != Phase.Playing) return;
            Debug.Log("[MemoryPhaseController] Cactus hit. Losing life.");
            _feedbackUI?.ShowMessage("Ouch! Cactus!", 0.9f);
            _progressTracker?.LoseLife();
        }

        private void OnLifeLost()
        {
            _rocketBoots?.CancelEffect();   // pickup stays disabled for this attempt
            _phase = Phase.Playing;
            _sequenceManager.ResetProgress();
            _landingDetector?.ResetCurrentPlatform();
            _playerController?.SetJumpMultiplier(1f);
            _playerController?.Respawn(_spawnPosition);
            // Design note: Row 0 must remain reachable from _spawnPosition.
            // Checkpoint/platform regeneration can be revisited in a later phase.
            Debug.Log("[MemoryPhaseController] Life lost. Respawning to start.");
        }

        private void OnGameOver()
        {
            _phase = Phase.Completed;
            Debug.Log("[MemoryPhaseController] Game over!");
        }

        private void ApplySessionConfig()
        {
            if (_levelSession == null) return;
            var config = _levelSession.CurrentConfig;
            if (config == null) { Debug.LogError("[MemoryPhaseController] No active LevelConfig in session.", this); return; }
            _sequenceManager.SetConfig(config);
            _spawner.SetConfig(config);
        }

        public void StartNextLevel()
        {
            if (_levelSession == null || _levelSession.IsFinalLevel) return;
            _levelSession.AdvanceToNextLevel();
            RestartLevel();
        }

        public void RestartLevel()
        {
            if (_sequenceManager == null || _spawner == null || _progressTracker == null) return;

            _rocketBoots?.CancelEffect();
            _rocketBootsPickup?.ResetPickup();  // re-enable pickup on retry / next level
            StopAllCoroutines();
            _phase = Phase.ShowingSequence;

            ApplySessionConfig();
            _progressTracker.Initialize(_sequenceManager.Config.StartingLives);
            _sequenceManager.GenerateSequence();

            if (_sequenceManager.Sequence == null || _sequenceManager.Sequence.Count == 0) return;

            _spawner.SpawnPlatforms(_sequenceManager.Sequence);
            _landingDetector?.ResetCurrentPlatform();
            _playerController?.SetJumpMultiplier(1f);
            _playerController?.Respawn(_spawnPosition);

            StartCoroutine(RunMemoryPhase());
            Debug.Log("[MemoryPhaseController] Level restarted.");
        }

        private void OnSequenceComplete()
        {
            _phase = Phase.Completed;
            int bonus = 50 + (_progressTracker != null ? _progressTracker.Lives * 20 : 0);
            _progressTracker?.AddScore(bonus);
            Debug.Log("[MemoryPhaseController] Level complete! Bonus: " + bonus);
            LevelCompleted?.Invoke();
        }
    }
}
