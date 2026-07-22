using System.Collections.Generic;
using JebbyJump.Story;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Modal story-card overlay (WorldExpansion100, phase P34F). Drives the pure
    // StoryCardQueue and renders the current card; Continue advances (marking
    // it seen), Skip marks the rest seen and closes. Auto-show helpers only
    // present NOT-yet-seen cards (StorySeenStore); Replay shows regardless with
    // a no-op markSeen so it never rewrites flags. Focus is captured on open and
    // restored on close (mirrors the shell panels).
    public class StoryCardPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _headline;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _skipButton;

        private StoryCardQueue _queue;
        private GameObject _opener;

        public bool IsShowing => _root != null && _root.activeSelf;

        private void Awake()
        {
            if (_continueButton != null) _continueButton.onClick.AddListener(OnContinue);
            if (_skipButton != null) _skipButton.onClick.AddListener(OnSkip);
            if (_root != null) _root.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_continueButton != null) _continueButton.onClick.RemoveListener(OnContinue);
            if (_skipButton != null) _skipButton.onClick.RemoveListener(OnSkip);
        }

        // ---- auto-show (marks seen as it advances) ----

        public void ShowOpeningIfUnseen()
            => ShowUnseen(new[] { StoryCardCatalog.Opening });

        public void ShowWorldIfUnseen(int worldNumber)
        {
            var card = StoryCardCatalog.ForWorld(worldNumber);
            if (card != null) ShowUnseen(new[] { card });
        }

        public void ShowEndingIfUnseen()
            => ShowUnseen(new[] { StoryCardCatalog.Ending });

        // Replay a specific card ignoring the seen flag (never rewrites it).
        public void Replay(StoryCard card)
        {
            if (card == null) return;
            Begin(new StoryCardQueue(new[] { card }, null));
        }

        private void ShowUnseen(IReadOnlyList<StoryCard> cards)
        {
            var q = StoryCardQueue.Unseen(cards, StorySeenStore.IsSeen, StorySeenStore.MarkSeen);
            if (!q.IsActive) return;   // everything already seen -> no-op
            Begin(q);
        }

        private void Begin(StoryCardQueue queue)
        {
            _queue = queue;
            _opener = EventSystem.current != null
                ? EventSystem.current.currentSelectedGameObject : null;
            if (_root != null) _root.SetActive(true);
            Render();
        }

        private void Render()
        {
            if (_queue == null || !_queue.IsActive) { Hide(); return; }
            var card = _queue.Current;
            if (_headline != null) _headline.text = card.Headline;
            if (_body != null) _body.text = card.Body;
            ShellFocusUtil.Select(_continueButton);
        }

        private void OnContinue()
        {
            if (_queue == null) return;
            _queue.Continue();   // marks current seen, advances
            Render();            // renders next, or hides when done
        }

        private void OnSkip()
        {
            if (_queue == null) { Hide(); return; }
            _queue.SkipAll();    // marks all remaining seen
            Hide();
        }

        private void Hide()
        {
            _queue = null;
            if (_root != null) _root.SetActive(false);
            ShellFocusUtil.Select(_opener);
        }
    }
}
