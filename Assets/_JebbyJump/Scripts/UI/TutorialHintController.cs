using System.Collections;
using JebbyJump.Level;
using JebbyJump.Sequence;
using TMPro;
using UnityEngine;

namespace JebbyJump.UI
{
    public class TutorialHintController : MonoBehaviour
    {
        [SerializeField] private GameObject _hintRoot;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private LevelSessionController _levelSession;
        [SerializeField] private MemoryPhaseController _phaseController;
        [SerializeField] private PlatformSpawner _spawner;

        private bool _hasShownIntro;
        private bool _hasShownWrongHint;
        private bool _hasShownCactusHint;
        private bool _hintsDone;
        private Coroutine _hideCoroutine;

        private bool IsLevelOne => _levelSession == null || _levelSession.CurrentLevelIndex == 0;

        private void Awake()
        {
            if (_hintRoot != null) _hintRoot.SetActive(false);
        }

        private void OnEnable()
        {
            if (_phaseController != null)
            {
                _phaseController.MemoryPhaseStarted += OnMemoryPhaseStarted;
                _phaseController.CorrectLanding     += OnCorrectLanding;
                _phaseController.WrongLanding       += OnWrongLanding;
            }
            if (_spawner != null) _spawner.CactusHit += OnCactusHit;
        }

        private void OnDisable()
        {
            if (_phaseController != null)
            {
                _phaseController.MemoryPhaseStarted -= OnMemoryPhaseStarted;
                _phaseController.CorrectLanding     -= OnCorrectLanding;
                _phaseController.WrongLanding       -= OnWrongLanding;
            }
            if (_spawner != null) _spawner.CactusHit -= OnCactusHit;
        }

        private void OnMemoryPhaseStarted()
        {
            if (!IsLevelOne || _hasShownIntro) return;
            _hasShownIntro = true;
            ShowHint("Remember the colors!\nThen jump on them in order.", 7f);
        }

        private void OnCorrectLanding()
        {
            if (IsLevelOne) { _hintsDone = true; HideHint(); }
        }

        private void OnWrongLanding()
        {
            if (!IsLevelOne || _hasShownWrongHint) return;
            _hasShownWrongHint = true;
            ShowHint("Wrong color costs a heart.\nLand on the matching platform!", 3.5f);
        }

        private void OnCactusHit()
        {
            if (_hasShownCactusHint) return;
            _hasShownCactusHint = true;
            ShowHint("Cactus hurts!\nStay away from the cactus side.", 3.5f);
        }

        private void ShowHint(string text, float duration)
        {
            if (_hintsDone || _hintText == null) return;
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            _hintText.text = text;
            if (_hintRoot != null) _hintRoot.SetActive(true);
            _hideCoroutine = StartCoroutine(HideAfter(duration));
        }

        private void HideHint()
        {
            if (_hideCoroutine != null) { StopCoroutine(_hideCoroutine); _hideCoroutine = null; }
            if (_hintRoot != null) _hintRoot.SetActive(false);
        }

        private IEnumerator HideAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (_hintRoot != null) _hintRoot.SetActive(false);
            _hideCoroutine = null;
        }
    }
}
