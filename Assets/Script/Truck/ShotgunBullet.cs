using UnityEngine;
using UnityEngine.Pool;

public class ShotgunBullet : Bullet<ShotgunBullet>
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isReleased)
            return;

        // Physics2D에서 CollisionMatrix에 설정해두었지만 추후 레이어 추가 시 문제 발생하지 않도록 별도처리
        if (!_targetLayer.Contains(collision.gameObject.layer))
            return;

        if (collision.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
        }

        ReturnToPool();
    }
}