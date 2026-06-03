using System;
using System.Collections;
using JebbyJump.Analytics;
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
        [SerializeField] private ActiveSkillController[] _activeSkills;
        [SerializeField] private BubbleShieldEffect _bubbleShield;
        [SerializeField] private PlayerAnimator _playerAnimator;
        [SerializeField] private LevelTimer _levelTimer;

        public event Action LevelCompleted;
        public event Action CorrectLanding;
        public event Action WrongLanding;
        public event Action MemoryPhaseStarted;

        private enum Phase { ShowingSequence, Playing, Completed }
        private Phase _phase;
        private Vector3 _spawnPosition;

        // Analytics-only label for how the current attempt began
        // ("continue"/"level_select"/"default" for the first run,
        // "retry"/"next_level" for restarts). Set before each
        // RunMemoryPhase. Does not affect gameplay.
        private string _attemptSource = "default";

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

        private IEnumerator Start()
        {
            if (_sequenceManager == null || _spawner == null || _displayUI == null) yield break;

            yield return new WaitForFixedUpdate();
            _spawnPosition = _playerController != null ? _playerController.transform.position : Vector3.zero;

            ApplySessionConfig();
            _progressTracker?.Initialize(_sequenceManager.Config.StartingLives);

            _sequenceManager.GenerateSequence();
            if (_sequenceManager.Sequence == null || _sequenceManager.Sequence.Count == 0) yield break;

            _spawner.SpawnPlatforms(_sequenceManager.Sequence);
            // First run of this scene: source came from the Main Menu
            // (continue / level_select) or defaults.
            _attemptSource = _levelSession != null
                ? _levelSession.LaunchSource
                : "default";
            StartCoroutine(RunMemoryPhase());
        }

        private IEnumerator RunMemoryPhase()
        {
            _phase = Phase.ShowingSequence;

            int levelIndex = _levelSession != null
                ? _levelSession.CurrentLevelIndex : 0;
            int sequenceLength = _sequenceManager.Sequence != null
                ? _sequenceManager.Sequence.Count : 0;
            AnalyticsService.Track("level_started",
                AnalyticsParam.Of("level_index", levelIndex),
                AnalyticsParam.Of("level_number", levelIndex + 1),
                AnalyticsParam.Of("source", _attemptSource ?? "default"));
            AnalyticsService.Track("memory_phase_started",
                AnalyticsParam.Of("level_index", levelIndex),
                AnalyticsParam.Of("level_number", levelIndex + 1),
                AnalyticsParam.Of("sequence_length", sequenceLength));

            SetAllSkillsUsable(false);
            _playerController?.SetJumpMultiplier(_sequenceManager.Config.MemoryPhaseJumpMultiplier);
            _displayUI.Show(_sequenceManager.Sequence);
            MemoryPhaseStarted?.Invoke();
            yield return new WaitForSeconds(_sequenceManager.Config.MemoryTimeSeconds);
            _displayUI.Hide();
            _feedbackUI?.ShowMessage("Go!", 1f);
            _phase = Phase.Playing;
            SetAllSkillsUsable(true);
            _playerController?.SetJumpMultiplier(1f);
            _levelTimer?.StartTimer();
            AnalyticsService.Track("gameplay_started",
                AnalyticsParam.Of("level_index", levelIndex),
                AnalyticsParam.Of("level_number", levelIndex + 1));
            Debug.Log("[MemoryPhaseController] Memory phase ended. Playing.");
        }

        private void OnLanded(Platform platform)
        {
            if (_phase != Phase.Playing) return;
            if (_sequenceManager.IsComplete) return;

            if (platform.RowIndex < _sequenceManager.CurrentStepIndex)
                return; // already-completed row, ignore

            if (platform.RowIndex > _sequenceManager.CurrentStepIndex)
            {
                if (TryShieldAbsorb()) { RespawnAfterShield(); return; }
                Debug.Log("[Sequence] Skipped to Row " + platform.RowIndex + " — expected Row " + _sequenceManager.CurrentStepIndex + ". Wrong.");
                _feedbackUI?.ShowMessage("Wrong color!", 0.9f);
                WrongLanding?.Invoke();
                EmitPlayerDamaged("wrong_color");
                _progressTracker?.LoseLife();
                return;
            }

            bool correct = platform.Color == _sequenceManager.ExpectedColor;
            if (correct)
            {
                _feedbackUI?.ShowMessage("Correct!", 0.7f);
                CorrectLanding?.Invoke();
                _sequenceManager.AdvanceStep();
            }
            else
            {
                if (TryShieldAbsorb()) { RespawnAfterShield(); return; }
                _feedbackUI?.ShowMessage("Wrong color!", 0.9f);
                WrongLanding?.Invoke();
                EmitPlayerDamaged("wrong_color");
                _progressTracker?.LoseLife();
            }
        }

        private void OnCactusHit()
        {
            if (_phase != Phase.Playing) return;
            if (TryShieldAbsorb()) { /* no respawn — Jebby keeps position after cactus block */ return; }
            _feedbackUI?.ShowMessage("Ouch! Cactus!", 0.9f);
            EmitPlayerDamaged("hazard");
            _progressTracker?.LoseLife();
        }

        // Returns true if a Bubble Shield was active and absorbed the hit.
        private bool TryShieldAbsorb() => _bubbleShield != null && _bubbleShield.TryConsume();

        // After a shield absorbs a wrong-landing hit we respawn Jebby to origin so
        // row progression isn't confused by Jebby standing on the wrong platform.
        private void RespawnAfterShield()
        {
            _landingDetector?.ResetCurrentPlatform();
            _playerController?.SetJumpMultiplier(1f);
            _playerController?.Respawn(_spawnPosition);
        }

        private void OnLifeLost()
        {
            CancelAllSkills();   // cooldowns stay spent
            _playerAnimator?.TriggerHurt();
            _phase = Phase.Playing;
            _sequenceManager.ResetProgress();
            _landingDetector?.ResetCurrentPlatform();
            _playerController?.SetJumpMultiplier(1f);
            _playerController?.Respawn(_spawnPosition);
            Debug.Log("[MemoryPhaseController] Life lost. Respawning to start.");
        }

        private void OnGameOver()
        {
            SetAllSkillsUsable(false);
            _levelTimer?.StopTimer();
            _phase = Phase.Completed;
            int levelIndex = _levelSession != null
                ? _levelSession.CurrentLevelIndex : 0;
            AnalyticsService.Track("level_failed",
                AnalyticsParam.Of("level_index", levelIndex),
                AnalyticsParam.Of("level_number", levelIndex + 1),
                AnalyticsParam.Of("reason", "lives_depleted"));
            Debug.Log("[MemoryPhaseController] Game over!");
        }

        // Emits player_damaged just before a life is deducted, so
        // remaining_lives reflects the post-hit count. source is known at
        // the call site (wrong_color / hazard).
        private void EmitPlayerDamaged(string source)
        {
            int levelIndex = _levelSession != null
                ? _levelSession.CurrentLevelIndex : 0;
            int remaining = _progressTracker != null
                ? Mathf.Max(0, _progressTracker.Lives - 1) : 0;
            AnalyticsService.Track("player_damaged",
                AnalyticsParam.Of("level_index", levelIndex),
                AnalyticsParam.Of("level_number", levelIndex + 1),
                AnalyticsParam.Of("remaining_lives", remaining),
                AnalyticsParam.Of("source", source));
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
            RestartLevel("next_level");
        }

        // source labels the attempt for analytics ("retry" by default,
        // "next_level" from StartNextLevel). No effect on restart behavior.
        public void RestartLevel(string source = "retry")
        {
            if (_sequenceManager == null || _spawner == null || _progressTracker == null) return;

            _attemptSource = source;
            ResetAllSkillsForLevel();
            _playerAnimator?.ResetToIdle();
            _levelTimer?.ResetTimer();
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
            SetAllSkillsUsable(false);
            _levelTimer?.StopTimer();
            _playerAnimator?.TriggerVictory();
            _phase = Phase.Completed;
            Debug.Log("[MemoryPhaseController] Level complete!");
            LevelCompleted?.Invoke();
        }

        // ── skill helpers ────────────────────────────────────────────────────
        private void SetAllSkillsUsable(bool usable)
        {
            if (_activeSkills == null) return;
            foreach (var s in _activeSkills) s?.SetCanUseSkill(usable);
        }

        private void CancelAllSkills()
        {
            if (_activeSkills == null) return;
            foreach (var s in _activeSkills) s?.CancelActiveSkill();
        }

        private void ResetAllSkillsForLevel()
        {
            if (_activeSkills == null) return;
            foreach (var s in _activeSkills) s?.ResetForLevel();
        }
    }
}
