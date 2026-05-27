using System;
using JebbyJump.Progression;
using UnityEngine;

namespace JebbyJump.Level
{
    public class LevelSessionController : MonoBehaviour
    {
        [SerializeField] private LevelConfig[] _levels;

        public event Action LevelChanged;

        public int CurrentLevelIndex { get; private set; }
        public int TotalLevels => _levels?.Length ?? 0;
        public bool IsFinalLevel => _levels == null || CurrentLevelIndex >= _levels.Length - 1;

        public LevelConfig CurrentConfig => (_levels != null && _levels.Length > 0)
            ? _levels[Mathf.Clamp(CurrentLevelIndex, 0, _levels.Length - 1)]
            : null;

        private void Awake()
        {
            // Default to Level 1 unless the player picked a level on the
            // Main Menu just before this scene loaded.
            CurrentLevelIndex = 0;
            if (_levels != null && _levels.Length > 0)
            {
                int requested = PendingLevelSelection.Index;
                if (requested >= 0 && requested < _levels.Length)
                    CurrentLevelIndex = requested;
            }
            // Consume the selection so a return-to-menu + new run does not
            // silently keep the previous index.
            PendingLevelSelection.Reset();

            if (_levels == null || _levels.Length == 0)
                Debug.LogError("[LevelSessionController] No levels assigned.", this);
        }

        public void AdvanceToNextLevel()
        {
            if (IsFinalLevel) return;
            CurrentLevelIndex++;
            Debug.Log("[LevelSessionController] Advanced to level " + (CurrentLevelIndex + 1));
            LevelChanged?.Invoke();
        }
    }
}
