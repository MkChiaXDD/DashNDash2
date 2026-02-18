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
        LoadOwned();
        RefreshUI();
    }

    public void TryBuy(ShopData item)
    {
        if (ownedItemIDs.Contains(item.itemID))
            return;

        if (coins < item.itemPrice)
            return;

        coins -= item.itemPrice;
        ownedItemIDs.Add(item.itemID);

        SaveOwned();
        RefreshUI();
    }

    private void RefreshUI()
    {
        ClearChildren(shopItemParent);
        ClearChildren(inventoryItemParent);

        // Build shop list (only NOT owned)
        foreach (var item in shopItems)
        {
            if (ownedItemIDs.Contains(item.itemID))
                continue;

            var go = Instantiate(shopItemPrefab, shopItemParent);
            var ui = go.GetComponent<ShopItemUI>();

            bool canBuy = coins >= item.itemPrice;
            ui.Setup(item, this, canBuy);
        }

        // Build inventory list (only owned)
        foreach (var item in shopItems)
        {
            if (!ownedItemIDs.Contains(item.itemID))
                continue;

            var go = Instantiate(inventoryItemPrefab, inventoryItemParent);
            var ui = go.GetComponent<InventoryItemUI>();
            ui.Setup(item);
        }
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    private const string OwnedKey = "OWNED_ITEM_IDS";

    private void SaveOwned()
    {
        // store as comma string: "1,4,7"
        var list = new List<int>(ownedItemIDs);
        string s = string.Join(",", list);
        PlayerPrefs.SetString(OwnedKey, s);
        PlayerPrefs.Save();
    }

    private void LoadOwned()
    {
        ownedItemIDs.Clear();

        string s = PlayerPrefs.GetString(OwnedKey, "");
        if (string.IsNullOrEmpty(s))
            return;

        string[] parts = s.Split(',');
        foreach (var p in parts)
        {
            if (int.TryParse(p, out int id))
                ownedItemIDs.Add(id);
        }
    }
}


