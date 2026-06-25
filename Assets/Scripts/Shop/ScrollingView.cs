using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingView : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("Scrolling")]
    [SerializeField] private bool smoothScroll = true;
    [SerializeField] private float scrollSpeed = 8f;

    private int currentIndex = 0;
    private int totalItems;

    private Vector2 targetPosition;

    private void Start()
    {
        StartCoroutine(lol());

        nextButton.onClick.AddListener(GoNext);
        previousButton.onClick.AddListener(GoPrevious);
    }

    private void Update()
    {
        if (smoothScroll)
        {
            content.anchoredPosition = Vector2.Lerp(
                content.anchoredPosition,
                targetPosition,
                Time.deltaTime * scrollSpeed);

            if (Vector2.Distance(content.anchoredPosition, targetPosition) < 0.1f)
            {
                content.anchoredPosition = targetPosition;
            }
        }
    }

    private void GoNext()
    {
        if (currentIndex < totalItems - 1)
        {
            currentIndex++;
            SnapToCurrentItem();
        }

        UpdateButtons();
    }

    private void GoPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            SnapToCurrentItem();
        }

        UpdateButtons();
    }

    private void SnapToCurrentItem()
    {
        if (totalItems <= 0)
            return;

        RectTransform item = content.GetChild(currentIndex) as RectTransform;

        float viewportCenter = viewport.rect.width * 0.5f;

        float itemCenter =
            item.anchoredPosition.x +
            (item.rect.width * item.pivot.x);

        Vector2 newPosition = content.anchoredPosition;
        newPosition.x = viewportCenter - itemCenter + 190;

        if (smoothScroll)
        {
            targetPosition = newPosition;
        }
        else
        {
            content.anchoredPosition = newPosition;
            targetPosition = newPosition;
        }
    }

    private void UpdateButtons()
    {
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < totalItems - 1;

        Debug.Log($"Current Index: {currentIndex}");
        Debug.Log($"Total Items: {totalItems}");
    }

    private IEnumerator lol()
    {
        yield return new WaitForSeconds(0.2f);

        totalItems = content.childCount;

        targetPosition = content.anchoredPosition;

        SnapToCurrentItem();
        UpdateButtons();
    }
}