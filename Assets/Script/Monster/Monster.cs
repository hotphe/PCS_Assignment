using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public class Monster : Entity, IResetable
{
    [SerializeField] protected LayerMask _landLayer;
    [SerializeField] protected LayerMask _targetLayer;
    protected IObjectPool<Monster> _pool;

    public bool IsLanded { get; protected set; }
    public bool CanTouchTarget { get; protected set; }
    public bool IsReleased { get; protected set; }

    public Vector2 MoveDirection { get; private set; } = Vector2.left;

    public event Action<IEnumerable<ContactPoint2D>> OnMoveForward;
    public event Action<IEnumerable<ContactPoint2D>> OnJump;
    public event Action<IEnumerable<ContactPoint2D>> OnPushback;
    public event Action OnAttack;

    public void Update()
    {
        TryAttack();
    }

    public void FixedUpdate()
    {
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        _col.GetContacts(contacts);
        TryPushback(contacts);
        TryJump(contacts);
        TryMove(contacts);

    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (_targetLayer.Contains(collision.gameObject.layer))
            CanTouchTarget = true;

        if (_landLayer.Contains(collision.gameObject.layer))
            IsLanded = true;
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (_targetLayer.Contains(collision.gameObject.layer))
            CanTouchTarget = false;

        if(_landLayer.Contains(collision.gameObject.layer))
            IsLanded = false;
    }

    private void TryAttack()
    {
        OnAttack?.Invoke();   
    }

    private void TryMove(IEnumerable<ContactPoint2D> contacts)
    {
        OnMoveForward?.Invoke(contacts);
    }

    private void TryJump(IEnumerable<ContactPoint2D> contacts)
    {
        OnJump?.Invoke(contacts);
    }

    private void TryPushback(IEnumerable<ContactPoint2D> contacts)
    {
        OnPushback?.Invoke(contacts);
    }

    public void SetPool(IObjectPool<Monster> pool)
    {
        _pool = pool;
    }


    public void SetLayer(int layer)
    {
        gameObject.layer = layer;

        var children = GetComponentsInChildren<SpriteRenderer>();

        foreach( var child in children )
            child.sortingLayerName = LayerMask.LayerToName(layer);
    }

    /// <summary>
    /// 캐릭터의 크기가 변경될 경우 사용
    /// </summary>
    public void ResetCachedBounds()
    {
        _cachedHeight = null;
        _cachedWidth = null;
    }

    public override UniTask Death()
    {
        if (!IsReleased)
        {
            IsReleased = true;
            _pool.Release(this);
        }
        return UniTask.CompletedTask;
    }

    public void ResetData()
    {
        CanTouchTarget = false;
        IsLanded = false;
        _col.enabled = true;
        IsReleased = false;
    }
}
