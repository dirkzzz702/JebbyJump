using System;
using JebbyJump.Analytics;
using JebbyJump.Audio;
using JebbyJump.Settings;
using UnityEngine;
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
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private AudioSettingsApplier _applier;

        private bool _initializing;
        private Action _onClosed;

        private void Awake()
        {
            if (_panelRoot != null) _panelRoot.SetActive(false);

            if (_musicSlider != null)
                _musicSlider.onValueChanged.AddListener(OnMusicChanged);
            if (_sfxSlider != null)
                _sfxSlider.onValueChanged.AddListener(OnSfxChanged);
            if (_muteToggle != null)
                _muteToggle.onValueChanged.AddListener(OnMuteChanged);
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
            if (_backButton != null)
                _backButton.onClick.RemoveListener(Close);
            if (_resetButton != null)
                _resetButton.onClick.RemoveListener(OnResetClicked);
        }

        public void Open()
        {
            PopulateFromStore();
            if (_panelRoot != null) _panelRoot.SetActive(true);
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
            _initializing = false;
        }

        private void OnMusicChanged(float value)
        {
            if (_initializing) return;
            AudioSettingsStore.MusicVolume = value;
            _applier?.ApplyAll();
            AnalyticsService.Track("settings_changed",
                AnalyticsParam.Of("setting_name", "music_volume"),
                AnalyticsParam.Of("value", AudioSettingsStore.MusicVolume));
        }

        private void OnSfxChanged(float value)
        {
            if (_initializing) return;
            AudioSettingsStore.SfxVolume = value;
            _applier?.ApplyAll();
            AnalyticsService.Track("settings_changed",
                AnalyticsParam.Of("setting_name", "sfx_volume"),
                AnalyticsParam.Of("value", AudioSettingsStore.SfxVolume));
        }

        private void OnMuteChanged(bool value)
        {
            if (_initializing) return;
            AudioSettingsStore.Muted = value;
            _applier?.ApplyAll();
            AnalyticsService.Track("settings_changed",
                AnalyticsParam.Of("setting_name", "muted"),
                AnalyticsParam.Of("value", value));
        }

        private void OnResetClicked()
        {
            AudioSettingsStore.ResetToDefaults();
            PopulateFromStore();
            _applier?.ApplyAll();
            AnalyticsService.Track("settings_changed",
                AnalyticsParam.Of("setting_name", "reset_defaults"),
                AnalyticsParam.Of("value", true));
        }
    }
}
