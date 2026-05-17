using UnityEngine;

namespace JebbyJump.UI
{
    // Phase 9 MVP: OnScreenButton components use existing keyboard control paths as a bridge.
    // JebbyInputActions.inputactions is unchanged — no new bindings added.
    // TODO: Add safe-area padding (notch / home bar) before shipping on mobile.
    public class MobileControlsToggle : MonoBehaviour
    {
        [SerializeField] private bool _forceVisibleInEditor = true;

        private void Awake()
        {
#if UNITY_EDITOR
            gameObject.SetActive(_forceVisibleInEditor);
#else
            gameObject.SetActive(Application.isMobilePlatform);
#endif
        }
    }
}
