using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Skin")]
public class SkinData : ScriptableObject
{
    public string skinId;                 // "red_ship", "neon_cat"
    public Sprite sprite;
    public ParticleSystem trailPrefab;
    public int price;
}
