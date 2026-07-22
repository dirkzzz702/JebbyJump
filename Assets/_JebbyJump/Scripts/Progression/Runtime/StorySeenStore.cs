using UnityEngine;

namespace JebbyJump.Story
{
    // Local PlayerPrefs record of which story cards the player has already
    // seen (WorldExpansion100, phase P34F). First auto-view of a card marks it
    // seen so it does not auto-show again; cards remain replayable from the UI
    // regardless of this flag (replay never reads or writes it). Mirrors the
    // WardrobeUnlockAcknowledgementStore pattern; never affects progression.
    //
    // Per-card key: jebby.story.seen.<cardId>
    public static class StorySeenStore
    {
        private const string KeyPrefix = "jebby.story.seen.";

        public static bool IsSeen(string cardId)
        {
            if (StoryCardCatalog.GetById(cardId) == null) return false; // unknown
            return PlayerPrefs.GetInt(KeyPrefix + cardId, 0) == 1;
        }

        // Ignores null / empty / unknown ids.
        public static void MarkSeen(string cardId)
        {
            if (StoryCardCatalog.GetById(cardId) == null) return;
            PlayerPrefs.SetInt(KeyPrefix + cardId, 1);
            PlayerPrefs.Save();
        }

        public static void Clear(string cardId)
        {
            if (string.IsNullOrEmpty(cardId)) return;
            PlayerPrefs.DeleteKey(KeyPrefix + cardId);
            PlayerPrefs.Save();
        }

        // Clears every per-card seen key (catalog-driven).
        public static void ResetAll()
        {
            foreach (var c in StoryCardCatalog.All)
                PlayerPrefs.DeleteKey(KeyPrefix + c.Id);
            PlayerPrefs.Save();
        }
    }
}
