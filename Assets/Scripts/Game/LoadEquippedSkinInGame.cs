using UnityEngine;

public class LoadEquippedSkinInGame : MonoBehaviour
{
    [SerializeField] private ShopData[] allSkins;      // drag all skin ShopData assets here
    [SerializeField] private PlayerSkinApplier applier;

    private const string EquippedSkinKey = "EQUIPPED_SKIN_ID";

    void Start()
    {
        int id = PlayerPrefs.GetInt(EquippedSkinKey, -1);

        ShopData chosen = null;
        for (int i = 0; i < allSkins.Length; i++)
        {
            if (allSkins[i] != null && allSkins[i].itemID == id)
            {
                chosen = allSkins[i];
                break;
            }
        }

        // fallback if nothing equipped
        if (chosen == null && allSkins.Length > 0)
            chosen = allSkins[0];

        applier.Apply(chosen);
    }
}