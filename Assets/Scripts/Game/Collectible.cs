using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private CollectibleData data;
    [SerializeField] private float scalePercent = 0.05f;

    private PooledObject pooledObject;

    private void Awake()
    {
        pooledObject = GetComponent<PooledObject>();
        transform.ScaleToScreenPercent(scalePercent, scalePercent);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("Collect");

        var dashMgr = other.GetComponent<DashManager>();
        if (dashMgr != null)
            dashMgr.IncreaseDash(data.changeAmount);

        pooledObject.ReturnToPool();
    }
}
