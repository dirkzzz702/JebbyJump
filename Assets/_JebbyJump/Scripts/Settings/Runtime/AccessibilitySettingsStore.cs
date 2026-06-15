using System;
using UnityEngine;

namespace JebbyJump.Settings
{
    // Local accessibility preferences, PlayerPrefs-backed (mirrors the
    // AudioSettingsStore pattern). P20 adds Reduce Motion only. Event-driven so
    // live UI (the wardrobe pose carousel) re-applies on change WITHOUT polling
    // PlayerPrefs every frame.
    //
    // PlayerPrefs key:
    //   jebby.settings.reduceMotion  (int 0/1, default 0)
    public static class AccessibilitySettingsStore
    {
        private const string ReduceMotionKey = "jebby.settings.reduceMotion";
        private const bool DefaultReduceMotion = false;

        // Raised after ReduceMotion actually changes (new value passed).
        public static event Action<bool> ReduceMotionChanged;

        public static bool ReduceMotion
        {
            get => PlayerPrefs.GetInt(
                ReduceMotionKey, DefaultReduceMotion ? 1 : 0) != 0;
            set
            {
                bool current = ReduceMotion;
                PlayerPrefs.SetInt(ReduceMotionKey, value ? 1 : 0);
                PlayerPrefs.Save();
                if (value != current) ReduceMotionChanged?.Invoke(value);
            }
        }

        // Dev-only / Reset Defaults: clears the key (back to default) and
        // notifies if that changed the effective value.
        public static void ResetToDefaults()
        {
            bool had = ReduceMotion;
            PlayerPrefs.DeleteKey(ReduceMotionKey);
            PlayerPrefs.Save();
            if (had != DefaultReduceMotion)
                ReduceMotionChanged?.Invoke(DefaultReduceMotion);
        }

        // Test isolation: clears subscribers so a leaked handler cannot fire.
        public static void ResetForTests() => ReduceMotionChanged = null;
    }
}
