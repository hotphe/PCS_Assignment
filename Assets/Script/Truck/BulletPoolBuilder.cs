using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPoolBuilder<T> where T : Bullet<T>
{
    private T _bulletPrefab;
    private int _capacity = 10;
    private int _poolMaxSize = 255;

    public BulletPoolBuilder<T> SetBullet(T bullet) 
    {  
        _bulletPrefab = bullet;
        return this;
    }

    public BulletPoolBuilder<T> SetPoolCapacity(int capacity) 
    { 
        _capacity = capacity;
        return this;
    }
    public BulletPoolBuilder<T> SetPoolMaxSize(int maxSize) 
    { 
        _poolMaxSize = maxSize;
        return this;
    }

    public IObjectPool<T> Build()
    {
        IObjectPool<T> _pool = new ObjectPool<T>(
            CreateBullet,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            _capacity,
            _poolMaxSize
            );

        return _pool;   
    }

    private T CreateBullet()
    {
        var bullet = Object.Instantiate(_bulletPrefab);

        if (bullet == null)
            return null;
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    private void OnTakeFromPool(T bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(T bullet)
    {
        bullet.gameObject.SetActive(false);
    }
    private void OnDestroyPoolObject(T bullet)
    {
        Object.Destroy(bullet);
    }

}
