using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Bullet<T> : MonoBehaviour where T : Bullet<T>
{
    protected LayerMask _targetLayer;
    protected float _damage;
    protected float _speed;
    protected bool _isReleased;
    protected Rigidbody2D _rb;

    protected IObjectPool<T> _pool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Initialize(float damage, float speed,float angle,Vector3 position,LayerMask targetLayer, IObjectPool<T> pool)
    {
        _damage = damage;
        _speed = speed;
        transform.localRotation = Quaternion.Euler(0, 0, angle);
        transform.position = position;
        _targetLayer = targetLayer;
        _pool = pool;
        _isReleased = false;
    }

    public virtual void Fire()
    {
        _rb.velocity = transform.right * _speed;
    }

    protected virtual void ReturnToPool()
    {
        if (_pool != null && this is T self)
        {
            _isReleased = true;
            _pool.Release(self);
        }
    }
}