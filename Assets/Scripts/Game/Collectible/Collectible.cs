using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private CollectibleData data;
    [SerializeField] private float scalePercent = 0.05f;
    [SerializeField] private float lifetime = 8f;

    private PooledObject pooledObject;
    private Coroutine lifetimeCoroutine;

    private void Awake()
    {
        pooledObject = GetComponent<PooledObject>();
        transform.ScaleToScreenPercent(scalePercent, scalePercent);
    }

    private void OnEnable()
    {
        lifetimeCoroutine = StartCoroutine(Lifetime());
    }

    private void OnDisable()
    {
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lifetime);

        pooledObject.ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("Collect");

        if (other.TryGetComponent(out DashManager dashMgr))
        {
            dashMgr.IncreaseDash(data.changeAmount);
        }

        pooledObject.ReturnToPool();
    }
}