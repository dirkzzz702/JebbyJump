namespace JebbyJump.Wardrobe.Visual
{
    // Which logical control should hold keyboard/gamepad focus. Pure decision
    // logic so the focus map is unit-testable without a scene; the panel maps
    // these to actual Selectables.
    public enum WardrobeFocusTarget
    {
        None = 0,
        EquippedRow = 1,
        FirstRow = 2,
        CeremonyEquip = 3,
        CeremonyContinue = 4,
        Opener = 5, // the Main Menu control that opened the panel
    }

    // Deterministic focus resolution for the wardrobe panel + unlock ceremony.
    public static class WardrobeFocusResolver
    {
        // Panel open / rebuild (no ceremony): equipped row if valid, else first.
        public static WardrobeFocusTarget ResolvePanelFocus(
            int rowCount, int equippedRowIndex)
        {
            if (rowCount <= 0) return WardrobeFocusTarget.None;
            return equippedRowIndex >= 0 && equippedRowIndex < rowCount
                ? WardrobeFocusTarget.EquippedRow
                : WardrobeFocusTarget.FirstRow;
        }

        // A ceremony page: Equip Now if it is enabled, otherwise Continue.
        public static WardrobeFocusTarget ResolveCeremonyFocus(bool equipEnabled)
            => equipEnabled
                ? WardrobeFocusTarget.CeremonyEquip
                : WardrobeFocusTarget.CeremonyContinue;

        // Panel close: restore focus to the opener (Main Menu Wardrobe button).
        public static WardrobeFocusTarget ResolveCloseFocus()
            => WardrobeFocusTarget.Opener;
    }
}
