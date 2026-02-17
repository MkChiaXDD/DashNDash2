using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<ShopData> shopItems;

    [Header("Shop UI")]
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Transform shopItemParent;

    [Header("InventoryUI")]
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform inventoryItemParent;

    [Header("Player Money")]
    [SerializeField] private int coins = 999;

    private HashSet<int> ownedItemIDs = new HashSet<int>();
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

    public void TryBuy(ShopData item)
    {
        // Already owned? stop
        if (ownedItemIDs.Contains(item.itemID))
            return;

        // Not enough money? stop
        if (coins < item.itemPrice)
            return;

        // Buy
        coins -= item.itemPrice;
        ownedItemIDs.Add(item.itemID);

        //SaveOwned();     // optional
        //RefreshUI();     // shop item disappears, inventory shows it
    }
}
