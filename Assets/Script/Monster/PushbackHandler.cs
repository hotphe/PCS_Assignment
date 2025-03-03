using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(Monster))]
public class PushbackHandler : MonoBehaviour
{
    [Tooltip("밑에 있는 대상을 뒤로 밀어낼 때 몬스터의 좌우 너비 + 마진값 만큼 밀어냄.")]
    [Min(0)][SerializeField] private float _pushbackMargin = 0.1f;
    [Tooltip("밑에 있는 대상을 Duration에 걸쳐서 밀어냄.")]
    [Min(0.2f)][SerializeField] private float _pushDuration = 0.2f;
    private Monster _monster;
    
    // Start is called before the first frame update
    private void Awake()
    {
        _monster = GetComponent<Monster>();
    }

    private void OnEnable()
    {
        if (_monster != null)
            _monster.OnPushback += HandlePushback;
    }

    private void OnDisable()
    {
        if (_monster != null)
            _monster.OnPushback -= HandlePushback;
    }

    private void HandlePushback(IEnumerable<ContactPoint2D> contacts)
    {
        // 내가 바닥에 닿으면 밀지않음
        if (_monster.IsLanded)
            return;
        
        // 내가 트럭에 달라붙어있지 않으면 밀지않음
        if (!_monster.CanTouchTarget)
            return;
        
        // 내 아래에 있는 몬스터들을 가져옴
        var bottomMonsters = contacts.Where(c => c.normal.y > 0)
            .Select(x => x.collider.GetComponent<Monster>())
            .Where(m=> m!=null)
            .Distinct();

        if (bottomMonsters.Count() == 0)
            return;

        foreach(var monster in bottomMonsters)
        {
            if (!monster.IsLanded)
                continue;
            
            monster.Rb.velocity = Vector2.zero;
            float pushbackPositionX = transform.position.x + _monster.Width + _pushbackMargin;
            // 부드러운 이동을 위해 DOTween 사용
            monster.Rb.DOMoveX(pushbackPositionX * _monster.MoveDirection.x * -1.0f, _pushDuration);
            return;
        }
    }

}
