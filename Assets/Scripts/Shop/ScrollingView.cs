using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingView : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentIndex = 0;
    private int totalItems;

    private void Start()
    {
        StartCoroutine(lol());

        nextButton.onClick.AddListener(GoNext);
        previousButton.onClick.AddListener(GoPrevious);
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
        {
            return;
        }

        RectTransform item = content.GetChild(currentIndex) as RectTransform;

        float viewportCenter = viewport.rect.width * 0.5f;

        float itemCenter =
            item.anchoredPosition.x +
            (item.rect.width * item.pivot.x);

        Vector2 newPosition = content.anchoredPosition;
        newPosition.x = viewportCenter - itemCenter + 170;

        content.anchoredPosition = newPosition;
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

        SnapToCurrentItem();
        UpdateButtons();
    }
}