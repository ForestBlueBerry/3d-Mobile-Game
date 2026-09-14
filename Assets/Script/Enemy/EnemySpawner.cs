using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform _carTransform;
    [SerializeField] private Collider _roadSegmentCollider; 

    [Header("Spawn Settings")]
    [SerializeField] private int _totalEnemies = 25;   
    [SerializeField] private float _minSpawnZ = 15f;    
    [SerializeField] private float _levelLength = 600f; 

    private IObjectPool<Enemy> _enemyPool;
    private readonly List<Enemy> _activeEnemies = new List<Enemy>();

    private void Awake()
    {
        _enemyPool = new ObjectPool<Enemy>(
            createFunc: () =>
            {
                Enemy enemy = Instantiate(_enemyPrefab);
                enemy.SetPool(_enemyPool);
                return enemy;
            },
            actionOnGet: enemy => enemy.gameObject.SetActive(true),
            actionOnRelease: enemy => enemy.gameObject.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy.gameObject),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    public void SpawnEnemiesForLevel()
    {
        ClearActiveEnemies();

        if (_roadSegmentCollider == null)
        {
            return;
        }
        Bounds bounds = _roadSegmentCollider.bounds;
        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float spawnY = bounds.max.y;

        float startZ = _carTransform.position.z + _minSpawnZ;
        float endZ = _carTransform.position.z + _levelLength;

        for (int i = 0; i < _totalEnemies; i++)
        {
            Enemy enemy = _enemyPool.Get();

            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(startZ, endZ);

            enemy.transform.position = new Vector3(randomX, spawnY, randomZ);
            enemy.transform.rotation = Quaternion.identity;
            enemy.Init(_carTransform);

            _activeEnemies.Add(enemy);
        }
    }
    public void ClearActiveEnemies()
    {
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _activeEnemies[i];
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                enemy.Despawn();
            }
        }
        _activeEnemies.Clear();
    }
}