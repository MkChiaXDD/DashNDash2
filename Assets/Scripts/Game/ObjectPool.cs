using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;

    private readonly List<GameObject> pool = new List<GameObject>();

    public void Initialize(GameObject prefab, int size)
    {
        this.prefab = prefab;
        initialSize = size;

        for (int i = 0; i < initialSize; i++)
        {
            CreateNew();
        }
    }

    public GameObject Get()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeSelf)
                return obj;
        }

        return CreateNew();
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform, false);
    }

    private GameObject CreateNew()
    {
        var obj = Instantiate(prefab, transform);
        obj.SetActive(false);

        // Assign pool reference
        var pooled = obj.GetComponent<PooledObject>();
        if (pooled == null)
            pooled = obj.AddComponent<PooledObject>();

        pooled.SetPool(this);

        pool.Add(obj);
        return obj;
    }

}
