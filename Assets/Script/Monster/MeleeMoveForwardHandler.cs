using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Monster), typeof(Rigidbody2D))]
public class MeleeMoveForwardHandler : MonoBehaviour
{
    [Min(0)][SerializeField] private float _minMoveSpeed;
    [Min(0)][SerializeField] private float _maxMoveSpeed;
    private float _moveSpeed;
    private Monster _monster;
    private Rigidbody2D _rb;
    // Start is called before the first frame update
    private void Awake()
    {
        _monster = GetComponent<Monster>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (_monster != null)
            _monster.OnMoveForward += HandleMove;
        _moveSpeed = UnityEngine.Random.Range(_minMoveSpeed, _maxMoveSpeed);
    }
    private void OnDisable()
    {
        if (_monster != null)
            _monster.OnMoveForward -= HandleMove;
    }
    
    private void HandleMove(IEnumerable<ContactPoint2D> contacts)
    {
        var frontMonsters = contacts.Where(c => c.normal.x > 0)
            .Select(x => x.collider.GetComponent<Monster>())
            .Where(m => m != null);

        // 앞에 몬스터가 없으면 전진
        if (frontMonsters.Count() == 0)
            _rb.velocity = new Vector2(_moveSpeed * _monster.MoveDirection.x, _rb.velocity.y);
    }
}
