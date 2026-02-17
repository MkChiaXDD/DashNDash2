using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopData> shopItems;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Transform shopItemParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitShopItems();
    }

    private void InitShopItems()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            GameObject newItem = Instantiate(shopItemPrefab, shopItemParent);
            TMP_Text itemName = newItem.transform.Find("ItemName").GetComponent<TMP_Text>();
            itemName.text = $"{shopItems[i].itemName}";
            Image itemImage = newItem.transform.Find("ItemImage").GetComponent<Image>();
            itemImage.sprite = shopItems[i].itemSprite;
            TMP_Text itemPrice = newItem.transform.Find("BuyBtn/ItemPrice").GetComponent<TMP_Text>();
            itemPrice.text = $"{shopItems[i].itemPrice}";
        }
    }
}
