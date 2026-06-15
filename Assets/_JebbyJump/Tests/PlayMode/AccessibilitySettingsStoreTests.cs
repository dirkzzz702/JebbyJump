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
    }
}
