using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using System.Linq;

public class ShotgunAttackHandler : MonoBehaviour
{
    [SerializeField] private Transform _weapon;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private ShotgunBullet _bulletPrefab;
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _spreadAngle;
    [SerializeField] private int _bulletCount = 3;
    [SerializeField] private LayerMask _targetLayer;

    [Min(0)][SerializeField] private float _cooltime;
    private CooltimeSystem _cooltimeSystem;
    private CancellationTokenSource _cts;

    private Camera _camera;
    private Camera MainCamera
    {
        get
        {
            if(_camera == null)
                _camera = Camera.main;
            return _camera;
        }
    }
    private Plane[] planes;
    private Vector3 targetPos;
    private Hero _hero;

    private IObjectPool<ShotgunBullet> _bulletPool;

    private void Awake()
    {
        _hero = GetComponent<Hero>();
        BulletPoolBuilder<ShotgunBullet> builder = new BulletPoolBuilder<ShotgunBullet>()
            .SetBullet(_bulletPrefab);

        _bulletPool = builder.Build();
        _cooltimeSystem = new CooltimeSystem(); ;
        planes = GeometryUtility.CalculateFrustumPlanes(MainCamera);
    }

    private void OnEnable()
    {
        if (_hero != null)
            _hero.OnAttack += HandleAttack;
    }

    private void OnDisable()
    {
        if (_hero != null)
            _hero.OnAttack -= HandleAttack;
    }
    private void HandleAttack()
    {
        SetAttackDirection();

        if (_cooltimeSystem.IsCooldown())
            return;

        DisposeCts();
        _cts = new CancellationTokenSource();
        _cooltimeSystem.StartCooldown(_cooltime, _cts.Token).Forget();

        FireBullet();
    }

    private void SetAttackDirection()
    {
        if (Input.GetMouseButton(0))
        {
            // 클릭 시 클릭한 지점을 공격 위치로 설정
            targetPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            // 화면에 보이는 몬스터들 중 가장 가까운 몬스터를 공격 위치로 설정
            Monster[] allMonsters = FindObjectsOfType<Monster>();
            List<Monster> _viewMonsters = new List<Monster>();
            foreach(Monster monster in allMonsters)
            {
                Renderer renderer = monster.GetComponentInChildren<Renderer>(); 
                if (renderer == null)
                    continue;
                // renderer가 화면에 보이면 true
                if(GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                    _viewMonsters.Add(monster);
            }
            Monster target = _viewMonsters.OrderBy(x=> Vector3.Distance(transform.position, x.transform.position))
                .FirstOrDefault();

            if(target != null)
                targetPos = target.transform.position;
        }

        targetPos.z = 0f;
        Vector3 direction = (targetPos - _weapon.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _weapon.eulerAngles = new Vector3(0, 0, angle);
    }


    private void FireBullet()
    {
        for (int i = 0; i < _bulletCount; i++)
        {
            ShotgunBullet bullet = _bulletPool.Get();
            float angle = UnityEngine.Random.Range(-_spreadAngle, _spreadAngle) + _weapon.eulerAngles.z;
            bullet.Initialize(_damage, _speed, angle,_firePoint.position, _targetLayer, _bulletPool);
            bullet.Fire();
        }
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