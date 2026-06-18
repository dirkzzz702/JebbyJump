using UnityEngine;

namespace JebbyJump.Shell
{
    // Canonical gameplay HUD + mobile-control layout in CanvasScaler reference
    // (1920x1080) units, anchored to the safe-area content box. HUD lives/level/
    // timer (HUDCanvas) and the controls + pause (MobileControlsCanvas) share one
    // SWSS 1920x1080 logical space, so a single layout drives the combined
    // cross-canvas overlap validation (P22 correction #5). Control sizes match
    // the real scene (move 130, jump 140, skill 100, pause 96).
    public static class GameplayLayoutMetrics
    {
        public const float EdgeInset = 24f;

        public const float LivesW = 220f, LivesH = 64f;
        public const float LevelW = 320f, LevelH = 64f;
        public const float TimerW = 220f, TimerH = 64f;
        public const float PauseSize = 96f;
        public const float MoveSize = 130f;
        public const float JumpSize = 140f;
        public const float SkillSize = 100f;
        public const float Gap = 24f;
        public const float SkillGap = 16f;
        public const float TimerPauseGap = 16f;

        public struct Layout
        {
            public Rect Lives, Level, Timer, Pause;
            public Rect MoveLeft, MoveRight, Jump, Skill1, Skill2, Skill3;
        }

        public static Layout Compute(Rect c)
        {
            var l = new Layout
            {
                // top row
                Lives = TopLeft(c, EdgeInset, EdgeInset, LivesW, LivesH),
                Level = TopCenter(c, EdgeInset, LevelW, LevelH),
                Timer = TopRight(c, EdgeInset, EdgeInset, TimerW, TimerH),
                // pause sits below the timer on the right (no overlap by design)
                Pause = TopRight(c, EdgeInset, EdgeInset + TimerH + TimerPauseGap,
                    PauseSize, PauseSize),
                // bottom-left move cluster
                MoveLeft = BottomLeft(c, EdgeInset, EdgeInset, MoveSize, MoveSize),
                MoveRight = BottomLeft(c, EdgeInset + MoveSize + Gap, EdgeInset,
                    MoveSize, MoveSize),
                // bottom-right action cluster (jump rightmost; skills to its left)
                Jump = BottomRight(c, EdgeInset, EdgeInset, JumpSize, JumpSize),
            };
            float sx = EdgeInset + JumpSize + Gap;
            l.Skill1 = BottomRight(c, sx, EdgeInset, SkillSize, SkillSize);
            sx += SkillSize + SkillGap;
            l.Skill2 = BottomRight(c, sx, EdgeInset, SkillSize, SkillSize);
            sx += SkillSize + SkillGap;
            l.Skill3 = BottomRight(c, sx, EdgeInset, SkillSize, SkillSize);
            return l;
        }

        private static Rect TopLeft(Rect c, float ox, float oy, float w, float h)
            => new Rect(c.xMin + ox, c.yMax - oy - h, w, h);
        private static Rect TopRight(Rect c, float ox, float oy, float w, float h)
            => new Rect(c.xMax - ox - w, c.yMax - oy - h, w, h);
        private static Rect TopCenter(Rect c, float oy, float w, float h)
            => new Rect(c.center.x - w / 2f, c.yMax - oy - h, w, h);
        private static Rect BottomLeft(Rect c, float ox, float oy, float w, float h)
            => new Rect(c.xMin + ox, c.yMin + oy, w, h);
        private static Rect BottomRight(Rect c, float ox, float oy, float w, float h)
            => new Rect(c.xMax - ox - w, c.yMin + oy, w, h);
    }
}
