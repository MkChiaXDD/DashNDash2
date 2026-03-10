using UnityEngine;

public class PlayerSkinApplier : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform trailSocket;

    private GameObject currentTrail;

    public void Apply(ShopData skin)
    {
        if (skin == null) return;

        // Apply animator override
        if (skin.animatorOverride != null)
            animator.runtimeAnimatorController = skin.animatorOverride;

        // Replace trail
        if (currentTrail != null)
            Destroy(currentTrail);

        if (skin.trailPrefab != null && trailSocket != null)
        {
            currentTrail = Instantiate(skin.trailPrefab, trailSocket);
            currentTrail.transform.localPosition = Vector3.zero;
            currentTrail.transform.localRotation = Quaternion.identity;
        }
    }
}
