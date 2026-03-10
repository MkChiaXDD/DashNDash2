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
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("InventoryUI")]
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform inventoryItemParent;

    private int coins;

    private HashSet<int> ownedItemIDs = new HashSet<int>();

    private const string EquippedSkinKey = "EQUIPPED_SKIN_ID";
    private int equippedSkinID = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadOwned();
        LoadCoins();
        LoadEquippedSkin();
        RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddCoins(5);
        }
    }

    public void TryBuy(ShopData item)
    {
        if (ownedItemIDs.Contains(item.itemID))
            return;

        if (coins < item.itemPrice)
            return;

        coins -= item.itemPrice;
        PlayerPrefs.SetInt(CoinsKey, coins);
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
        // Build inventory list (only owned)
        foreach (var item in shopItems)
        {
            if (!ownedItemIDs.Contains(item.itemID))
                continue;

            var go = Instantiate(inventoryItemPrefab, inventoryItemParent);
            var ui = go.GetComponent<InventoryItemUI>();

            bool isEquipped = (item.itemID == equippedSkinID);
            ui.Setup(item, this, isEquipped);
        }

        // Refresh coin display
        coinText.text = coins.ToString();
    }

    private void LoadEquippedSkin()
    {
        equippedSkinID = PlayerPrefs.GetInt(EquippedSkinKey, -1);
    }

    public void Equip(ShopData item)
    {
        // only allow equip if owned
        if (!ownedItemIDs.Contains(item.itemID))
            return;

        equippedSkinID = item.itemID;
        PlayerPrefs.SetInt(EquippedSkinKey, equippedSkinID);
        PlayerPrefs.Save();

        // Apply immediately if player exists in this scene (shop+game same scene)
        var applier = FindFirstObjectByType<PlayerSkinApplier>();
        if (applier != null)
            applier.Apply(item);

        RefreshUI();
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    private const string OwnedKey = "OWNED_ITEM_IDS";
    private const string CoinsKey = "Coins";

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

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt(CoinsKey, 0);
    }

    private void AddCoins(int amount)
    {
        coins += amount;

        // save
        PlayerPrefs.SetInt(CoinsKey, coins);
        PlayerPrefs.Save();

        // UI update (if you want it to reflect immediately)
        if (coinText != null)
            coinText.text = coins.ToString();

        Debug.Log($"Added {amount} coins. Total coins = {coins}");
    }
}


