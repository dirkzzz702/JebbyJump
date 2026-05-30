using JebbyJump.Settings;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // Pure persistence + clamping for the audio settings foundation.
    // SetUp/TearDown preserve any pre-existing PlayerPrefs so running the
    // suite does not wipe a developer's local audio preferences.
    public class AudioSettingsStoreTests
    {
        private const string MusicKey = "jebby.audio.musicVolume";
        private const string SfxKey   = "jebby.audio.sfxVolume";
        private const string MuteKey  = "jebby.audio.muted";

        private bool  _hadMusic, _hadSfx, _hadMute;
        private float _savedMusic, _savedSfx;
        private int   _savedMute;

        [SetUp]
        public void SetUp()
        {
            _hadMusic = PlayerPrefs.HasKey(MusicKey);
            _hadSfx   = PlayerPrefs.HasKey(SfxKey);
            _hadMute  = PlayerPrefs.HasKey(MuteKey);
            _savedMusic = PlayerPrefs.GetFloat(MusicKey, 1f);
            _savedSfx   = PlayerPrefs.GetFloat(SfxKey, 1f);
            _savedMute  = PlayerPrefs.GetInt(MuteKey, 0);
            AudioSettingsStore.ResetToDefaults();
        }

        [TearDown]
        public void TearDown()
        {
            if (_hadMusic) PlayerPrefs.SetFloat(MusicKey, _savedMusic);
            else           PlayerPrefs.DeleteKey(MusicKey);
            if (_hadSfx)   PlayerPrefs.SetFloat(SfxKey, _savedSfx);
            else           PlayerPrefs.DeleteKey(SfxKey);
            if (_hadMute)  PlayerPrefs.SetInt(MuteKey, _savedMute);
            else           PlayerPrefs.DeleteKey(MuteKey);
            PlayerPrefs.Save();
        }

        [Test]
        public void Defaults_AreFullVolumeAndUnmuted()
        {
            Assert.AreEqual(1f, AudioSettingsStore.MusicVolume);
            Assert.AreEqual(1f, AudioSettingsStore.SfxVolume);
            Assert.IsFalse(AudioSettingsStore.Muted);
        }

        [Test]
        public void MusicVolume_ClampsToZeroOneRange()
        {
            AudioSettingsStore.MusicVolume = 1.5f;
            Assert.AreEqual(1f, AudioSettingsStore.MusicVolume);

            AudioSettingsStore.MusicVolume = -0.5f;
            Assert.AreEqual(0f, AudioSettingsStore.MusicVolume);

            AudioSettingsStore.MusicVolume = 0.5f;
            Assert.AreEqual(0.5f, AudioSettingsStore.MusicVolume);
        }

        [Test]
        public void SfxVolume_ClampsToZeroOneRange()
        {
            AudioSettingsStore.SfxVolume = 2f;
            Assert.AreEqual(1f, AudioSettingsStore.SfxVolume);

            AudioSettingsStore.SfxVolume = -1f;
            Assert.AreEqual(0f, AudioSettingsStore.SfxVolume);

            AudioSettingsStore.SfxVolume = 0.25f;
            Assert.AreEqual(0.25f, AudioSettingsStore.SfxVolume);
        }

        [Test]
        public void Muted_PersistsTrueAndFalse()
        {
            AudioSettingsStore.Muted = true;
            Assert.IsTrue(AudioSettingsStore.Muted);

            AudioSettingsStore.Muted = false;
            Assert.IsFalse(AudioSettingsStore.Muted);
        }

        [Test]
        public void Volumes_PersistRoundTrip()
        {
            AudioSettingsStore.MusicVolume = 0.3f;
            AudioSettingsStore.SfxVolume = 0.7f;
            Assert.AreEqual(0.3f, AudioSettingsStore.MusicVolume);
            Assert.AreEqual(0.7f, AudioSettingsStore.SfxVolume);
        }

        [Test]
        public void ResetToDefaults_RestoresDefaultValues()
        {
            AudioSettingsStore.MusicVolume = 0.1f;
            AudioSettingsStore.SfxVolume = 0.2f;
            AudioSettingsStore.Muted = true;

            AudioSettingsStore.ResetToDefaults();

            Assert.AreEqual(1f, AudioSettingsStore.MusicVolume);
            Assert.AreEqual(1f, AudioSettingsStore.SfxVolume);
            Assert.IsFalse(AudioSettingsStore.Muted);
        }
    }
}
