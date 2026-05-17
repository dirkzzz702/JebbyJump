using System;
using System.Collections.Generic;
using JebbyJump.Core;
using JebbyJump.Level;
using UnityEngine;

namespace JebbyJump.Sequence
{
    public class ColorSequenceManager : MonoBehaviour
    {
        [SerializeField] private LevelConfig _config;

        public event Action SequenceComplete;

        public LevelConfig Config => _config;
        public IReadOnlyList<PlatformColor> Sequence { get; private set; }
        public int CurrentStepIndex { get; private set; }
        public bool IsComplete => Sequence != null && CurrentStepIndex >= Sequence.Count;
        public PlatformColor ExpectedColor => Sequence[CurrentStepIndex]; // only safe when !IsComplete

        private void Awake()
        {
            if (_config == null)
                Debug.LogError("[ColorSequenceManager] Config not assigned.", this);
        }

        public void GenerateSequence()
        {
            if (_config == null)
            {
                Debug.LogError("[ColorSequenceManager] Cannot generate — config not assigned.", this);
                return;
            }
            if (_config.SequenceLength <= 0)
            {
                Debug.LogError("[ColorSequenceManager] Cannot generate — sequenceLength must be > 0.", this);
                return;
            }
            if (_config.AvailableColors == null || _config.AvailableColors.Length == 0)
            {
                Debug.LogError("[ColorSequenceManager] Cannot generate — availableColors is empty.", this);
                return;
            }

            var list = new List<PlatformColor>();
            for (int i = 0; i < _config.SequenceLength; i++)
                list.Add(_config.AvailableColors[UnityEngine.Random.Range(0, _config.AvailableColors.Length)]);
            Sequence = list;
            CurrentStepIndex = 0;
            Debug.Log($"[ColorSequenceManager] Sequence: {string.Join(" → ", list)}");
        }

        public void ResetProgress()
        {
            CurrentStepIndex = 0;
            Debug.Log("[ColorSequenceManager] Sequence progress reset.");
        }

        public void AdvanceStep()
        {
            if (IsComplete) return;
            CurrentStepIndex++;
            if (IsComplete)
                SequenceComplete?.Invoke();
        }
    }
}
