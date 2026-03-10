using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;

    [Header("Equip UI")]
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipButtonText; // optional
    [SerializeField] private GameObject equippedTag;   // optional (a small "Equipped" badge)

    private ShopData data;
    private ShopManager manager;

    public void Setup(ShopData data, ShopManager manager, bool isEquipped)
    {
        this.data = data;
        this.manager = manager;

        nameText.text = data.itemName;
        iconImage.sprite = data.itemSprite;

        if (equippedTag != null) equippedTag.SetActive(isEquipped);

        if (equipButtonText != null)
            equipButtonText.text = isEquipped ? "Equipped" : "Equip";

        equipButton.interactable = !isEquipped;

        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(OnEquipClicked);
    }

    private void OnEquipClicked()
    {
        manager.Equip(data);
    }
}