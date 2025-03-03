using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class CooltimeSystem
{
    private bool _isCooldown;

    public bool IsCooldown()
    {
        return _isCooldown;
    }

    public async UniTask StartCooldown(float cooltime, CancellationToken cancellationToken)
    {
        _isCooldown = true;

        try
        {
            await UniTask.WaitForSeconds(cooltime, cancellationToken: cancellationToken);
            _isCooldown = false;
        }
        catch (OperationCanceledException)
        {
            _isCooldown = false;
        }
    }
}
