using System;
using System.Collections.Generic;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure sequential queue/presenter for the unlock ceremony. Acknowledge and
    // equip are injected delegates (the panel supplies the acknowledgement
    // store + the WardrobeStore equip path), so this is fully unit-testable
    // with no engine/scene. Acknowledgement happens ONLY on Continue / EquipNow
    // (never on construction, render, or panel close). A failed equip does not
    // acknowledge or advance.
    public sealed class WardrobeCeremonyPresenter
    {
        private readonly IReadOnlyList<CosmeticItemDefinition> _items;
        private readonly Action<string> _acknowledge;
        private readonly Func<string, bool> _tryEquip;
        private int _index;

        public WardrobeCeremonyPresenter(
            IReadOnlyList<CosmeticItemDefinition> items,
            Action<string> acknowledge, Func<string, bool> tryEquip)
        {
            _items = items ?? Array.Empty<CosmeticItemDefinition>();
            _acknowledge = acknowledge;
            _tryEquip = tryEquip;
            _index = 0;
        }

        public bool IsActive => _index < _items.Count;
        public CosmeticItemDefinition Current => IsActive ? _items[_index] : null;
        public int Count => _items.Count;
        public int Position => IsActive ? _index + 1 : _items.Count; // 1-based

        // Acknowledge the current outfit and advance. Returns whether a next
        // item remains active.
        public bool Continue()
        {
            if (!IsActive) return false;
            _acknowledge?.Invoke(Current.Id);
            _index++;
            return IsActive;
        }

        // Equip the current outfit via the injected path; only on success
        // acknowledge + advance. Returns true iff the equip succeeded. A failed
        // equip leaves the queue untouched (no acknowledge, no advance).
        public bool EquipNow()
        {
            if (!IsActive) return false;
            string id = Current.Id;
            bool equipped = _tryEquip != null && _tryEquip(id);
            if (!equipped) return false;
            _acknowledge?.Invoke(id);
            _index++;
            return true;
        }
    }
}
