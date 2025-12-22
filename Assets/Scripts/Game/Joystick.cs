using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Joystick : MonoBehaviour
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private RectTransform background;
    [SerializeField] private float handleRange = 50f;

    private Vector2 input = Vector2.zero;
    private Canvas canvas;

    private Image[] backgroundImages;
    private Image[] handleImages;

    private bool isDragging = false;
    private Vector2 originalAnchoredPos;

    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => new Vector2(Horizontal, Vertical);

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        backgroundImages = background.GetComponentsInChildren<Image>(true);
        handleImages = handle.GetComponentsInChildren<Image>(true);

        // Save original joystick position
        originalAnchoredPos = background.anchoredPosition;

        SetJoystickVisible(true);
        ResetJoystick();
    }

    private void Update()
    {
        // Pointer DOWN ? teleport joystick
        if (IsPointerDownThisFrame())
        {
            Vector2 screenPos = GetPointerPosition();
            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPos);

            background.anchoredPosition = localPos;
            isDragging = true;
        }

        // Pointer HELD ? update direction
        if (isDragging && IsPointerDown())
        {
            Vector2 screenPos = GetPointerPosition();
            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPos);

            UpdateInput(localPos);
        }

        // Pointer UP ? reset joystick
        if (isDragging && IsPointerUpThisFrame())
        {
            ResetJoystick();
        }
    }

    private void UpdateInput(Vector2 localPos)
    {
        input = localPos / handleRange;
        input = input.magnitude > 1f ? input.normalized : input;
        handle.anchoredPosition = input * handleRange;
    }

    private void ResetJoystick()
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        background.anchoredPosition = originalAnchoredPos;
        isDragging = false;
    }

    private void SetJoystickVisible(bool visible)
    {
        foreach (var img in backgroundImages)
            img.enabled = visible;

        foreach (var img in handleImages)
            img.enabled = visible;
    }

    // ===== Input System helpers =====

    private bool IsPointerDownThisFrame()
    {
        var mouse = Mouse.current;
        var touch = Touchscreen.current;

        return (mouse != null && mouse.leftButton.wasPressedThisFrame) ||
               (touch != null && touch.primaryTouch.press.wasPressedThisFrame);
    }

    private bool IsPointerDown()
    {
        var mouse = Mouse.current;
        var touch = Touchscreen.current;

        return (mouse != null && mouse.leftButton.isPressed) ||
               (touch != null && touch.primaryTouch.press.isPressed);
    }

    private bool IsPointerUpThisFrame()
    {
        var mouse = Mouse.current;
        var touch = Touchscreen.current;

        return (mouse != null && mouse.leftButton.wasReleasedThisFrame) ||
               (touch != null && touch.primaryTouch.press.wasReleasedThisFrame);
    }

    private Vector2 GetPointerPosition()
    {
        var touch = Touchscreen.current;
        if (touch != null && touch.primaryTouch.press.isPressed)
            return touch.primaryTouch.position.ReadValue();

        var mouse = Mouse.current;
        if (mouse != null)
            return mouse.position.ReadValue();

        return Vector2.zero;
    }
}
