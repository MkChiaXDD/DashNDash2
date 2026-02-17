using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    private ShopData data;
    private ShopManager manager;

    public void Setup(ShopData data, ShopManager manager,bool canBuy)
    {
        this.data = data;
        this.manager = manager;

        nameText.text = data.itemName;
        iconImage.sprite = data.itemSprite;
        priceText.text = data.itemPrice.ToString();

        buyButton.interactable = canBuy;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void OnBuyClicked()
    {
        manager.TryBuy(data);
    }
}
