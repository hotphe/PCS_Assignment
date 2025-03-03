using UnityEngine;

public class DamagePopupHandler : MonoBehaviour
{
    private IDamageable _damaeable;
    private void Awake()
    {
        _damaeable = GetComponent<IDamageable>();
    }

    private void OnEnable()
    {
        if (_damaeable != null)
            _damaeable.OnDamage += PopupDamage;
    }
    private void OnDisable()
    {
        if( _damaeable != null )
            _damaeable.OnDamage -= PopupDamage;
    }

    private void PopupDamage(float currentHealth, float maxHealth, float damage)
    {
        DamagePopupManager.Instance.Popup(transform.position, damage);
    }


}
