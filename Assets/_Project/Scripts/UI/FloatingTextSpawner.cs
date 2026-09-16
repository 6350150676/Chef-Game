using UnityEngine;
using UnityEngine.Pool;

namespace YesChef.UI
{
    /// <summary>Spawns pooled <see cref="FloatingTextUI"/> callouts over world positions.</summary>
    public sealed class FloatingTextSpawner : MonoBehaviour
    {
        [SerializeField] private FloatingTextUI prefab;
        [SerializeField, Min(1)] private int maxPooled = 12;

        private ObjectPool<FloatingTextUI> pool;
        private Canvas rootCanvas;
        private Camera worldCamera;

        private void Awake()
        {
            rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
            worldCamera = Camera.main;
            pool = new ObjectPool<FloatingTextUI>(
                CreateItem,
                item => item.gameObject.SetActive(true),
                item => item.gameObject.SetActive(false),
                item => Destroy(item.gameObject),
                collectionCheck: false,
                defaultCapacity: 4,
                maxSize: maxPooled);
        }

        public void Spawn(Vector3 worldPosition, string text, Color color) => pool.Get().Play(worldPosition, text, color);

        private FloatingTextUI CreateItem()
        {
            FloatingTextUI item = Instantiate(prefab, transform);
            item.Initialize(rootCanvas, worldCamera, pool.Release);
            return item;
        }
    }
}
