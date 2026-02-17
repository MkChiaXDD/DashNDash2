using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;

    public void Setup(ShopData data)
    {
        nameText.text = data.itemName;
        iconImage.sprite = data.itemSprite;
    }
}
