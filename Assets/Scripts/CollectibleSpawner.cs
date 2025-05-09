using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player transform - initial spawn reference")]
    [SerializeField] private Transform player;
    [Tooltip("SpeedManager for consistent Y-offset across devices")]
    [SerializeField] private SpeedManager speedManager;

    [Header("Collectible Settings")]
    [Tooltip("Collectible prefabs to pool and spawn")]
    [SerializeField] private GameObject[] collectiblePrefabs;
    [Tooltip("Initial pool size per collectible type")]
    [SerializeField] private int poolSizePerType = 10;

    [Header("Playfield Settings")]
    [Tooltip("Playfield width % of screen width - collectibles will be clamped within this")]
    [Range(0f, 1f)]
    [SerializeField] private float fieldWidthPercent = 0.75f;

    [Header("Spawn Positioning")]
    [Tooltip("Horizontal random range per spawn based on screen width percentage (0–1)")]
    [Range(0f, 1f)]
    [SerializeField] private float spawnXPercent = 0.2f;
    [Tooltip("Number of discrete X slots across the range; reduces clustering")]
    [Range(2, 10)]
    [SerializeField] private int xSegments = 5;

    private List<GameObject>[] _pools;
    private Vector3[] _lastSpawnPos;
    private int[] _lastSlotIndex;
    private float _xRangeWorld;
    private float _yOffsetWorld;
    private float _halfField;
    private float _camX;

    void Awake()
    {
        if (speedManager == null)
            Debug.LogError("CollectibleSpawner: SpeedManager reference is missing.");

        // pre-calc world offsets
        _xRangeWorld = ScreenSizeHelper.PercentWidthToWorld(spawnXPercent);
        // use SpeedManager for vertical offset (consistent across devices)
        Vector2 dashDist = speedManager.GetDashDistance();
        _yOffsetWorld = dashDist.y * 0.8f;

        float screenW = ScreenSizeHelper.ScreenWorldWidth;
        _halfField = screenW * fieldWidthPercent * 0.5f;
        _camX = Camera.main.transform.position.x;

        int types = collectiblePrefabs.Length;
        _pools = new List<GameObject>[types];
        _lastSpawnPos = new Vector3[types];
        _lastSlotIndex = new int[types];

        for (int i = 0; i < types; i++)
        {
            _pools[i] = new List<GameObject>(poolSizePerType);
            _lastSpawnPos[i] = player != null ? player.position : Vector3.zero;
            _lastSlotIndex[i] = -1;

            for (int n = 0; n < poolSizePerType; n++)
            {
                GameObject go = Instantiate(collectiblePrefabs[i], transform);
                SpawnAtNextPosition(i, go);
                _pools[i].Add(go);
            }
        }
    }

    /// <summary>
    /// Calculates and applies the next spawn position for the given pool index and object.
    /// </summary>
    private void SpawnAtNextPosition(int poolIndex, GameObject go)
    {
        int idx;
        do { idx = Random.Range(0, xSegments); }
        while (idx == _lastSlotIndex[poolIndex]);
        _lastSlotIndex[poolIndex] = idx;

        float slotWidth = (_xRangeWorld * 2f) / (xSegments - 1);
        float offsetX = -_xRangeWorld + idx * slotWidth;

        Vector3 spawnPos = _lastSpawnPos[poolIndex] + Vector3.up * _yOffsetWorld;
        spawnPos.x += offsetX;
        spawnPos.x = Mathf.Clamp(spawnPos.x, _camX - _halfField, _camX + _halfField);

        go.transform.position = spawnPos;
        go.SetActive(true);
        _lastSpawnPos[poolIndex] = spawnPos;
    }

    /// <summary>
    /// Returns a spawned collectible to the next position in its chain.
    /// </summary>
    public void ReturnToPool(GameObject collectible)
    {
        if (collectible == null) return;

        for (int i = 0; i < _pools.Length; i++)
        {
            if (_pools[i].Contains(collectible))
            {
                SpawnAtNextPosition(i, collectible);
                collectible.transform.SetParent(transform, false);
                return;
            }
        }
    }
}
