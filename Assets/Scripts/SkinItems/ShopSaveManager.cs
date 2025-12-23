using UnityEngine;

public static class ShopSaveManager
{
    private const string SHOP_KEY = "SHOP_DATA";

    public static ShopSaveData Load()
    {
        //For first launch
        if (!PlayerPrefs.HasKey(SHOP_KEY))
        {
            ShopSaveData data = new ShopSaveData();
            data.unlockedSkins.Add("default");
            data.equippedSkinId = "default";
            Save(data);
            return data;
        }

        string json = PlayerPrefs.GetString(SHOP_KEY);
        ShopSaveData loadedData = JsonUtility.FromJson<ShopSaveData>(json);
        return loadedData;
    }

    public static void Save(ShopSaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SHOP_KEY, json);
        PlayerPrefs.Save();
    }
}
