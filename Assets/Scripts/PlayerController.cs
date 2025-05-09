using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement & Dash")]
    [SerializeField] private SpeedManager speedManager;
    [SerializeField] private DashManager dashMgr;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("UI Controls")]
    [Tooltip("On-screen joystick (optional) for aiming")]
    [SerializeField] private Joystick joystick;

    [Header("Scaling")]
    [SerializeField, Range(0.01f, 1f)] private float scalePercent = 0.1f;

    [Header("Aim Line")]
    [SerializeField] private LineRenderer aimLine;
    [SerializeField, Tooltip("Width of the aim line")] private float aimLineWidth = 0.05f;
    [SerializeField, Tooltip("Color of the aim line")] private Color aimLineColor = Color.green;
    [SerializeField, Tooltip("Dotted material (optional)")] private Material dottedMaterial;
    [SerializeField, Tooltip("Spacing for dotted line")] private float dotSpacing = 0.5f;

    private Rigidbody2D _rb;
    private Vector2 aimDir = Vector2.right;
    private bool isDashing;
    private float normalGrav;

    private Vector2 DashDist => speedManager != null ? speedManager.GetDashDistance() : Vector2.zero;

    void Awake()
    {
        // Initialize LineRenderer
        if (aimLine == null) aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.useWorldSpace = true;
        aimLine.startWidth = aimLineWidth;
        aimLine.endWidth = aimLineWidth;
        aimLine.startColor = aimLineColor;
        aimLine.endColor = aimLineColor;
        if (dottedMaterial != null)
        {
            aimLine.material = dottedMaterial;
            aimLine.textureMode = LineTextureMode.Tile;
        }
    }

    void Start()
    {
        // Scale player based on screen width
        transform.ScaleToScreenWidthPercent(scalePercent);
        _rb = GetComponent<Rigidbody2D>();
        normalGrav = _rb.gravityScale;
        if (dashMgr == null) dashMgr = FindFirstObjectByType<DashManager>();
    }

    void Update()
    {
        if (isDashing) return;

        // Determine aim direction: joystick or pointer
        Vector2 inputDir = Vector2.zero;
        if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
        {
            inputDir = joystick.Direction;
        }
        else if (Pointer.current != null)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            // Provide a valid Z distance
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));
            inputDir = ((Vector2)worldPos - _rb.position);
        }
        if (inputDir.sqrMagnitude > 0.001f)
            aimDir = inputDir.normalized;

        // Update aim line
        Vector3 from = transform.position;
        Vector3 to = from + (Vector3)aimDir * DashDist.x;
        aimLine.SetPosition(0, from);
        aimLine.SetPosition(1, to);
        if (dottedMaterial != null && aimLine.textureMode == LineTextureMode.Tile)
            aimLine.material.mainTextureScale = new Vector2(DashDist.x / dotSpacing, 1f);

        // Dash on space or UI button
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && dashMgr.GetDashCount() > 0)
        {
            StartCoroutine(Dash());
            dashMgr.UseDash();
        }
    }

    public void OnButtonDash()
    {
        if (dashMgr.GetDashCount() > 0 && !isDashing)
        {
            StartCoroutine(Dash());
            dashMgr.UseDash();
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        _rb.gravityScale = 0;
        _rb.linearVelocity = Vector2.zero;

        Vector2 start = _rb.position;
        Vector2 end = start + aimDir * DashDist.x;
        float t = 0f;

        while (t < dashDuration)
        {
            _rb.MovePosition(Vector2.Lerp(start, end, t / dashDuration));
            t += Time.deltaTime;
            yield return null;
        }
        _rb.MovePosition(end);

        _rb.gravityScale = normalGrav;
        _rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    void OnDisable()
    {
        if (aimLine != null) aimLine.enabled = false;
    }
}
