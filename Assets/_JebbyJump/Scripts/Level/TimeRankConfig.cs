using UnityEngine;

namespace JebbyJump.Level
{
    public enum TimeRank { S, A, B, C }

    [CreateAssetMenu(fileName = "TimeRankConfig", menuName = "JebbyJump/TimeRankConfig")]
    public class TimeRankConfig : ScriptableObject
    {
        [Tooltip("Clear time at or below this gets S rank.")]
        [SerializeField] private float _sThreshold = 10f;
        [Tooltip("Clear time at or below this gets A rank.")]
        [SerializeField] private float _aThreshold = 15f;
        [Tooltip("Clear time at or below this gets B rank. Anything slower is C.")]
        [SerializeField] private float _bThreshold = 25f;

        public TimeRank GetRank(float seconds)
        {
            if (seconds <= _sThreshold) return TimeRank.S;
            if (seconds <= _aThreshold) return TimeRank.A;
            if (seconds <= _bThreshold) return TimeRank.B;
            return TimeRank.C;
        }

        private void OnValidate()
        {
            _sThreshold = Mathf.Max(0.1f, _sThreshold);
            _aThreshold = Mathf.Max(_sThreshold + 0.1f, _aThreshold);
            _bThreshold = Mathf.Max(_aThreshold + 0.1f, _bThreshold);
        }
    }
}
