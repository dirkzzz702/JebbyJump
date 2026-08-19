using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Tone of a gameplay feedback message, driving the "Burst Word" glow colour.
    public enum FeedbackTone { Neutral, Positive, Negative, Kickoff }

    // "Burst Word" gameplay feedback: a big outlined word (cream fill + gold edge)
    // with a soft colour-glow + sparkles that pops in, holds, and fades out. The glow
    // colour is themed by the message tone. Structure is built + wired by the
    // BuildGameplayFeedback editor scaffold.
    public class GameFeedbackUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _popup;      // scales / holds / fades as one unit
        [SerializeField] private CanvasGroup _group;        // fade
        [SerializeField] private TextMeshProUGUI _word;     // the outlined word
        [SerializeField] private Image _glow;               // radial glow behind, tinted per tone
        [SerializeField] private Image[] _sparkles;         // small white stars (decor)

        private static readonly Color NeutralGlow  = new Color(1f, 0.78f, 0.32f);   // warm gold
        private static readonly Color PositiveGlow = new Color(0.34f, 0.86f, 0.50f); // green
        private static readonly Color NegativeGlow = new Color(0.96f, 0.40f, 0.30f); // coral
        private static readonly Color KickoffGlow  = new Color(1f, 0.73f, 0.16f);    // sunny gold

        private Coroutine _co;

        private void Awake()
        {
            if (_popup != null) _popup.gameObject.SetActive(false);
        }

        // Back-compat overload (defaults to Neutral).
        public void ShowMessage(string message, float duration = 0.8f)
            => ShowMessage(message, duration, FeedbackTone.Neutral);

        public void ShowMessage(string message, float duration, FeedbackTone tone)
        {
            if (_word == null || _popup == null) return;
            _word.text = message;
            if (_glow != null)
            {
                var c = ToneGlow(tone);
                _glow.color = new Color(c.r, c.g, c.b, 0.9f);
            }
            if (_co != null) StopCoroutine(_co);
            _popup.gameObject.SetActive(true);
            _co = StartCoroutine(Play(Mathf.Max(0f, duration)));
        }

        private static Color ToneGlow(FeedbackTone t)
        {
            switch (t)
            {
                case FeedbackTone.Positive: return PositiveGlow;
                case FeedbackTone.Negative: return NegativeGlow;
                case FeedbackTone.Kickoff:  return KickoffGlow;
                default:                    return NeutralGlow;
            }
        }

        private IEnumerator Play(float holdDuration)
        {
            const float popDur = 0.22f, fadeDur = 0.2f;

            // pop in: scale 0.5 -> 1.08 (overshoot) -> 1.0, fading up
            if (_group != null) _group.alpha = 0f;
            float t = 0f;
            while (t < popDur)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / popDur);
                float s = k < 0.6f
                    ? Mathf.Lerp(0.5f, 1.08f, k / 0.6f)
                    : Mathf.Lerp(1.08f, 1f, (k - 0.6f) / 0.4f);
                _popup.localScale = new Vector3(s, s, 1f);
                if (_group != null) _group.alpha = Mathf.Clamp01(k * 1.6f);
                yield return null;
            }
            _popup.localScale = Vector3.one;
            if (_group != null) _group.alpha = 1f;

            yield return new WaitForSeconds(holdDuration);

            // fade out
            float f = 0f;
            while (f < fadeDur)
            {
                f += Time.deltaTime;
                if (_group != null) _group.alpha = 1f - Mathf.Clamp01(f / fadeDur);
                yield return null;
            }
            _popup.gameObject.SetActive(false);
            if (_group != null) _group.alpha = 1f;
            _co = null;
        }
    }
}
