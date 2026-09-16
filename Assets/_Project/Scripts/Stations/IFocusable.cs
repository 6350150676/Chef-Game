namespace YesChef.Stations
{
    /// <summary>Receives notice when an interactor is targeting it (for highlighting or presence checks).</summary>
    public interface IFocusable
    {
        void SetFocused(bool isFocused);
    }
}
