using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement & Dash")]
    [SerializeField] private SpeedManager speedManager;
    [SerializeField] private DashManager dashMgr;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float rotationOffset;

    [Header("UI Controls")]
    [Tooltip("On-screen joystick (optional) for aiming")]
    [SerializeField] private Joystick joystick;

    [Header("Scaling")]
    [SerializeField, Range(0.01f, 1f)] private float scalePercent = 0.1f;

    [Header("Aim Indicator (Rectangle)")]
    [SerializeField] private Transform aimIndicator; // long rectangle
    [SerializeField] private float aimThickness = 0.15f;

    private Rigidbody2D rb;
    private Vector2 aimDir = Vector2.right;
    private bool isDashing;
    private float normalGrav;

    private Vector2 DashDist => speedManager != null
        ? speedManager.GetDashDistance()
        : Vector2.zero;

    // ---------------- UNITY ----------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        normalGrav = rb.gravityScale;
    }

    private void Start()
    {
        transform.ScaleToScreenWidthPercent(scalePercent);

        if (dashMgr == null)
            dashMgr = FindFirstObjectByType<DashManager>();
    }

    private void Update()
    {
        if (isDashing) return;

        UpdateAimDirection();
        RotateTowardsAim();
        UpdateAimIndicator();

        // Keyboard dash
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            dashMgr.GetDashCount() > 0)
        {
            StartCoroutine(Dash());
            dashMgr.UseDash();
        }
    }

    // ---------------- INPUT ----------------

    private void UpdateAimDirection()
    {
        Vector2 inputDir = Vector2.zero;

        // Joystick first (mobile)
        if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
        {
            inputDir = joystick.Direction;
        }
        // Pointer fallback (mouse / touch)
        else if (Pointer.current != null)
        {
            Vector2 screenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane)
            );

            inputDir = (Vector2)worldPos - rb.position;
        }

        if (inputDir.sqrMagnitude > 0.001f)
            aimDir = inputDir.normalized;
    }

    // ---------------- AIM ----------------

    private void RotateTowardsAim()
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    private void UpdateAimIndicator()
    {
        if (aimIndicator == null || joystick == null)
            return;

        bool joystickActive = joystick.Direction.sqrMagnitude > 0.01f;

        // Show ONLY while joystick is being used
        aimIndicator.gameObject.SetActive(joystickActive);
    }

    // ---------------- DASH ----------------

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
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        Vector2 start = rb.position;
        Vector2 end = start + aimDir * DashDist.x;

        float t = 0f;
        while (t < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(start, end, t / dashDuration));
            t += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(end);

        rb.gravityScale = normalGrav;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    // ---------------- CLEANUP ----------------

    private void OnDisable()
    {
        if (aimIndicator != null)
            aimIndicator.gameObject.SetActive(false);
    }
}
