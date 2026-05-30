using JebbyJump.Settings;
using UnityEngine;

namespace JebbyJump.Audio
{
    // Applies AudioSettingsStore values to the running audio engine.
    // Null-safe so it works in MainMenu (no audio sources, mute still
    // takes effect via AudioListener.volume) and in Game (wired to the
    // existing AudioFeedbackController AudioSource). One per scene's
    // Canvas; SettingsPanelController calls ApplyAll() on each change.
    public class AudioSettingsApplier : MonoBehaviour
    {
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;

        private void Awake()
        {
            ApplyAll();
        }

        public void ApplyAll()
        {
            if (_sfxSource != null)
                _sfxSource.volume = AudioSettingsStore.SfxVolume;
            if (_musicSource != null)
                _musicSource.volume = AudioSettingsStore.MusicVolume;

            // AudioListener.volume is a static that affects all listeners
            // app-wide; this is the simplest master mute without a mixer.
            AudioListener.volume = AudioSettingsStore.Muted ? 0f : 1f;
        }
    }
}
