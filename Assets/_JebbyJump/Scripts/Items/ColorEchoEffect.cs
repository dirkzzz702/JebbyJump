using JebbyJump.Sequence;
using JebbyJump.UI;
using UnityEngine;

namespace JebbyJump.Items
{
    // Instant peek skill — flashes the next expected color via short text feedback.
    // Does not advance the sequence, does not pause time, does not reveal more than
    // the immediate next color.
    public class ColorEchoEffect : ActiveSkillEffect
    {
        [SerializeField] private ColorSequenceManager _sequenceManager;
        [SerializeField] private GameFeedbackUI _feedbackUI;
        [SerializeField] private float _displayDuration = 1.8f;

        public override bool IsActive => false; // no lingering state

        public override void Activate()
        {
            if (_sequenceManager == null || _sequenceManager.IsComplete)
            {
                _feedbackUI?.ShowMessage("No next color", 0.8f);
                return;
            }
            var next = _sequenceManager.ExpectedColor;
            _feedbackUI?.ShowMessage($"Next: {next}", _displayDuration);
            Debug.Log($"[ColorEcho] Revealed next color: {next}");
        }

        public override void CancelEffect() { /* nothing to cancel — instant effect */ }
    }
}
