using System.Collections;
using JebbyJump.Platforms;
using JebbyJump.Player;
using UnityEngine;

namespace JebbyJump.Sequence
{
    public class MemoryPhaseController : MonoBehaviour
    {
        [SerializeField] private ColorSequenceManager _sequenceManager;
        [SerializeField] private PlayerLandingDetector _landingDetector;
        [SerializeField] private SequenceDisplayUI _displayUI;
        [SerializeField] private PlayerController _playerController;

        private enum Phase { ShowingSequence, Playing, Completed }
        private Phase _phase;

        private void Awake()
        {
            if (_sequenceManager == null) Debug.LogError("[MemoryPhaseController] SequenceManager not assigned.", this);
            if (_landingDetector == null) Debug.LogError("[MemoryPhaseController] LandingDetector not assigned.", this);
            if (_displayUI == null) Debug.LogError("[MemoryPhaseController] DisplayUI not assigned.", this);
            if (_playerController == null) Debug.LogError("[MemoryPhaseController] PlayerController not assigned.", this);
        }

        private void OnEnable()
        {
            if (_landingDetector != null) _landingDetector.LandedOnPlatform += OnLanded;
            if (_sequenceManager != null) _sequenceManager.SequenceComplete += OnSequenceComplete;
        }

        private void OnDisable()
        {
            if (_landingDetector != null) _landingDetector.LandedOnPlatform -= OnLanded;
            if (_sequenceManager != null) _sequenceManager.SequenceComplete -= OnSequenceComplete;
        }

        private void Start()
        {
            _sequenceManager.GenerateSequence();
            StartCoroutine(RunMemoryPhase());
        }

        private IEnumerator RunMemoryPhase()
        {
            _phase = Phase.ShowingSequence;
            _playerController?.SetJumpMultiplier(_sequenceManager.Config.MemoryPhaseJumpMultiplier);
            _displayUI.Show(_sequenceManager.Sequence);
            yield return new WaitForSeconds(_sequenceManager.Config.MemoryTimeSeconds);
            _displayUI.Hide();
            _phase = Phase.Playing;
            _playerController?.SetJumpMultiplier(1f);
            Debug.Log("[MemoryPhaseController] Memory phase ended. Playing.");
        }

        private void OnLanded(Platform platform)
        {
            if (_phase != Phase.Playing) return;
            if (_sequenceManager.IsComplete) return;

            // Row gating: only the platform whose row matches the current step index advances the sequence.
            // Color correctness (expected vs got) is NOT validated here — that belongs to Phase 8.
            if (platform.RowIndex != _sequenceManager.CurrentStepIndex)
            {
                Debug.Log($"[Sequence] Ignored Row {platform.RowIndex} — waiting for Row {_sequenceManager.CurrentStepIndex}");
                return;
            }

            Debug.Log($"[Sequence] Step {_sequenceManager.CurrentStepIndex + 1}/{_sequenceManager.Sequence.Count} — Expected: {_sequenceManager.ExpectedColor}, Got: {platform.Color}");
            _sequenceManager.AdvanceStep();
        }

        private void OnSequenceComplete()
        {
            _phase = Phase.Completed;
            Debug.Log("[MemoryPhaseController] Sequence complete!");
        }
    }
}
