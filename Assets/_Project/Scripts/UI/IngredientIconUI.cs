using UnityEngine;
using UnityEngine.UI;
using YesChef.Ingredients;

namespace YesChef.UI
{
    public sealed class IngredientIconUI : MonoBehaviour
    {
        [Tooltip("Dimmed once the ingredient is delivered. The delivered mark sits outside it and stays bright.")]
        [SerializeField] private CanvasGroup content;
        [SerializeField] private Image tile;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject deliveredMark;
        [SerializeField, Range(0f, 1f)] private float deliveredAlpha = 0.45f;
        [Tooltip("How far the ingredient's UI color is washed toward white behind the icon.")]
        [SerializeField, Range(0f, 1f)] private float tileWash = 0.55f;

        public void Show(IngredientDefinition ingredient, bool isDelivered)
        {
            tile.color = Color.Lerp(ingredient.UIColor, Color.white, tileWash);
            icon.sprite = ingredient.Icon;
            SetDelivered(isDelivered);
        }

        public void SetDelivered(bool isDelivered)
        {
            content.alpha = isDelivered ? deliveredAlpha : 1f;
            if (deliveredMark.activeSelf != isDelivered)
                deliveredMark.SetActive(isDelivered);
        }
    }
}
