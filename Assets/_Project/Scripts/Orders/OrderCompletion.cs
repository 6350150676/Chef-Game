namespace YesChef.Orders
{
    public readonly struct OrderCompletion
    {
        public OrderCompletion(int windowIndex, Order order, int points)
        {
            WindowIndex = windowIndex;
            Order = order;
            Points = points;
        }

        public int WindowIndex { get; }
        public Order Order { get; }
        public int Points { get; }
    }
}
