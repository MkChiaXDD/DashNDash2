using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement & Dash")]
    [SerializeField] private SpeedManager speedManager;
    [SerializeField] private DashManager dashMgr;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("UI Controls")]
    [Tooltip("Optional on-screen joystick for aiming")]
    [SerializeField] private Joystick joystick;

    [Header("Scaling")]
    [SerializeField, Range(0.01f, 1f)] private float scalePercent = 0.1f;

    [Header("Aim Line")]
    [Tooltip("LineRenderer to draw the aim line")]
    [SerializeField] private LineRenderer aimLine;
    [SerializeField, Tooltip("Width of the aim line")] private float aimLineWidth = 0.05f;
    [SerializeField, Tooltip("Color of the aim line")] private Color aimLineColor = Color.green;

    private Rigidbody2D _rb;
    private Vector2 aimDir = Vector2.right;
    private bool isDashing;
    private float normalGrav;

    private Vector2 DashDist => speedManager != null ? speedManager.GetDashDistance() : Vector2.zero;

    void Awake()
    {
        // Ensure LineRenderer is set up
        if (aimLine == null)
            aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.useWorldSpace = true;
        aimLine.startWidth = aimLineWidth;
        aimLine.endWidth = aimLineWidth;
        aimLine.startColor = aimLineColor;
        aimLine.endColor = aimLineColor;
    }

    void Start()
    {
        // scale the player
        transform.ScaleToScreenWidthPercent(scalePercent);

        _rb = GetComponent<Rigidbody2D>();
        normalGrav = _rb.gravityScale;

        if (dashMgr == null)
            dashMgr = FindFirstObjectByType<DashManager>();
    }

    void Update()
    {
        if (isDashing) return;

        // Determine input direction
        Vector2 inputDir = Vector2.zero;
        if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
        {
            inputDir = joystick.Direction;
        }
        else
        {
            Vector3 mw = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            inputDir = ((Vector2)mw - _rb.position).normalized;
        }

        if (inputDir.sqrMagnitude > 0.001f)
            aimDir = inputDir;

        // update runtime aim line
        Vector3 from = transform.position;
        Vector3 to = from + (Vector3)aimDir * DashDist.x;
        aimLine.SetPosition(0, from);
        aimLine.SetPosition(1, to);
    }

    public void OnButtonDash()
    {
        if (dashMgr.GetDashCount() > 0)
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
        if (aimLine != null)
            aimLine.enabled = false;
    }
}
