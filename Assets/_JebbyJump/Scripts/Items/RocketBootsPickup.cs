using UnityEngine;

namespace JebbyJump.Items
{
    [RequireComponent(typeof(Collider2D))]
    public class RocketBootsPickup : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var effect = other.GetComponentInParent<RocketBootsEffect>();
            if (effect == null)
            {
                Debug.LogWarning("[RocketBootsPickup] Player triggered pickup but no RocketBootsEffect found.", this);
                return;
            }
            Debug.Log("[RocketBoots] Collected.");
            effect.Activate();
            gameObject.SetActive(false);
        }

        // Called by MemoryPhaseController.RestartLevel() on retry / next level.
        // Life loss does NOT call this — pickup stays disabled for that attempt.
        public void ResetPickup()
        {
            gameObject.SetActive(true);
        }
    }
}
