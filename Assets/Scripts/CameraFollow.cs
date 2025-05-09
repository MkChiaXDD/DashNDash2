using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Tooltip("The transform of your player")]
    [SerializeField] private Transform target;
    [Tooltip("How quickly the camera catches up")]
    [SerializeField] private float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private float highestY;

    void Start()
    {
        // start at the camera’s initial Y
        highestY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // only update if the player has moved above our previous high-water mark
        if (target.position.y > highestY)
            highestY = target.position.y;

        // lock X to 0, Y to that highestY, keep Z
        Vector3 desiredPos = new Vector3(0f, highestY, transform.position.z);

        // smooth follow
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smoothTime
        );
    }
}
