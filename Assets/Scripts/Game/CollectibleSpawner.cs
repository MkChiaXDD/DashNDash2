using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private SpeedManager speedManager;

    [Header("Collectible Settings")]
    [SerializeField] private GameObject[] collectiblePrefabs;
    [SerializeField] private int poolSizePerType = 10;

    [Header("Playfield Settings")]
    [Range(0f, 1f)][SerializeField] private float fieldWidthPercent = 0.75f;

    [Header("Spawn Positioning")]
    [Range(0f, 1f)][SerializeField] private float spawnXPercent = 0.2f;
    [Range(2, 10)][SerializeField] private int xSegments = 5;

    [Header("Bias Settings")]
    [Tooltip("0 = no bias, 1 = strongly prefer opposite side")]
    [Range(0f, 1f)][SerializeField] private float crossSideBias = 0.7f;
    [Tooltip("How much to de-emphasize repeating the same slot")]
    [Range(0f, 1f)][SerializeField] private float repeatSlotPenalty = 0.75f;

    [Header("Row Scheduling")]
    [Tooltip("How many rows ahead of the player to keep spawned")]
    [SerializeField] private int rowsAhead = 12;
    [Tooltip("How many rows to prewarm at Start")]
    [SerializeField] private int startRows = 6;
    [Tooltip("If true, choose a random prefab type per row; else stick to startTypeIndex")]
    [SerializeField] private bool randomizeTypePerRow = true;
    [SerializeField] private int startTypeIndex = 0;

    // Pools
    private List<GameObject>[] _pools;

    // GLOBAL chain state (one timeline across all prefabs)
    private Vector3 _lastSpawnPos;
    private int _lastSlotIndex = -1;

    // Cached world values
    private float _xRangeWorld;
    private float _yOffsetWorld;
    private float _halfField;
    private float _camX;

    // Row scheduler
    private float _nextSpawnY;

    void Awake()
    {
        if (player == null) Debug.LogWarning("CollectibleSpawner: Player not set.");
        if (speedManager == null) Debug.LogWarning("CollectibleSpawner: SpeedManager not set.");
        if (collectiblePrefabs == null || collectiblePrefabs.Length == 0)
            Debug.LogError("CollectibleSpawner: No collectible prefabs assigned.");

        _xRangeWorld = ScreenSizeHelper.PercentWidthToWorld(spawnXPercent);

        Vector2 dashDist = speedManager != null ? speedManager.GetDashDistance() : new Vector2(0f, 5f);
        _yOffsetWorld = dashDist.y * 0.8f;

        float screenW = ScreenSizeHelper.ScreenWorldWidth;
        _halfField = screenW * fieldWidthPercent * 0.5f;
        _camX = Camera.main != null ? Camera.main.transform.position.x : 0f;

        _lastSpawnPos = player != null ? player.position : Vector3.zero;
        _lastSlotIndex = -1;

        // Build pools (inactive)
        int types = collectiblePrefabs != null ? collectiblePrefabs.Length : 0;
        _pools = new List<GameObject>[types];
        for (int i = 0; i < types; i++)
        {
            _pools[i] = new List<GameObject>(poolSizePerType);
            for (int n = 0; n < poolSizePerType; n++)
            {
                var go = Instantiate(collectiblePrefabs[i], transform);
                go.SetActive(false); // don't spawn yet
                _pools[i].Add(go);
            }
        }

        // Start the scheduler at the player's current Y
        _nextSpawnY = _lastSpawnPos.y;

        // Prewarm a few rows ahead
        PrewarmRows(Mathf.Max(0, startRows));
    }

    void Update()
    {
        if (player == null || _pools == null || _pools.Length == 0) return;

        // Keep exactly one collectible per row, rowsAhead ahead of the player
        float targetY = player.position.y + rowsAhead * _yOffsetWorld;

        while (_nextSpawnY < targetY)
        {
            SpawnRowAtY(_nextSpawnY);
            _nextSpawnY += _yOffsetWorld; // advance exactly one row
        }
    }

    // --------- Spawning (one per row) ----------

    private void PrewarmRows(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRowAtY(_nextSpawnY);
            _nextSpawnY += _yOffsetWorld;
        }
    }

    private void SpawnRowAtY(float y)
    {
        int type = randomizeTypePerRow
            ? Random.Range(0, _pools.Length)
            : Mathf.Clamp(startTypeIndex, 0, _pools.Length - 1);

        var go = GetFromPool(type);
        if (go == null) return;

        int idx = PickBiasedIndexGlobal();

        float slotWidth = (_xRangeWorld * 2f) / (xSegments - 1);
        float offsetX = -_xRangeWorld + idx * slotWidth;

        Vector3 spawnPos;
        // advance exactly one row from lastSpawnPos, but lock Y to scheduler's 'y'
        spawnPos = new Vector3(_lastSpawnPos.x, y, _lastSpawnPos.z);
        spawnPos += Vector3.up * 0f; // (explicitly 0, we already set Y)
        spawnPos.x += offsetX;
        spawnPos.x = Mathf.Clamp(spawnPos.x, _camX - _halfField, _camX + _halfField);

        go.transform.position = spawnPos;
        go.SetActive(true);

        _lastSpawnPos = spawnPos;    // advance the single global chain
        _lastSlotIndex = idx;
    }

    // --------- Pool helpers ----------

    private GameObject GetFromPool(int type)
    {
        foreach (var go in _pools[type])
            if (!go.activeSelf) return go;

        // Optional: expand pool if needed
        var extra = Instantiate(collectiblePrefabs[type], transform);
        extra.SetActive(false);
        _pools[type].Add(extra);
        return extra;
    }

    // --------- Return API (optional, for cleanup) ----------

    public void ReturnToPool(GameObject collectible)
    {
        if (collectible == null) return;

        for (int i = 0; i < _pools.Length; i++)
        {
            if (_pools[i].Contains(collectible))
            {
                collectible.SetActive(false);
                collectible.transform.SetParent(transform, false);
                return;
            }
        }
    }

    // --------- Cross-side bias (global chain) ----------

    private int PickBiasedIndexGlobal()
    {
        float lastX = _lastSpawnPos.x;

        // bias to opposite side of where we last were relative to camera center
        float desiredSign = (lastX >= _camX) ? -1f : 1f;

        // stronger push when far from center
        float centerDistance01 = _halfField > 0f
            ? Mathf.Clamp01(Mathf.Abs(lastX - _camX) / _halfField)
            : 0f;
        float biasScale = crossSideBias * centerDistance01;

        float slotWidth = (xSegments > 1)
            ? (_xRangeWorld * 2f) / (xSegments - 1)
            : 0f;

        float sum = 0f;
        float[] weights = new float[xSegments];

        for (int i = 0; i < xSegments; i++)
        {
            float off = -_xRangeWorld + i * slotWidth; // <0 left, >0 right
            float alignment = Mathf.Max(0f, Mathf.Sign(desiredSign) * Mathf.Sign(off));
            float distance01 = (_xRangeWorld > 0f) ? Mathf.Abs(off) / _xRangeWorld : 0f;

            float w = 1f + biasScale * alignment * (0.5f + 0.5f * distance01);
            if (i == _lastSlotIndex)
                w *= (1f - repeatSlotPenalty);

            weights[i] = w;
            sum += w;
        }

        if (sum <= 0f) return Random.Range(0, xSegments);

        float r = Random.value * sum;
        for (int i = 0; i < xSegments; i++)
        {
            r -= weights[i];
            if (r <= 0f) return i;
        }
        return xSegments - 1;
    }
}
