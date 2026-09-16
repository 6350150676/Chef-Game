namespace YesChef.Stations
{
    /// <summary>What an interaction would accomplish, so presentation can guide the player without knowing station types.</summary>
    public enum InteractionKind
    {
        /// <summary>Nothing would happen. The prompt text explains why.</summary>
        None,
        TakeIngredient,
        StartPreparing,
        CollectPrepared,
        Serve,
        Discard
    }
}
