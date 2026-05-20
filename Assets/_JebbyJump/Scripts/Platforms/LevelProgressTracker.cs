using System;
using UnityEngine;

namespace JebbyJump.Level
{
    public class LevelProgressTracker : MonoBehaviour
    {
        public event Action LifeLost;
        public event Action GameOver;
        public event Action<int> LivesChanged;

        public int Lives { get; private set; }
        public bool IsGameOver => Lives <= 0;

        public void Initialize(int startingLives)
        {
            Lives = startingLives;
            LivesChanged?.Invoke(Lives);
            Debug.Log("[Progress] Lives: " + Lives);
        }

        public void LoseLife()
        {
            if (IsGameOver) return;
            Lives--;
            Debug.Log("[Progress] Lives remaining: " + Lives);
            LivesChanged?.Invoke(Lives);
            if (Lives <= 0)
                GameOver?.Invoke();
            else
                LifeLost?.Invoke();
        }
    }
}
