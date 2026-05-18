using System;
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
            CurrentLevelIndex = 0;
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
