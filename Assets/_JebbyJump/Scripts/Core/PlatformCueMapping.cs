namespace JebbyJump.Core
{
    // Accessibility (P22): an explicit, stable non-color cue token for each
    // platform color, shared by BOTH the memory-phase swatches (presentation)
    // and the spawned platforms (response) so they always show the same cue.
    //
    // Numbers are used deliberately: guaranteed-legible in any TMP font and
    // requiring no new art. The mapping is an explicit switch (NOT (int)color
    // ordinal arithmetic) so reordering the enum cannot silently shift cues.
    // None has no cue. Cues are only rendered when the Memory Cues setting is
    // ON; they never affect sequence order, timing, colors, or validation.
    public static class PlatformCueMapping
    {
        public static string CueFor(PlatformColor color) => color switch
        {
            PlatformColor.Red    => "1",
            PlatformColor.Blue   => "2",
            PlatformColor.Green  => "3",
            PlatformColor.Yellow => "4",
            PlatformColor.Purple => "5",
            PlatformColor.Orange => "6",
            _                    => "",
        };

        public static bool HasCue(PlatformColor color) => color != PlatformColor.None;
    }
}
