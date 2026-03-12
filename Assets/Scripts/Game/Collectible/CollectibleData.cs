using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectibleData", menuName = "Game/Collectible Data")]
public class CollectibleData : ScriptableObject
{
    public string collectibleName;
    public int changeAmount;
}
