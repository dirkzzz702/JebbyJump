using System;
using System.Collections.Generic;

namespace JebbyJump.Story
{
    // Pure sequential presenter for a run of story cards (WorldExpansion100,
    // phase P34F). The engine/UI layer supplies the cards to show and a
    // markSeen delegate (StorySeenStore.MarkSeen); this stays fully
    // unit-testable with no engine or scene. Mirrors WardrobeCeremonyPresenter.
    //
    // A card is marked seen ONLY when it is advanced past (Continue) or when
    // the run is skipped (SkipAll) - never on construction or render. Replaying
    // cards from the UI should pass a NO-OP markSeen so replay never rewrites
    // flags.
    public sealed class StoryCardQueue
    {
        private readonly IReadOnlyList<StoryCard> _cards;
        private readonly Action<string> _markSeen;
        private int _index;

        public StoryCardQueue(IReadOnlyList<StoryCard> cards, Action<string> markSeen)
        {
            _cards = cards ?? Array.Empty<StoryCard>();
            _markSeen = markSeen;
            _index = 0;
        }

        public bool IsActive => _index < _cards.Count;
        public StoryCard Current => IsActive ? _cards[_index] : null;
        public int Count => _cards.Count;
        public int Position => IsActive ? _index + 1 : _cards.Count; // 1-based

        // Mark the current card seen and advance. Returns whether a card
        // remains active afterwards.
        public bool Continue()
        {
            if (!IsActive) return false;
            _markSeen?.Invoke(Current.Id);
            _index++;
            return IsActive;
        }

        // Skip the rest of the run: mark every remaining card seen and end.
        public void SkipAll()
        {
            while (IsActive)
            {
                _markSeen?.Invoke(_cards[_index].Id);
                _index++;
            }
        }

        // Build a queue of only the not-yet-seen cards from a candidate list,
        // in order (the common auto-show case). Cards already seen are skipped.
        public static StoryCardQueue Unseen(
            IReadOnlyList<StoryCard> candidates,
            Func<string, bool> isSeen, Action<string> markSeen)
        {
            var pending = new List<StoryCard>();
            if (candidates != null)
                foreach (var c in candidates)
                    if (c != null && (isSeen == null || !isSeen(c.Id)))
                        pending.Add(c);
            return new StoryCardQueue(pending, markSeen);
        }
    }
}
