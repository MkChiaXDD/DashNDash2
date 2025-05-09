using UnityEngine;
using UnityEngine.UI;

public class DashManager : MonoBehaviour
{
    [SerializeField] private Slider dashDisplay;
    [SerializeField] private int maxDashes = 3;
    public int currentDashes;

    void Start()
    {
        currentDashes = maxDashes;
        UpdateDashDisplay();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            IncreaseDash(1);
            UpdateDashDisplay();
        }
    }

    public void IncreaseDash(int increaseAmt)
    {
        currentDashes = Mathf.Min(currentDashes + increaseAmt, maxDashes);
        UpdateDashDisplay();
    }

    public void UseDash()
    {
        if (currentDashes > 0)
            currentDashes--;
        UpdateDashDisplay();
    }

    private void UpdateDashDisplay()
    {
        // Update slider range and value
        dashDisplay.minValue = 0f;
        dashDisplay.maxValue = maxDashes;
        dashDisplay.value = currentDashes;

        // Parent rect for tick size
        RectTransform sliderRect = dashDisplay.GetComponent<RectTransform>();

        // Remove old ticks
        for (int i = sliderRect.childCount - 1; i >= 0; i--)
        {
            var child = sliderRect.GetChild(i);
            if (child.name.StartsWith("Tick_"))
                Destroy(child.gameObject);
        }

        // Spawn new ticks on top, half-height of the slider
        for (int i = 1; i < maxDashes; i++)
        {
            float norm = (float)i / maxDashes;
            GameObject tick = new GameObject("Tick_" + i, typeof(RectTransform), typeof(Image));
            tick.transform.SetParent(sliderRect, false);
            tick.transform.SetAsLastSibling();

            var img = tick.GetComponent<Image>();
            img.color = Color.black;

            var rt = tick.GetComponent<RectTransform>();
            // Position at normalized X, vertically centered and half the parent's height
            rt.anchorMin = new Vector2(norm, 0.25f);
            rt.anchorMax = new Vector2(norm, 0.75f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(2, 0);
        }
    }

    public int GetDashCount()
    {
        return currentDashes;
    }
}

//hello
