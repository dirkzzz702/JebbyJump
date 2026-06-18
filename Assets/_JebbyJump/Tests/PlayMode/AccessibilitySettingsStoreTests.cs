using JebbyJump.Settings;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P20 accessibility settings: Reduce Motion persistence + change event.
    public class AccessibilitySettingsStoreTests
    {
        [SetUp]
        public void SetUp() => Reset();

        [TearDown]
        public void TearDown() => Reset();

        private static void Reset()
        {
            AccessibilitySettingsStore.ResetForTests();
            PlayerPrefs.DeleteKey("jebby.settings.reduceMotion");
            PlayerPrefs.DeleteKey("jebby.settings.memoryCues");
            PlayerPrefs.Save();
        }

        [Test]
        public void Default_IsFalse()
        {
            Assert.IsFalse(AccessibilitySettingsStore.ReduceMotion);
        }

        [Test]
        public void Set_Persists()
        {
            AccessibilitySettingsStore.ReduceMotion = true;
            Assert.IsTrue(AccessibilitySettingsStore.ReduceMotion);
        }

        [Test]
        public void ResetToDefaults_RestoresFalse()
        {
            AccessibilitySettingsStore.ReduceMotion = true;
            AccessibilitySettingsStore.ResetToDefaults();
            Assert.IsFalse(AccessibilitySettingsStore.ReduceMotion);
        }

        [Test]
        public void Change_RaisesEventWithNewValue()
        {
            int calls = 0;
            bool last = false;
            AccessibilitySettingsStore.ReduceMotionChanged += v =>
            { calls++; last = v; };
            AccessibilitySettingsStore.ReduceMotion = true;
            Assert.AreEqual(1, calls);
            Assert.IsTrue(last);
        }

        [Test]
        public void SettingSameValue_DoesNotRaiseEvent()
        {
            int calls = 0;
            AccessibilitySettingsStore.ReduceMotionChanged += _ => calls++;
            AccessibilitySettingsStore.ReduceMotion = false; // already false
            Assert.AreEqual(0, calls);
        }

        // P22 Memory Cues (opt-in, default OFF) - same persistence + event
        // contract as Reduce Motion.
        [Test]
        public void MemoryCues_DefaultIsFalse()
        {
            Assert.IsFalse(AccessibilitySettingsStore.MemoryCues);
        }

        [Test]
        public void MemoryCues_Set_Persists()
        {
            AccessibilitySettingsStore.MemoryCues = true;
            Assert.IsTrue(AccessibilitySettingsStore.MemoryCues);
        }

        [Test]
        public void MemoryCues_ResetToDefaults_RestoresFalse()
        {
            AccessibilitySettingsStore.MemoryCues = true;
            AccessibilitySettingsStore.ResetToDefaults();
            Assert.IsFalse(AccessibilitySettingsStore.MemoryCues);
        }

        [Test]
        public void MemoryCues_Change_RaisesEventWithNewValue()
        {
            int calls = 0;
            bool last = false;
            AccessibilitySettingsStore.MemoryCuesChanged += v =>
            { calls++; last = v; };
            AccessibilitySettingsStore.MemoryCues = true;
            Assert.AreEqual(1, calls);
            Assert.IsTrue(last);
        }

        [Test]
        public void ResetToDefaults_RestoresBothAccessibilitySettings()
        {
            AccessibilitySettingsStore.ReduceMotion = true;
            AccessibilitySettingsStore.MemoryCues = true;
            AccessibilitySettingsStore.ResetToDefaults();
            Assert.IsFalse(AccessibilitySettingsStore.ReduceMotion);
            Assert.IsFalse(AccessibilitySettingsStore.MemoryCues);
        }
    }
}
