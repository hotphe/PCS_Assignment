using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary> 현재 체력 , 최대 체력, 받은 데미지를 매개변수로 사용합니다.  </summary>
    public event Action<float, float, float> OnDamage;
    public void TakeDamage(float damage);
}
