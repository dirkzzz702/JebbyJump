using System.Collections.Generic;
using JebbyJump.Story;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    // P34F story-card foundation: catalog integrity, the pure queue presenter,
    // and the seen-store persistence. All pure logic (StorySeenStore uses
    // PlayerPrefs, which is available in EditMode) - the overlay UI + scene
    // triggers are a separate engine-layer increment.
    public class StoryCardTests
    {
        // ---- catalog ----

        [Test]
        public void Catalog_HasOpening_TenWorlds_AndEnding()
        {
            var all = StoryCardCatalog.All;
            Assert.AreEqual(12, all.Count);
            int opening = 0, world = 0, ending = 0;
            foreach (var c in all)
            {
                if (c.Kind == StoryCardKind.Opening) opening++;
                else if (c.Kind == StoryCardKind.World) world++;
                else if (c.Kind == StoryCardKind.Ending) ending++;
            }
            Assert.AreEqual(1, opening, "one opening card");
            Assert.AreEqual(10, world, "ten world cards");
            Assert.AreEqual(1, ending, "one ending card");
        }

        [Test]
        public void Catalog_WorldCards_CoverWorlds1To10_Once()
        {
            var seen = new HashSet<int>();
            for (int n = 1; n <= 10; n++)
            {
                var card = StoryCardCatalog.ForWorld(n);
                Assert.IsNotNull(card, "missing card for world " + n);
                Assert.AreEqual(StoryCardKind.World, card.Kind);
                Assert.AreEqual(n, card.WorldNumber);
                Assert.IsTrue(seen.Add(n));
            }
            Assert.IsNull(StoryCardCatalog.ForWorld(0));
            Assert.IsNull(StoryCardCatalog.ForWorld(11));
        }

        [Test]
        public void Catalog_IdsUnique_HeadlinesPresent_BodiesWithinThirtyWords()
        {
            var ids = new HashSet<string>();
            foreach (var c in StoryCardCatalog.All)
            {
                Assert.IsFalse(string.IsNullOrEmpty(c.Id), "empty id");
                Assert.IsTrue(ids.Add(c.Id), "duplicate id " + c.Id);
                Assert.IsFalse(string.IsNullOrWhiteSpace(c.Headline), c.Id + " headline");
                Assert.IsFalse(string.IsNullOrWhiteSpace(c.Body), c.Id + " body");
                int words = c.Body.Split(
                    new[] { ' ', '\n', '\t' },
                    System.StringSplitOptions.RemoveEmptyEntries).Length;
                Assert.LessOrEqual(words, 30, c.Id + " body exceeds 30 words (" + words + ")");
            }
        }

        [Test]
        public void Catalog_GetById_And_OpeningEndingAccessors()
        {
            Assert.AreEqual(StoryCardCatalog.OpeningId, StoryCardCatalog.Opening.Id);
            Assert.AreEqual(StoryCardCatalog.EndingId, StoryCardCatalog.Ending.Id);
            Assert.IsNull(StoryCardCatalog.GetById("nope"));
            Assert.IsNull(StoryCardCatalog.GetById(null));
        }

        // ---- queue presenter ----

        [Test]
        public void Queue_Continue_MarksSeenInOrder_ThenEnds()
        {
            var cards = new List<StoryCard>
            {
                StoryCardCatalog.ForWorld(1), StoryCardCatalog.ForWorld(2),
            };
            var marked = new List<string>();
            var q = new StoryCardQueue(cards, marked.Add);

            Assert.IsTrue(q.IsActive);
            Assert.AreEqual(cards[0].Id, q.Current.Id);
            Assert.AreEqual(1, q.Position);

            Assert.IsTrue(q.Continue());              // still active (card 2)
            Assert.AreEqual(cards[1].Id, q.Current.Id);
            Assert.IsFalse(q.Continue());             // past the last -> inactive
            Assert.IsFalse(q.IsActive);
            Assert.IsNull(q.Current);

            CollectionAssert.AreEqual(
                new[] { cards[0].Id, cards[1].Id }, marked);
        }

        [Test]
        public void Queue_SkipAll_MarksEveryRemainingCardSeen()
        {
            var cards = new List<StoryCard>(StoryCardCatalog.All);
            var marked = new HashSet<string>();
            var q = new StoryCardQueue(cards, id => marked.Add(id));

            q.Continue();          // sees the first
            q.SkipAll();           // skips the other 11

            Assert.IsFalse(q.IsActive);
            Assert.AreEqual(12, marked.Count, "skip must mark all cards seen");
        }

        [Test]
        public void Queue_Unseen_FiltersOutAlreadySeen()
        {
            var candidates = new List<StoryCard>
            {
                StoryCardCatalog.ForWorld(1),
                StoryCardCatalog.ForWorld(2),
                StoryCardCatalog.ForWorld(3),
            };
            var seen = new HashSet<string> { StoryCardCatalog.ForWorld(2).Id };
            var q = StoryCardQueue.Unseen(candidates, seen.Contains, null);

            Assert.AreEqual(2, q.Count);
            Assert.AreEqual(StoryCardCatalog.ForWorld(1).Id, q.Current.Id);
            q.Continue();
            Assert.AreEqual(StoryCardCatalog.ForWorld(3).Id, q.Current.Id);
        }

        [Test]
        public void Queue_EmptyOrNull_IsInactive()
        {
            Assert.IsFalse(new StoryCardQueue(null, null).IsActive);
            Assert.IsFalse(new StoryCardQueue(new List<StoryCard>(), null).IsActive);
        }

        // ---- seen store (PlayerPrefs) ----

        private readonly List<string> _touched = new List<string>();

        [TearDown]
        public void TearDown()
        {
            foreach (var id in _touched) StorySeenStore.Clear(id);
            _touched.Clear();
        }

        [Test]
        public void SeenStore_MarkThenIsSeen_Idempotent()
        {
            string id = StoryCardCatalog.OpeningId;
            _touched.Add(id);
            StorySeenStore.Clear(id);
            Assert.IsFalse(StorySeenStore.IsSeen(id));

            StorySeenStore.MarkSeen(id);
            Assert.IsTrue(StorySeenStore.IsSeen(id));
            StorySeenStore.MarkSeen(id);               // idempotent
            Assert.IsTrue(StorySeenStore.IsSeen(id));

            StorySeenStore.Clear(id);
            Assert.IsFalse(StorySeenStore.IsSeen(id));
        }

        [Test]
        public void SeenStore_UnknownId_IgnoredOnReadAndWrite()
        {
            Assert.IsFalse(StorySeenStore.IsSeen("story.nope"));
            Assert.DoesNotThrow(() => StorySeenStore.MarkSeen("story.nope"));
            Assert.IsFalse(StorySeenStore.IsSeen("story.nope"));
            Assert.IsFalse(StorySeenStore.IsSeen(null));
        }
    }
}
