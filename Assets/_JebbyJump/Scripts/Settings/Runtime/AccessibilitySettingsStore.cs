using System;
using UnityEngine;

namespace JebbyJump.Settings
{
    // Local accessibility preferences, PlayerPrefs-backed (mirrors the
    // AudioSettingsStore pattern). Event-driven so live consumers re-apply on
    // change WITHOUT polling PlayerPrefs every frame.
    //   P20: Reduce Motion (cosmetic UI motion only).
    //   P22: Memory Cues (opt-in non-color cue on memory swatches + platforms).
    //
    // PlayerPrefs keys:
    //   jebby.settings.reduceMotion  (int 0/1, default 0)
    //   jebby.settings.memoryCues    (int 0/1, default 0)
    public static class AccessibilitySettingsStore
    {
        private const string ReduceMotionKey = "jebby.settings.reduceMotion";
        private const bool DefaultReduceMotion = false;

        private const string MemoryCuesKey = "jebby.settings.memoryCues";
        private const bool DefaultMemoryCues = false;

        // Raised after ReduceMotion actually changes (new value passed).
        public static event Action<bool> ReduceMotionChanged;

        // Raised after MemoryCues actually changes (new value passed).
        public static event Action<bool> MemoryCuesChanged;

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

        public static bool MemoryCues
        {
            get => PlayerPrefs.GetInt(
                MemoryCuesKey, DefaultMemoryCues ? 1 : 0) != 0;
            set
            {
                bool current = MemoryCues;
                PlayerPrefs.SetInt(MemoryCuesKey, value ? 1 : 0);
                PlayerPrefs.Save();
                if (value != current) MemoryCuesChanged?.Invoke(value);
            }
        }

        // Dev-only / Reset Defaults: clears the keys (back to default) and
        // notifies if that changed the effective value.
        public static void ResetToDefaults()
        {
            bool hadReduceMotion = ReduceMotion;
            bool hadMemoryCues = MemoryCues;
            PlayerPrefs.DeleteKey(ReduceMotionKey);
            PlayerPrefs.DeleteKey(MemoryCuesKey);
            PlayerPrefs.Save();
            if (hadReduceMotion != DefaultReduceMotion)
                ReduceMotionChanged?.Invoke(DefaultReduceMotion);
            if (hadMemoryCues != DefaultMemoryCues)
                MemoryCuesChanged?.Invoke(DefaultMemoryCues);
        }

        // Test isolation: clears subscribers so a leaked handler cannot fire.
        public static void ResetForTests()
        {
            ReduceMotionChanged = null;
            MemoryCuesChanged = null;
        }
    }
}
