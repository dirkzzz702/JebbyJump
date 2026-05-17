using System;
using UnityEngine;

namespace JebbyJump.Level
{
    public class LevelProgressTracker : MonoBehaviour
    {
        public event Action LifeLost;
        public event Action GameOver;
        public event Action<int> ScoreChanged;
        public event Action<int> LivesChanged;

        public int Lives { get; private set; }
        public int Score { get; private set; }
        public bool IsGameOver => Lives <= 0;

        public void Initialize(int startingLives)
        {
            Lives = startingLives;
            Score = 0;
            LivesChanged?.Invoke(Lives);
            Debug.Log("[Progress] Lives: " + Lives + "  Score: " + Score);
        }

        public void AddScore(int amount)
        {
            Score += amount;
            ScoreChanged?.Invoke(Score);
            Debug.Log("[Progress] Score: " + Score);
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
