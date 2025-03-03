using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using System.Threading;
using System;

[RequireComponent(typeof(Monster), typeof(Rigidbody2D))]
public class JumpHandler : MonoBehaviour
{
    [Tooltip("수직아래 기준 각도 내에 충돌점이 있을경우 점프 가능")]
    [Range(0f, 90f)][SerializeField] private float _groundDetectionAngle = 45f;

    [Tooltip("전방 각도 내에 넘어갈 수 있는 오브젝트가 있을 경우 점프")]
    [Range(0f, 90f)][SerializeField] private float _jumpDetectionAngle = 30f;
    [Tooltip("체크시 항상 최대 점프력 만큼 점프")]
    [SerializeField] private bool _jumpMaxAlways;
    [Min(0)][SerializeField] private float _maxJumpHeight;
    [Tooltip("점프시 대상을 넘기 위한 마진값")]
    [Min(0)][SerializeField] private float _jumpMargin;

    [Min(0)][SerializeField] private float _minCooltime;
    [Min(0)][SerializeField] private float _maxCooltime;

    private float _maxJumpVelocity;

    private CooltimeSystem _cooltimeSystem;
    private CancellationTokenSource _cts;

    private Monster _monster;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _rb = GetComponent<Rigidbody2D>();
        _cooltimeSystem = new CooltimeSystem();
        _maxJumpVelocity = GetJumpVelocity(_maxJumpHeight);
    }

    private void OnEnable()
    {
        if (_monster != null)
            _monster.OnJump += HandleJump;
    }

    private void OnDisable()
    {
        if (_monster != null)
            _monster.OnJump -= HandleJump;
        DisposeCts();
    }

    private void Update()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, float.MinValue, GetJumpVelocity(_maxJumpHeight)));
    }

    private void HandleJump(IEnumerable<ContactPoint2D> contacts)
    {
        if (_cooltimeSystem.IsCooldown())
            return;

        if (!CanJump(contacts))
            return;

        // 중복 방지 hashset
        var jumpableEntities = new HashSet<Entity>();
        
        foreach(var contact in contacts)
        {
            var jumpable = contact.collider.GetComponent<Entity>();

            if (jumpable is not IJumpoverable)
                continue;

            if (!NeedJump(jumpable.transform))
                continue;

            jumpableEntities.Add(jumpable);
        }

        if (jumpableEntities.Count() == 0)
            return;

        float totalHeight = _jumpMargin;
        float yPositionDiff = 0f;
        foreach (var jumpable in jumpableEntities)
        {
            yPositionDiff = jumpable.transform.position.y - transform.position.y;
            totalHeight += jumpable.Height + yPositionDiff;
            // 넘어야할 대상들의 총 높이가 내 점프력보다 크다면 점프하지않음.
            if (totalHeight > _maxJumpHeight)
                return;
        }

        DisposeCts();
        _cts = new CancellationTokenSource();

        float cooltime = UnityEngine.Random.Range(_minCooltime,_maxCooltime);

        _cooltimeSystem.StartCooldown(cooltime, _cts.Token).Forget();
        
        if (_jumpMaxAlways)
            totalHeight = _maxJumpHeight;
        PerformJump(GetJumpVelocity(totalHeight));
    }

    private float GetJumpVelocity(float jumpHeight)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        return Mathf.Sqrt(2 * gravity * jumpHeight);
    }

    /// <summary>
    /// (1/2)mv^2 = mgh 이용
    /// </summary>
    private void PerformJump(float jumpVelocity)
    {
        _rb.velocity = new Vector2(0, jumpVelocity);
    }

    /// <summary>
    /// 대상들중 하나라도 밟고 뛰어오를수 있는 위치에 있는지 체크
    /// </summary>
    /// <param name="contacts"></param>
    /// <returns></returns>
    private bool CanJump(IEnumerable<ContactPoint2D> contacts)
    {
        foreach (var contact in contacts)
        {
            float angle = Vector2.Angle(Vector2.up, contact.normal);
            if (angle <= _groundDetectionAngle)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 대상이 점프 체크 범위 안에 있는지 체크
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool NeedJump(Transform target)
    {
        Vector2 direction = target.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360f;

        if (angle >= 180 - _jumpDetectionAngle && angle <= 180 + _jumpDetectionAngle)
            return true;
        return false;
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