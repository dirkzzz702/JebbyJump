using System.Collections;
using TMPro;
using UnityEngine;

namespace JebbyJump.UI
{
    public class GameFeedbackUI : MonoBehaviour
    {
        [SerializeField] private GameObject _feedbackRoot;
        [SerializeField] private TextMeshProUGUI _feedbackText;

        private Coroutine _hideCoroutine;

        private void Awake()
        {
            if (_feedbackRoot != null) _feedbackRoot.SetActive(false);
        }

        public void ShowMessage(string message, float duration = 0.8f)
        {
            if (_feedbackText == null) return;
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            _feedbackText.text = message;
            if (_feedbackRoot != null) _feedbackRoot.SetActive(true);
            _hideCoroutine = StartCoroutine(HideAfter(duration));
        }

        private IEnumerator HideAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (_feedbackRoot != null) _feedbackRoot.SetActive(false);
            _hideCoroutine = null;
        }
    }
}
