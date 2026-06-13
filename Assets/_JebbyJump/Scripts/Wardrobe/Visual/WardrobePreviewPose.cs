using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // UI-only pose identity for the in-panel preview carousel. Deliberately
    // NOT tied to gameplay Animator state names/hashes - this is presentation
    // data for the wardrobe panel only.
    public enum WardrobePreviewPose
    {
        Idle = 0,
        Run = 1,
        Jump = 2,
        Fall = 3,
        Land = 4,
        Hurt = 5,
        Victory = 6,
    }

    // One frame of the UI preview carousel: which pose, its sprite, and how
    // long to show it (UI presentation seconds, not gameplay timing).
    public readonly struct WardrobePreviewFrame
    {
        public readonly WardrobePreviewPose Pose;
        public readonly Sprite Sprite;
        public readonly float DurationSeconds;

        public WardrobePreviewFrame(
            WardrobePreviewPose pose, Sprite sprite, float durationSeconds)
        {
            Pose = pose;
            Sprite = sprite;
            DurationSeconds = durationSeconds;
        }
    }
}
