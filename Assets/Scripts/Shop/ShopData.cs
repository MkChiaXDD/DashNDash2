using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Scriptable Objects/ShopData")]
public class ShopData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public Sprite itemSprite;
    public int itemPrice;
}
