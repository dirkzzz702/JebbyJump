using UnityEngine;

namespace JebbyJump.Settings
{
    // Local audio preferences, PlayerPrefs-backed. Pure persistence with
    // 0..1 clamping; no audio engine references so the test asmdef can
    // exercise it without touching Unity audio.
    //
    // PlayerPrefs keys:
    //   jebby.audio.musicVolume  (float, default 1.0, clamped 0..1)
    //   jebby.audio.sfxVolume    (float, default 1.0, clamped 0..1)
    //   jebby.audio.muted        (int 0/1,  default 0)
    public static class AudioSettingsStore
    {
        private const string MusicKey = "jebby.audio.musicVolume";
        private const string SfxKey   = "jebby.audio.sfxVolume";
        private const string MuteKey  = "jebby.audio.muted";

        private const float DefaultMusic = 1f;
        private const float DefaultSfx   = 1f;
        private const bool  DefaultMute  = false;

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MusicKey, DefaultMusic);
            set
            {
                float clamped = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(MusicKey, clamped);
                PlayerPrefs.Save();
            }
        }

        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(SfxKey, DefaultSfx);
            set
            {
                float clamped = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(SfxKey, clamped);
                PlayerPrefs.Save();
            }
        }

        public static bool Muted
        {
            get => PlayerPrefs.GetInt(MuteKey, DefaultMute ? 1 : 0) != 0;
            set
            {
                PlayerPrefs.SetInt(MuteKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        // Dev-only: clears the three keys so a fresh-install state can be
        // restored. Used by the optional Reset Defaults button and tests.
        public static void ResetToDefaults()
        {
            PlayerPrefs.DeleteKey(MusicKey);
            PlayerPrefs.DeleteKey(SfxKey);
            PlayerPrefs.DeleteKey(MuteKey);
            PlayerPrefs.Save();
        }
    }
}
