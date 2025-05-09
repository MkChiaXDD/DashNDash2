using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpeedManager speedManager;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float scalePercent = 0.1f;

    private Rigidbody2D _rb;
    private Vector2 aimDir = Vector2.right;
    private bool isDashing;
    private float normalGrav;

    private DashManager dashMgr;

    private Vector2 DashDist => speedManager != null
        ? speedManager.GetDashDistance()
        : Vector2.zero;

    void Start()
    {
        transform.ScaleToScreenHeightPercent(scalePercent);
        _rb = GetComponent<Rigidbody2D>();
        dashMgr = FindFirstObjectByType<DashManager>();
        normalGrav = _rb.gravityScale;
    }

    void Update()
    {
        if (isDashing) return;

        // aim at cursor/tap � do it in Vector2 so subtraction isn't ambiguous
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 myPosition = (Vector2)transform.position;
        Vector2 dir = mouseWorld - myPosition;
        if (dir.sqrMagnitude > 0.001f)
            aimDir = dir.normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (dashMgr.GetDashCount() > 0)
            {
                StartCoroutine(Dash());
                dashMgr.UseDash();
            }
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        _rb.gravityScale = 0;
        _rb.linearVelocity = Vector2.zero;

        Vector2 start = _rb.position;
        float dist = DashDist.x;        // same on X and Y
        Vector2 dashVec = aimDir * dist;
        Vector2 end = start + dashVec;
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

        Gizmos.color = Color.cyan;
        Vector3 from = transform.position;
        float dist = DashDist.x;
        Vector3 to = from + (Vector3)(aimDir * dist);
        Gizmos.DrawLine(from, to);
        Gizmos.DrawSphere(to, 0.1f);
    }
}
