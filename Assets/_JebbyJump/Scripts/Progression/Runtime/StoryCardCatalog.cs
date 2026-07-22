using System.Collections.Generic;

namespace JebbyJump.Story
{
    // The fixed set of 12 story cards (WorldExpansion100, phase P34F): one
    // opening, one before each of the ten worlds, and a World-10 ending. Copy
    // is taken verbatim from design doc 06 (lightweight "magical travel
    // postcard" tone, each body <= 30 words). Ids are stable snake/dot wire
    // keys used by StorySeenStore.
    public static class StoryCardCatalog
    {
        public const string OpeningId = "story.open";
        public const string EndingId = "story.ending";

        private static readonly StoryCard[] _cards =
        {
            new StoryCard(OpeningId, StoryCardKind.Opening, 0,
                "A Rainbow Calls",
                "Far away, a Rainbow Tower shines. Jebby packs a tiny bag and "
                + "hops into the sky to find it. Ready to jump?"),

            new StoryCard("story.W01", StoryCardKind.World, 1,
                "Cloud Meadow",
                "Soft clouds drift like pillows. Somewhere past them, a glimmer "
                + "waits. Hop carefully - remember the colours!"),
            new StoryCard("story.W02", StoryCardKind.World, 2,
                "Whispering Woods",
                "The forest hums with light. The tower peeks through the leaves. "
                + "Follow the glow."),
            new StoryCard("story.W03", StoryCardKind.World, 3,
                "Singing Crystals",
                "Caves sparkle and chime. A bright shaft points the way down and "
                + "onward."),
            new StoryCard("story.W04", StoryCardKind.World, 4,
                "Golden Dunes",
                "Warm sand, friendly sun. Over the next dune, the tower rises "
                + "taller."),
            new StoryCard("story.W05", StoryCardKind.World, 5,
                "Where Sea Floats Up",
                "Reefs drift in the sky. The tower stands on a far floating "
                + "island."),
            new StoryCard("story.W06", StoryCardKind.World, 6,
                "Candy Kingdom",
                "Everything smells sweet! The tower glows candy-bright ahead."),
            new StoryCard("story.W07", StoryCardKind.World, 7,
                "Clockwork Heights",
                "Gears turn and steam puffs. The tower ticks, so close now."),
            new StoryCard("story.W08", StoryCardKind.World, 8,
                "Moonlit Dreams",
                "Stars twinkle softly. Under the moon, the tower shines near."),
            new StoryCard("story.W09", StoryCardKind.World, 9,
                "Emberpeaks",
                "Warm embers glow. The tower stands bold against the firelit "
                + "sky."),
            new StoryCard("story.W10", StoryCardKind.World, 10,
                "The Rainbow Tower",
                "You made it to the tower itself! One last radiant climb to the "
                + "top."),

            new StoryCard(EndingId, StoryCardKind.Ending, 0,
                "Home of the Rainbow",
                "Jebby reaches the top - the whole sky lights up! Every colour, "
                + "every jump, brought you here."),
        };

        public static IReadOnlyList<StoryCard> All => _cards;

        public static StoryCard GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            foreach (var c in _cards)
                if (c.Id == id) return c;
            return null;
        }

        public static StoryCard Opening => GetById(OpeningId);
        public static StoryCard Ending => GetById(EndingId);

        // The "before world N" card (1-based). Null when out of range.
        public static StoryCard ForWorld(int worldNumber)
        {
            foreach (var c in _cards)
                if (c.Kind == StoryCardKind.World && c.WorldNumber == worldNumber)
                    return c;
            return null;
        }
    }
}
