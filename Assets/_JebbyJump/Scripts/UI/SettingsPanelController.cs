using System;
using System.Collections.Generic;
using JebbyJump.Analytics;
using JebbyJump.Audio;
using JebbyJump.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JebbyJump.UI
{
    // Settings panel: music slider, SFX slider, mute toggle, back button,
    // and an optional Reset Defaults button. Populates from
    // AudioSettingsStore on Open and writes back on each change, calling
    // the applier so changes take effect immediately.
    //
    // _initializing guard prevents the slider/toggle event handlers from
    // re-writing the store while we're seeding their values from the
    // store on Open.
    public class SettingsPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Toggle _muteToggle;
        [SerializeField] private Toggle _reduceMotionToggle;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private AudioSettingsApplier _applier;

        private bool _initializing;
        private Action _onClosed;
        private GameObject _opener; // restore focus here on Close (P21)

        private void Awake()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);

            if (_musicSlider != null)
                _musicSlider.onValueChanged.AddListener(OnMusicChanged);
            if (_sfxSlider != null)
                _sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            if (_muteToggle != null)
                _muteToggle.onValueChanged.AddListener(OnMuteChanged);
            if (_reduceMotionToggle != null)
                _reduceMotionToggle.onValueChanged.AddListener(OnReduceMotionChanged);
            if (_backButton != null)
                _backButton.onClick.AddListener(Close);
            if (_resetButton != null)
                _resetButton.onClick.AddListener(OnResetClicked);
        }

        private void OnDestroy()
        {
            if (_musicSlider != null)
                _musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
            if (_sfxSlider != null)
                _sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
            if (_muteToggle != null)
                _muteToggle.onValueChanged.RemoveListener(OnMuteChanged);
            if (_reduceMotionToggle != null)
                _reduceMotionToggle.onValueChanged.RemoveListener(OnReduceMotionChanged);
            if (_backButton != null)
                _backButton.onClick.RemoveListener(Close);
            if (_resetButton != null)
                _resetButton.onClick.RemoveListener(OnResetClicked);
        }

        public void Open()
        {
            _opener = EventSystem.current != null
                ? EventSystem.current.currentSelectedGameObject : null;
            PopulateFromStore();
            if (_panelRoot != null) _panelRoot.SetActive(true);
            BuildNavigationAndFocus();
        }

        // P21: explicit nav Music -> SFX -> Mute -> Reduce Motion -> Reset ->
        // Back; sliders keep Left/Right value adjustment (only Up/Down links
        // set). Initial focus = first available control.
        private void BuildNavigationAndFocus()
        {
            var items = new List<Selectable>
            {
                _musicSlider, _sfxSlider, _muteToggle,
                _reduceMotionToggle, _resetButton,
            };
            ShellFocusUtil.BuildVerticalNavigation(items, _backButton);
            foreach (var s in items)
                if (s != null) { ShellFocusUtil.Select(s); return; }
            ShellFocusUtil.Select(_backButton);
        }

        // Pause-menu path: open with a callback that fires on Close so the
        // caller can restore its own UI (e.g. the Pause panel). Does not
        // touch Time.timeScale or PauseState - the pause invariants are
        // preserved by NOT changing them here.
        public void Open(Action onClosed)
        {
            _onClosed = onClosed;
            Open();
        }

        public void Close()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);
            // Clear before invoking so a callback that itself reopens the
            // panel cannot leak a stale handler.
            var cb = _onClosed;
            _onClosed = null;
            cb?.Invoke();
            // Restore focus AFTER the callback (the Pause path re-activates the
            // Pause panel in cb, so the opener button is active again).
            ShellFocusUtil.Select(_opener);
        }

        private void PopulateFromStore()
        {
            _initializing = true;
            if (_musicSlider != null)
                _musicSlider.value = AudioSettingsStore.MusicVolume;
            if (_sfxSlider != null)
                _sfxSlider.value = AudioSettingsStore.SfxVolume;
            if (_muteToggle != null)
                _muteToggle.isOn = AudioSettingsStore.Muted;
            if (_reduceMotionToggle != null)
                _reduceMotionToggle.isOn = AccessibilitySettingsStore.ReduceMotion;
            _initializing = false;
        }

        private void OnMusicChanged(float value)
        {
            if (_initializing) return;
            AudioSettingsStore.MusicVolume = value;
            _applier?.ApplyAll();
            AnalyticsService.Track(AnalyticsEvents.SettingsChanged,
                AnalyticsParam.Of(AnalyticsParams.SettingName, "music_volume"),
                AnalyticsParam.Of(AnalyticsParams.Value, AudioSettingsStore.MusicVolume));
        }

        private void OnSfxChanged(float value)
        {
            if (_initializing) return;
            AudioSettingsStore.SfxVolume = value;
            _applier?.ApplyAll();
            AnalyticsService.Track(AnalyticsEvents.SettingsChanged,
                AnalyticsParam.Of(AnalyticsParams.SettingName, "sfx_volume"),
                AnalyticsParam.Of(AnalyticsParams.Value, AudioSettingsStore.SfxVolume));
        }

        private void OnMuteChanged(bool value)
        {
            if (_initializing) return;
            AudioSettingsStore.Muted = value;
            _applier?.ApplyAll();
            AnalyticsService.Track(AnalyticsEvents.SettingsChanged,
                AnalyticsParam.Of(AnalyticsParams.SettingName, "muted"),
                AnalyticsParam.Of(AnalyticsParams.Value, value));
        }

        // Reduce Motion (P20 accessibility): cosmetic UI motion only - it never
        // touches gameplay movement/timers/physics. Reuses the generic
        // settings_changed analytic (no new event).
        private void OnReduceMotionChanged(bool value)
        {
            if (_initializing) return;
            AccessibilitySettingsStore.ReduceMotion = value;
            AnalyticsService.Track(AnalyticsEvents.SettingsChanged,
                AnalyticsParam.Of(AnalyticsParams.SettingName, "reduce_motion"),
                AnalyticsParam.Of(AnalyticsParams.Value, value));
        }

        private void OnResetClicked()
        {
            AudioSettingsStore.ResetToDefaults();
            AccessibilitySettingsStore.ResetToDefaults();
            PopulateFromStore();
            _applier?.ApplyAll();
            AnalyticsService.Track(AnalyticsEvents.SettingsChanged,
                AnalyticsParam.Of(AnalyticsParams.SettingName, "reset_defaults"),
                AnalyticsParam.Of(AnalyticsParams.Value, true));
        }
    }
}
