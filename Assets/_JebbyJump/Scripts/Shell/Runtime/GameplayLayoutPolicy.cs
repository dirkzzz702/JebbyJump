using UnityEngine;

namespace JebbyJump.Shell
{
    // Pure validation of the gameplay HUD + control layout against the safe-area
    // content box: every element stays inside the safe area and key controls do
    // not overlap. Drives the combined cross-canvas check (P22 correction #5):
    // PauseButton vs Timer and vs the skill controls, across the landscape
    // safe-area matrix.
    public static class GameplayLayoutPolicy
    {
        private const float Eps = 0.5f;

        public static bool Contains(Rect content, Rect e)
            => e.xMin >= content.xMin - Eps && e.xMax <= content.xMax + Eps
            && e.yMin >= content.yMin - Eps && e.yMax <= content.yMax + Eps;

        public static bool Overlaps(Rect a, Rect b)
            => a.xMin < b.xMax - Eps && a.xMax > b.xMin + Eps
            && a.yMin < b.yMax - Eps && a.yMax > b.yMin + Eps;

        public static bool AllWithin(GameplayLayoutMetrics.Layout l, Rect content)
            => Contains(content, l.Lives) && Contains(content, l.Level)
            && Contains(content, l.Timer) && Contains(content, l.Pause)
            && Contains(content, l.MoveLeft) && Contains(content, l.MoveRight)
            && Contains(content, l.Jump) && Contains(content, l.Skill1)
            && Contains(content, l.Skill2) && Contains(content, l.Skill3);

        // Correction #5: the pause button must clear the timer and all skill
        // controls (and jump) so it can never sit under a control or the timer.
        public static bool PauseClear(GameplayLayoutMetrics.Layout l)
            => !Overlaps(l.Pause, l.Timer)
            && !Overlaps(l.Pause, l.Jump)
            && !Overlaps(l.Pause, l.Skill1)
            && !Overlaps(l.Pause, l.Skill2)
            && !Overlaps(l.Pause, l.Skill3);

        // Top row reads cleanly and the two control clusters never collide.
        public static bool ClustersClear(GameplayLayoutMetrics.Layout l)
            => !Overlaps(l.Lives, l.Level)
            && !Overlaps(l.Level, l.Timer)
            && !Overlaps(l.MoveRight, l.Skill3)
            && !Overlaps(l.MoveRight, l.Jump);
    }
}
