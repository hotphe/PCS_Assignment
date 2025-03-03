using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Monster _monsterPrefab;
    [Min(0)][SerializeField] private float _minCooltime;
    [Min(0)][SerializeField] private float _maxCooltime;
    [SerializeField] private int _maxSpawnCount;
    [SerializeField] [SingleLayer] private int _spawnLayer;

    private int _currentSpawnCount;
    private IObjectPool<Monster> _pool;
    private CooltimeSystem _cooltimeSystem;
    private CancellationTokenSource _cts;

    private int _capacity = 10;
    private int _poolMaxSize = 255;

    private void Awake()
    {
        _pool = new ObjectPool<Monster>(
            CreateMonster,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            _capacity,
            _poolMaxSize
            );

        InitializeCooltimeSystem();
    }

    // 시작시 쿨타임 걸고 시작함.
    private void InitializeCooltimeSystem()
    {
        _cooltimeSystem = new CooltimeSystem();
        DisposeCts();
        _cts = new CancellationTokenSource();
        float cooltime = UnityEngine.Random.Range(_minCooltime, _maxCooltime);
        _cooltimeSystem.StartCooldown(cooltime, _cts.Token).Forget();
    }

    private void Update()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (_cooltimeSystem.IsCooldown())
            return;

        if (_currentSpawnCount >= _maxSpawnCount)
            return;

        DisposeCts();
        _cts = new CancellationTokenSource();
        float cooltime = UnityEngine.Random.Range(_minCooltime, _maxCooltime);
        _cooltimeSystem.StartCooldown(cooltime, _cts.Token).Forget();

        Monster monster = _pool.Get();
        monster.SetPool(_pool);
        monster.SetLayer(_spawnLayer);
        var resetables = monster.GetComponents<IResetable>();

        foreach (var resetable in resetables)
            resetable.ResetData();
        monster.transform.position = transform.position;
    }

    private Monster CreateMonster()
    {
        var monsterObject = Instantiate(_monsterPrefab);
        if (monsterObject == null)
            return null;
        monsterObject.gameObject.SetActive(false);
        return monsterObject;
    }

    private void OnTakeFromPool(Monster monster)
    {
        _currentSpawnCount++;
        monster.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(Monster monster)
    {
        _currentSpawnCount--;
        monster.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(Monster monster)
    {
        Destroy(monster.gameObject);
    }

    private void DisposeCts()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private void OnDestroy()
    {
        DisposeCts();
    }
}
