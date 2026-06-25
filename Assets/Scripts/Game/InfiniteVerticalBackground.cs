using UnityEngine;

public class InfiniteVerticalBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform[] backgrounds;

    [Header("Settings")]
    [SerializeField] private float backgroundHeight = 10f;
    [SerializeField] private float parallaxSpeed = 0.5f;

    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(
            0f,
            cameraDelta.y * parallaxSpeed,
            0f
        );

        lastCameraPosition = cameraTransform.position;

        LoopBackgrounds();
    }

    private void LoopBackgrounds()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (cameraTransform.position.y - backgrounds[i].position.y > backgroundHeight)
            {
                backgrounds[i].position += Vector3.up * backgroundHeight * backgrounds.Length;
            }
            else if (backgrounds[i].position.y - cameraTransform.position.y > backgroundHeight)
            {
                backgrounds[i].position -= Vector3.up * backgroundHeight * backgrounds.Length;
            }
        }
    }
}