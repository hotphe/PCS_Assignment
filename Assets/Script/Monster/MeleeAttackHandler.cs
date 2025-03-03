using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Monster),typeof(Animator))]
public class MeleeAttackHandler : MonoBehaviour
{
    [SerializeField] private float _damage;
    [Tooltip("공격범위(사각형)의 중심)")]
    [SerializeField] private Vector2 _attackPoint;
    [Tooltip("공격범위(사각형)의 높이")]
    [SerializeField] private float _attackHeight;
    [Tooltip("공격범위(사각형)의 너비")]
    [SerializeField] private float _attackWidth;
    [Tooltip("한번에 공격할 수 있는 객체 수")]
    [SerializeField] private int _attackCount = 1;

    [SerializeField] private LayerMask _targetLayer;

    private Collider2D[] hitColliders;

    private Monster _monster;
    private Animator _animator;
    private CancellationTokenSource _cts;

    private bool _isCooldown = false;

    // Start is called before the first frame update
    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (_monster != null)
            _monster.OnAttack += HandleAttack;
    }

    private void OnDisable()
    {
        if (_monster != null)
            _monster.OnAttack -= HandleAttack;
    }

    private void HandleAttack()
    {
        Vector2 boxCenter = (Vector2)transform.position +_attackPoint;
        
        hitColliders = new Collider2D[_attackCount];

        int colCount = Physics2D.OverlapBoxNonAlloc(
            boxCenter,                 
            new Vector2(_attackWidth, _attackHeight),
            0f,                        
            hitColliders,              
            _targetLayer               
        );

#if UNITY_EDITOR
        DrawDebugBox(boxCenter, _attackWidth, _attackHeight, Color.red);
#endif
        if (colCount == 0)
        {
            _animator.SetBool("IsAttacking", false); // 제공된 IsAttacking parameter 사용.
            return;
        }

        if (_isCooldown)
            return;
        
        _animator.SetBool("IsAttacking", true); // 제공된 IsAttacking parameter 사용.
    }

    private void DrawDebugBox(Vector2 boxCenter, float width, float height, Color color)
    {
        Vector2 halfSize = new Vector2(width / 2, height / 2);

        // 박스의 네 꼭짓점 계산
        Vector2 topLeft = boxCenter + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = boxCenter + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = boxCenter + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = boxCenter + new Vector2(halfSize.x, -halfSize.y);

        // 박스의 네 변을 그리기
        Debug.DrawLine(topLeft, topRight, color);       // 위쪽 선
        Debug.DrawLine(topRight, bottomRight, color);   // 오른쪽 선
        Debug.DrawLine(bottomRight, bottomLeft, color); // 아래쪽 선
        Debug.DrawLine(bottomLeft, topLeft, color);     // 왼쪽 선
    }
    /// <summary>
    /// 제공된 Attack AnimationClip의 AnimationEvent 사용.
    /// bool로 관리하므로 공격 쿨타임 시스템은 사용하지 않았습니다.
    /// </summaryD>
    public void OnAttack()
    {
        foreach (var col in hitColliders)
        {
            if (col == null)
                continue;

            if (col.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(_damage);
        }
    }
}
