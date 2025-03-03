using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Monster, IJumpoverable
{
    public override async UniTask Death()
    {
        if (!IsReleased)
        {
            IsReleased = true;
            _col.enabled = false;
            await UniTask.NextFrame(); // 한프레임 뒤에 Release합니다.
            _pool.Release(this);
        }
    }
}
