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

    // remember floor’s scene position so we never move it
    private Vector3 _floorOriginalPos;

    void Awake()
    {
        if (floor)
            _floorOriginalPos = floor.transform.position;
    }

    void OnValidate() => UpdateBorders();
    void Start() => UpdateBorders();
    void LateUpdate() { if (Application.isPlaying) UpdateBorders(); }

    private void UpdateBorders()
    {
        var cam = Camera.main;
        if (cam == null) return;

        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;
        float fieldW = worldW * fieldWidthPercent;
        Vector3 camPos = cam.transform.position;

        // —— FLOOR —— (only scale X, never move)
        if (floor)
        {
            var bc = floor.GetComponent<BoxCollider2D>();
            if (bc)
            {
                Vector3 ls = floor.transform.localScale;
                ls.x = fieldW / bc.size.x;
                floor.transform.localScale = ls;

                // restore original position
                floor.transform.position = _floorOriginalPos;
            }
        }

        // —— LEFT WALL ——
        if (leftWall)
        {
            var bc = leftWall.GetComponent<BoxCollider2D>();
            if (bc)
            {
                Vector3 ls = leftWall.transform.localScale;
                ls.y = worldH / bc.size.y;
                leftWall.transform.localScale = ls;

                float halfW = (bc.size.x * ls.x) / 2f;
                float x = camPos.x - fieldW / 2f - halfW;
                leftWall.transform.position = new Vector3(x, camPos.y, leftWall.transform.position.z);
            }
        }

        // —— RIGHT WALL ——
        if (rightWall)
        {
            var bc = rightWall.GetComponent<BoxCollider2D>();
            if (bc)
            {
                Vector3 ls = rightWall.transform.localScale;
                ls.y = worldH / bc.size.y;
                rightWall.transform.localScale = ls;

                float halfW = (bc.size.x * ls.x) / 2f;
                float x = camPos.x + fieldW / 2f + halfW;
                rightWall.transform.position = new Vector3(x, camPos.y, rightWall.transform.position.z);
            }
        }
    }
}
