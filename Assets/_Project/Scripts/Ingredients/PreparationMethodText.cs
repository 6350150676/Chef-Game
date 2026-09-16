namespace YesChef.Ingredients
{
    /// <summary>Player-facing wording for each preparation method. Returns constants, so it's safe to call every frame.</summary>
    public static class PreparationMethodText
    {
        public static string Verb(this PreparationMethod method) => method switch
        {
            PreparationMethod.Chop => "Chop",
            PreparationMethod.Cook => "Cook",
            _ => string.Empty
        };

        public static string InProgress(this PreparationMethod method) => method switch
        {
            PreparationMethod.Chop => "Chopping",
            PreparationMethod.Cook => "Cooking",
            _ => string.Empty
        };

        public static string Done(this PreparationMethod method) => method switch
        {
            PreparationMethod.Chop => "Chopped",
            PreparationMethod.Cook => "Cooked",
            _ => string.Empty
        };

        public static string DoneCallout(this PreparationMethod method) => method switch
        {
            PreparationMethod.Chop => "Chopped!",
            PreparationMethod.Cook => "Cooked!",
            _ => string.Empty
        };

        public static string NeedsFirst(this PreparationMethod method) => method switch
        {
            PreparationMethod.Chop => "Needs chopping first",
            PreparationMethod.Cook => "Needs cooking first",
            _ => "Ready to serve"
        };

        /// <summary>Short tag for order tickets.</summary>
        public static string Tag(this PreparationMethod method) => method switch
        {
            PreparationMethod.Chop => "CHOP",
            PreparationMethod.Cook => "COOK",
            _ => "READY"
        };
    }
}
