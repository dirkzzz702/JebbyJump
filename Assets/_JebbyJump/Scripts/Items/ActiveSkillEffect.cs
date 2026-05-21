using UnityEngine;

namespace JebbyJump.Items
{
    // Common base for active skill effects so ActiveSkillController can hold a
    // polymorphic reference without coupling to a specific effect type.
    public abstract class ActiveSkillEffect : MonoBehaviour
    {
        public abstract bool IsActive { get; }
        public abstract void Activate();
        public abstract void CancelEffect();
    }
}
