using DG.Tweening;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageable;
    [SerializeField] private Transform _fillBarDelay;
    [SerializeField] private Transform _fillBar;
    [SerializeField] private float _delayDuration = 0.5f;
    private void OnEnable()
    {
        if (_damageable != null)
            _damageable.Value.OnDamage += UpdateHealthBar;
    }

    private void OnDisable()
    {
        if (_damageable != null)
            _damageable.Value.OnDamage -= UpdateHealthBar;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth, float damage)
    {
        _fillBar.localScale = new Vector3(currentHealth / maxHealth,1f, 1f);
        _fillBarDelay.DOScaleX(currentHealth / maxHealth, _delayDuration);
    }


}