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

        private enum Phase { ShowingSequence, Playing, Completed }
        private Phase _phase;

        private void Awake()
        {
            if (_sequenceManager == null) Debug.LogError("[MemoryPhaseController] SequenceManager not assigned.", this);
            if (_landingDetector == null) Debug.LogError("[MemoryPhaseController] LandingDetector not assigned.", this);
            if (_displayUI == null) Debug.LogError("[MemoryPhaseController] DisplayUI not assigned.", this);
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
            _displayUI.Show(_sequenceManager.Sequence);
            yield return new WaitForSeconds(_sequenceManager.Config.MemoryTimeSeconds);
            _displayUI.Hide();
            _phase = Phase.Playing;
            Debug.Log("[MemoryPhaseController] Memory phase ended. Playing.");
        }

        private void OnLanded(Platform platform)
        {
            if (_phase != Phase.Playing) return;
            if (_sequenceManager.IsComplete) return;
            // Phase 6: advance on every landing for testing. Correct/wrong validation belongs to Phase 8.
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
