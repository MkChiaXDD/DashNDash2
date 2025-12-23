using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private SpeedManager speedManager;

    [Header("Collectibles")]
    [SerializeField] private GameObject[] collectiblePrefabs;
    [SerializeField] private int poolSizePerType = 10;

    [Header("Playfield")]
    [Range(0f, 1f)][SerializeField] private float fieldWidthPercent = 0.75f;

    [Header("Spawn Positioning")]
    [Range(0f, 1f)][SerializeField] private float spawnXPercent = 0.2f;
    [Range(2, 10)][SerializeField] private int xSegments = 5;

    [Header("Bias")]
    [Range(0f, 1f)][SerializeField] private float crossSideBias = 0.7f;
    [Range(0f, 1f)][SerializeField] private float repeatSlotPenalty = 0.75f;

    [Header("Row Scheduling")]
    [SerializeField] private int rowsAhead = 12;
    [SerializeField] private int startRows = 6;
    [SerializeField] private bool randomizeTypePerRow = true;
    [SerializeField] private int startTypeIndex = 0;

    private ObjectPool[] pools;

    private Vector3 lastSpawnPos;
    private int lastSlotIndex = -1;

    private float xRangeWorld;
    private float yOffsetWorld;
    private float halfField;
    private float camX;

    private float nextSpawnY;

    void Awake()
    {
        xRangeWorld = ScreenSizeHelper.PercentWidthToWorld(spawnXPercent);

        Vector2 dashDist = speedManager != null ? speedManager.GetDashDistance() : Vector2.up * 5f;
        yOffsetWorld = dashDist.y * 0.8f;

        float screenW = ScreenSizeHelper.ScreenWorldWidth;
        halfField = screenW * fieldWidthPercent * 0.5f;
        camX = Camera.main != null ? Camera.main.transform.position.x : 0f;

        lastSpawnPos = player.position;

        // Build pools
        pools = new ObjectPool[collectiblePrefabs.Length];
        for (int i = 0; i < collectiblePrefabs.Length; i++)
        {
            var poolGO = new GameObject($"{collectiblePrefabs[i].name}_Pool");
            poolGO.transform.SetParent(transform);

            var pool = poolGO.AddComponent<ObjectPool>();
            pool.Initialize(collectiblePrefabs[i], poolSizePerType);
            pools[i] = pool;
        }

        nextSpawnY = lastSpawnPos.y;
        PrewarmRows(startRows);
    }

    void Update()
    {
        float targetY = player.position.y + rowsAhead * yOffsetWorld;

        while (nextSpawnY < targetY)
        {
            SpawnRow(nextSpawnY);
            nextSpawnY += yOffsetWorld;
        }
    }

    private void PrewarmRows(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnRow(nextSpawnY);
            nextSpawnY += yOffsetWorld;
        }
    }

    private void SpawnRow(float y)
    {
        int type = randomizeTypePerRow
            ? Random.Range(0, pools.Length)
            : Mathf.Clamp(startTypeIndex, 0, pools.Length - 1);

        var obj = pools[type].Get();

        int slot = PickBiasedIndex();

        float slotWidth = (xRangeWorld * 2f) / (xSegments - 1);
        float x = -xRangeWorld + slot * slotWidth;

        Vector3 pos = new Vector3(
            Mathf.Clamp(x, camX - halfField, camX + halfField),
            y,
            0f
        );

        obj.transform.position = pos;
        obj.SetActive(true);

        lastSpawnPos = pos;
        lastSlotIndex = slot;
    }

    private int PickBiasedIndex()
    {
        // If this is the first spawn, allow full range
        if (lastSlotIndex < 0)
            return Random.Range(0, xSegments);

        int min = Mathf.Max(0, lastSlotIndex - 1);
        int max = Mathf.Min(xSegments - 1, lastSlotIndex + 1);

        float desiredSign = (lastSpawnPos.x >= camX) ? -1f : 1f;

        float slotWidth = (xSegments > 1)
            ? (xRangeWorld * 2f) / (xSegments - 1)
            : 0f;

        float sum = 0f;
        float[] weights = new float[max - min + 1];

        int idx = 0;
        for (int i = min; i <= max; i++)
        {
            float off = -xRangeWorld + i * slotWidth;
            float alignment = Mathf.Max(0f, Mathf.Sign(desiredSign) * Mathf.Sign(off));

            float w = 1f + crossSideBias * alignment;

            if (i == lastSlotIndex)
                w *= (1f - repeatSlotPenalty);

            weights[idx++] = w;
            sum += w;
        }

        float r = Random.value * sum;
        idx = 0;

        for (int i = min; i <= max; i++)
        {
            r -= weights[idx++];
            if (r <= 0f)
                return i;
        }

        return lastSlotIndex;
    }

}
