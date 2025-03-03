using System;
using UnityEngine;

public class HealthHandler : MonoBehaviour, IResetable, IDamageable
{
    [SerializeField] private GameObject _healthObject;
    [SerializeField] private float _maxHealth;
    private Entity _entity;
    private float _currentHealth;

    public event Action<float, float, float> OnDamage;

    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, _maxHealth);

            if (_currentHealth <= 0)
                _entity.Death();
        }
    }

    private void Awake()
    {
        _entity = GetComponent<Entity>();
        _currentHealth = _maxHealth;
    }

    public void ResetData()
    {
        _healthObject.SetActive(false);
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(_healthObject.activeSelf.Equals(false))
            _healthObject.SetActive(true);

        CurrentHealth -= damage;
        OnDamage?.Invoke(_currentHealth, _maxHealth, damage);
    }
}
