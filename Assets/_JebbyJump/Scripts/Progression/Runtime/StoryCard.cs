namespace JebbyJump.Story
{
    // A single story "postcard" (WorldExpansion100, phase P34F). Data only.
    // Lives in the JebbyJump.Progression.Runtime assembly (story "seen" flags
    // are local progress) but keeps its own namespace.
    public enum StoryCardKind { Opening, World, Ending }

    public sealed class StoryCard
    {
        public string Id { get; }
        public StoryCardKind Kind { get; }
        public int WorldNumber { get; }   // 1..10 for World cards; 0 otherwise
        public string Headline { get; }
        public string Body { get; }       // <= 30 words, child-safe

        public StoryCard(string id, StoryCardKind kind, int worldNumber,
            string headline, string body)
        {
            Id = id;
            Kind = kind;
            WorldNumber = worldNumber;
            Headline = headline;
            Body = body;
        }
    }
}
