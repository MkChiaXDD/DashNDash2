using UnityEngine;

[ExecuteAlways]
public class BorderController : MonoBehaviour
{
    [Header("Assign border GameObjects (must have BoxCollider2D)")]
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject floor;

    [Header("Playfield width % of screen width (0–1)")]
    [Range(0f, 1f)]
    [SerializeField] private float fieldWidthPercent = 0.75f;

    private void OnValidate()
    {
        UpdateBorders();
    }

    private void Start()
    {
        UpdateBorders();
    }

    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            UpdateBorders();
        }
    }

    private void UpdateBorders()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;
        float fieldW = worldW * fieldWidthPercent;

        Vector3 camPos = cam.transform.position;

        // FLOOR
        if (floor != null)
        {
            BoxCollider2D bc = floor.GetComponent<BoxCollider2D>();

            if (bc != null)
            {
                Vector3 ls = floor.transform.localScale;
                ls.x = fieldW / bc.size.x;
                floor.transform.localScale = ls;

                floor.transform.position = new Vector3(
                    camPos.x,
                    floor.transform.position.y,
                    floor.transform.position.z
                );
            }
        }

        // LEFT WALL
        if (leftWall != null)
        {
            BoxCollider2D bc = leftWall.GetComponent<BoxCollider2D>();

            if (bc != null)
            {
                Vector3 ls = leftWall.transform.localScale;
                ls.y = worldH / bc.size.y;
                leftWall.transform.localScale = ls;

                float x = camPos.x - fieldW * 0.5f;

                leftWall.transform.position = new Vector3(
                    x,
                    camPos.y,
                    leftWall.transform.position.z
                );
            }
        }

        // RIGHT WALL
        if (rightWall != null)
        {
            BoxCollider2D bc = rightWall.GetComponent<BoxCollider2D>();

            if (bc != null)
            {
                Vector3 ls = rightWall.transform.localScale;
                ls.y = worldH / bc.size.y;
                rightWall.transform.localScale = ls;

                float x = camPos.x + fieldW * 0.5f;

                rightWall.transform.position = new Vector3(
                    x,
                    camPos.y,
                    rightWall.transform.position.z
                );
            }
        }
    }
}