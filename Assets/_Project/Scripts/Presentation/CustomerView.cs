using UnityEngine;
using YesChef.Ingredients;
using YesChef.Orders;

namespace YesChef.Presentation
{
    /// <summary>
    /// The customer at a window: pops in with a new order, reddens as they wait, and hops off happily once served.
    /// </summary>
    public sealed class CustomerView : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private CustomerWindow window;
        [SerializeField] private GameObject customer;
        [SerializeField] private TransformTweener tweener;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private Color calmColor = new Color(0.3f, 0.55f, 0.9f);
        [SerializeField] private Color impatientColor = new Color(0.9f, 0.2f, 0.15f);
        [SerializeField, Min(0f)] private float leaveDelaySeconds = 0.5f;

        private MaterialPropertyBlock propertyBlock;
        private float leaveTimer;

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            customer.SetActive(false);
        }

        private void OnEnable()
        {
            window.OrderAssigned += HandleOrderAssigned;
            window.IngredientServed += HandleIngredientServed;
            window.OrderFulfilled += HandleOrderFulfilled;
        }

        private void OnDisable()
        {
            window.OrderAssigned -= HandleOrderAssigned;
            window.IngredientServed -= HandleIngredientServed;
            window.OrderFulfilled -= HandleOrderFulfilled;
        }

        private void Update()
        {
            if (leaveTimer > 0f)
            {
                leaveTimer -= Time.deltaTime;
                if (leaveTimer <= 0f && window.CurrentOrder == null)
                    customer.SetActive(false);
                return;
            }

            Order order = window.CurrentOrder;
            if (order == null)
                return;

            float impatience = Mathf.Clamp01(order.ElapsedSeconds / Mathf.Max(1, order.BaseValue));
            propertyBlock.SetColor(BaseColorId, Color.Lerp(calmColor, impatientColor, impatience));
            bodyRenderer.SetPropertyBlock(propertyBlock);
        }

        private void HandleOrderAssigned(Order order)
        {
            leaveTimer = 0f;
            customer.SetActive(true);
            tweener.PopIn();
        }

        private void HandleIngredientServed(IngredientDefinition ingredient) => tweener.Punch(0.15f);

        private void HandleOrderFulfilled(CustomerWindow fulfilledWindow)
        {
            tweener.Punch(0.4f);
            leaveTimer = leaveDelaySeconds;
        }
    }
}
