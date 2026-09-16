namespace YesChef.Stations
{
    /// <summary>What interacting would do right now, or why it can't be used.</summary>
    public readonly struct InteractionPrompt
    {
        private InteractionPrompt(InteractionKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public InteractionKind Kind { get; }
        public string Text { get; }
        public bool IsAvailable => Kind != InteractionKind.None;

        public static InteractionPrompt Available(InteractionKind kind, string text) => new InteractionPrompt(kind, text);

        public static InteractionPrompt Blocked(string reason) => new InteractionPrompt(InteractionKind.None, reason);
    }
}
