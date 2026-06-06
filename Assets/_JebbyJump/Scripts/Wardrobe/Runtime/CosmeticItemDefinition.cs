namespace JebbyJump.Wardrobe
{
    // Cosmetic categories. Outfits only in P9; more categories later.
    public enum CosmeticCategory
    {
        Outfit = 0,
    }

    // Immutable definition of a cosmetic item. P9 ships outfits only.
    // Purely descriptive data: no sprite/art/Resources references, no
    // gameplay stats. RequiredStars is a P8 PLACEHOLDER threshold.
    public sealed class CosmeticItemDefinition
    {
        public string Id { get; }
        public string DisplayName { get; }
        public CosmeticCategory Category { get; }
        public string Description { get; }
        public int RequiredStars { get; }
        public bool AlwaysUnlocked { get; }

        public CosmeticItemDefinition(
            string id,
            string displayName,
            CosmeticCategory category,
            string description,
            int requiredStars,
            bool alwaysUnlocked)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            Description = description;
            RequiredStars = requiredStars;
            AlwaysUnlocked = alwaysUnlocked;
        }
    }
}
