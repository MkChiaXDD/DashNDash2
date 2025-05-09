using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private CollectibleData data;
    [SerializeField] private float scalePercent = 0.05f;
    private CollectibleSpawner spawner;

    private void Start()
    {
        spawner = FindFirstObjectByType<CollectibleSpawner>();
        transform.ScaleToScreenPercent(scalePercent, scalePercent);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var dashMgr = other.GetComponent<DashManager>();

        dashMgr.IncreaseDash(data.changeAmount);

        spawner.ReturnToPool(gameObject);
    }
}
