using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player transform - initial spawn reference")]
    [SerializeField] private Transform player;

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
    [Tooltip("Vertical offset per spawn based on screen height percentage (0–1)")]
    [Range(0f, 1f)]
    [SerializeField] private float spawnHeightPercent = 0.1f;
    [Tooltip("Horizontal random range per spawn based on screen width percentage (0–1)")]
    [Range(0f, 1f)]
    [SerializeField] private float spawnXPercent = 0.2f;
    [Tooltip("Number of discrete X slots across the range; reduces clustering")]
    [Range(2, 10)]
    [SerializeField] private int xSegments = 5;

    private List<GameObject>[] _pools;
    private Vector3[] _lastSpawnPos;
    private int[] _lastSlotIndex;

    void Awake()
    {
        int types = collectiblePrefabs.Length;
        _pools = new List<GameObject>[types];
        _lastSpawnPos = new Vector3[types];
        _lastSlotIndex = new int[types];

        float xRangeWorld = ScreenSizeHelper.PercentWidthToWorld(spawnXPercent);
        float yOffsetWorld = ScreenSizeHelper.PercentHeightToWorld(spawnHeightPercent);
        float halfField = ScreenSizeHelper.ScreenWorldWidth * fieldWidthPercent * 0.5f;
        float camX = Camera.main.transform.position.x;

        for (int i = 0; i < types; i++)
        {
            _pools[i] = new List<GameObject>(poolSizePerType);
            _lastSpawnPos[i] = player != null ? player.position : Vector3.zero;
            _lastSlotIndex[i] = -1;

            for (int n = 0; n < poolSizePerType; n++)
            {
                GameObject go = Instantiate(collectiblePrefabs[i], transform);

                // pick a new random slot different from last
                int idx;
                do { idx = Random.Range(0, xSegments); }
                while (idx == _lastSlotIndex[i]);
                _lastSlotIndex[i] = idx;

                float slotWidth = (xRangeWorld * 2f) / (xSegments - 1);
                float offsetX = -xRangeWorld + idx * slotWidth;

                Vector3 spawnPos = _lastSpawnPos[i] + Vector3.up * yOffsetWorld;
                spawnPos.x += offsetX;

                // clamp within playfield borders
                spawnPos.x = Mathf.Clamp(spawnPos.x, camX - halfField, camX + halfField);

                go.transform.position = spawnPos;
                go.SetActive(true);

                _pools[i].Add(go);
                _lastSpawnPos[i] = spawnPos;
            }
        }
    }

    /// <summary>
    /// Returns a spawned collectible to the next position in its chain.
    /// </summary>
    public void ReturnToPool(GameObject collectible)
    {
        if (collectible == null) return;

        float xRangeWorld = ScreenSizeHelper.PercentWidthToWorld(spawnXPercent);
        float yOffsetWorld = ScreenSizeHelper.PercentHeightToWorld(spawnHeightPercent);
        float halfField = ScreenSizeHelper.ScreenWorldWidth * fieldWidthPercent * 0.5f;
        float camX = Camera.main.transform.position.x;

        for (int i = 0; i < _pools.Length; i++)
        {
            if (_pools[i].Contains(collectible))
            {
                // pick a new random slot different from last
                int idx;
                do { idx = Random.Range(0, xSegments); }
                while (idx == _lastSlotIndex[i]);
                _lastSlotIndex[i] = idx;

                float slotWidth = (xRangeWorld * 2f) / (xSegments - 1);
                float offsetX = -xRangeWorld + idx * slotWidth;

                Vector3 spawnPos = _lastSpawnPos[i] + Vector3.up * yOffsetWorld;
                spawnPos.x += offsetX;

                // clamp within playfield borders
                spawnPos.x = Mathf.Clamp(spawnPos.x, camX - halfField, camX + halfField);

                collectible.transform.position = spawnPos;
                _lastSpawnPos[i] = spawnPos;
                collectible.transform.SetParent(transform, false);
                return;
            }
        }
    }
}
