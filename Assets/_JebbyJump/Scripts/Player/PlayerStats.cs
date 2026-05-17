using UnityEngine;

namespace JebbyJump.Player
{
    // Stats reset at level start only. Future temporary item effects manage their own duration.
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private PlayerMovementConfig _config;

        public float JumpForce { get; set; }
        public float MoveSpeed { get; set; }

        private void Awake()
        {
            if (_config == null)
                Debug.LogError("[PlayerStats] PlayerMovementConfig not assigned.", this);
            ResetToBase();
        }

        public void ResetToBase()
        {
            if (_config == null) return;
            JumpForce = _config.JumpForce;
            MoveSpeed = _config.MoveSpeed;
        }
    }
}
