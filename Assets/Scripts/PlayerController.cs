using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
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

    private Rigidbody2D _rb;
    private Vector2 aimDir = Vector2.right;
    private bool isDashing;
    private float normalGrav;

    private Vector2 DashDist => speedManager != null
        ? speedManager.GetDashDistance()
        : Vector2.zero;

    void Start()
    {
        // scale the player sprite relative to screen height
        transform.ScaleToScreenHeightPercent(scalePercent);

        _rb = GetComponent<Rigidbody2D>();
        normalGrav = _rb.gravityScale;

        // if you didn�t drag DashManager in, try to find one
        if (dashMgr == null)
            dashMgr = FindFirstObjectByType<DashManager>();
    }

    void Update()
    {
        if (isDashing) return;

        Vector2 inputDir = Vector2.zero;

        // 1) Joystick has priority
        if (joystick != null && joystick.Direction.sqrMagnitude > 0.01f)
        {
            inputDir = joystick.Direction;
        }
        else
        {
            // 2) Fallback to mouse/touch
            Vector3 mw = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldMouse = new Vector2(mw.x, mw.y);
            inputDir = (worldMouse - _rb.position).normalized;
        }

        // update aim only if there�s meaningful input
        if (inputDir.sqrMagnitude > 0.001f)
            aimDir = inputDir;

        // dash on space
        if (Input.GetKeyDown(KeyCode.Space) && dashMgr.GetDashCount() > 0)
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
        Vector2 end = start + aimDir * DashDist.x; // same X/Y distance
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

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || speedManager == null) return;

        Gizmos.color = Color.green;
        Vector3 from = transform.position;
        Vector3 to = from + (Vector3)aimDir * DashDist.x;
        Gizmos.DrawLine(from, to);
        Gizmos.DrawSphere(to, 0.1f);
    }
}
