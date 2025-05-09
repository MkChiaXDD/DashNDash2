using UnityEngine;
using UnityEngine.UI;

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


    public float Horizontal => input.x;
    public float Vertical => input.y;
    public Vector2 Direction => new Vector2(Horizontal, Vertical);

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        backgroundImages = background.GetComponentsInChildren<Image>(true);
        handleImages = handle.GetComponentsInChildren<Image>(true);
        SetJoystickVisible(false);
    }

    private void Update()

    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPos = Input.mousePosition;
            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPos);

            background.anchoredPosition = localPos;
            SetJoystickVisible(true);
            isDragging = true;
            UpdateInput(localPos);
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 screenPos = Input.mousePosition;
            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPos);

            UpdateInput(localPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            SetJoystickVisible(false);
            isDragging = false;


        }
    }

    private void UpdateInput(Vector2 localPos)

    {
        input = localPos / handleRange;
        input = (input.magnitude > 1f) ? input.normalized : input;
        handle.anchoredPosition = input * handleRange;
    }

    private void SetJoystickVisible(bool visible)

    {
        foreach (var img in backgroundImages)
            img.enabled = visible;

        foreach (var img in handleImages)
            img.enabled = visible;
    }
}