using UnityEngine;

namespace JebbyJump.Level
{
    // Tracks elapsed seconds for a single level attempt. The MemoryPhaseController
    // starts the timer when the Playing phase begins (after the memory sequence)
    // and stops it on level complete or game over.
    public class LevelTimer : MonoBehaviour
    {
        private float _elapsed;
        private bool  _running;

        public float Elapsed => _elapsed;
        public bool  IsRunning => _running;

        public void StartTimer()
        {
            _elapsed = 0f;
            _running = true;
        }

        public void StopTimer()  => _running = false;
        public void ResetTimer() { _elapsed = 0f; _running = false; }

        private void Update()
        {
            if (_running) _elapsed += Time.deltaTime;
        }
    }
}
