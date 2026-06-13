using System.Collections.Generic;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure timing state for the in-panel preview carousel. Engine-free and
    // unit-testable: the panel feeds it frames + unscaled delta time and reads
    // the current frame. Safe for empty sequences, zero/negative deltas, and
    // large deltas (advances across multiple frames in one Tick).
    public sealed class WardrobePreviewPlayer
    {
        private static readonly IReadOnlyList<WardrobePreviewFrame> Empty =
            System.Array.Empty<WardrobePreviewFrame>();

        private IReadOnlyList<WardrobePreviewFrame> _frames = Empty;
        private int _index;
        private float _elapsed;

        public bool HasFrames => _frames.Count > 0;
        public int Index => _index;
        public WardrobePreviewFrame Current =>
            HasFrames ? _frames[_index] : default;

        // Replaces the sequence and restarts from the first frame.
        public void SetFrames(IReadOnlyList<WardrobePreviewFrame> frames)
        {
            _frames = frames ?? Empty;
            _index = 0;
            _elapsed = 0f;
        }

        public void Clear() => SetFrames(null);

        // Advances by unscaled delta time. Consumes as many frames as the
        // delta covers (large-spike safe); guards against non-positive
        // durations to avoid an infinite loop.
        public void Tick(float unscaledDeltaTime)
        {
            if (!HasFrames || unscaledDeltaTime <= 0f) return;

            _elapsed += unscaledDeltaTime;
            int guard = 0;
            while (HasFrames && guard++ < 1024)
            {
                float duration = _frames[_index].DurationSeconds;
                if (duration <= 0f) { _elapsed = 0f; break; }
                if (_elapsed < duration) break;
                _elapsed -= duration;
                _index = (_index + 1) % _frames.Count;
            }
        }
    }
}
