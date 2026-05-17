using System;
using UnityEngine;

namespace JebbyJump.Obstacles
{
    public class CactusObstacle : MonoBehaviour
    {
        public event Action PlayerHit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Debug.Log("[CactusObstacle] Player hit cactus.");
            PlayerHit?.Invoke();
        }
    }
}
