using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<SkinData> allSkins;
    [SerializeField] private Transform contentDrawer;
    [SerializeField] private GameObject shopItemPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadAllShopItems();
    }

    private void LoadAllShopItems()
    {
        for (int i = 0; i < allSkins.Count; i++)
        {
            GameObject newItem = Instantiate(shopItemPrefab, contentDrawer);
            TMP_Text itemName = newItem.transform.Find("ItemName").GetComponent<TMP_Text>();
            Image itemImage = newItem.transform.Find("ItemImage").GetComponent<Image>();
            TMP_Text itemPrice = newItem.transform.Find("ItemPrice").GetComponent<TMP_Text>();

            itemName.text = allSkins[i].skinId;
            itemImage.sprite = allSkins[i].sprite;
            itemPrice.text = $"{allSkins[i].price}";
        }
    }
}
