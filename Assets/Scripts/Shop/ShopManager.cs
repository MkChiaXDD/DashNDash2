using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<ShopData> shopItems;

    [Header("Shop UI")]
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Transform shopItemParent;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform inventoryItemParent;

    [Header("Base Skin")]
    [SerializeField] private ShopData baseSkin;

    private int coins;
    private HashSet<int> ownedItemIDs = new HashSet<int>();

    private const string OwnedKey = "OWNED_ITEM_IDS";
    private const string CoinsKey = "Coins";
    private const string EquippedSkinKey = "EQUIPPED_SKIN_ID";

    private int equippedSkinID = -1;

    void Start()
    {
        LoadOwned();
        LoadCoins();

        if (baseSkin == null)
        {
            Debug.LogError("baseSkin is NOT assigned in Inspector.");
            return;
        }

        if (!shopItems.Contains(baseSkin))
            shopItems.Add(baseSkin);

        ownedItemIDs.Add(baseSkin.itemID);

        int equipped = PlayerPrefs.GetInt(EquippedSkinKey, -1);
        if (equipped == -1)
        {
            PlayerPrefs.SetInt(EquippedSkinKey, baseSkin.itemID);
            PlayerPrefs.Save();
            equipped = baseSkin.itemID;
        }

        equippedSkinID = equipped;

        SaveOwned();
        RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            AddCoins(5);
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

    public void Equip(ShopData item)
    {
        if (!ownedItemIDs.Contains(item.itemID))
            return;

        equippedSkinID = item.itemID;
        PlayerPrefs.SetInt(EquippedSkinKey, equippedSkinID);
        PlayerPrefs.Save();

        var applier = FindFirstObjectByType<PlayerSkinApplier>();
        if (applier != null)
            applier.Apply(item);

        RefreshUI();
    }

    private void RefreshUI()
    {
        ClearChildren(shopItemParent);
        ClearChildren(inventoryItemParent);

        foreach (var item in shopItems)
        {
            if (ownedItemIDs.Contains(item.itemID))
                continue;

            var go = Instantiate(shopItemPrefab, shopItemParent);
            var ui = go.GetComponent<ShopItemUI>();
            ui.Setup(item, this, coins >= item.itemPrice);
        }

        foreach (var item in shopItems)
        {
            if (!ownedItemIDs.Contains(item.itemID))
                continue;

            var go = Instantiate(inventoryItemPrefab, inventoryItemParent);
            var ui = go.GetComponent<InventoryItemUI>();
            ui.Setup(item, this, item.itemID == equippedSkinID);
        }

        if (coinText != null)
            coinText.text = coins.ToString();
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    private void SaveOwned()
    {
        string s = string.Join(",", new List<int>(ownedItemIDs));
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
            if (int.TryParse(p, out int id))
                ownedItemIDs.Add(id);
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt(CoinsKey, 0);
    }

    private void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt(CoinsKey, coins);
        PlayerPrefs.Save();

        if (coinText != null)
            coinText.text = coins.ToString();

        Debug.Log($"Added {amount} coins. Total coins = {coins}");
    }
}