namespace YesChef.Ingredients
{
    /// <summary>How an ingredient must be processed before it can be served. Stations declare which method they perform.</summary>
    public enum PreparationMethod
    {
        None,
        Chop,
        Cook
    }
}
