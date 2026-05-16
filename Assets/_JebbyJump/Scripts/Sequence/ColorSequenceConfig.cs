using JebbyJump.Core;
using UnityEngine;

namespace JebbyJump.Sequence
{
    [CreateAssetMenu(fileName = "SequenceConfig", menuName = "JebbyJump/SequenceConfig")]
    public class ColorSequenceConfig : ScriptableObject
    {
        [SerializeField] private int _sequenceLength = 4;
        [SerializeField] private float _memoryTimeSeconds = 5f;
        [SerializeField] private float _memoryPhaseJumpMultiplier = 0.35f;
        [SerializeField] private PlatformColor[] _availableColors =
        {
            PlatformColor.Red,
            PlatformColor.Blue,
            PlatformColor.Yellow,
        };

        public int SequenceLength => _sequenceLength;
        public float MemoryTimeSeconds => _memoryTimeSeconds;
        public float MemoryPhaseJumpMultiplier => _memoryPhaseJumpMultiplier;
        public PlatformColor[] AvailableColors => _availableColors;
    }
}
