using UnityEngine;

namespace YesChef.Orders
{
    /// <summary>
    /// Strategy for creating orders. Assign a different generator asset to the OrderManager to change how
    /// orders are produced (tutorial scripts, difficulty ramps) without changing the manager.
    /// </summary>
    public abstract class OrderGenerator : ScriptableObject
    {
        public abstract Order CreateOrder();
    }
}
