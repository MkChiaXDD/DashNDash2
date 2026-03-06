using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkinData
{
    public int index;
    public Animation skinPrefab;
}

public class LoadSkin : MonoBehaviour
{
    [SerializeField] private List<SkinData> skinDatas;
}
