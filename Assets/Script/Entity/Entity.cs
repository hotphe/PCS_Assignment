using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Entity : MonoBehaviour
{
    protected Collider2D _col;
    public Rigidbody2D Rb { get; private set; }

    // bounds 계산은 비용이 상대적으로 크므로 캐싱해서 사용
    protected float? _cachedHeight;
    protected float? _cachedWidth;
    public float Height
    {
        get
        {
            if (_cachedHeight.HasValue)
                return _cachedHeight.Value;

            if (_col == null && !TryGetComponent<Collider2D>(out _col))
                return 0;

            switch (_col)
            {
                case CircleCollider2D circle:
                    _cachedHeight = circle.radius * 2.0f;
                    break;
                case BoxCollider2D box:
                    _cachedHeight = box.bounds.size.y + box.edgeRadius * 2.0f;
                    break;
                default:
                    _cachedHeight = _col.bounds.size.y;
                    break;
            }
            return _cachedHeight.Value;
        }
    }
    public float Width
    {
        get
        {
            if (_cachedWidth.HasValue)
                return _cachedWidth.Value;

            if (_col == null && !TryGetComponent<Collider2D>(out _col))
                return 0;

            switch(_col)
            {
                case CircleCollider2D circle:
                    _cachedWidth = circle.radius * 2.0f;
                    break;
                case BoxCollider2D box:
                    _cachedWidth = box.bounds.size.x + box.edgeRadius * 2.0f;
                    break;
                default:
                    _cachedWidth = _col.bounds.size.x;
                    break;
            }

            return _cachedWidth.Value;
        }
    }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    public virtual UniTask Death()
    {
        return UniTask.CompletedTask;
    }

}
