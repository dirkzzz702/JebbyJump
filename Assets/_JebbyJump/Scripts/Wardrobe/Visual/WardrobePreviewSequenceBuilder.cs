using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure builder for the in-panel preview carousel. Emits frames in the
    // approved child-friendly order, pulling UI sprites from the (extended)
    // WardrobePreviewLibrary. UI-only; no catalog-count assumptions; no
    // gameplay/Animator dependency. Durations are UI presentation placeholders.
    public static class WardrobePreviewSequenceBuilder
    {
        // Canonical order + UI durations (seconds). Hurt is only included when
        // explicitly requested (excluded from the default showcase loop).
        private static readonly (WardrobePreviewPose pose, float duration)[] Order =
        {
            (WardrobePreviewPose.Idle, 1.2f),
            (WardrobePreviewPose.Run, 0.7f),
            (WardrobePreviewPose.Jump, 0.7f),
            (WardrobePreviewPose.Fall, 0.7f),
            (WardrobePreviewPose.Land, 0.5f),
            (WardrobePreviewPose.Hurt, 0.7f),
            (WardrobePreviewPose.Victory, 1.0f),
        };

        // Reduce-motion aware: when reduceMotion is true, returns a single
        // static Idle frame (no pose cycling, no decorative motion) so the
        // accessibility setting freezes the carousel. Otherwise behaves exactly
        // like the 3-arg Build.
        public static IReadOnlyList<WardrobePreviewFrame> Build(
            string outfitId, WardrobePreviewLibrary library, bool includeHurt,
            bool reduceMotion)
        {
            if (!reduceMotion) return Build(outfitId, library, includeHurt);

            var idle = new List<WardrobePreviewFrame>(1);
            if (library != null
                && library.TryGetPose(
                    outfitId, WardrobePreviewPose.Idle, out Sprite sprite)
                && sprite != null)
            {
                idle.Add(new WardrobePreviewFrame(
                    WardrobePreviewPose.Idle, sprite, 1f));
            }
            return idle;
        }

        public static IReadOnlyList<WardrobePreviewFrame> Build(
            string outfitId, WardrobePreviewLibrary library, bool includeHurt)
        {
            var frames = new List<WardrobePreviewFrame>();
            if (library == null) return frames;

            foreach (var (pose, duration) in Order)
            {
                if (pose == WardrobePreviewPose.Hurt && !includeHurt) continue;
                if (library.TryGetPose(outfitId, pose, out Sprite sprite)
                    && sprite != null)
                {
                    frames.Add(new WardrobePreviewFrame(pose, sprite, duration));
                }
            }
            return frames;
        }
    }
}
