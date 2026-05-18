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
                Debug.LogWarning("[RocketBootsPickup] Player touched pickup but no RocketBootsEffect found.", this);
                return;
            }
            effect.Activate();
            gameObject.SetActive(false);
        }

        public void ResetPickup()
        {
            gameObject.SetActive(true);
        }
    }
}
